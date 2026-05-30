import { GlobalSettingsModel } from "./apiClient/data-contracts";

export const msalConfig = {
  auth: {
    clientId: "get-from-api",
    authority: "get-from-api",
    knownAuthorities: [""],
    authorityMetadata: "",
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: false,
  },
};

// not sure if we need this or not, consider that we'll probably use gravatar by default, rather than reach out to the graph endpoint for the avatar, and we should have the user info in the claims
export const loginRequest = {
  scopes: ["User.Read"],
};

export const webApiConfig = {
  origin: "",
};

export const defaultGlobalSettings: GlobalSettingsModel = {
  applicationMode: "Development",
  googleClientId: "",
  redirectUri: "",
};
