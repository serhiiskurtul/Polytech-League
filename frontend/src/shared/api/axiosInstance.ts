import axios, { AxiosError } from 'axios';

export const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://polytech-league-production.up.railway.app';
export const API_URL = `${BASE_URL}/api`;

const api = axios.create({
  baseURL: API_URL,
  withCredentials: true
});

let isRefreshing = false;
let failedQueue: any[] = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

api.interceptors.response.use(
  response => response,
  async (error: AxiosError) => {
    const originalRequest = error.config;
    const status = error.response?.status;

    if (status === 401 && originalRequest && !originalRequest.url?.includes('/login') && !originalRequest.url?.includes('/refresh') && !(originalRequest as any)._retry) {
      if (isRefreshing) {
        return new Promise(function(resolve, reject) {
          failedQueue.push({ resolve, reject });
        })
          .then(() => {
            return api(originalRequest);
          })
          .catch(err => {
            return Promise.reject(err);
          });
      }

      (originalRequest as any)._retry = true;
      isRefreshing = true;

      try {
        await axios.post(`${API_URL}/refresh`, {}, { withCredentials: true });
        processQueue(null);
        return api(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        // Опціонально: перенаправлення на логін або очищення стану
        // window.location.href = '/login';
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    if (
      (status === 401 || status === 403) &&
      originalRequest?.url !== "/users/me"
    ) {
      console.warn('Не авторизовано або заборонено');
    }

    return Promise.reject(error);
  }
);

export default api;
