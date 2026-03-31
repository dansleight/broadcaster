namespace Broadcaster.SpaModels;

public class GlobalSettingsModel
{
    #region Properties

    public string ApplicationMode { get; }
    public MsalSettingsModel? MsalSettings { get; }

    #endregion

    #region Constructor

    public GlobalSettingsModel(string applicationMode, IConfiguration configuration)
    {
        ApplicationMode = applicationMode;
        MsalSettings = MsalSettingsModel.GetMsalSettings(configuration);
    }

    #endregion

}
