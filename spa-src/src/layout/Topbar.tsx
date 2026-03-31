import { useSettingsContext } from "../contexts/UseContexts";
import { UserInfo } from "./horizontalcomponents/UserInfo";
import { LightDarkMode } from "./horizontalcomponents/LightDarkMode";
import { layoutConfig } from "../layoutConfig";
import classNames from "classnames";
import { useMemo } from "react";
import { Brand } from "./horizontalcomponents/Brand";

export const Topbar = () => {
  const { darkMode } = useSettingsContext();

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
