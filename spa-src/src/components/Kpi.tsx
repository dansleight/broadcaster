import { IconDefinition } from "@fortawesome/fontawesome-common-types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useState } from "react";
import { Card, Col, Row } from "react-bootstrap";
import { useSettingsContext } from "../contexts/UseContexts";

export enum KpiColor {
  Primary,
  Success,
  Info,
  Warning,
  Danger,
  Generic,
}

type KpiProps = {
  color: KpiColor | undefined;
  title: string;
  value: string;
  progress?: number | undefined;
  icon: IconDefinition;
};

export function Kpi({ color, title, value, progress, icon }: KpiProps) {
  const { darkMode } = useSettingsContext();
  const [_color, setColor] = useState<KpiColor>(KpiColor.Generic);
  const [_progress, setProgress] = useState<number | undefined>(undefined);
  const [borderStart, setBorderStart] = useState<string>(
    "border-start-primary",
  );
  const [text, setText] = useState<string>("text-body");

  useEffect(() => {
    switch (color) {
      case KpiColor.Primary:
        setBorderStart("border-start-primary");
        setText("text-primary");
        break;
      case KpiColor.Success:
        setBorderStart("border-start-success");
        setText("text-success");
        break;
      case KpiColor.Info:
        setBorderStart("border-start-info");
        setText("text-info");
        break;
      case KpiColor.Warning:
        setBorderStart("border-start-warning");
        setText("text-warning");
        break;
      case KpiColor.Danger:
        setBorderStart("border-start-danger");
        setText("text-danger");
        break;
      default:
        setBorderStart(darkMode ? "border-start-light" : "border-start-dark");
        setText("text-body");
        break;
    }
  }, [_color, darkMode]);

  useEffect(() => {
    if (color) setColor(color);
    if (progress && progress > 0 && progress < 1) setProgress(progress * 100);
    else if (progress && progress < 0) setProgress(0);
    else if (progress && progress > 100) setProgress(100);
    else setProgress(progress);
  }, [color, title, value, progress, icon]);

  return (
    <Card className={borderStart + " shadow h-100 py-2"}>
      <Card.Body>
        <Row className="no-gutters align-items-center">
          <Col className="me-2">
            <div
              className={text + " text-xs font-weight-bold text-uppercase mb-1"}
            >
              {title}
            </div>
            {_progress ? (
              <div className="row no-gutters align-items-center">
                <div className="col-auto">
                  <div className="h5 mb-0 font-weight-bold text-body">
                    {value}
                  </div>
                </div>
                <div className="col">
                  <div className="progress progress-sm me-2">
                    <div
                      className="progress-bar bg-info"
                      role="progressbar"
                      style={{ width: `${_progress}%` }}
                      aria-valuenow={_progress}
                      aria-valuemin={0}
                      aria-valuemax={100}
                    ></div>
                  </div>
                </div>
              </div>
            ) : (
              <div className="h5 mb-0 font-weight-bold text-body">{value}</div>
            )}
          </Col>
          <Col className="col-auto">
            <FontAwesomeIcon
              icon={icon}
              size="2x"
              className="text-body text-opacity-25"
            />
          </Col>
        </Row>
      </Card.Body>
    </Card>
  );
}
/*
<div className="col me-2">
  <div className="text-xs font-weight-bold text-info text-uppercase mb-1">
    Tasks
  </div>
  <div className="row no-gutters align-items-center">
    <div className="col-auto">
      <div className="h5 mb-0 font-weight-bold text-body">
        50%
      </div>
    </div>
    <div className="col">
      <div className="progress progress-sm me-2">
        <div
          className="progress-bar bg-info"
          role="progressbar"
          style={{ width: "50%" }}
          aria-valuenow={50}
          aria-valuemin={0}
          aria-valuemax={100}
        ></div>
      </div>
    </div>
  </div>
</div>
*/
