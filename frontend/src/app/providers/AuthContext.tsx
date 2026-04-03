import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import api from "../../shared/api/axiosInstance";
import { AuthResponse } from "../../shared/types/auth.types";

type User = AuthResponse['user'];

interface AuthContextType {
  user: User | null;
  login: (userData: User) => void;
  logout: () => Promise<void>;
  loading: boolean;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      try {
        const response = await api.get("/users/me");
        setUser(response.data);
      } catch (error: any) {
        if (error.response?.status !== 401) {
          console.error(error);
        }
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = (userData: User) => {
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

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
