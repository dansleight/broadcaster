import { createContext, useContext } from "react";
import { GridBreakpoint } from "../models/Enums";
import { Api } from "../apiClient/Api";
import {
  FullStreamState,
  GlobalSettingsModel,
  UserObject,
} from "../apiClient/data-contracts";

// ---- Settings Context -----------------------------------------------------------------------
type SettingsContextType = {
  setBodyAttribute: (
    attribute: string,
    value: string | null | undefined,
  ) => void;
  setHtmlAttribute: (
    attribute: string,
    value: string | null | undefined,
  ) => void;
  sidebarToggled: boolean;
  setSidebarToggled: (sidebarToggled: boolean) => void;
  toggleSidebar: () => void;
  breakpoint: GridBreakpoint;
  darkMode: boolean;
  setDarkMode: (darkMode: boolean) => void;
  globalSettings: GlobalSettingsModel;
  noAuthApi: Api;
};

export const SettingsContext = createContext({} as SettingsContextType);

export const useSettingsContext = () => useContext(SettingsContext);

// ---- Identity Context -----------------------------------------------------------------------
type IdentityContextType = {
  handleLogout: () => void;
  user: UserObject;
  authToken: string | null;
  setAuthToken: (token: string | null) => void;
};

export const IdentityContext = createContext({} as IdentityContextType);

export const useIdentityContext = () => useContext(IdentityContext);

// ---- Session Context -----------------------------------------------------------------------
type SessionContextType = {
  api: Api;
  getApiBearer: () => Promise<string | undefined>;
  streamState: FullStreamState;
};

export const SessionContext = createContext({} as SessionContextType);

export const useSessionContext = () => useContext(SessionContext);
