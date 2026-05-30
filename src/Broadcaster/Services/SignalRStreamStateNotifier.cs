using Broadcaster.Hubs;
using Broadcaster.Stream;
using Microsoft.AspNetCore.SignalR;

namespace Broadcaster.Services;

public class SignalRStreamStateNotifier : IStreamStateNotifier
{
    private readonly IHubContext<StatusHub> _hubContext;

    public SignalRStreamStateNotifier(IHubContext<StatusHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(FullStreamState fullStreamState)
    {
        await _hubContext.Clients.All.SendAsync("StreamState", fullStreamState);
    }
}