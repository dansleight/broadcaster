import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { SettingsProvider } from "./contexts/SettingsContext.tsx";
import { LoadingWrapper } from "./components/LoadingWrapper.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <SettingsProvider messageWrapper={LoadingWrapper}>
      <App />
    </SettingsProvider>
  </StrictMode>,
);
