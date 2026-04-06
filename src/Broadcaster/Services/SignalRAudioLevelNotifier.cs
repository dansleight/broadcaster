using System;
using Broadcaster.Hubs;
using Broadcaster.Stream;
using Microsoft.AspNetCore.SignalR;

namespace Broadcaster.Services;

public class SignalRAudioLevelNotifier : IAudioLevelNotifier
{
    private readonly IHubContext<AudioLevelHub> _hubContext;

    public SignalRAudioLevelNotifier(IHubContext<AudioLevelHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(double level)
    {
        await _hubContext.Clients.All.SendAsync("AudioLevel", level);
    }
}

public class SignalRStreamStateNotifier : IStreamStateNotifier
{
    private readonly IHubContext<AudioLevelHub> _hubContext;

    public SignalRStreamStateNotifier(IHubContext<AudioLevelHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(StreamState streamState)
    {
        await _hubContext.Clients.All.SendAsync("StreamState", streamState);
    }
}
