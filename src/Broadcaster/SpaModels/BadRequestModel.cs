using System;

namespace Broadcaster.SpaModels;

public class BadRequestModel
{
    #region Properties

    public string Message { get; set; } = null!;
    public string? UserMessage { get; set; }

    #endregion
}
