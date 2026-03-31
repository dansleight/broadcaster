import { faGears } from "@fortawesome/free-solid-svg-icons";
import { Col, Row } from "react-bootstrap";
import "simplebar-react/dist/simplebar.min.css";
import { Headcrumb } from "../components/Headcrumb";
import { Kpi, KpiColor } from "../components/Kpi";
import { useNavigate } from "react-router-dom";

export function Dashboard() {
  const navigate = useNavigate();

  return (
    <div id="dashboard-page" className="container-fluid">
      <Headcrumb title="Dashboard" />

      <Row>
        <Col md={6} lg={4} className="mb-4">
          <div
            style={{ cursor: "pointer" }}
            onClick={() => navigate("/manage")}
          >
            <Kpi
              color={KpiColor.Generic}
              title="Manage"
              value="Start/Stop Broadcast Elements"
              icon={faGears}
            />
          </div>
        </Col>
      </Row>
    </div>
  );
}
