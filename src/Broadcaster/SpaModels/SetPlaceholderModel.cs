using System;

namespace Broadcaster.SpaModels;

public class SetPlaceholderModel
{
    #region Properties

    private PlaceholderType placeholderType = PlaceholderType.Pre;
    public PlaceholderType PlaceholderType
    {
        get => placeholderType;
        set
        {
            placeholderType = value;
            if (value == PlaceholderType.Sacrament && MusicType is null)
                MusicType = global::MusicType.Sacrament;
        }
    }
    public MusicType? MusicType { get; set; }

    #endregion
}
