import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: { port: 3034 },
  css: {
    preprocessorOptions: {
      scss: {
        quietDeps: true,
        silenceDeprecations: [
          "legacy-js-api",
          "color-functions",
          "import",
          "global-builtin",
          "slash-div",
          "if-function",
        ],
      },
    },
  },
});
