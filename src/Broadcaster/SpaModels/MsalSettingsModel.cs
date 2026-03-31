using System;

namespace Broadcaster.SpaModels;

public class MsalSettingsModel
{
    #region Properties

    public string ClientId { get; set; }
    public string Authority { get; set; }
    public string ApiScope { get; set; }
    public string? Provider { get; set; }

    #endregion

    #region Constructor

    public MsalSettingsModel(string clientId, string tenantId, string apiScope)
    {
        ClientId = clientId;
        Authority = $"https://login.microsoftonline.com/{tenantId}";
        ApiScope = apiScope;
    }

    #endregion

    #region Static Methods

    public static MsalSettingsModel? GetMsalSettings(IConfiguration configuration)
    {
        string? clientId = configuration.GetSection("EntraId:ClientId").Get<string>();
        string? tenantId = configuration.GetSection("EntraId:TenantId").Get<string>();
        string? webApiScope = configuration.GetSection("EntraId:WebApiScope").Get<string>();

        if (
            string.IsNullOrWhiteSpace(clientId)
            || string.IsNullOrWhiteSpace(tenantId)
            || string.IsNullOrWhiteSpace(webApiScope))
        {
            return null;
        }

        return new MsalSettingsModel(clientId, tenantId, webApiScope);
    }

    #endregion
}
