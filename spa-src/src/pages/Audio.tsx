import { Button, Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useSessionContext } from "../contexts/UseContexts";
import { useCallback, useEffect, useRef, useState } from "react";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { webApiConfig } from "../appConfig";

export function Audio() {
  const { getApiBearer } = useSessionContext();
  const [audioLevel, setAudioLevel] = useState<number>(0);
  const connRef = useRef<HubConnection | undefined>(undefined);

  const closeConn = useCallback(() => {
    const conn = connRef.current;
    if (conn) {
      conn.off("AudioLevel", setAudioLevel);
      conn.stop().catch(console.error);
      connRef.current = undefined;
    }
  }, [setAudioLevel]);

  useEffect(() => {
    const signalRHubUri = webApiConfig.origin + "/hub/audio-level";
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
        newConn.on("AudioLevel", setAudioLevel);
      })
      .catch((e) => console.error("SignalR Connection failed: ", e));

    return () => {
      closeConn();
    };
  }, []);

  return (
    <Container fluid>
      <Headcrumb title="Audio Test" />
      <Row className="mb-2">
        <Col>
          <h2>Audio Level Test</h2>

          <div className="progress my-3" role="progressbar">
            <div
              className="progress-bar"
              style={{ width: `${audioLevel * 100}%` }}
            />
          </div>
          <input type="text" value={audioLevel} disabled />
        </Col>
      </Row>
      <Row className="mb-2">
        <Col>
          <Button onClick={closeConn}>Close</Button>
        </Col>
      </Row>
    </Container>
  );
}
