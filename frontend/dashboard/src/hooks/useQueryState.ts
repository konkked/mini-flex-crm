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

/**
 * Hook to manage query parameters in URL without overwriting existing ones.
 * @param key The query parameter key.
 * @param parse Function to parse the query parameter value.
 * @param serialize Function to serialize the value to a string.
 * @returns [value, setValue] to get and update the query parameter.
 */
export const useGenQueryState = <T>(
  key: string,
  parse: (value: string | undefined) => T | undefined,
  serialize: (value: T | undefined) => string | undefined
) => {
  const [searchParams, setSearchParams] = useSearchParams();

  // Get the current value of the key and parse it
  const value = searchParams.get(key);
  const parsedValue = value ? parse(value) : null;

  // Function to update only the specified key, keeping others unchanged
  const setValue = (newValue: T | null) => {
    const newParams = new URLSearchParams(searchParams);
    if (newValue === null || newValue === undefined) {
      newParams.delete(key);
    } else {
      newParams.set(key, serialize(newValue) ?? "");
    }
    setSearchParams(newParams, { replace: true });
  };

  return [parsedValue, setValue] as const;
};



/**
 * Hook to manage query parameters in URL without overwriting existing ones.
 * Allows batch updates and removes `null` or `undefined` parameters.
 * @param parsers Object where keys are query parameter names and values are parsing functions.
 * @param serializers Object where keys are query parameter names and values are serialization functions.
 * @returns [values, setValues] to get and update query parameters.
 */
export const useGenQueryParamsState = <T extends Record<string, any>>(
  parsers: { [K in keyof T]: (value: string | undefined) => T[K] | undefined },
  serializers: { [K in keyof T]: (value: T[K] | undefined) => string | undefined }
) => {
  const [searchParams, setSearchParams] = useSearchParams();

  // Extract and parse all relevant query params
  const values = Object.keys(parsers).reduce((acc, key) => {
    const value = searchParams.get(key);
    acc[key as keyof T] = value ? parsers[key as keyof T](value) : undefined;
    return acc;
  }, {} as Partial<T>);

  // Function to update multiple query parameters at once
  const setValues = (newValues: Partial<T>) => {
    const newParams = new URLSearchParams(searchParams);

    Object.entries(newValues).forEach(([key, value]) => {
      if (value === null || value === undefined) {
        newParams.delete(key);
      } else {
        newParams.set(key, serializers[key as keyof T](value) ?? "");
      }
    });

    setSearchParams(newParams, { replace: true });
  };

  return [values as T, setValues] as const;
};
