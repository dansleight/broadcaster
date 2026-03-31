import { useEffect, useState } from "react";
import { useSettingsContext } from "../contexts/UseContexts";

type ToggleSidebarSvgProps = {
  reverse?: boolean;
  size?: "sm" | "md" | "lg";
};

export function ToggleSidebarSvg({
  reverse,
  size = "md",
}: ToggleSidebarSvgProps) {
  const { sidebarToggled } = useSettingsContext();
  const [flipper, setFlipper] = useState<boolean>(true);

  useEffect(() => {
    if (reverse) setFlipper(false);
    else setFlipper(true);
  }, [reverse]);
  return (
    <>
      {sidebarToggled == flipper ? (
        <svg
          xmlns="http://www.w3.org/2000/svg"
          height={size == "sm" ? "1em" : size == "lg" ? "2em" : "1.5em"}
          viewBox="0 0 960 960"
          className="sidebar-toggle"
          fill="currentColor"
        >
          <rect
            x={20}
            y={20}
            width={160}
            height={920}
            className="sidebar-toggle-sidebar"
          />
          <path
            className="sidebar-toggle-arrow"
            d="M900 480l-240-240-56 58 142 142h-456v80h456l-142 142 56 58z"
          />
        </svg>
      ) : (
        <svg
          xmlns="http://www.w3.org/2000/svg"
          height={size == "sm" ? "1em" : size == "lg" ? "2em" : "1.5em"}
          viewBox="0 0 960 960"
          className="sidebar-toggle"
          fill="currentColor"
        >
          <rect
            x={20}
            y={20}
            width={420}
            height={920}
            className="sidebar-toggle-sidebar"
          />
          <path
            className="sidebar-toggle-arrow"
            d="M500 480l240-240 56 58-142 142h306v80h-306l142 142-56 58z"
          />
        </svg>
      )}
    </>
  );
}
