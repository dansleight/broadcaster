namespace Broadcaster.SpaModels;

public class GlobalSettingsModel
{
    #region Properties

    public string ApplicationMode { get; }
    public string GoogleClientId { get; }

    #endregion

    #region Constructor

    public GlobalSettingsModel(string applicationMode, IConfiguration configuration)
    {
        ApplicationMode = applicationMode;
        GoogleClientId = "1054831369991-80knpnri346p8gvb9qv2bna1iqkaq52a.apps.googleusercontent.com";
    }

    #endregion

}
