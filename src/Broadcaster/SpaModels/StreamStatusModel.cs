using System;

namespace Broadcaster.SpaModels;

public class StreamStatusModel
{
    #region Properties

    public string Status { get; set; } = null!;

    #endregion

    #region Constructors

    public StreamStatusModel() { }

    public StreamStatusModel(string status)
    {
        Status = status;
    }

    #endregion
}
