import {
  faCopy,
  faList,
  faSignOut,
  faUser,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Dropdown } from "bootstrap";
import { useEffect, useRef } from "react";
import {
  useIdentityContext,
  useSessionContext,
} from "../../contexts/UseContexts";
import { Link } from "react-router-dom";
import { hashCode } from "../../models/Utilities";

export const UserInfo = () => {
  const { getApiBearer } = useSessionContext();
  const { name, username, handleLogout } = useIdentityContext();
  const dropdownCreated = useRef<boolean>(false);

  const copyFullBearerToClipboard = async () => {
    copyBearerToClipboard(true);
  };

  const copyBearerToClipboard = async (full: boolean = false) => {
    const bearer: string | undefined = await getApiBearer();
    if (bearer && full) {
      navigator.clipboard.writeText(`Bearer ${bearer}`);
    } else if (bearer) {
      navigator.clipboard.writeText(bearer);
    } else {
      navigator.clipboard.writeText(
        "Something went wrong and the bearer wasn't retrieved."
      );
    }
  };

  useEffect(() => {
    if (dropdownCreated.current) return;
    dropdownCreated.current = true;
    Dropdown.getOrCreateInstance(document.querySelector("#userinfo-dropdown")!);
  }, []);

  return (
    <li id="userinfo-dropdown" className="nav-item dropdown no-arrow">
      <a
        className="nav-link dropdown-toggle"
        href="#"
        id="userDropdown"
        role="button"
        data-bs-toggle="dropdown"
        aria-haspopup="true"
        aria-expanded="false"
      >
        <span className="me-2 d-none d-lg-inline small">{name}</span>
        <img
          className="img-profile rounded-circle"
          src={"https://gravatar.com/avatar/" + hashCode(username) + "?d=retro"}
        />
      </a>
      {/* Dropdown - User Information */}
      <div
        className="dropdown-menu dropdown-menu-end shadow animated--grow-in"
        aria-labelledby="userDropdown"
      >
        <h6 className="dropdown-header d-lg-none">{name}</h6>
        <h6 className="dropdown-header">{username}</h6>
        <Link className="dropdown-item" to="/user-profile">
          <FontAwesomeIcon
            icon={faUser}
            size="sm"
            fixedWidth
            className="me-2 text-gray-400"
          />
          Profile
        </Link>
        <Link className="dropdown-item" to="/user-settings">
          <FontAwesomeIcon
            icon={faUser}
            size="sm"
            fixedWidth
            className="me-2 text-gray-400"
          />
          Settings
        </Link>
        <Link className="dropdown-item" to="/activity-log">
          <FontAwesomeIcon
            icon={faList}
            size="sm"
            fixedWidth
            className="me-2 text-gray-400"
          />
          Activity Log
        </Link>
        <div className="dropdown-divider"></div>
        <a className="dropdown-item" onClick={() => copyBearerToClipboard()}>
          <FontAwesomeIcon
            icon={faCopy}
            size="sm"
            fixedWidth
            className="me-2 text-gray-400"
          />
          Bearer to Clipboard
        </a>
        <a
          className="dropdown-item"
          onClick={() => copyBearerToClipboard(true)}
        >
          <FontAwesomeIcon
            icon={faCopy}
            size="sm"
            fixedWidth
            className="me-2 text-gray-400"
          />
          Full Bearer to Clipboard
        </a>
        <a
          className="dropdown-item"
          onClick={handleLogout}
          data-toggle="modal"
          data-target="#logoutModal"
        >
          <FontAwesomeIcon
            icon={faSignOut}
            size="sm"
            fixedWidth
            className="me-2 text-gray-400"
          />
          Logout
        </a>
      </div>
    </li>
  );
};
