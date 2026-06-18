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

export const forgotPassword = async (email: string): Promise<void> => {
  await api.post('/auth/forgot-password', { email });
};

export const verifyResetCode = async (data: { email: string; code: string; }): Promise<void> => {
  await api.post('/auth/verify-reset-code', data);
};

export const resetPassword = async (data: { email: string; code: string; newPassword: string; }): Promise<void> => {
  await api.post('/auth/reset-password', data);
};
