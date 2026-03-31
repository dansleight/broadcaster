import { faCircleHalfStroke } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useSettingsContext } from "../../contexts/UseContexts";
import { MouseEventHandler } from "react";

export function LightDarkMode() {
  const { darkMode, setDarkMode } = useSettingsContext();

  const toggleDarkMode: MouseEventHandler<HTMLAnchorElement> = (event) => {
    event.preventDefault();
    setDarkMode(!darkMode);
  };

  return (
    <li id="light-dark-switch" className="nav-item mx-1">
      <a
        className="nav-link"
        id="light-dark-switch-link"
        role="button"
        onClick={toggleDarkMode}
      >
        <FontAwesomeIcon icon={faCircleHalfStroke} />
      </a>
    </li>
  );
}
