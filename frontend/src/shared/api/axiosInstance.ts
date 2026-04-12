import axios, { AxiosError } from 'axios';

export const BASE_URL = 'http://localhost:5000';
export const API_URL = `${BASE_URL}/api`;

const api = axios.create({
  baseURL: API_URL,
  withCredentials: true
});

api.interceptors.response.use(
  response => response,
  (error: AxiosError) => {
    const status = error.response?.status;
    const url = error.config?.url;
    if (
      (status === 401 || status === 403) &&
      url !== "/users/me"
    ) {
      console.warn('Не авторизовано або заборонено');
    }

    return Promise.reject(error);
  }
);

export default api;
