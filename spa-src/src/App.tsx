import "./App.css";
import "./assets/scss/theme.scss";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { NotFound } from "./pages/NotFound";
import { routes } from "./routes";
import { Layout } from "./layout/Layout";
import { useEffect, useState } from "react";
import { MenuItem } from "./models/Interfaces";
import { getUserMenuItems } from "./models/Utilities";
import { genericMenuBase, sidebarMenuBase } from "./menuConfig";
import { msalConfig } from "./appConfig";
import {
  AuthenticationResult,
  Configuration,
  EventMessage,
  EventType,
  PublicClientApplication,
} from "@azure/msal-browser";
import {
  AuthenticatedTemplate,
  MsalProvider,
  UnauthenticatedTemplate,
} from "@azure/msal-react";
import { IdentityProvider } from "./contexts/IdentityContext";
import { SessionProvider } from "./contexts/SessionContext";
import { LoadingWrapper } from "./components/LoadingWrapper";
import { useSettingsContext } from "./contexts/UseContexts";
import { IsMs } from "./Utils/GlobalSettingsHelper";

function App() {
  const [msalInstance, setMsalInstance] = useState<
    PublicClientApplication | undefined
  >(undefined);
  const [sidebarMenu, setSidebarMenu] = useState<MenuItem[]>([]);
  const [navbarMenu, setNavbarMenu] = useState<MenuItem[]>([]);
  const { globalSettings } = useSettingsContext();

  const msalConfigFull = {
    ...msalConfig,
    redirectUri: window.location.origin,
  };

  async function fetchOpenIdConfig(authority: string): Promise<string> {
    try {
      const response = await fetch(
        `${authority}/.well-known/openid-configuration`,
      );
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
      const data = await response.json();
      // Convert the JSON object to a string
      return JSON.stringify(data);
    } catch (error) {
      console.error("Error fetching OpenID configuration:", error);
      throw error; // Rethrow to allow caller to handle
    }
  }

  const configureMsalInstance = (config: Configuration) => {
    // console.log("configureMsalInstance: ", config);
    const instance = new PublicClientApplication(config);

    instance.addEventCallback((event: EventMessage) => {
      if (
        msalInstance &&
        msalInstance.getAllAccounts().length > 0 &&
        event.eventType === EventType.LOGIN_SUCCESS &&
        event.payload
      ) {
        const payload = event.payload as AuthenticationResult;
        const account = payload.account;
        msalInstance.setActiveAccount(account);
      }
    });
    setMsalInstance(instance);
  };

  useEffect(() => {
    setSidebarMenu(getUserMenuItems(sidebarMenuBase, ["Admin"]));
    setNavbarMenu(getUserMenuItems(genericMenuBase, ["Admin"]));
  }, [genericMenuBase, sidebarMenuBase]);

  useEffect(() => {
    if (globalSettings.msalSettings !== null) {
      msalConfigFull.auth.clientId = globalSettings.msalSettings.clientId;
      msalConfigFull.auth.authority = globalSettings.msalSettings.authority;
      if (!IsMs(globalSettings)) {
        // since we're using Microsoft's MSAL libraries,
        // when we use something else, like Okta
        // we need to do a few things differently.
        fetchOpenIdConfig(globalSettings.msalSettings.authority).then(
          (openIdConfig) => {
            msalConfigFull.auth.authorityMetadata = openIdConfig;
            msalConfigFull.auth.knownAuthorities = [
              msalConfigFull.auth.authority,
            ];
            configureMsalInstance(msalConfigFull);
          },
        );
      } else configureMsalInstance(msalConfigFull);
    }
  }, [globalSettings]);

  return (
    <>
      {msalInstance && (
        <MsalProvider instance={msalInstance}>
          <BrowserRouter basename="/">
            <IdentityProvider messageWrapper={LoadingWrapper}>
              <AuthenticatedTemplate>
                <SessionProvider messageWrapper={LoadingWrapper}>
                  <Routes>
                    {routes.map((route, idx) => (
                      <Route
                        path={route.path}
                        key={idx}
                        element={
                          <Layout
                            title={route.title}
                            sidebarMenu={sidebarMenu}
                            navbarMenu={navbarMenu}
                          >
                            {route.component}
                          </Layout>
                        }
                      />
                    ))}
                    <Route
                      path="*"
                      element={
                        <Layout
                          title="Not Found"
                          sidebarMenu={sidebarMenu}
                          navbarMenu={navbarMenu}
                        >
                          <NotFound />
                        </Layout>
                      }
                    />
                  </Routes>
                </SessionProvider>
              </AuthenticatedTemplate>
              <UnauthenticatedTemplate>
                <em>Unauthenticated, please refresh.</em>
              </UnauthenticatedTemplate>
            </IdentityProvider>
          </BrowserRouter>
        </MsalProvider>
      )}
    </>
  );
}

export default App;
