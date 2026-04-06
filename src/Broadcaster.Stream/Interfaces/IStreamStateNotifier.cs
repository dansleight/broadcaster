using System;

namespace Broadcaster.Stream;

public interface IStreamStateNotifier
{
    Task NotifyAsync(StreamState streamState);
}
