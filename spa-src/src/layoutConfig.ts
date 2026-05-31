import { NavbarTheme, SidebarTheme, TopbarTheme } from "./models/Enums";
import { LayoutConfig } from "./models/Interfaces";

/*
------------------------------------------------------------------------------------------------------
Layout Configuration
 - sideBar: indicates whether to include a sidebar or leave it
 - sideBarFull: indicates whether the sidebar is full-height, or starts below the topbar and/or navbar
 - topBar: indicates whether to include a topbar

 NOTE: The active sidebar theme will inform the brand background
------------------------------------------------------------------------------------------------------
*/
export const layoutConfig: LayoutConfig = {
  includeSidebar: true,
  sidebarTheme: SidebarTheme.Primary,
  sidebarDarkTheme: SidebarTheme.Primary,
  sidebarFull: true,

  includeTopbar: true,
  topbarTheme: TopbarTheme.Tertiary,
  topbarDarkTheme: TopbarTheme.Dark,

  includeNavbar: false,
  navbarTheme: NavbarTheme.Light,
  navbarDarkTheme: NavbarTheme.Dark,

  defaultTitle: "Broadcast",
  titleSuffix: " - Broadcast",
} as LayoutConfig;
