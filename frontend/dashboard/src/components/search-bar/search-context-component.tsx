import React, { createContext, useContext, useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

const SearchContext = createContext<any>(null);

export const useSearch = () => useContext(SearchContext);

export const SearchProvider = ({ children }: { children: React.ReactNode }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useState(new URLSearchParams(location.search));

  useEffect(() => {
    setSearchParams(new URLSearchParams(location.search));
  }, [location.search]);

  const updateSearchQuery = (newQuery: Record<string, string>) => {
    const params = new URLSearchParams(searchParams);
    Object.entries(newQuery).forEach(([key, value]) => {
      if (value) params.set(key, value);
      else params.delete(key);
    });

    navigate({ search: params.toString() }, { replace: true });
  };

  const getQueryValue = (key: string) => searchParams.get(key) || "";

  return (
    <SearchContext.Provider value={{ updateSearchQuery, getQueryValue }}>
      {children}
    </SearchContext.Provider>
  );
};
