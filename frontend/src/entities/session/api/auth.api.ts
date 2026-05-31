import api from '@/shared/api/axiosInstance';
import { LoginCredentials, AuthResponse } from '@/entities/session/model/auth.types';

export const login = async (credentials: LoginCredentials): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>('/login', credentials);
  return response.data; // { accessToken, refreshToken }
};

export const logout = async (refreshToken: string): Promise<void> => {
  await api.post('/logout', {
    refreshToken,
  });
};
