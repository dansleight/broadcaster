import React, { useEffect, useRef, useState } from "react";

export const WaitBar: React.FC = () => {
  return (
    <div className="progress mt-3" role="progressbar">
      <div
        className="progress-bar progress-bar-striped progress-bar-animated"
        style={{ width: "100%" }}
      />
    </div>
  );
};
