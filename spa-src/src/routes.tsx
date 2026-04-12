import { Dashboard } from "./pages/Dashboard";
import { Preview } from "./pages/Preview";
import { Audio } from "./pages/Audio";
import { Manager } from "./pages/Manager";

export const routes = [
  { path: "/", component: <Dashboard />, title: "Dashboard" },
  { path: "/preview", component: <Preview />, title: "Preview" },
  { path: "/manage", component: <Manager />, title: "Manage" },
  { path: "/audio", component: <Audio />, title: "Audio" },
];
