import { Dashboard } from "./pages/Dashboard";
import { Preview } from "./pages/Preview";
import { Manage } from "./pages/Manage";

export const routes = [
  { path: "/", component: <Dashboard />, title: "Dashboard" },
  { path: "/preview", component: <Preview />, title: "Preview" },
  { path: "/manage", component: <Manage />, title: "Manage" },
];
