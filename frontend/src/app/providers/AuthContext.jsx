import { createContext, useContext, useEffect, useState } from "react";
import api from "../../shared/api/axiosInstance.js";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      try {
        const response = await api.get("/users/me");
        setUser(response.data);
      } catch (error) {
        setUser(null); // гість — user = null
      } finally {
        setLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = (userData) => {
    setUser(userData);
  };

  const logout = async () => {
    try {
      await api.post("/logout");
    } catch { }

    localStorage.removeItem('token');
    setUser(null);
  };

  const isAuthenticated = !!user; // true — залогінений, false — гість

  return (
    <AuthContext.Provider value={{ user, login, logout, loading, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);