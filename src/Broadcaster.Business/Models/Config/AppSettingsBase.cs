using System;

namespace Broadcaster.Business.Models.Config;

public class AppSettingsBase
{
    public ApplicationMode ApplicationMode { get; init; }
}

public enum ApplicationMode
{
    Undefined = 0,
    Development,
    Acceptance,
    Production
}