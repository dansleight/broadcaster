(function () {
  window.addEventListener("load", function () {
    const ui = SwaggerUIBundle({
      url: "/swagger/v1/swagger.json",
      dom_id: "#swagger-ui",
      presets: [
        SwaggerUIBundle.presets.apis,
        SwaggerUIBundle.SwaggerUIStandalonePreset,
      ],
      onComplete: function () {
        // Override the authorize button behavior
        const originalAuthorize = ui.authActions.authorize;
        ui.authActions.authorize = function (auth) {
          if (auth && auth.schema && auth.schema.get("name") === "Bearer") {
            let token = auth.value;
            // Add "Bearer " prefix if missing
            if (token && !token.startsWith("Bearer ")) {
              auth.value = "Bearer " + token.trim();
            }
          }
          return originalAuthorize.apply(this, arguments);
        };

        // Override request interceptor to ensure Bearer prefix
        ui.getSystem()
          .getState()
          .setIn(["spec", "requestInterceptor"], (req) => {
            if (req.headers.Authorization) {
              let token = req.headers.Authorization;
              if (!token.startsWith("Bearer ")) {
                req.headers.Authorization = "Bearer " + token.trim();
              }
            }
            return req;
          });
      },
    });
  });
})();
