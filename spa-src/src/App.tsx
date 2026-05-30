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
import { SessionProvider } from "./contexts/SessionContext";
import { LoadingWrapper } from "./components/LoadingWrapper";
import { GoogleOAuthProvider } from "@react-oauth/google";
import { useSettingsContext } from "./contexts/UseContexts";
import { IdentityProvider } from "./contexts/IdentityContext";

function App() {
  const { globalSettings } = useSettingsContext();
  const [sidebarMenu, setSidebarMenu] = useState<MenuItem[]>([]);
  const [navbarMenu, setNavbarMenu] = useState<MenuItem[]>([]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setSidebarMenu(getUserMenuItems(sidebarMenuBase, ["Admin"]));
    setNavbarMenu(getUserMenuItems(genericMenuBase, ["Admin"]));
  }, [genericMenuBase, sidebarMenuBase]);

  return (
    <>
      <BrowserRouter basename="/">
        <GoogleOAuthProvider clientId={globalSettings.googleClientId || ""}>
          <IdentityProvider messageWrapper={LoadingWrapper}>
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
          </IdentityProvider>
        </GoogleOAuthProvider>
      </BrowserRouter>
    </>
  );
}

export default App;
