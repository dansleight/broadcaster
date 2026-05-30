import { useSessionContext, useSettingsContext } from "../contexts/UseContexts";
import { UserInfo } from "./horizontalcomponents/UserInfo";
import { LightDarkMode } from "./horizontalcomponents/LightDarkMode";
import { layoutConfig } from "../layoutConfig";
import classNames from "classnames";
import { useMemo } from "react";
import { Brand } from "./horizontalcomponents/Brand";
import { StreamState } from "../apiClient/data-contracts";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faImage, faPlay } from "@fortawesome/free-solid-svg-icons";

export const Topbar = () => {
  const { darkMode } = useSettingsContext();
  const { streamState } = useSessionContext();

  const topbarClass = useMemo(() => {
    if (!darkMode) return layoutConfig.topbarTheme;
    return layoutConfig.topbarDarkTheme;
  }, [darkMode, layoutConfig]);

  return (
    <nav
      id="layout-topbar"
      className={classNames(
        "horizontal-bar navbar navbar-expand static-top " + topbarClass,
        {},
      )}
    >
      {/* 
        Conditions where the brand should show up:
        - All sizes when there is no sidebar 
        - SM and XS when there is a sidebar */}
      <Brand
        className={classNames(
          darkMode ? layoutConfig.sidebarDarkTheme : layoutConfig.sidebarTheme,
          {
            "d-md-none": layoutConfig.includeSidebar,
          },
        )}
      />

      {/* Create space when the navbar-brand-icon is visible, since it is absolute positioned  */}
      <div
        className={classNames("hbar-brand-icon-spacer", {
          "d-md-none": layoutConfig.includeSidebar,
        })}
      ></div>
      {/* Sidebar Toggle (navbar)
          Conditions where the toggle should show:
          - all sizes when there is a sidebar
          - SM an XS when there is a navbar, maybe MD as well, based on need
          ** should be its own component if there is no sidebar or navbar, and there is still a desire to have it
      */}
      {layoutConfig.includeSidebar && layoutConfig.sidebarFull && (
        <div style={{ width: "1rem" }}></div>
      )}

      <div className="ms-3">
        {streamState.streamState == StreamState.Idle ? (
          <h3 className="text-secondary">Idle</h3>
        ) : streamState.streamState == StreamState.Live ? (
          <h3 className="text-success">
            <FontAwesomeIcon icon={faPlay} beat />
            {" Live"}
          </h3>
        ) : streamState.streamState == StreamState.Placeholder ? (
          <h3 className="text-primary">
            <FontAwesomeIcon icon={faImage} beat />
            Phld: {streamState.placeholderImage?.replace(".jpg", "")}
          </h3>
        ) : (
          <h3 className="text-danger">Unknown Stream State</h3>
        )}
      </div>

      {/* Right Nav */}
      <ul className="navbar-nav ms-auto">
        {/* Nav Item - Light Dark Mode */}
        <LightDarkMode />

        <div className="hbar-divider d-none d-sm-block"></div>

        {/* Nav Item - User Information */}
        <UserInfo />
      </ul>
    </nav>
  );
};
