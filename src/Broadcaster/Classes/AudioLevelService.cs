using System;
using System.Text.RegularExpressions;
using Broadcaster.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Broadcaster.Classes;

public class AudioLevelService
{
    private readonly IHubContext<AudioLevelHub> _hubContext;
    private static readonly Regex RmsRegex = new(@"lavfi\.astats\.Overall\.RMS_level=(-?\d+\.?\d*)", RegexOptions.Compiled);

    public AudioLevelService(IHubContext<AudioLevelHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task ProcessFfmpegLine(string line)
    {
        var match = RmsRegex.Match(line);
        if (!match.Success) return;

        if (!double.TryParse(match.Groups[1].Value, out var db)) return;

        // Convert dB to 0.0-1.0 float, clamped to -60dB floor
        var level = Math.Clamp((db + 60.0) / 60.0, 0.0, 1.0);
        await _hubContext.Clients.All.SendAsync("AudioLevel", level);
    }
}
