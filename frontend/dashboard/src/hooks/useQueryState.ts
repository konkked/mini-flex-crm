import { useSearchParams } from "react-router-dom";

/**
 * Hook to manage query parameters in URL without overwriting existing ones.
 * @param key The query parameter key.
 * @returns [value, setValue] to get and update the query parameter.
 */
export const useQueryState = (key: string) => {
  const [searchParams, setSearchParams] = useSearchParams();

  // Get the current value of the key
  const value = searchParams.get(key) || "";

  // Function to update only the specified key, keeping others unchanged
  const setValue = (newValue: string | null) => {
    const newParams = new URLSearchParams(searchParams);
    if (newValue) {
      newParams.set(key, newValue);
    } else {
      newParams.delete(key);
    }
    setSearchParams(newParams, { replace: true });
  };

  return [value, setValue] as const;
};
