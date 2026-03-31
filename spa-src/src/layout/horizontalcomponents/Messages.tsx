import { faEnvelope } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Dropdown } from "bootstrap";
import { useEffect, useRef } from "react";

export const Messages = () => {
  const dropdownCreated = useRef<boolean>(false);

  useEffect(() => {
    if (dropdownCreated.current) return;
    dropdownCreated.current = true;
    Dropdown.getOrCreateInstance(document.querySelector("#messages-dropdown")!);
  }, []);

  return (
    <li id="messages-dropdown" className="nav-item dropdown no-arrow mx-1">
      <a
        className="nav-link dropdown-toggle"
        href="#"
        id="messagesDropdown"
        role="button"
        data-bs-toggle="dropdown"
        aria-haspopup="true"
        aria-expanded="false"
      >
        <FontAwesomeIcon icon={faEnvelope} fixedWidth />
        {/* Counter - Messages */}
        <span className="badge badge-counter text-success">7</span>
      </a>
      {/* Dropdown - Messages */}
      <div
        className="dropdown-list dropdown-menu dropdown-menu-end shadow animated--grow-in"
        aria-labelledby="messagesDropdown"
      >
        <h6 className="dropdown-header bg-success border-success">
          Message Center
        </h6>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="dropdown-list-image me-3">
            <img className="rounded-circle" src="/images/face2.png" alt="..." />
            <div className="status-indicator bg-success"></div>
          </div>
          <div className="font-weight-bold">
            <div className="text-truncate">
              Hi there! I am wondering if you can help me with a problem I've
              been having.
            </div>
            <div className="small text-gray-500">Emily Fowler · 58m</div>
          </div>
        </a>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="dropdown-list-image me-3">
            <img className="rounded-circle" src="/images/face3.png" alt="..." />
            <div className="status-indicator"></div>
          </div>
          <div>
            <div className="text-truncate">
              I have the photos that you ordered last month, how would you like
              them sent to you?
            </div>
            <div className="small text-gray-500">Jae Chun · 1d</div>
          </div>
        </a>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="dropdown-list-image me-3">
            <img className="rounded-circle" src="/images/face4.png" alt="..." />
            <div className="status-indicator bg-warning"></div>
          </div>
          <div>
            <div className="text-truncate">
              Last month's report looks great, I am very happy with the progress
              so far, keep up the good work!
            </div>
            <div className="small text-gray-500">Morgan Alvarez · 2d</div>
          </div>
        </a>
        <a className="dropdown-item d-flex align-items-center" href="#">
          <div className="dropdown-list-image me-3">
            <img className="rounded-circle" src="/images/face5.png" alt="..." />
            <div className="status-indicator bg-success"></div>
          </div>
          <div>
            <div className="text-truncate">
              Am I a good boy? The reason I ask is because someone told me that
              people say this to all dogs, even if they aren't good...
            </div>
            <div className="small text-gray-500">Chicken the Dog · 2w</div>
          </div>
        </a>
        <a className="dropdown-item text-center small text-gray-500" href="#">
          Read More Messages
        </a>
      </div>
    </li>
  );
};
