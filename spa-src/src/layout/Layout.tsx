import { ReactNode, useEffect } from "react";
import { useSettingsContext } from "../contexts/UseContexts";
import { layoutConfig } from "../layoutConfig";
import { Sidebar } from "../layout/sidebar";
import { Topbar } from "./Topbar";
import { Navbar } from "./Navbar";
import { MenuItem } from "../models/Interfaces";

type LayoutProps = {
  children: ReactNode;
  title?: string;
  sidebarMenu: MenuItem[];
  navbarMenu: MenuItem[];
};

export const Layout = ({
  children,
  title,
  sidebarMenu,
  navbarMenu,
}: LayoutProps) => {
  const { sidebarToggled, setBodyAttribute } = useSettingsContext();

  useEffect(() => {
    document.title = title ? "Broadcast: " + title : "Broadcast";
    setBodyAttribute(
      "data-navbar-include",
      layoutConfig.includeNavbar ? "true" : "false",
    );
    setBodyAttribute(
      "data-topbar-include",
      layoutConfig.includeTopbar ? "true" : "false",
    );
    setBodyAttribute(
      "data-sidebar-include",
      layoutConfig.includeSidebar ? "true" : "false",
    );
    setBodyAttribute(
      "data-sidebar-full",
      layoutConfig.includeSidebar && layoutConfig.sidebarFull
        ? "true"
        : "false",
    );
  }, [layoutConfig, children, title]);

  return (
    <div id="wrapper" className={sidebarToggled ? "sidebar-toggled" : ""}>
      {layoutConfig.includeSidebar && <Sidebar menuItems={sidebarMenu} />}
      {layoutConfig.includeTopbar && <Topbar />}
      {layoutConfig.includeNavbar && <Navbar menuItems={navbarMenu} />}

      {/* Content Wrapper */}
      <div id="content-wrapper" className="d-flex flex-column">
        {children}
      </div>
    </div>
  );
};
