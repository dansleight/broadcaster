import { Link, useLocation } from "react-router-dom";
import { MenuItem } from "../../models/Interfaces";
import { faCircleDot } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";

type HMenuProps = {
  menuitems: MenuItem[];
  sm?: boolean;
  xs?: boolean;
};

export const HMenu = ({ menuitems, sm, xs }: HMenuProps) => {
  return (
    <div className="d-none d-lg-inline-block">
      <ul
        className={classNames("navbar-nav hmenu", {
          "hmenu-sm": sm && !xs,
          "hmenu-xs": xs,
        })}
      >
        <RenderMenuItems items={menuitems} depth={0} />
      </ul>
    </div>
  );
};

type RenderMenuItemsProps = {
  items?: MenuItem[];
  depth: number;
};

const RenderMenuItems = ({ items, depth }: RenderMenuItemsProps) => {
  if (items === undefined) {
    return <></>;
  }

  return (
    <>
      {items.map((menuItem, idx) => (
        <RenderMenuItem key={idx} item={menuItem} depth={depth} idx={idx} />
      ))}
    </>
  );
};

type RenderMenuItemProps = {
  item: MenuItem;
  depth: number;
  idx: number;
};

const RenderMenuItem = ({ item, depth, idx }: RenderMenuItemProps) => {
  const loc = useLocation();

  if (depth == 0) {
    if (item.group) {
      return (
        <>
          <div className="hbar-divider d-none d-sm-block"></div>
          <li className="nav-item text-uppercase text-bottom">
            <span className="nav-link badge opacity-50">{item.label}</span>
          </li>
          <RenderMenuItems items={item.items} depth={0} />
        </>
      );
    }

    if (item.items === undefined || item.items.length == 0) {
      return (
        <li className="nav-item me-1">
          <Link to={item.path ?? "#"} className="nav-link">
            <FontAwesomeIcon
              icon={item.icon ?? faCircleDot}
              size="sm"
              fixedWidth
              className="me-1"
            />
            <span>{item.label}</span>
          </Link>
        </li>
      );
    }

    return (
      <li
        className={classNames("nav-item dropdown me-1", {
          active: loc.pathname === item.path,
        })}
      >
        <Link
          to="#"
          id={`navbardropdown${idx}`}
          className="nav-link dropdown-toggle"
          data-bs-toggle="dropdown"
        >
          <FontAwesomeIcon
            icon={item.icon ?? faCircleDot}
            size="sm"
            fixedWidth
            className="me-1"
          />
          <span>{item.label}</span>
        </Link>
        <div className="dropdown-menu" aria-labelledby={`navbardropdown${idx}`}>
          <ul className="list-unstyled">
            <RenderMenuItems items={item.items} depth={depth + 1} />
          </ul>
        </div>
      </li>
    );
  } else {
    if (item.group) {
      return (
        <>
          <li className="menu-title">{item.label}</li>
          <RenderMenuItems items={item.items} depth={0} />
        </>
      );
    }
    return (
      <Link to={item.path ?? "#"} className="dropdown-item">
        <span>{item.label}</span>
      </Link>
    );
  }
};
