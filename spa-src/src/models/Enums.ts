export enum TopbarTheme {
  Light = "bg-light text-bg-light",
  Dark = "bg-dark text-bg-dark",
  Primary = "bg-primary text-bg-primary",
  Secondary = "bg-secondary text-bg-secondary",
  Tertiary = "bg-tertiary text-bg-tertiary",
  Black = "bg-black text-gray-200",
  Deep = "bg-gray-900 text-gray-200",
}

export enum NavbarTheme {
  Light = "bg-light text-bg-light",
  Dark = "bg-dark text-bg-dark",
  Primary = "bg-primary text-bg-primary",
  Secondary = "bg-secondary text-bg-secondary",
  Tertiary = "bg-tertiary text-bg-tertiary",
  Black = "bg-black text-gray-200",
  Deep = "bg-gray-900 text-gray-200",
}

export enum SidebarTheme {
  Light = "bg-light text-bg-light",
  Dark = "bg-dark text-bg-dark",
  Primary = "bg-primary text-bg-primary",
  Secondary = "bg-secondary text-bg-secondary",
  Tertiary = "bg-tertiary text-bg-tertiary",
  Black = "bg-black text-gray-200",
  Deep = "bg-gray-900 text-gray-200",
  Info = "bg-info text-white",
}

/*
------------------------------------------------------------------------------------------------------
Grid Breakpoints
  This enum and array must be defined to match the values that bootstrap is using.
------------------------------------------------------------------------------------------------------
*/
export enum GridBreakpoint {
  xs = 0,
  sm = 576,
  md = 768,
  lg = 992,
  xl = 1200,
  xxl = 1400,
}

export const gridBreakpoints: GridBreakpoint[] = [
  GridBreakpoint.xs,
  GridBreakpoint.sm,
  GridBreakpoint.md,
  GridBreakpoint.lg,
  GridBreakpoint.xl,
  GridBreakpoint.xxl,
];
