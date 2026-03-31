import {
  faBell,
  faDonate,
  faExclamationTriangle,
  faFileAlt,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Dropdown } from "bootstrap";
import { useEffect, useRef } from "react";

type AlertsProps = {
  className?: string;
};

export const Alerts = ({ className }: AlertsProps) => {
  const dropdownCreated = useRef<boolean>(false);

  useEffect(() => {
    if (dropdownCreated.current) return;
    dropdownCreated.current = true;
    Dropdown.getOrCreateInstance(document.querySelector("#alerts-dropdown")!);
  }, []);

  return (
    <li
      id="alerts-dropdown"
      className={"nav-item dropdown no-arrow mx-1 " + (className ?? "")}
    >
      <a
        className="nav-link dropdown-toggle"
        href="#"
        id="alertsDropdown"
        role="button"
        data-bs-toggle="dropdown"
        aria-haspopup="true"
        aria-expanded="false"
      >
        <FontAwesomeIcon icon={faBell} fixedWidth />
        {/* Counter - Alerts */}
        <span className="badge badge-counter text-info">3+</span>
      </a>
      {/* Dropdown - Alerts */}
      <div
        className="dropdown-list dropdown-menu dropdown-menu-end shadow animated--grow-in"
        aria-labelledby="alertsDropdown"
      >
        <h6 className="dropdown-header">Alerts Center</h6>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="me-3">
            <div className="icon-circle bg-primary">
              <FontAwesomeIcon icon={faFileAlt} className="text-white" />
            </div>
          </div>
          <div>
            <div className="small text-gray-500">December 12, 2019</div>
            <span className="font-weight-bold">
              A new monthly report is ready to download!
            </span>
          </div>
        </a>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="me-3">
            <div className="icon-circle bg-success">
              <FontAwesomeIcon icon={faDonate} className="text-white" />
            </div>
          </div>
          <div>
            <div className="small text-gray-500">December 7, 2019</div>
            $290.29 has been deposited into your account!
          </div>
        </a>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="me-3">
            <div className="icon-circle bg-warning">
              <FontAwesomeIcon
                icon={faExclamationTriangle}
                className="text-white"
              />
            </div>
          </div>
          <div>
            <div className="small text-gray-500">December 2, 2019</div>
            Spending Alert: We've noticed unusually high spending for your
            account.
          </div>
        </a>
        <a className="dropdown-item text-center small text-gray-500" href="#">
          Show All Alerts
        </a>
      </div>
    </li>
  );
};
