import { Link } from "react-router-dom";
import { Logo } from "../../components/Logo";
import { layoutConfig } from "../../layoutConfig";
import { useSettingsContext } from "../../contexts/UseContexts";

type BrandProps = {
  className?: string;
};

export const Brand = ({ className }: BrandProps) => {
  const { toggleSidebar } = useSettingsContext();

  return (
    <>
      {layoutConfig.includeSidebar ? (
        <>
          <a
            className={
              "hbar-brand-icon d-flex align-items-center justify-content-center " +
              (className ?? "")
            }
            onClick={(e) => {
              e.preventDefault();
              toggleSidebar();
            }}
          >
            <div className="my-lg-0 mw-100">
              <Logo size={30} />
            </div>
          </a>
        </>
      ) : (
        <>
          <Link
            to="/"
            className={
              "hbar-brand-icon d-flex align-items-center justify-content-center " +
              (className ?? "")
            }
          >
            <div className="my-lg-0 mw-100">
              <Logo size={30} />
            </div>
          </Link>
        </>
      )}
    </>
  );
};
