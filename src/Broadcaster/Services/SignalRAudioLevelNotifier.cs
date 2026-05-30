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
