using System;

namespace Broadcaster.Business.Models.Config;

public class DataAccessSettings
{
    public Dictionary<string, string>? ConnectionStrings { get; set; }
}
