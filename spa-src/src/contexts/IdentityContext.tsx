import React, {
  ComponentType,
  ReactNode,
  useEffect,
  useRef,
  useState,
} from "react";
import { useGoogleLogin } from "@react-oauth/google";
import { IdentityContext, useSettingsContext } from "./UseContexts";
import { UserObject } from "../apiClient/data-contracts";
import { Button } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faG } from "@fortawesome/free-solid-svg-icons";
import { webApiConfig } from "../appConfig";

type IdentityProviderProps = {
  children: ReactNode;
  messageWrapper?: ComponentType<{ waiting?: boolean; children: ReactNode }>;
};

export const IdentityProvider = ({
  children,
  messageWrapper,
}: IdentityProviderProps) => {
  const { noAuthApi } = useSettingsContext();
  const [waiting, setWaiting] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const effectCalled = useRef(false);
  const [authToken, setAuthToken] = useState<string | null>(null);
  const [user, setUser] = useState<UserObject | undefined>(undefined);

  const handleLoginSuccess = (codeResponse: { code: string }) => {
    try {
      noAuthApi
        .googleAuthCallback({ code: codeResponse.code })
        .then((res) => {
          setAuthToken(res.data.accessToken);
          console.log("✅ Logged in - token stored");
          setUser(res.data.user);
        })
        .catch((err) => {
          throw new Error(`Backend error: ${err.error}`);
        })
        .finally(() => {});
    } catch (err: any) {
      console.error("❌ Login failed:", err);
      setError(err);
    }
  };

  console.info("webApiConfig.redirectUri: ", webApiConfig.redirectUri);

  const login = useGoogleLogin({
    flow: "auth-code",
    scope: "openid email profile https://www.googleapis.com/auth/youtube",
    ux_mode: "popup", // "popup" is usually smoother; change to "redirect" if you prefer
    onSuccess: handleLoginSuccess, // Pass the function directly
    redirect_uri: webApiConfig.redirectUri,
    onError: (error) => {
      console.error("Google login error:", error);
    },
  });

  useEffect(() => {
    if (effectCalled.current) return;
    effectCalled.current = true;

    // going to see if we can just get a jwt token based on the http-only cookie
    console.info(
      "attempting to get a jwt token based on existing refresh token",
    );
    noAuthApi
      .googleAuthRefresh()
      .then((res) => {
        setAuthToken(res.data.accessToken);
        setUser(res.data.user);
      })
      .unauthorized(() => {
        console.info(
          "failed to get a jwt token based on existing refresh token",
        );
        // do nothing, we'll fall into a login with google scenario.
      })
      .finally(() => {
        console.info("hit the finally block, turning off waiting...");
        setWaiting(false);
      });
  }, []);

  const Wrapper = messageWrapper ?? React.Fragment;

  return (
    <>
      {waiting ? (
        <Wrapper>
          <em>Getting ready...</em>
        </Wrapper>
      ) : error ? (
        <Wrapper waiting={false}>
          <em>{error}</em>
        </Wrapper>
      ) : authToken === null || user === undefined ? (
        <Wrapper waiting={false}>
          <p>
            <em>You are not logged in.</em>
          </p>
          <Button variant="danger" onClick={login}>
            <FontAwesomeIcon icon={faG} />
            {" Log In with Google"}
          </Button>
        </Wrapper>
      ) : (
        <IdentityContext.Provider
          value={{
            handleLogout: () => {
              localStorage.clear();
              sessionStorage.clear();
              document.location.replace("about://blank");
            },
            user: user,
            authToken: authToken,
            setAuthToken: setAuthToken,
          }}
        >
          {children}
        </IdentityContext.Provider>
      )}
    </>
  );
};
