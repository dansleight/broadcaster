using System;

namespace Broadcaster.SpaModels;

public class AddPlaceholderModel
{
    #region Properties

    private string? unit;
    public string? Unit
    {
        get => string.IsNullOrWhiteSpace(unit) ? null : unit;
        set { unit = value; }
    }
    public required string Name { get; set; }
    public required IFormFile File { get; set; }

    #endregion
}
