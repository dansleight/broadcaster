import { Link, NavLink, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleDot } from "@fortawesome/free-solid-svg-icons";
import MetisMenu from "@metismenu/react";
import { MenuItem, MenuProps } from "../../../models/Interfaces";
import classNames from "classnames";

export const FullSidebar = ({ menuItems }: MenuProps) => {
  return (
    <>
      <div id="sidebar-metis-menu">
        <MetisMenu className="list-unstyled mb-0">
          <RenderFullItems items={menuItems} depth={0} />
        </MetisMenu>
      </div>
    </>
  );
};

type RenderFullItemsProps = {
  items?: MenuItem[];
  depth: number;
};

const RenderFullItems = ({ items, depth }: RenderFullItemsProps) => {
  if (items === undefined) {
    return <></>;
  }
  return (
    <>
      {items.map((menuItem, idx) => (
        <RenderFullItem key={idx} item={menuItem} depth={depth} />
      ))}
    </>
  );
};

type RenderFullItemProps = {
  item: MenuItem;
  depth: number;
};

const RenderFullItem = ({ item, depth }: RenderFullItemProps) => {
  const loc = useLocation();

  if (item.group) {
    return (
      <>
        {!depth && <hr className="sidebar-divider" />}
        <li className={classNames("menu-title", { "menu-sub-title": depth })}>
          {item.label}
        </li>
        <RenderFullItems items={item.items} depth={depth} />
      </>
    );
  }

  if (item.items === undefined || item.items.length == 0) {
    return (
      <li className={loc.pathname === item.path ? "active" : ""}>
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
        <RenderFullItems items={item.items} depth={++depth} />
      </ul>
    </li>
  );
};
