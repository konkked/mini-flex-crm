import { useState, useEffect } from 'react';
import { getUserFromToken, logout as logoutUser } from '../lib/svc/auth';

export const useAuth = () => {
  const [user, setUser] = useState(getUserFromToken());
  const [isAuthenticated, setIsAuthenticated] = useState(!!user);

  useEffect(() => {
    setUser(getUserFromToken());
    setIsAuthenticated(!!user);
  }, []);

  const logout = () => {
    logoutUser();
    setUser(null);
    setIsAuthenticated(false);
  };

  return { user, isAuthenticated, logout };
};
