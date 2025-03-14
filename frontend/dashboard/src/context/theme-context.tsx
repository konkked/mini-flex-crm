import React, { createContext, useContext, useEffect, useState } from 'react';
import { getCurrentTheme } from '../api';

const ThemeContext = createContext<string>('professional');

interface ThemeProviderProps {
  children: React.ReactNode;
}

export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
  const [theme, setTheme] = useState<string>('professional');

  useEffect(() => {
    const currentTheme = getCurrentTheme();
    const prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const themeWithMode = prefersDarkMode ? `${currentTheme}-dark` : currentTheme;
    setTheme(themeWithMode);
  }, []);

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  return (
    <ThemeContext.Provider value={theme}>
      {children}
    </ThemeContext.Provider>
  );
};

export const useTheme = () => useContext(ThemeContext);