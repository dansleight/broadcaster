using Broadcaster.Common;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Broadcaster.Stream;

public class StreamManager : IAsyncDisposable
{
    private readonly ILogger<StreamManager> _logger;
    private readonly BroadcastSettings _settings;
    private readonly IAudioLevelNotifier _audioLevelNotifier;
    private readonly IStreamStateNotifier _streamStateNotifier;
    private readonly ArtifactHelper _artifactHelper;
    private static string? _videoDeviceId;
    private static string? _audioDeviceId;

    // Broadcast process
    private CancellationTokenSource? _currentCts;
    private CommandTask<CommandResult>? _currentTask;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly SemaphoreSlim _previewLock = new(1, 1);
    private readonly string _pidFile = "/tmp/bwb_ffmpeg.pid";
    private static readonly Regex EbuRegex =
        new(@"\[Parsed_ebur128_\d+ @ [^\]]+\]\s+t:\s+[\d.]+\s+TARGET:[-\d]+\s+LUFS\s+M:\s*(-?\d+\.?\d*)", RegexOptions.Compiled);

    public StreamState StreamState { get; private set; } = StreamState.Idle;
    public string PlaceholderImage { get; private set; } = string.Empty;
    public string PlaceholderMusic { get; private set; } = string.Empty;
    public FullStreamState FullStreamState => new(
        StreamState,
        string.IsNullOrWhiteSpace(PlaceholderImage) ? null : PlaceholderImage,
        string.IsNullOrWhiteSpace(PlaceholderMusic) ? null : PlaceholderMusic);

    public StreamManager(
        ILogger<StreamManager> logger,
        IOptions<BroadcastSettings> settings,
        IAudioLevelNotifier audioLevelNotifier,
        IStreamStateNotifier streamStateNotifier,
        ArtifactHelper artifactHelper)
    {
        _logger = logger;
        _settings = settings.Value;
        _audioLevelNotifier = audioLevelNotifier;
        _streamStateNotifier = streamStateNotifier;
        _artifactHelper = artifactHelper;

        // Fire-and-forget orphan cleanup on startup
        _ = ReclaimOrphanAsync();
    }

    #region Public Methods

    public CommandTask<CommandResult>? GetCurrentTask()
    {
        return _currentTask;
    }

    public async Task RunPlaceholderAsync(
            string imagePath,
            string musicDir = "music/default/",
            string? rtmpUrl = null)
    {
        rtmpUrl ??= _settings.RtmpUri;
        _logger.LogInformation("Starting placeholder → {RtmpUrl}", rtmpUrl);

        if (!imagePath.StartsWith('/'))
            imagePath = Path.Combine(_artifactHelper.ArtifactPath, "slides", imagePath);
        if (!imagePath.EndsWith(".jpg"))
            imagePath = imagePath + ".jpg";

        string playlistPath = generatePlaylist(musicDir);

        var cmd = Cli.Wrap("ffmpeg")
            .WithArguments(new[]
            {
                "-hide_banner", "-re",
                "-loop", "1", "-i", imagePath,
                "-stream_loop", "-1", "-f", "concat", "-safe", "0", "-i", playlistPath,
                "-c:v", "libx264", "-preset", "veryfast", "-tune", "stillimage",
                "-b:v", "1000k", "-maxrate", "1000k", "-bufsize", "2000k",
                "-g", "60", "-keyint_min", "60",
                "-c:a", "aac", "-b:a", "128k",
                "-f", "flv", rtmpUrl
            })
            .WithStandardErrorPipe(PipeTarget.ToDelegate(LogFfmpegLine))
            .WithValidation(CommandResultValidation.None);

        await ReplaceCommandAsync(cmd);
        await SetStateAsync(StreamState.Placeholder, imagePath, musicDir);
    }

    public async Task RunLiveVideoAsync(string? rtmpUrl = null)
    {
        rtmpUrl ??= _settings.RtmpUri;
        _logger.LogInformation("Starting live video → {RtmpUrl}", rtmpUrl);

        string? videoDevice = null;
        string? audioDevice = null;

        try
        {
            videoDevice = await getVideoDevice();
            audioDevice = await getAudioDevice();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Hardware devices not found.");
        }

        if (string.IsNullOrEmpty(videoDevice))
        {
            if (_artifactHelper.ApplicationMode == "Development")
            {
                await RunDemoVideoAsync(rtmpUrl);
                return;
            }
            else
            {
                throw new Exception("Hardware to support broadcasting doesn't seem to be in place.");
            }
        }

        await RunHardwareLiveAsync(videoDevice, audioDevice!, rtmpUrl);
    }

    public async Task StopAsync()
    {
        await _lock.WaitAsync();
        try
        {
            await CancelCurrentCommandAsync();
            SafeDeletePidFile();
        }
        finally
        {
            _lock.Release();
        }
        await SetStateAsync(StreamState.Idle);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _lock.Dispose();
    }

    public string? GetAudioDeviceId() => _audioDeviceId;

