import { GlobalSettingsModel } from "../apiClient/data-contracts";

export const IsMs = (globalSettings: GlobalSettingsModel) => {
  if (
    globalSettings.msalSettings?.provider == null ||
    globalSettings.msalSettings.provider == undefined ||
    globalSettings.msalSettings.provider == "" ||
    globalSettings.msalSettings.provider.toLowerCase().startsWith("entra") ||
    globalSettings.msalSettings.provider.toLowerCase().startsWith("azure") ||
    globalSettings.msalSettings.provider.toLowerCase().startsWith("microsoft")
  )
    return true;
  return false;
};
