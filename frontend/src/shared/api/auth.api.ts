import api from "./axiosInstance";
import { LoginCredentials, AuthResponse } from "../types/auth.types";

export const login = async (credentials: LoginCredentials): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>('/login', credentials);
  return response.data; // { accessToken, refreshToken }
};

export const logout = async (refreshToken: string): Promise<void> => {
  await api.post('/logout', {
    refreshToken,
  });
};
