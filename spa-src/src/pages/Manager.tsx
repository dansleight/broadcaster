import { Button, Card, Col, Container, Form, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useSessionContext } from "../contexts/UseContexts";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {
  PlaceholderObject,
  StreamStatusModel,
} from "../apiClient/data-contracts";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { webApiConfig } from "../appConfig";

export function Manager() {
  const { api, getApiBearer } = useSessionContext();
  const [stateChanging, setStateChanging] = useState<boolean>(false);
  const [status, setStatus] = useState<StreamStatusModel | undefined>(
    undefined,
  );
  const [unit, setUnit] = useState<string | undefined>(undefined);
  const [placeholderId, setPlaceholderId] = useState<number | undefined>(
    undefined,
  );
  const [audioTrackId, setAudioTrackId] = useState<number | undefined>(
    undefined,
  );

  const [placeholders, setPlaceholders] = useState<PlaceholderObject[]>([]);

  const connRef = useRef<HubConnection | undefined>(undefined);

  const handleSetPlaceholder = () => {
    setStateChanging(true);
    api
      .broadcastSetPlaceholder({
        placeholderId: placeholderId!,
        audioTrackId: 0,
      })
      .then((res) => {
        console.log(res);
      })
      .finally(() => setStateChanging(false));
  };

  const handleGoLive = () => {
    setStateChanging(true);
    api
      .broadcastSetLive()
      .then((res) => {
        console.log(res);
      })
      .finally(() => setStateChanging(false));
  };

  const handleStopAll = () => {
    setStateChanging(true);
    api
      .broadcastStopAll()
      .then((res) => {
        console.log(res);
      })
      .finally(() => setStateChanging(false));
  };

  const closeConn = useCallback(() => {
    const conn = connRef.current;
    if (conn) {
      conn.off("Status", setStatus);
      conn.stop().catch(console.error);
      connRef.current = undefined;
    }
  }, [setStatus]);

  useEffect(() => {
    api.infoGetPlaceholders().then((res) => {
      setPlaceholders(res.data);
    });

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
        newConn.on("Status", setStatus);
      })
      .catch((e) => console.error("SignalR Connection failed: ", e));

    return () => {
      closeConn();
    };
  }, []);

  const units = useMemo(() => {
    if (!placeholders || placeholders.length === 0) {
      return undefined;
    }

    const uniqueUnits = [...new Set(placeholders.map((x) => x.unit))];

    return uniqueUnits.sort((a, b) => {
      // null comes first
      if (a === null) return -1;
      if (b === null) return 1;

      // Both are strings → sort alphabetically (case-insensitive)
      return (a ?? "").localeCompare(b ?? "");
    });
  }, [placeholders]);

  const images = useMemo(() => {
    return placeholders.filter((p) => p.unit == unit);
  }, [placeholders, unit]);

  useEffect(() => {
    if (!images || images.length === 0) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setPlaceholderId(undefined);
      return;
    }

    const currentIsValid = images.some(
      (p) => p.placeholderId === placeholderId,
    );

    if (!currentIsValid) {
      setPlaceholderId(images[0].placeholderId);
    }
  }, [images]);

  return (
    <Container fluid>
      <Headcrumb title="Broadcast Manager" />
      <Row>
        <Col className="text-center">
          <div
            style={{ maxWidth: "640px" }}
            className="m-2 border border-primary"
          >
            {status && (
              <>
                <h1>{status.status}</h1>
              </>
            )}
          </div>
        </Col>
      </Row>
      <Row>
        <Col>
          <Card>
            <Card.Header className="d-flex justify-content-between">
              <Card.Title>Placeholder</Card.Title>
              <Button
                size="sm"
                onClick={handleSetPlaceholder}
                variant="primary"
                disabled={stateChanging}
              >
                Set Placeholder
              </Button>
            </Card.Header>
            <Card.Body>
              <Form.Group className="mb-2">
                <Form.Label>Unit</Form.Label>
                {units && units.length > 1 ? (
                  <>
                    <Form.Select
                      value={unit}
                      onChange={(e) =>
                        setUnit(
                          e.target.value != "" ? e.target.value : undefined,
                        )
                      }
                      aria-label="Select Unit"
                    >
                      {units.map((u, i) => (
                        <option key={i} value={u ?? ""}>
                          {u ?? "- generic -"}
                        </option>
                      ))}
                    </Form.Select>
                  </>
                ) : units && units.length == 1 ? (
                  <>
                    <Form.Control disabled value={unit} />
                  </>
                ) : (
                  <em>No available units.</em>
                )}
              </Form.Group>
              <Form.Group className="mb-2">
                <Form.Label>Image</Form.Label>
                <Form.Select
                  value={placeholderId}
                  onChange={(e) => setPlaceholderId(+e.target.value)}
                  aria-label="Select Placeholder Image"
                >
                  {images.map((p, i) => (
                    <option key={i} value={p.placeholderId}>
                      {p.name}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-2">
                <Form.Label>Music</Form.Label>
                <Form.Select aria-label="Select Placeholder Music">
                  <option value={1}>Pre-Meeting</option>
                  <option value={2}>Ordinance</option>
                  <option value={3}>Post-Meeting</option>
                </Form.Select>
              </Form.Group>
            </Card.Body>
          </Card>
        </Col>
      </Row>
      <Row>
        <Col>
          <Card>
            <Card.Body>
              <Button variant="success" className="me-2" onClick={handleGoLive}>
                Go Live
              </Button>
              <Button variant="danger" className="me-2" onClick={handleStopAll}>
                Stop
              </Button>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}
