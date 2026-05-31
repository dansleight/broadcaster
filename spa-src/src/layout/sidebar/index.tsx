import { useEffect, useRef, useState } from "react";
import SimpleBar from "simplebar-react";
import "simplebar-react/dist/simplebar.min.css";
import { Link, useLocation } from "react-router-dom";
import { FullSidebar } from "./components/FullSidebar";
import { useSettingsContext } from "../../contexts/UseContexts";
import { NarrowSidebar } from "./components/NarrowSidebar";
import { layoutConfig } from "../../layoutConfig";
import classNames from "classnames";
import { GridBreakpoint } from "../../models/Enums";
import { Logo } from "../../components/Logo";
import { MenuProps } from "../../models/Interfaces";
import { ToggleSidebarSvg } from "../../components/ToggleSidebarSvg";

export const Sidebar = ({ menuItems }: MenuProps) => {
  const {
    sidebarToggled,
    toggleSidebar,
    setSidebarToggled,
    breakpoint,
    darkMode,
  } = useSettingsContext();
  const ref = useRef<any>({});
  const [sidebarClass, setSidebarClass] = useState<string>("");

  const location = useLocation();

  useEffect(() => {
    if (breakpoint < GridBreakpoint.sm && sidebarToggled) {
      setSidebarToggled(false);
    }
  }, [location]);

  useEffect(() => {
    if (darkMode) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setSidebarClass(layoutConfig.sidebarDarkTheme);
    } else {
      setSidebarClass(layoutConfig.sidebarTheme);
    }
  }, [darkMode, layoutConfig]);

  return (
    <div
      className={classNames("sidebar shadow " + sidebarClass, {
        toggled: sidebarToggled,
      })}
      id="layout-sidebar"
    >
      {/* Sidebar - Toggle */}
      <div
        id="sidebar-toggle-button"
        className={classNames(
          "bg-body text-bg-body rounded-circle border border-primary border-3 d-flex justify-content-center align-items-center",
          { "text-white": darkMode, "text-black": !darkMode },
        )}
        onClick={toggleSidebar}
      >
        <ToggleSidebarSvg reverse={breakpoint <= GridBreakpoint.sm} size="sm" />
      </div>

      {/* Sidebar - Brand 
        If the sidebar is not full, the brand and the first divider should be hidden in XS and SM
      */}

      <Link
        className={classNames(
          "sidebar-brand d-flex align-items-center justify-content-center",
          {
            "d-none d-md-flex": !layoutConfig.sidebarFull,
          },
        )}
        to="/"
      >
        <div className="sidebar-brand-icon">
          <Logo size={30} />
        </div>
        <div className="sidebar-brand-text mx-3">Broadcast</div>
      </Link>

      {/* Divider */}
      <hr
        className={classNames("sidebar-divider my-0", {
          "d-none d-md-block": !layoutConfig.sidebarFull,
        })}
      />

      {!sidebarToggled ? (
        <SimpleBar ref={ref} className="sb-wrapper">
          <FullSidebar menuItems={menuItems} />
        </SimpleBar>
      ) : (
        <div className="bs-wrapper">
          <NarrowSidebar menuItems={menuItems} />
        </div>
      )}

      {/* Divider */}
      <hr className="sidebar-divider d-none d-md-block" />
    </div>
  );
};
