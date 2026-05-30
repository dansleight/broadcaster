import { ComponentType, ReactNode, useEffect, useMemo, useState } from "react";
import { GridBreakpoint, gridBreakpoints } from "../models/Enums";
import { SettingsContext } from "./UseContexts";
import useCookie from "react-use-cookie";
import { GlobalSettingsModel } from "../apiClient/data-contracts";
import { defaultGlobalSettings, webApiConfig } from "../appConfig";
import { Api } from "../apiClient/Api";
import React from "react";

type SettingsProviderProps = {
  children: ReactNode;
  messageWrapper?: ComponentType<{ waiting?: boolean; children: ReactNode }>;
};

export function SettingsProvider({
  children,
  messageWrapper,
}: SettingsProviderProps) {
  const [globalSettings, setGlobalSettings] = useState<GlobalSettingsModel>(
    defaultGlobalSettings as GlobalSettingsModel,
  );
  const [loaded, setLoaded] = useState<boolean | undefined>(undefined);
  const [sidebarToggled, setSidebarToggled] = useState<boolean>(false);
  const [darkMode, setDarkMode] = useState<boolean>(false);
  const [darkModeCookie, setDarkModeCookie] = useCookie("darkmode", "false");

  const toggleSidebar = () => setSidebarToggled(!sidebarToggled);

  const setBodyAttribute = (
    attribute: string,
    value: string | null | undefined,
  ) => {
    if (document.body) {
      if (!value || value.trim() == "")
        document.body.removeAttribute(attribute);
      else document.body.setAttribute(attribute, value);
    }
  };

  const setHtmlAttribute = (
    attribute: string,
    value: string | null | undefined,
  ) => {
    if (document.documentElement) {
      if (!value || value.trim() == "")
        document.documentElement.removeAttribute(attribute);
      else document.documentElement.setAttribute(attribute, value);
    }
  };

  const noAuthApi = useMemo(() => {
    return new Api({ baseUrl: webApiConfig.origin });
  }, [webApiConfig]);

  useEffect(() => {
    const getSettings = async () => {
      noAuthApi
        .settingsGet()
        .then((res) => {
          setGlobalSettings(res.data);
          setLoaded(true);
        })
        .catch((e) => {
          console.error(e);
          setGlobalSettings(defaultGlobalSettings as GlobalSettingsModel);
          setLoaded(false);
        });
    };
    getSettings();
  }, [webApiConfig]);

  useEffect(() => {
    setHtmlAttribute("data-bs-theme", darkMode ? "dark" : "light");
    if (darkModeCookie != (darkMode ? "true" : "false"))
      setDarkModeCookie(darkMode ? "true" : "false");
  }, [darkMode]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    if (darkModeCookie === "true") setDarkMode(true);
    // establish some defaults for page loading
    // we could load the layoutConfig here and instantiate it, however
    // we don't know if the developer will want a server-side implementation
    // so we're just clearing them. The App for now will load the
    // layoutConfig and establish the attributes
    setBodyAttribute("data-topbar", undefined);
    setBodyAttribute("data-navbar", undefined);
    setBodyAttribute("data-sidebar", undefined);
  }, []);

  // Wanting to track the screen size, and surface it as a context property
  const [breakpoint, setBreakpoint] = useState<GridBreakpoint>(
    gridBreakpoints[0],
  );

  useEffect(() => {
    let currentBp: GridBreakpoint = GridBreakpoint.xs;
    const calcBreakpoint = () => {
      const innerWidth: number = window.innerWidth;
      let bp: GridBreakpoint = gridBreakpoints[0];
      for (let i = 1; i < gridBreakpoints.length; i++) {
        if (innerWidth < gridBreakpoints[i]) break;
        bp = gridBreakpoints[i];
      }
      if (bp !== currentBp) {
        currentBp = bp;
        setBreakpoint(bp);
      }
    };

    window.addEventListener("resize", calcBreakpoint);
    calcBreakpoint();

    return () => window.removeEventListener("resize", calcBreakpoint);
  }, []);

  const Wrapper = messageWrapper ?? React.Fragment;

  return (
    <SettingsContext.Provider
      value={{
        setBodyAttribute,
        setHtmlAttribute,
        sidebarToggled,
        setSidebarToggled,
        toggleSidebar,
        breakpoint,
        darkMode,
        setDarkMode,
        globalSettings,
        noAuthApi,
      }}
    >
      {loaded === undefined ? (
        <Wrapper>
          <em>Retrieving settings from the server...</em>
        </Wrapper>
      ) : loaded === false ? (
        <div className="text-center">
          <h3 className="text-danger">Failed to load settings...</h3>
          <p>
            I'm sorry, I was unable to get the necessary settings from the
            server in order to continue.
          </p>
          <p>
            Consider waiting a few minutes and trying to refresh. If the problem
            persists, contact support.
          </p>
        </div>
      ) : (
        <>{children}</>
      )}
    </SettingsContext.Provider>
  );
}
