import { useState, useEffect } from 'react';

export const useDebounce = <K>(value: K, delay: number): K => {
  const [debouncedValue, setDebouncedValue] = useState<K>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
};

export default useDebounce;
