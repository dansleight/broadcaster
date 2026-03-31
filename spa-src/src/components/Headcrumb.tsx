import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

type Crumb = {
  title: string;
  path?: string;
};

type HeadcrumbProps = {
  title?: string;
  parent?: string;
  titles?: string[] | undefined;
  paths?: string[];
};

export function Headcrumb({ title, parent, titles, paths }: HeadcrumbProps) {
  const [_title, setTitle] = useState<string>("unknown");
  const [crumbs, setCrumbs] = useState<Crumb[]>([]);

  useEffect(() => {
    const workingTitles: string[] | undefined = titles
      ? [...titles]
      : undefined;
    let workingCrumbs: Crumb[] = [];

    if (title) setTitle(title);
    else if (workingTitles && workingTitles.length) {
      setTitle(workingTitles.pop()!);
    }

    if (workingTitles && workingTitles.length) {
      for (let i = 0; i < workingTitles.length; i++) {
        const crumb: Crumb = { title: workingTitles[i] };
        if (paths && paths.length > i && paths[i].trim() != "")
          crumb.path = paths[i];
        workingCrumbs.push(crumb);
      }
    } else if (parent) {
      workingCrumbs.push({ title: parent });
    }
    setCrumbs(workingCrumbs);
  }, [title, parent, titles, paths]);

  return (
    <div className="d-sm-flex align-items-center justify-content-between mb-4">
      <h1 className="h5 headcrumb mb-0 text-uppercase text-body-emphasis">
        {crumbs && crumbs.length > 0 && (
          <>
            {crumbs.map((crumb, idx) => (
              <span key={idx} className="fw-light text-body-secondary">
                {crumb.path !== undefined ? (
                  <Link to={crumb.path}>{crumb.title}</Link>
                ) : (
                  <>{crumb.title}</>
                )}{" "}
                /{" "}
              </span>
            ))}
          </>
        )}
        {_title}
      </h1>
    </div>
  );
}
