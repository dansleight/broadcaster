import { faSearch } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Dropdown } from "bootstrap";
import { useEffect, useRef } from "react";

type SearchDropdownProps = {
  className?: string;
};

export const SearchDropdown = ({ className }: SearchDropdownProps) => {
  const dropdownCreated = useRef<boolean>(false);

  useEffect(() => {
    if (dropdownCreated.current) return;
    dropdownCreated.current = true;
    Dropdown.getOrCreateInstance(document.querySelector("#search-dropdown")!);
  }, []);

  return (
    <li
      id="search-dropdown"
      className={"nav-item dropdown no-arrow " + (className ? className! : "")}
    >
      <a
        className="nav-link dropdown-toggle"
        href="#"
        id="searchDropdown"
        role="button"
        data-bs-toggle="dropdown"
        aria-haspopup="true"
        aria-expanded="false"
      >
        <FontAwesomeIcon icon={faSearch} fixedWidth />
      </a>

      {/* Dropdown - Messages */}
      <div
        id="search-dropdown"
        className="dropdown-menu dropdown-menu-end p-2 shadow animated--grow-in"
        aria-labelledby="searchDropdown"
      >
        <form className="form-inline me-auto w-100 hbar-search">
          <div className="input-group">
            <input
              type="text"
              className="form-control border-0 small"
              placeholder="Search for..."
              aria-label="Search"
              aria-describedby="basic-addon2"
            />
            <div className="input-group-append">
              <button className="btn btn-primary" type="button">
                <FontAwesomeIcon icon={faSearch} size="sm" />
              </button>
            </div>
          </div>
        </form>
      </div>
    </li>
  );
};
