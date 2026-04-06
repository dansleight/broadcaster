using System;
using Microsoft.AspNetCore.SignalR;

namespace Broadcaster.Hubs;

public class AudioLevelHub : Hub
{
    private readonly ILogger<AudioLevelHub> _logger;

    public AudioLevelHub(ILogger<AudioLevelHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

}
