import { Button, Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useEffect, useRef, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faStop,
  faVolumeUp,
  faVolumeMute,
  faVideoCamera,
  faInfoCircle,
} from "@fortawesome/free-solid-svg-icons";

const BASE_URL = "http://localhost:5034";
const VIDEO_STREAM_URL = `${BASE_URL}/video/preview.mjpeg`;
const VIDEO_STOP_URL = `${BASE_URL}/video/stop`;
const AUDIO_STREAM_URL = `${BASE_URL}/audio/preview.mp3`;
const AUDIO_STOP_URL = `${BASE_URL}/audio/stop`;

// Module-level flag — survives strict mode's mount/unmount/remount cycle
// and is reset only when the module is reloaded (i.e. a full page refresh)
let videoActive = false;
let audioActive = false;

export function Preview() {
  const imgRef = useRef<HTMLImageElement>(null);
  const audioRef = useRef<HTMLAudioElement>(null);
  const [streamActiveState, setStreamActiveState] = useState<boolean>(false);
  const [audioActiveState, setAudioActiveState] = useState<boolean>(false);

  const startVideoStream = () => {
    if (videoActive) return;
    videoActive = true;
    setStreamActiveState(videoActive);
    if (imgRef.current) {
      imgRef.current.src = `${VIDEO_STREAM_URL}?t=${Date.now()}`;
    }
  };

  const stopVideoStream = () => {
    if (!videoActive) return;
    videoActive = false;
    setStreamActiveState(videoActive);
    if (imgRef.current) imgRef.current.src = "/images/stopped.jpg";
    navigator.sendBeacon(VIDEO_STOP_URL);
  };

  const toggleVideoStream = () => {
    if (videoActive) stopVideoStream();
    else {
      if (imgRef.current) imgRef.current.src = "/images/starting.jpg";
      startVideoStream();
    }
  };

  const startAudioStream = () => {
    if (audioActive) return;
    audioActive = true;
    setAudioActiveState(audioActive);
    if (audioRef.current) {
      audioRef.current.src = `${AUDIO_STREAM_URL}?t=${Date.now()}`;
      audioRef.current
        .play()
        .catch((e) => console.error("Audio play failed: ", e));
    }
  };

  const stopAudioStream = () => {
    if (!audioActive) return;
    audioActive = false;
    setAudioActiveState(audioActive);
    if (audioRef.current) {
      audioRef.current.pause();
      audioRef.current.currentTime = 0;
    }
    navigator.sendBeacon(AUDIO_STOP_URL);
  };

  const toggleAudioStream = () => {
    if (audioActive) stopAudioStream();
    else startAudioStream();
  };

  const stopBothStreams = () => {
    try {
      stopVideoStream();
    } catch {}
    try {
      stopAudioStream();
    } catch {}
  };

  useEffect(() => {
    const handleUnload = () => stopBothStreams();
    window.addEventListener("beforeunload", handleUnload);

    return () => {
      window.removeEventListener("beforeunload", handleUnload);
      stopBothStreams();
    };
  }, []);

  return (
    <Container fluid>
      <Headcrumb title="Stream Preview" />
      <Row>
        <Col md={6} className="d-flex mb-2">
          <div className="py-2 px-3 border border-secondary bg-secondary text-bg-secondary">
            <FontAwesomeIcon
              icon={streamActiveState ? faVideoCamera : faStop}
            />
          </div>
          <div className="flex-grow-1 d-grid gap-2">
            <Button
              variant={streamActiveState ? "danger" : "success"}
              onClick={toggleVideoStream}
            >
              <FontAwesomeIcon
                icon={streamActiveState ? faStop : faVideoCamera}
                className="me-1"
              />
              {streamActiveState ? "Stop Video" : " Start Video"}
            </Button>
          </div>
        </Col>
        <Col md={6} className="d-flex mb-2">
          <div className="py-2 px-3 border border-secondary bg-secondary text-bg-secondary">
            <FontAwesomeIcon
              icon={audioActiveState ? faVolumeMute : faVolumeMute}
            />
          </div>
          <div className="flex-grow-1 d-grid gap-2">
            <Button
              variant={audioActiveState ? "danger" : "success"}
              onClick={toggleAudioStream}
            >
              <FontAwesomeIcon
                icon={audioActive ? faVolumeMute : faVolumeUp}
                className="me-2"
              />
              {audioActive ? "Stop Audio" : "Start Audio"}
            </Button>
          </div>
        </Col>
      </Row>
      <Row>
        <Col className="mb-2 text-center">
          <img
            ref={imgRef}
            src="/images/stopped.jpg"
            style={{ width: "853px", maxWidth: "100%" }}
            className="border border-secondary"
            alt="Live Preview"
            onError={(e) => console.error("Preview image error:", e)}
            onClick={toggleVideoStream}
          />
        </Col>
      </Row>
      <Row>
        <Col className="mb-2">
          <div className="alert alert-warning">
            <h5>
              <FontAwesomeIcon icon={faInfoCircle} /> NOTES:
            </h5>
            <ul>
              <li>There may be a few second delay in starting the streams.</li>
              <li>
                The streams will likely not be in sync, that is not an
                indication of a problem.
              </li>
            </ul>
          </div>
        </Col>
      </Row>

      {/* Hidden audio player */}
      <audio ref={audioRef} controls={false} />
    </Container>
  );
}
