type LogoProps = {
  size?: number;
};

export function Logo({ size }: LogoProps) {
  const parts = (
    <g id="logo_1" data-name="layer_1">
      <g>
        <path
          fill="currentColor"
          data-name="part1"
          d="M96 64c-35.3 0-64 28.7-64 64l0 256c0 35.3 28.7 64 64 64l256 0c35.3 0 64-28.7 64-64l0-256c0-35.3-28.7-64-64-64L96 64zM464 336l73.5 58.8c4.2 3.4 9.4 5.2 14.8 5.2 13.1 0 23.7-10.6 23.7-23.7l0-240.6c0-13.1-10.6-23.7-23.7-23.7-5.4 0-10.6 1.8-14.8 5.2L464 176 464 336z"
        />
      </g>
    </g>
  );

  if (size)
    return (
      <svg
        id="logo"
        width={size}
        height={size}
        data-name="layer_0"
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 576 512"
      >
        {parts}
      </svg>
    );

  return (
    <svg
      id="logo"
      data-name="layer_0"
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 576 512"
    >
      {parts}
    </svg>
  );
}
