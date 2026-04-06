import { Button, Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useSessionContext } from "../contexts/UseContexts";
import { useEffect, useState } from "react";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { webApiConfig } from "../appConfig";

export function Audio() {
  const { getApiBearer } = useSessionContext();
  const [audioLevel, setAudioLevel] = useState<number>(0);
  const [conn, setConn] = useState<HubConnection | undefined>(undefined);

  const closeConn = () => {
    if (conn) {
      conn.off("AudioLevel", setAudioLevel);
      conn.stop();
      setConn(undefined);
    }
  };

  useEffect(() => {
    const signalRHubUri = webApiConfig.origin + "/hub/audio-level";
    const newConn: HubConnection = new HubConnectionBuilder()
      .withUrl(signalRHubUri, {
        accessTokenFactory: async () => (await getApiBearer())!,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();
    setConn(newConn);
    return () => {
      closeConn();
    };
  }, []);

  useEffect(() => {
    if (conn) {
      conn
        .start()
        .then(() => {
          conn.on("AudioLevel", setAudioLevel);
        })
        .catch((e) => console.log("SignalR Connection failed: ", e));
    }
  }, [conn]);

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
