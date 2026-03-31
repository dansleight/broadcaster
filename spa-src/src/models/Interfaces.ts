import { IconDefinition } from "@fortawesome/fontawesome-common-types";
import { NavbarTheme, SidebarTheme, TopbarTheme } from "./Enums";

export interface LayoutConfig {
  includeSidebar: boolean;
  sidebarTheme: SidebarTheme;
  sidebarDarkTheme: SidebarTheme;
  sidebarFull: boolean;

  includeTopbar: boolean;
  topbarTheme: TopbarTheme;
  topbarDarkTheme: TopbarTheme;

  includeNavbar: boolean;
  navbarTheme: NavbarTheme;
  navbarDarkTheme: NavbarTheme;

  defaultTitle?: string;
  titlePrefix?: string;
  titleSuffix?: string;
}

export interface MenuProps {
  menuItems: MenuItem[];
}

export interface MenuItem {
  path?: string;
  label: string;
  icon?: IconDefinition;
  items?: MenuItem[];
  group?: boolean;
  roles?: string | string[];
}
