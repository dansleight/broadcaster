import { useMsal } from "@azure/msal-react";
import { ComponentType, ReactNode, useEffect, useMemo, useRef, useState } from "react";
import { loginRequest } from "../appConfig";
import React from "react";
import { IdentityContext, useSettingsContext } from "./UseContexts";
import { RedirectRequest } from "@azure/msal-browser";
import { IsMs } from "../Utils/GlobalSettingsHelper";

type IdentityProviderProps = {
  children: ReactNode;
  messageWrapper?: ComponentType<{ children: ReactNode }>;
};

export const IdentityProvider = ({
  children,
  messageWrapper,
}: IdentityProviderProps) => {
  const { globalSettings } = useSettingsContext();
  const msal = useMsal();
  const { accounts, instance } = msal;
  const [waiting, setWaiting] = useState<boolean>(true);
  const [redirecting, setRedirecting] = useState<boolean | undefined>(
    undefined,
  );
  // const [name, setName] = useState<string>("unknown");
  // const [username, setUsername] = useState<string>("unknown");
  const effectCalled = useRef<boolean>(false);

  // if (
  //   instance.getActiveAccount() === null &&
  //   instance.getAllAccounts().length > 0
  // ) {
  //   instance.setActiveAccount(instance.getAllAccounts()[0]);
  // }

  const handleLogin = () => {
    const scopes: string[] = [];
    const redirectParams: RedirectRequest = {
      scopes: scopes,
      redirectUri: document.location.origin,
    };
    if (IsMs(globalSettings)) {
      // since this is Entra, we're going to:
      // 1) set the scope so we can read user info
      // 2) force a particular workflow
      redirectParams.scopes = loginRequest.scopes;
      redirectParams.extraQueryParameters = { msafed: "0" };
    } else {
      // since this isn't Entra, we're going to
      // set the scopes from globalSettings
      redirectParams.scopes = [globalSettings.msalSettings!.apiScope];
    }
    console.log("redirectParams: ", redirectParams);
    instance.loginRedirect(redirectParams).catch((e) => {
      console.error(e);
    });
  };

  const getAccount = () => {
    if (!instance.getActiveAccount()) {
      if (instance.getAllAccounts().length > 0)
        instance.setActiveAccount(instance.getAllAccounts()[0]);
    }
    return instance.getActiveAccount()!;
  }

  const handleLogout = () => {
    // if you are creating an appliation that is "internal" the logout redirect could be useless and annoying, so, we can also just clear out the session data and start over
    localStorage.clear();
    sessionStorage.clear();
    document.location.replace("about://blank");
    // instance.logoutRedirect({
    //   postLogoutRedirectUri: "/",
    // });
  };

  const name = useMemo(() => {
    if (accounts.length > 0) {
      return accounts[0].name ?? accounts[0].username;
    }
    return "unknown";
  }, [accounts]);

  const username = useMemo(() => {
    if (accounts.length > 0) {
      return accounts[0].username;
    }
    return "unknown";
  }, [accounts]);

  // so, here, we are going to automatically log the user in, remove this entire section to handle login via the handleLogin call
  useEffect(() => {
    if (effectCalled.current) return;
    effectCalled.current = true;
    const waitForRedirectPromise = async () => {
      // the instance is initalized here, no issues
      instance.handleRedirectPromise().then((res) => {
        if (res !== null) {
          setRedirecting(false);
        } else {
          // getting null here doesn't seem to indicate the user isn't logged in
          // things behave better here if we wait a second
          setTimeout(() => {
            const currentAccounts = instance.getAllAccounts();
            if (!currentAccounts || currentAccounts.length < 1) {
              setRedirecting(true);
              setTimeout(handleLogin, 500);
            } else {
              setRedirecting(false);
            }
          }, 1000);
        }
      });
    };

    instance.initialize().then(() => {
      waitForRedirectPromise().then(() => {
        setWaiting(false);
      });
    });
  }, []);

  const Wrapper = messageWrapper ?? React.Fragment;

  return (
    <>
      {waiting || redirecting === undefined ? (
        <Wrapper>
          <em>Getting ready...</em>
        </Wrapper>
      ) : redirecting ? (
        <Wrapper>
          <em>Redirecting to Sign-in...</em>
        </Wrapper>
      ) : (
        <IdentityContext.Provider
          value={{
            handleLogin,
            handleLogout,
            getAccount,
            name,
            username,
          }}
        >
          {children}
        </IdentityContext.Provider>
      )}
    </>
  );
};
