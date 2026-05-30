
namespace Broadcaster.Stream;

public class FullStreamState
{
    #region Properties

    public StreamState StreamState { get; }
    public string? PlaceholderImage { get; }
    public string? PlaceholderMusic { get; }

    #endregion

    #region Constructor

    public FullStreamState(StreamState streamState, string? placeholderImage, string? placeholderMusic)
    {
        StreamState = streamState;
        PlaceholderImage = null;
        PlaceholderMusic = null;
        if (placeholderImage is not null)
            PlaceholderImage = placeholderImage.Split('/', StringSplitOptions.None).Last();
        if (placeholderMusic is not null)
            PlaceholderMusic = placeholderMusic.Split('/', StringSplitOptions.None).Last();
    }

    public FullStreamState(StreamState streamState)
    {
        StreamState = streamState;
    }

    #endregion
}