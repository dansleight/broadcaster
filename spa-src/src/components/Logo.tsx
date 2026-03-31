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
          d="
              M 30 0
              L 73 0
              L 43 100
              L 0 100
              z
              M 77 10
              L 97 10
              L 73 90
              L 53 90
              z
              M 20 42
              L 20 58
              L 80 55
              L 80 45
              z"
        />
        <circle fill="currentColor" cx="13" cy="13" r="11" />
        <circle fill="currentColor" cx="87" cy="82" r="5" />
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
        viewBox="0 0 100 100"
      >
        {parts}
      </svg>
    );

  return (
    <svg
      id="logo"
      data-name="layer_0"
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 100 100"
    >
      {parts}
    </svg>
  );
}
