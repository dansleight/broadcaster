import { faGauge, faGears } from "@fortawesome/free-solid-svg-icons";
import { MenuItem } from "./models/Interfaces";

/*
------------------------------------------------------------------------------------------------------
Menu Configuration
  Each of the components included in the layoutConfig require menus, unless you are hardcoding them.
  Creating the menus in sections so the can be organized differently for different purposes may behelpful.
    - 


------------------------------------------------------------------------------------------------------
*/
const dashboard: MenuItem = { path: "/", label: "Dashboard", icon: faGauge };
// const admin: MenuItem = {
//   path: "/admin",
//   label: "Admin",
//   icon: faGear,
//   roles: "Admin",
// };

// const preview: MenuItem = {
//   path: "/preview",
//   label: "Preview",
//   icon: faVideoCamera,
// };
const manage: MenuItem = {
  path: "/manage",
  label: "Manage",
  icon: faGears,
};
// const audio: MenuItem = {
//   path: "/audio",
//   label: "Audio",
//   icon: faVolumeHigh,
// };

export const genericMenuBase: MenuItem[] = [
  dashboard,
  // preview,
  manage,
  // audio,
];

export const sidebarMenuBase: MenuItem[] = genericMenuBase;
