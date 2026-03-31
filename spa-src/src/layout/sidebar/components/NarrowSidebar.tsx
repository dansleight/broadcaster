import { NavLink, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleDot } from "@fortawesome/free-solid-svg-icons";
import { MenuItem, MenuProps } from "../../../models/Interfaces";

export const NarrowSidebar = ({ menuItems }: MenuProps) => {
  return (
    <div id="sidebar-narrow" className="flex-column nav">
      <RenderMenuItems items={menuItems} depth={0} />
    </div>
  );
};

type RenderMenuItemsProps = {
  items?: MenuItem[];
  depth: number;
};

const RenderMenuItems = ({ items, depth }: RenderMenuItemsProps) => {
  if (items === undefined) return <></>;
  return (
    <>
      {items.map((menuItem, idx) => (
        <RenderMenuItem key={idx} item={menuItem} depth={depth} />
      ))}
    </>
  );
};

type RenderMenuItemProps = {
  item: MenuItem;
  depth: number;
};

const RenderMenuItem = ({ item, depth }: RenderMenuItemProps) => {
  if (item.group) {
    return (
      <>
        {!depth && <hr className="sidebar-divider" />}
        <div className="menu-title">{item.label}</div>
        <RenderMenuItems items={item.items} depth={depth} />
      </>
    );
  }

  if (item.items === undefined || item.items.length == 0) {
    return (
      <NavLink
        className="nav-link dropdown-item"
        to={item.path ?? "/#"}
        data-discover="true"
      >
        {!depth ? (
          <>
            <FontAwesomeIcon
              icon={item.icon ?? faCircleDot}
              size="sm"
              fixedWidth
            />
            <span>{item.label}</span>
          </>
        ) : (
          <span>{item.label}</span>
        )}
      </NavLink>
    );
  }

  const location = useLocation();

  return (
    <div className="nav-item openright">
      <a
        aria-expanded="false"
        role="button"
        className={
          location?.pathname.startsWith(item.path ?? "zzz")
            ? "openright-toggle nav-link active"
            : "openright-toggle nav-link"
        }
        tabIndex={0}
        href="#"
        onClick={(e) => e.preventDefault()}
      >
        {!depth ? (
          <>
            <FontAwesomeIcon
              icon={item.icon ?? faCircleDot}
              size="sm"
              fixedWidth
            />
            <span>{item.label}</span>
          </>
        ) : (
          <span>{item.label}</span>
        )}
      </a>
      <div className="dropdown-menu">
        <RenderMenuItems items={item.items} depth={++depth} />
      </div>
    </div>
  );
};
