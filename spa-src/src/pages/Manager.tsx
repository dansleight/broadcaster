import { Button, Card, Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useSessionContext } from "../contexts/UseContexts";
import { useMemo, useState } from "react";
import classNames from "classnames";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBreadSlice,
  faDoorClosed,
  faImage,
  faPlay,
  faSpinner,
  faStop,
  IconDefinition,
} from "@fortawesome/free-solid-svg-icons";
import {
  PlaceholderType,
  SetPlaceholderModel,
  StreamState,
} from "../apiClient/data-contracts";

enum MeetingStatus {
  Pre = "Pre",
  Live = "Live",
  Sacrament = "Sacrament",
  Post = "Post",
  Stopped = "Stopped",
}

export function Manager() {
  const { api, streamState } = useSessionContext();
  const [stateChanging, setStateChanging] = useState<boolean>(false);

  const setPlaceholder = (placeholderType: PlaceholderType) => {
    setStateChanging(true);
    api
      .broadcastSetPlaceholder({
        placeholderType: placeholderType,
      } as SetPlaceholderModel)
      .then((res) => console.log(res))
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

  const status: MeetingStatus = useMemo(() => {
    if (streamState.streamState == StreamState.Live) {
      console.log(
        `streamState.streamState is ${streamState.streamState}, setting to MeetingStatus.Live`,
      );
      return MeetingStatus.Live;
    }
    if (streamState.placeholderImage?.includes("pre") ?? false) {
      console.log(
        `streamState.streamState is ${streamState.streamState}, streamstate.placeholderImage is ${streamState.placeholderImage}. Setting to MeetingStatus.Pre`,
      );
      return MeetingStatus.Pre;
    }
    if (streamState.placeholderImage?.includes("sacrament") ?? false) {
      console.log(
        `streamState.streamState is ${streamState.streamState}, streamstate.placeholderImage is ${streamState.placeholderImage}. Setting to MeetingStatus.Sacrament`,
      );
      return MeetingStatus.Sacrament;
    }

    if (streamState.placeholderImage?.includes("post") ?? false) {
      console.log(
        `streamState.streamState is ${streamState.streamState}, streamstate.placeholderImage is ${streamState.placeholderImage}. Setting to MeetingStatus.Post`,
      );
      return MeetingStatus.Post;
    }
    return MeetingStatus.Stopped;
  }, [streamState]);

  return (
    <Container fluid>
      <Headcrumb title="Broadcast Manager" />
      {/* <Row>
        <Col className="text-center">
          <div className="m-2 border border-primary">
            {status ? (
              <>
                <h1>{status.toString() ?? "unknown"}</h1>
              </>
            ) : (
              <h1>No Status</h1>
            )}
          </div>
        </Col>
      </Row> */}
      <Row>
        <Col>
          <Row>
            <Col>
              <StopButton
                status={status}
                handleStopAll={handleStopAll}
                stateChanging={stateChanging}
              />
            </Col>
          </Row>
        </Col>
      </Row>
      <Row>
        <Col xs={12} sm={6} md={4} lg={3}>
          <BCard
            active={status == MeetingStatus.Pre}
            label="Pre-Meeting"
            icon={faImage}
            action={() => setPlaceholder(PlaceholderType.Pre)}
            src="/api/image/pre.jpg"
            stateChanging={stateChanging}
          />
        </Col>
        <Col xs={12} sm={6} md={4} lg={3}>
          <BCard
            active={status == MeetingStatus.Live}
            label="Go Live"
            icon={faPlay}
            action={handleGoLive}
            buttonLabel="Go Live"
            src="/api/image/stream.jpg"
            stateChanging={stateChanging}
          />
        </Col>
        <Col xs={12} sm={6} md={4} lg={3}>
          <BCard
            active={status == MeetingStatus.Sacrament}
            label="Sacrament"
            icon={faBreadSlice}
            action={() => setPlaceholder(PlaceholderType.Sacrament)}
            src="/api/image/sacrament.jpg"
            stateChanging={stateChanging}
          />
        </Col>
        <Col xs={12} sm={6} md={4} lg={3}>
          <BCard
            active={status == MeetingStatus.Post}
            label="Post-Meeting"
            icon={faDoorClosed}
            action={() => setPlaceholder(PlaceholderType.Post)}
            src="/api/image/post.jpg"
            stateChanging={stateChanging}
          />
        </Col>
      </Row>
      <Row>
        <Col>
          <StopButton
            status={status}
            handleStopAll={handleStopAll}
            stateChanging={stateChanging}
          />
        </Col>
      </Row>
    </Container>
  );
}

type StopButtonArgs = {
  status: MeetingStatus;
  handleStopAll: () => void;
  stateChanging: boolean;
};

const StopButton = ({
  status,
  handleStopAll,
  stateChanging,
}: StopButtonArgs) => {
  return (
    <Card className={status == MeetingStatus.Stopped ? "mb-2" : "mb-2"}>
      <Card.Body className="d-grid gap-2">
        <Button
          variant={status == MeetingStatus.Stopped ? "secondary" : "danger"}
          className="me-2"
          onClick={handleStopAll}
          size="lg"
          disabled={status == MeetingStatus.Stopped}
        >
          {status == MeetingStatus.Stopped && stateChanging ? (
            <FontAwesomeIcon icon={faSpinner} spin />
          ) : (
            <FontAwesomeIcon icon={faStop} size="lg" />
          )}
          {status == MeetingStatus.Stopped ? "Stopped" : "Stop"}
        </Button>
      </Card.Body>
    </Card>
  );
};

type BCardArgs = {
  active: boolean;
  label: string;
  icon: IconDefinition;
  action: () => void;
  buttonLabel?: string;
  src: string;
  stateChanging: boolean;
};

const BCard = ({
  active,
  label,
  icon,
  action,
  src,
  stateChanging,
}: BCardArgs) => {
  return (
    <Card
      className={classNames("mb-2", {
        "card-secondary": !active,
        "card-primary": active,
      })}
      onClick={action}
    >
      <Card.Body
        className={classNames("", {
          "bg-success text-bg-success": active,
          "bg-secondary text-bg-secondary": !active,
        })}
      >
        <div className="d-flex justify-content-between">
          <h5>{label}</h5>
          <span>
            {active && stateChanging ? (
              <FontAwesomeIcon icon={faSpinner} spin />
            ) : (
              <FontAwesomeIcon icon={icon} beat={active} size="lg" />
            )}
          </span>
        </div>
        <img
          src={src}
          alt={label}
          style={
            active
              ? { width: "100%", border: "1px solid #ccc" }
              : {
                  width: "100%",
                  border: "1px solid #ccc",
                  filter: "brightness(1.35) saturate(0.7)", // Lighten + slight desaturation
                  opacity: 0.65, // Optional subtle fade
                }
          }
        />
      </Card.Body>
    </Card>
  );
};
