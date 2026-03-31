import { faSearch } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useState } from "react";

export const BarSearch = () => {
  const [searchText, setSearchText] = useState<string>("");
  const [focused, setFocused] = useState<boolean>(false);

  return (
    <form className="form-inline hbar-search">
      <div className="input-group">
        <input
          type="text"
          className={
            focused
              ? "form-control border-primary border-1 small"
              : "form-control border-tertiary border-1 small"
          }
          placeholder="Search for..."
          aria-label="Search"
          aria-describedby="basic-addon2"
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          onFocus={() => setFocused(true)}
          onBlur={() => setFocused(false)}
        />
        <div className="input-group-append">
          <button
            className={focused ? "btn btn-primary" : "btn btn-tertiary"}
            type="button"
          >
            <FontAwesomeIcon icon={faSearch} size="sm" />
          </button>
        </div>
      </div>
    </form>
  );
};
