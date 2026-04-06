import { Dashboard } from "./pages/Dashboard";
import { Preview } from "./pages/Preview";
import { Manage } from "./pages/Manage";
import { Audio } from "./pages/Audio";

export const routes = [
  { path: "/", component: <Dashboard />, title: "Dashboard" },
  { path: "/preview", component: <Preview />, title: "Preview" },
  { path: "/manage", component: <Manage />, title: "Manage" },
  { path: "/audio", component: <Audio />, title: "Audio" },
];
