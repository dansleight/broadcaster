import { faCopy, faSignOut, faUser } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Dropdown } from "bootstrap";
import { useEffect, useRef } from "react";
import {
  useIdentityContext,
  useSessionContext,
} from "../../contexts/UseContexts";
import { Link } from "react-router-dom";
import { hashCode } from "../../models/Utilities";
import { InRole } from "../../Utils/UserHelper";

export const UserInfo = () => {
  const { getApiBearer } = useSessionContext();
  const { user, handleLogout } = useIdentityContext();
  const dropdownCreated = useRef<boolean>(false);

  const copyBearerToClipboard = async () => {
    const bearer: string | undefined = await getApiBearer();
    if (bearer) {
      navigator.clipboard.writeText(bearer);
    } else {
      navigator.clipboard.writeText(
        "Something went wrong and the bearer wasn't retrieved.",
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
        {user.picture ? (
          <img className="img-profile rounded-circle" src={user.picture} />
        ) : (
          <img
            className="img-profile rounded-circle"
            src={
              "https://gravatar.com/avatar/" + hashCode(user.email) + "?d=retro"
            }
          />
        )}
      </a>
      {/* Dropdown - User Information */}
      <div
        className="dropdown-menu dropdown-menu-end shadow animated--grow-in"
        aria-labelledby="userDropdown"
      >
        <h6 className="dropdown-header d-lg-none">{user.name}</h6>
        <h6 className="dropdown-header">{user.email}</h6>
        <Link className="dropdown-item" to="/user-settings">
          <FontAwesomeIcon
            icon={faUser}
            size="sm"
            className="me-2 text-gray-400"
          />
          Settings
        </Link>
        <div className="dropdown-divider"></div>
        {InRole(user, "developer") && (
          <a className="dropdown-item" onClick={() => copyBearerToClipboard()}>
            <FontAwesomeIcon
              icon={faCopy}
              size="sm"
              className="me-2 text-gray-400"
            />
            Bearer to Clipboard
          </a>
        )}

        <a
          className="dropdown-item"
          onClick={handleLogout}
          data-toggle="modal"
          data-target="#logoutModal"
        >
          <FontAwesomeIcon
            icon={faSignOut}
            size="sm"
            className="me-2 text-gray-400"
          />
          Logout
        </a>
      </div>
    </li>
  );
};