    public void SetAudioDeviceId(string audioDeviceId)
    {
        _audioDeviceId = audioDeviceId;
    }

    public string? getVideoDeviceId() => _videoDeviceId;

    public void SetVideoDeviceId(string videoDeviceId)
    {
        _videoDeviceId = videoDeviceId;
    }

    #endregion

    #region Private Helpers

    private async Task SetStateAsync(StreamState? streamState, string? placeholderImage = null, string? placeholderMusic = null)
    {
        if (streamState.HasValue) StreamState = streamState.Value;
        PlaceholderImage = placeholderImage ?? string.Empty;
        PlaceholderMusic = placeholderMusic ?? string.Empty;

        await _streamStateNotifier.NotifyAsync(FullStreamState);
    }

    private async Task ReplaceCommandAsync(Command command)
    {
        await _lock.WaitAsync();
        try
        {
            await CancelCurrentCommandAsync();

            _currentCts = new CancellationTokenSource();

            _logger.LogInformation($"running command: {command.ToString()}");

            _currentTask = command.ExecuteAsync(_currentCts.Token);

            // PID is available immediately
            if (_currentTask.ProcessId > 0)
            {
                await File.WriteAllTextAsync(_pidFile, _currentTask.ProcessId.ToString());
                _logger.LogInformation("Streaming process started (PID {Pid})", _currentTask.ProcessId);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task CancelCurrentCommandAsync()
    {
        var cts = Interlocked.Exchange(ref _currentCts, null);
        var task = Interlocked.Exchange(ref _currentTask, null);

        if (cts is null) return;

        try
        {
            cts.Cancel(); // graceful SIGTERM on Linux

            if (task is not null)
            {
                var timeout = Task.Delay(7000);
                var completed = await Task.WhenAny(task, timeout);

                if (completed == timeout)   // ← fixed comparison
                {
                    _logger.LogWarning("Process did not exit cleanly within timeout — it was killed");
                }
                else
                {
                    _logger.LogInformation("Streaming process stopped gracefully");
                }
            }
        }
        finally
        {
            cts.Dispose();
        }
    }

    private int _ebur128Counter = 0;
    private async Task LogFfmpegLine(string line)
    {
        _logger.LogDebug("FFmpeg: {Line}", line);

        var match = EbuRegex.Match(line);
        if (match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var db))
        {
            if (++_ebur128Counter % 5 != 0) return;

            var level = Math.Clamp((db + 60.0) / 60.0, 0.0, 1.0);
            await _audioLevelNotifier.NotifyAsync(level);
        }
    }

    private async Task ReclaimOrphanAsync()
    {
        try
        {
            if (!File.Exists(_pidFile)) return;

            var pidText = await File.ReadAllTextAsync(_pidFile);
            if (int.TryParse(pidText.Trim(), out var pid) && pid > 0)
            {
                using var process = Process.GetProcessById(pid);
                if (!process.HasExited)
                {
                    _logger.LogInformation("Found orphaned FFmpeg (PID {Pid}), terminating", pid);
                    process.Kill(true);
                }
            }
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogWarning(ex, "Could not reclaim orphan process");
        }
        finally
        {
            SafeDeletePidFile();
        }
    }

    private void SafeDeletePidFile()
    {
        try { File.Delete(_pidFile); } catch { }
    }

    public async Task DetermineDevicesAsync()
    {
        // make sure to install the tools: 
        // $ sudo apt install v4l-utils alsa-utils ffmpeg

        // Video
        var vidRes = await Cli.Wrap("v4l2-ctl")
            .WithArguments(new[] { "--list-devices" })
            .ExecuteBufferedAsync();

        string vidOutput = vidRes.StandardOutput;
        string vidErr = vidRes.StandardError;

        if (vidRes.ExitCode != 0)
            throw new Exception($"v4l2-ctl failed with exit code {vidRes.ExitCode}\nError: {vidErr}");

        _logger.LogInformation(vidOutput);

        var vidMatch = Regex.Match(vidOutput,
            @"^USB.*Video:[\s\S]*?(/dev/video\d+)",
            RegexOptions.Multiline);

        if (vidMatch.Success)
        {
            string devicePath = vidMatch.Groups[1].Value;   // e.g. "/dev/video6"
            _videoDeviceId = devicePath;
        }
        else
        {
            Console.WriteLine("No USB video device found in the list.");
        }

        // Audio
        var audRes = await Cli.Wrap("arecord").WithArguments(new[] { "-l" }).ExecuteBufferedAsync();

        string audOutput = audRes.StandardOutput;
        string audErr = audRes.StandardError;

        if (audRes.ExitCode != 0)
            throw new Exception($"arecord failed with exit code {audRes.ExitCode}\nError: {audErr}");

        _logger.LogInformation(audOutput);

        var audMatch = Regex.Match(audOutput,
            @"^card\s+(\d+):.*USB.*Video",
            RegexOptions.Multiline | RegexOptions.IgnoreCase);

        if (audMatch.Success)
        {
            string cardNumber = audMatch.Groups[1].Value;   // e.g. "3"
            _audioDeviceId = $"hw:{cardNumber},0";
        }
        else
        {
            Console.WriteLine("No USB video audio card found in arecord -l output.");
        }
    }

    private async Task<string> getVideoDevice()
    {
        if (_videoDeviceId == null) await DetermineDevicesAsync();
        if (_videoDeviceId == null)
            throw new Exception("Unable to determine the Video Device to use");
        return _videoDeviceId;
    }

    private async Task<string> getAudioDevice()
    {
        if (_audioDeviceId == null) await DetermineDevicesAsync();
        if (_audioDeviceId == null)
            throw new Exception("Unable to determine Audio Device to use");
        return _audioDeviceId;
    }

    private string generatePlaylist(string musicDir) //TODO: change this to private
    {
        if (!musicDir.StartsWith('/'))
            musicDir = Path.Combine(_artifactHelper.ArtifactPath, "music/", musicDir);
        string[] files = Directory.GetFiles(musicDir, "*.mp3");
        string filePath = Path.Combine(_artifactHelper.ArtifactPath, "music/playlist.txt");

        Random.Shared.Shuffle(files);

        StringBuilder playlist = new();
        playlist.AppendLine("ffconcat version 1.0");

        foreach (string file in files)
        {
            playlist.AppendLine($"file {file}");
        }

        playlist.AppendLine($"file {filePath}");
        File.WriteAllText(filePath, playlist.ToString());

        return filePath;
    }

    private async Task RunDemoVideoAsync(string rtmpUrl)
    {
        string demoPath = Path.Combine(_artifactHelper.ArtifactPath, "come-thou-fount-demo.mp4");

        if (!File.Exists(demoPath))
        {
            _logger.LogError("Demo video not found at {Path}. Please create it first.", demoPath);
            await SetStateAsync(StreamState.Idle);
            return;
        }

        _logger.LogInformation("Using demo video loop: {Path}", demoPath);

        var cmd = Cli.Wrap("ffmpeg")
            .WithArguments(new[]
            {
            "-hide_banner", "-re",
            "-stream_loop", "-1", "-i", demoPath,
            "-c:v", "libx264", "-preset", "veryfast", "-tune", "zerolatency",
            "-b:v", "1000k", "-maxrate", "1000k", "-bufsize", "2000k",
            "-g", "60", "-keyint_min", "60",
            "-c:a", "aac", "-b:a", "128k",
            "-f", "flv", rtmpUrl
            })
            .WithStandardErrorPipe(PipeTarget.ToDelegate(LogFfmpegLine))
            .WithValidation(CommandResultValidation.None);

        await ReplaceCommandAsync(cmd);
        await SetStateAsync(StreamState.Live, placeholderImage: null, placeholderMusic: null); // or add a "Demo" state if you want
    }

    private async Task RunHardwareLiveAsync(string videoDevice, string audioDevice, string rtmpUrl)
    {
        _logger.LogInformation("Using hardware: Video={Video}, Audio={Audio}", videoDevice, audioDevice);

        var v4l2 = Cli.Wrap("v4l2-ctl")
            .WithArguments(new[]
            {
            "-d", videoDevice,
            "--set-fmt-video=width=640,height=480,pixelformat=MJPG",
            "--set-parm=30",
            "--stream-mmap",
            "--stream-count=0",
            "--stream-to=-"
            });

        var ffmpeg = Cli.Wrap("ffmpeg")
            .WithArguments(new[]
            {
            "-hide_banner",

            // Video
            "-thread_queue_size", "4096",
            "-f", "mjpeg",
            "-use_wallclock_as_timestamps", "1",
            "-framerate", "30",
            "-i", "pipe:0",

            // Audio
            "-thread_queue_size", "4096",
            "-itsoffset", "0.74",
            "-f", "alsa",
            "-ar", "48000",
            "-i", audioDevice,

            // Filters + Encoding
            "-vf", "format=yuv420p",
            "-c:v", "libx264", "-preset", "veryfast", "-tune", "zerolatency", "-profile:v", "high",
            "-b:v", "1000k", "-maxrate", "1000k", "-bufsize", "1000k",
            "-g", "30", "-keyint_min", "30",

            "-af", "alimiter=level_in=0.9:level_out=0.9:limit=0.8:attack=5:release=50,aresample=async=1,ebur128=peak=true",
            "-c:a", "aac", "-ac", "2", "-ar", "48000", "-b:a", "128k",

            "-f", "flv", rtmpUrl
            })
            .WithStandardErrorPipe(PipeTarget.ToDelegate(LogFfmpegLine))
            .WithValidation(CommandResultValidation.None);

        var pipeline = v4l2 | ffmpeg;
        await ReplaceCommandAsync(pipeline);
        await SetStateAsync(StreamState.Live);
    }

    #endregion
}
