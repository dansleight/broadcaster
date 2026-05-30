import React, {
  ComponentType,
  ReactNode,
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { Api } from "../apiClient/Api";
import { webApiConfig } from "../appConfig";
import {
  SessionContext,
  useIdentityContext,
  useSettingsContext,
} from "./UseContexts";
import { Button, Modal } from "react-bootstrap";
import classNames from "classnames";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { FullStreamState, StreamState } from "../apiClient/data-contracts";

type SessionProviderProps = {
  children: ReactNode;
  messageWrapper?: ComponentType<{ children: ReactNode }>;
};

export function SessionProvider({
  children,
  messageWrapper,
}: SessionProviderProps) {
  const { globalSettings } = useSettingsContext();
  const [showApiError, setShowApiError] = useState<boolean>(false);
  const [apiErrorMessage, setApiErrorMessage] = useState<string>("");
  const [apiErrorDetails, setApiErrorDetails] = useState<string | undefined>(
    undefined,
  );
  const [showErrorDetails, setShowErrorDetails] = useState<boolean>(false);
  const [severeError, setSevereError] = useState<boolean>(false);
  const { authToken } = useIdentityContext();
  const connRef = useRef<HubConnection | undefined>(undefined);
  const [streamState, setStreamState] = useState<FullStreamState>({
    streamState: StreamState.Idle,
  } as FullStreamState);

  const handleErrorModalClose = () => {
    setShowApiError(false);
    setApiErrorMessage("");
    setApiErrorDetails(undefined);
    setShowErrorDetails(false);
    setSevereError(false);
  };

  const handleApiError = (error: any) => {
    console.error("handleApiError error:", error);
    if (error.status && typeof error.status == "number") {
      // going to assume that we have an HttpResponse
      if (error.status == 400) {
        if (error.error.userMessage)
          setApiErrorMessage(error.error.userMessage);
        else if (error.error.message) setApiErrorMessage(error.error.message);
        else if (error.error.detail) setApiErrorMessage(error.error.detail);
        else setApiErrorMessage("Server returned BadRequest");
      } else if (error.status == 404) {
        setApiErrorMessage(
          "Server returned status code 404: Not Found. The record request does not exist.",
        );
      } else if (error.status == 500) {
        setSevereError(true);
        if (error.error.detail) setApiErrorMessage(error.error.detail);
        else if (error.error.Message) {
          setApiErrorMessage(error.error.Message);
          if (error.error.StackTraceString)
            setApiErrorDetails(error.error.StackTraceString);
        } else {
          setApiErrorMessage(
            "Server reported a status code 500: Internal Server Error.",
          );
        }
      } else {
        setApiErrorMessage(
          `Server returned a status code ${error.status}: ${error.statusText}`,
        );
      }
    } else {
      setApiErrorMessage("unspecified error, check logs");
    }
    setShowApiError(true);
  };

  const getApiBearer = async () => {
    // await instance.initialize();
    // const request: any = {
    //   scopes: [globalSettings.msalSettings!.apiScope],
    //   accounts: getAccount()
    // };
    // const authenticationResult = await instance
    //   .acquireTokenSilent(request as SilentRequest)
    //   .catch((e: any) => {
    //     console.error(e);
    //   });
    // if (authenticationResult) return authenticationResult!.accessToken;
    return undefined;
  };

  const api: Api | undefined = useMemo(() => {
    if (!globalSettings) return undefined;

    return new Api({
      baseUrl: webApiConfig.origin,
      securityWorker: async () => {
        if (!authToken) {
          throw new Error("No auth token available");
        }
        return {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        };
      },
      unhandledErrorHandler: handleApiError,
    });
  }, [globalSettings, authToken]);

  const closeConn = useCallback(() => {
    const conn = connRef.current;
    if (conn) {
      conn.off("StreamState", setStreamState);
      conn.stop().catch(console.error);
      connRef.current = undefined;
    }
  }, [setStreamState]);

  useEffect(() => {
    api?.broadcastGetStreamState().then((res) => setStreamState(res.data));

    const signalRHubUri = webApiConfig.origin + "/hub/status";
    const newConn: HubConnection = new HubConnectionBuilder()
      .withUrl(signalRHubUri, {
        accessTokenFactory: async () => (await getApiBearer())!,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connRef.current = newConn;

    newConn
      .start()
      .then(() => {
        newConn.on("StreamState", setStreamState);
      })
      .catch((e) =>
        console.error("SignalR Connection in SessionContext failed: ", e),
      );

    return () => {
      closeConn();
    };
  }, []);

  const Wrapper = messageWrapper ?? React.Fragment;

  return (
    <>
      {api === undefined ? (
        <Wrapper>
          <em>Getting ready...</em>
        </Wrapper>
      ) : (
        <SessionContext.Provider value={{ api, getApiBearer, streamState }}>
          <Modal
            show={showApiError}
            onHide={handleErrorModalClose}
            size="lg"
            backdrop="static"
          >
            <Modal.Header
              className={classNames("", {
                "bg-danger text-bg-danger": severeError,
                "bg-warning text-bg-warning": !severeError,
              })}
            >
              <Modal.Title>Unhandled Error</Modal.Title>
            </Modal.Header>
            <Modal.Body>
              <p>
                An unhandled error has occured while communicating with the API
                server.
              </p>
              <p className="text-danger">{apiErrorMessage}</p>
              {apiErrorDetails && (
                <>
                  {showErrorDetails ? (
                    <>
                      <a
                        href="#"
                        className="muted"
                        onClick={(e) => {
                          e.preventDefault();
                          setShowErrorDetails(false);
                        }}
                      >
                        Hide Details
                      </a>
                      <div className="border border-tertiary mt-2 p-1">
                        <pre>{apiErrorDetails}</pre>
                      </div>
                    </>
                  ) : (
                    <a
                      href="#"
                      className="muted"
                      onClick={(e) => {
                        e.preventDefault();
                        setShowErrorDetails(true);
                      }}
                    >
                      Show Details
                    </a>
                  )}
                </>
              )}
            </Modal.Body>
            <Modal.Footer>
              <Button variant="secondary" onClick={handleErrorModalClose}>
                Close
              </Button>
            </Modal.Footer>
          </Modal>
          {children}
        </SessionContext.Provider>
      )}
    </>
  );
}
