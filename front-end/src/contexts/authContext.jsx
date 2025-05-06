import React, { createContext, useContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import { getUserById } from '~/services/user.service';

const AuthContext = createContext({
  user: null,
  userId: null,
  isLoading: true,
  login: () => {},
  logout: () => {},
});

export const useAuthContext = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [userId, setUserId] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const login = async (token) => {
    window.localStorage.setItem('token', token);
    await fetchUser();
  };

  const logout = () => {
    window.localStorage.removeItem('token');
    setUser(null);
    setUserId(null);
    setIsLoading(false);
  };

  const fetchUser = async () => {
    const token = window.localStorage.getItem('token');
    if (!token) {
      setUser(null);
      setUserId(null);
      setIsLoading(false);
      return;
    }

    try {
      const decodedToken = jwtDecode(token);
      const id = decodedToken.sub;
      const sessionUser = await getUserById(id);
      if (!sessionUser) {
        throw new Error('User not found');
      }
      setUser(sessionUser);
      setUserId(id);
    } catch (err) {
      logout();
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchUser();
  }, []);

  const contextValue = {
    user,
    userId,
    isLoading,
    login,
    logout,
  };

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};
