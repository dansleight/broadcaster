import { Button, Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useSessionContext } from "../contexts/UseContexts";
import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowRotateRight,
  faGear,
  faStop,
} from "@fortawesome/free-solid-svg-icons";
import { StreamStatusModel } from "../apiClient/data-contracts";

export function Manage() {
  const { api } = useSessionContext();

  const [statusWaiting, setStatusWaiting] = useState<boolean>(false);
  const [status, setStatus] = useState<string | undefined>(undefined);
  const [previewWaiting, setPreviewWaiting] = useState<boolean>(false);
  const [previewStatus, setPreviewStatus] = useState<
    StreamStatusModel | undefined
  >(undefined);
  const [liveWaiting, setLiveWaiting] = useState<boolean>(false);
  const [liveStatus, setLiveStatus] = useState<StreamStatusModel | undefined>(
    undefined,
  );
  const [stopWaiting, setStopWaiting] = useState<boolean>(false);
  const [stopStatus, setStopStatus] = useState<StreamStatusModel | undefined>(
    undefined,
  );

  // const handleStartDummyBroadcast = () => {
  //   setDummyWaiting(true);
  //   api
  //     .broadcastScheduleDummy()
  //     .then((res) => {
  //       setDummyStatus(res.data);
  //     })
  //     .finally(() => setDummyWaiting(false));
  // };

  const handleStartPreMeeting = () => {
    clearMessages();
    setPreviewWaiting(true);
    api
      .broadcastSetPlaceholder({ placeholderId: 1, audioTrackId: 0 })
      .then((res) => {
        setPreviewStatus(res.data);
      })
      .finally(() => setPreviewWaiting(false));
  };

  const handleStartSacrament = () => {
    clearMessages();
    setPreviewWaiting(true);
    api
      .broadcastSetPlaceholder({ placeholderId: 2, audioTrackId: 0 })
      .then((res) => {
        setPreviewStatus(res.data);
      })
      .finally(() => setPreviewWaiting(false));
  };

  const handleStartLiveBroadcast = () => {
    clearMessages();
    setLiveWaiting(true);
    api
      .broadcastSetLive()
      .then((res) => setLiveStatus(res.data))
      .finally(() => setLiveWaiting(false));
  };

  const handleStopAllBroadcast = () => {
    clearMessages();
    setStopWaiting(true);
    api
      .broadcastStopAll()
      .then((res) => setStopStatus(res.data))
      .finally(() => setStopWaiting(false));
  };

  const handleGetStatus = () => {
    clearMessages();
    setStatusWaiting(true);
    api
      .broadcastGetCurrentTask()
      .then((res) => setStatus(res.data))
      .finally(() => setStatusWaiting(false));
  };

  const clearMessages = () => {
    setLiveStatus(undefined);
    setPreviewStatus(undefined);
    setStopStatus(undefined);
  };

  return (
    <Container fluid>
      <Headcrumb title="Manage Streaming" />
      <Row>
        {/* <Col md={3} className="mb-2">
          <div className="mb-2">
            <Button variant="primary" onClick={handleStartDummyBroadcast}>
              <FontAwesomeIcon
                icon={dummyWaiting ? faArrowRotateRight : faGear}
                spin={dummyWaiting}
              />
              {" Schedule Dummy"}
            </Button>
          </div>
          <div>{dummyStatus && dummyStatus.status}</div>
        </Col> */}
        <Col md={3} className="mb-2">
          <h3>Wait Images</h3>
          <div className="mb-2">
            <Button
              variant="info"
              className="m-2"
              onClick={handleStartPreMeeting}
            >
              <FontAwesomeIcon
                icon={previewWaiting ? faArrowRotateRight : faGear}
                spin={previewWaiting}
              />
              {" Set Pre-Meeting Placeholder"}
            </Button>
            <Button
              variant="info"
              className="m-2"
              onClick={handleStartSacrament}
            >
              <FontAwesomeIcon
                icon={previewWaiting ? faArrowRotateRight : faGear}
                spin={previewWaiting}
              />
              {" Set Sacrament Placeholder"}
            </Button>
          </div>
          <div>{previewStatus && previewStatus.status}</div>
        </Col>
        <Col md={3} className="mb-2">
          <h3>Live Video</h3>
          <div className="mb-2">
            <Button
              variant="info"
              className="m-2"
              onClick={handleStartLiveBroadcast}
            >
              <FontAwesomeIcon
                icon={liveWaiting ? faArrowRotateRight : faGear}
                spin={liveWaiting}
              />
              {" Set Live"}
            </Button>
          </div>
          <div>{liveStatus && liveStatus.status}</div>
        </Col>
        <Col md={3} className="mb-2">
          <h3>Stop Broadcast</h3>
          <div>
            <Button
              variant="danger"
              className="m-2"
              onClick={handleStopAllBroadcast}
            >
              <FontAwesomeIcon
                icon={stopWaiting ? faArrowRotateRight : faStop}
                spin={stopWaiting}
              />
              {" Stop All"}
            </Button>
          </div>
          <div>{stopStatus && stopStatus.status}</div>
        </Col>
        <Col md={3} className="mb-2">
          <h3>Get Status</h3>
          <div>
            <Button variant="info" className="m-2" onClick={handleGetStatus}>
              <FontAwesomeIcon
                icon={statusWaiting ? faArrowRotateRight : faStop}
                spin={statusWaiting}
              />
              {" Get Status"}
            </Button>
          </div>
          <div>{status}</div>
        </Col>
      </Row>
    </Container>
  );
}
