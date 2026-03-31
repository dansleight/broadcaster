using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Broadcaster.Stream;

public class StreamManager : IAsyncDisposable
{
    private readonly ILogger<StreamManager> _logger;
    private readonly BroadcastSettings _settings;
    private static string? _videoDeviceId;
    private static string? _audioDeviceId;

    private CancellationTokenSource? _currentCts;
    private CommandTask<CommandResult>? _currentTask;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _pidFile = "/tmp/bwb_ffmpeg.pid";

    public StreamManager(ILogger<StreamManager> logger, IOptions<BroadcastSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

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
            string musicPath,
            string? rtmpUrl = null)
    {
        rtmpUrl ??= _settings.RtmpUri;
        _logger.LogInformation("Starting placeholder → {RtmpUrl}", rtmpUrl);

        var cmd = Cli.Wrap("ffmpeg")
            .WithArguments(new[]
            {
                "-hide_banner", "-re",
                "-loop", "1", "-i", imagePath,
                "-stream_loop", "-1", "-i", musicPath,
                "-c:v", "libx264", "-preset", "veryfast", "-tune", "stillimage",
                "-b:v", "1000k", "-maxrate", "1000k", "-bufsize", "2000k",
                "-g", "60", "-keyint_min", "60",
                "-c:a", "aac", "-b:a", "128k",
                "-f", "flv", rtmpUrl
            })
            .WithStandardErrorPipe(PipeTarget.ToDelegate(LogFfmpegLine))
            .WithValidation(CommandResultValidation.None);

        await ReplaceCommandAsync(cmd);
    }

    public async Task RunLiveVideoAsync(string? rtmpUrl = null)
    {
        string videoDevice = await getVideoDevice();
        string audioDevice = await getAudioDevice();
        rtmpUrl ??= _settings.RtmpUri;
        _logger.LogInformation("Starting live video → {RtmpUrl}", rtmpUrl);

        var v4l2 = Cli.Wrap("v4l2-ctl")
            .WithArguments(new[]
            {
                "-d", videoDevice,
                "--stream-mmap",
                "--stream-to=-",
                "--set-fmt-video=width=640,height=480,pixelformat=YUYV"
            });
        _logger.LogInformation($"vrl2: {v4l2.ToString()}");

        var ffmpeg = Cli.Wrap("ffmpeg")
            .WithArguments(new[]
            {
                "-hide_banner",
                "-thread_queue_size", "1024",

                // Video Input
                "-f", "rawvideo",
                "-pixel_format", "yuyv422",
                "-video_size", "640x480",
                "-framerate", "30",
                "-i", "pipe:0",

                // Audio Input
                "-thread_queue_size", "1024",
                "-f", "alsa",
                "-i", audioDevice,

                // Video Encoding
                "-c:v", "libx264", "-preset", "veryfast", "-tune", "zerolatency",
                "-b:v", "1000k", "-maxrate", "1000k", "-bufsize", "1000k",
                "-g", "30", "-keyint_min", "30",

                // Audio Encoding
                "-c:a", "aac",
                "-ac", "2",
                "-ar", "48000",
                "-b:a", "128k",

                // Finish
                "-f", "flv", rtmpUrl
            })
            // .WithArguments(new[]
            // {
            //     "-hide_banner",
            //     "-thread_queue_size", "512",
            //     "-f", "rawvideo", "-pixel_format", "yuyv422", "-video_size", "640x480", "-framerate", "30",
            //     "-i", "pipe:0",
            //     "-thread_queue_size", "512",
            //     "-f", "alsa", "-ac", "1", "-use_wallclock_as_timestamps", "1",
            //     "-i", audioDevice,
            //     "-c:v", "libx264", "-preset", "veryfast", "-tune", "zerolatency",
            //     "-b:v", "1000k", "-maxrate", "1000k", "-bufsize", "1000k",
            //     "-g", "30", "-keyint_min", "30",
            //     "-c:a", "aac", "-ac", "2", "-b:a", "128k",
            //     "-f", "flv", rtmpUrl
            // })
            .WithStandardErrorPipe(PipeTarget.ToDelegate(LogFfmpegLine))
            .WithValidation(CommandResultValidation.None);
        _logger.LogInformation($"ffmpeg: {ffmpeg.ToString()}");

        var pipeline = v4l2 | ffmpeg;   // ← still works exactly the same

        await ReplaceCommandAsync(pipeline);
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
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _lock.Dispose();
    }

    #endregion

    #region Private Helpers

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

    private void LogFfmpegLine(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
            _logger.LogDebug("FFmpeg: {Line}", line);
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

    private async Task DetermineDevicesAsync()
    {
        // Video
        var vidRes = await Cli.Wrap("v4l2-ctl")
            .WithArguments(new[] { "--list-devices" })
            .ExecuteBufferedAsync();

        string vidOutput = vidRes.StandardOutput;
        string vidErr = vidRes.StandardError;

        if (vidRes.ExitCode != 0)
            throw new Exception($"v4l2-ctl failed with exit code {vidRes.ExitCode}\nError: {vidErr}");

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

    #endregion
}
