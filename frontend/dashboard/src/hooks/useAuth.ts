import { useState, useEffect } from 'react';
import api, { getCurrentUser, setAuthToken } from '../api';

export const useAuth = () => {
  const [user, setUser] = useState(getCurrentUser());
  const [isAuthenticated, setIsAuthenticated] = useState(!!user);

  useEffect(() => {
    setUser(getCurrentUser());
    setIsAuthenticated(!!user);
  }, []);

  const logout = () => {
    setAuthToken(null);
    setUser(null);
    setIsAuthenticated(false);
  };

  return { user, isAuthenticated, logout };
};
