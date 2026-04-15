export interface LoginCredentials {
  email: string;
  password?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user?: {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    role: number | string;
    createdAt: string;
  };
}
