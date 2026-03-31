import { IconDefinition } from "@fortawesome/fontawesome-common-types";
import { MenuItem } from "../../models/Interfaces";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleDot, faEllipsisV } from "@fortawesome/free-solid-svg-icons";
import MetisMenu from "@metismenu/react";
import { Link, NavLink, useLocation } from "react-router-dom";
import { useEffect } from "react";
import { Dropdown } from "bootstrap";

type DropMenuProps = {
  menuitems: MenuItem[];
  icon?: IconDefinition;
  id?: string;
  className?: string;
};

export const DropMenu = ({ menuitems, icon, id, className }: DropMenuProps) => {
  const loc = useLocation();

  useEffect(() => {
    const el = document.getElementById((id ?? "dropmenu") + "-dropdown");
    if (el) {
      const dd = Dropdown.getOrCreateInstance(el);
      dd.hide();
    }
  }, [loc]);

  return (
    <li
      id={(id ?? "dropmenu") + "-dropdown-list-item"}
      className={
        "nav-item dropdown dropmenu no-arrow mx-1 " + (className ?? "")
      }
    >
      <a
        id={(id ?? "dropmenu") + "-dropdown"}
        className="nav-link dropdown-toggle"
        href="#"
        role="button"
        data-bs-toggle="dropdown"
        data-bs-auto-close="outside"
        aria-haspopup="true"
        aria-expanded="false"
      >
        <FontAwesomeIcon icon={icon ?? faEllipsisV} fixedWidth />
      </a>
      <div
        className="dropdown-menu dropdown-menu-end shadow animiated--grow-in"
        aria-labelledby={(id ?? "dropmenu") + "-dropdown"}
      >
        <MetisMenu className="list-unstyled mb-0">
          <RenderItems items={menuitems} depth={0} />
        </MetisMenu>
      </div>
    </li>
  );
};

type RenderItemsProps = {
  items?: MenuItem[];
  depth: number;
};

const RenderItems = ({ items, depth }: RenderItemsProps) => {
  if (items === undefined) return <></>;
  return (
    <>
      {items.map((menuitem, idx) => (
        <RenderItem item={menuitem} depth={depth} key={idx} />
      ))}
    </>
  );
};

type RenderItemProps = {
  item: MenuItem;
  depth: number;
};

const RenderItem = ({ item, depth }: RenderItemProps) => {
  const loc = useLocation();
  if (item.group) {
    return (
      <>
        {!depth && <hr />}
        <li className="menu-title">{item.label}</li>
        <RenderItems items={item.items} depth={depth} />
      </>
    );
  }

  if (item.items === undefined || item.items.length == 0) {
    return (
      <li>
        <NavLink to={item.path ?? "/"}>
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
      </li>
    );
  }

  return (
    <li className={loc.pathname === item.path ? "active" : ""}>
      <Link to="#" className="has-arrow">
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
      </Link>
      <ul className="sub-menu list-unstyled">
        <RenderItems items={item.items} depth={depth + 1} />
      </ul>
    </li>
  );
};
