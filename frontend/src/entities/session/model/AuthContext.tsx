import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import api from "@/shared/api/axiosInstance";
import { AuthResponse } from '@/entities/session/model/auth.types';
import { subscribeUserToPush } from '@/shared/lib/push/pushService';

type User = AuthResponse['user'];

interface AuthContextType {
  user: User | null;
  login: (userData: User) => void;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
  loading: boolean;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchUser = async () => {
    try {
      const token = localStorage.getItem('token');
      if (token) {
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      }
      
      const response = await api.get("/users/me");
      setUser(response.data);
    } catch (error: any) {
      if (error.response?.status !== 401) {
        console.error(error);
      }
      setUser(null);
    }
  };

  useEffect(() => {
    const initAuth = async () => {
      const refreshToken = localStorage.getItem('refreshToken');
      if (!localStorage.getItem('token') && !refreshToken) {
        setLoading(false);
        return;
      }

      setLoading(true);
      await fetchUser();
      setLoading(false);
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
    localStorage.removeItem('refreshToken');
    delete api.defaults.headers.common['Authorization'];
    setUser(null);
  };

  const refreshUser = async () => {
    await fetchUser();
  };

  const isAuthenticated = !!user; // true — залогінений, false — гість

  return (
    <AuthContext.Provider value={{ user, login, logout, refreshUser, loading, isAuthenticated }}>
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
