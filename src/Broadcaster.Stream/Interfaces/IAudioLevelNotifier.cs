using System;

namespace Broadcaster.Stream;

public interface IAudioLevelNotifier
{
    Task NotifyAsync(double level);
}
