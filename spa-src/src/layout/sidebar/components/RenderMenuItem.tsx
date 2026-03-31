// import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
// import { faCircleDot } from "@fortawesome/free-solid-svg-icons";
// import { Link } from "react-router-dom";
// import React from "react";
// import { MenuItem } from "../../../models/Interfaces";

// type RenderMenuItemProps = {
//   menuItem: MenuItem;
// };

// export const RenderMenuItem = ({ menuItem }: RenderMenuItemProps) => {
//   return (
//     <>
//       {menuItem.items && menuItem.items.length > 0 ? (
//         <li className="nav-item">
//           <a
//             className="nav-link collapsed"
//             href="#"
//             data-toggle="collapse"
//             aria-expanded="true"
//           >
//             <FontAwesomeIcon
//               icon={menuItem.icon ?? faCircleDot}
//               size="sm"
//               fixedWidth
//             />
//             <span>{menuItem.label}</span>
//           </a>
//           <div className="collapse">
//             <div className="bg-white py-2 collapse-inner rounded">
//               {menuItem.items.map((subMenuItem, subIdx) => (
//                 <React.Fragment key={subIdx}>
//                   {subMenuItem.group ? (
//                     <h6 className="collapse-header">Custom Components:</h6>
//                   ) : (
//                     <Link
//                       className="collapse-item"
//                       to={subMenuItem.path ?? "/"}
//                     >
//                       {subMenuItem.label}
//                     </Link>
//                   )}
//                 </React.Fragment>
//               ))}
//             </div>
//           </div>
//         </li>
//       ) : (
//         <li className="nav-item active">
//           <Link className="nav-link" to={menuItem.path ?? "/"}>
//             <FontAwesomeIcon
//               icon={menuItem.icon ?? faCircleDot}
//               size="sm"
//               fixedWidth
//             />
//             <span>{menuItem.label}</span>
//           </Link>
//         </li>
//       )}
//     </>
//   );
// };
