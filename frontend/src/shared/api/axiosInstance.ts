import axios, { AxiosError } from 'axios';

const getApiUrl = (): string => {
  let url = import.meta.env.VITE_API_URL || import.meta.env.VITE_API_BASE_URL || '';
  if (!url) {
    return '';
  }
  if (!url.startsWith('http://') && !url.startsWith('https://') && !url.startsWith('//') && !url.startsWith('/')) {
    url = `https://${url}`;
  }
  if (url.endsWith('/')) {
    url = url.slice(0, -1);
  }
  if (!url.endsWith('/api')) {
    url = `${url}/api`;
  }
  return url;
};

export const API_URL = getApiUrl();
export const BASE_URL = API_URL.endsWith('/api') ? API_URL.slice(0, -4) : API_URL;

const api = axios.create({
  baseURL: API_URL,
  withCredentials: true,
  headers: {
    'Cache-Control': 'no-cache, no-store, must-revalidate',
    Pragma: 'no-cache',
    Expires: '0'
  }
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      if (config.headers && typeof config.headers.set === 'function') {
        config.headers.set('Authorization', `Bearer ${token}`);
      } else {
        config.headers['Authorization'] = `Bearer ${token}`;
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

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

      const currentRefreshToken = localStorage.getItem('refreshToken');

      const performRefresh = async () => {
        try {
          const refreshToken = localStorage.getItem('refreshToken');
          const response = await axios.post(`${API_URL}/refresh`, { refreshToken }, { withCredentials: true });
          
          const { accessToken, refreshToken: newRefreshToken } = response.data;
          localStorage.setItem('token', accessToken);
          localStorage.setItem('refreshToken', newRefreshToken);

          processQueue(null, accessToken);
          return api(originalRequest);
        } catch (refreshError: any) {
          if (refreshError.response && [400, 401, 403].includes(refreshError.response.status)) {
            processQueue(refreshError, null);
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
            window.location.href = '/login';
          } else {
            processQueue(refreshError, null);
          }
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      };

      if (navigator.locks) {
        try {
          const controller = new AbortController();
          const timeoutId = setTimeout(() => controller.abort(), 5000);
          
          return await navigator.locks.request('refresh_token_lock', { signal: controller.signal }, async () => {
            clearTimeout(timeoutId);
            const tokenNow = localStorage.getItem('refreshToken');
            if (tokenNow !== currentRefreshToken) {
              // Another tab already refreshed the token
              isRefreshing = false;
              processQueue(null, localStorage.getItem('token'));
              return api(originalRequest);
            }
            return await performRefresh();
          });
        } catch (lockError: any) {
          // If the lock times out (AbortError) or Web Locks API fails (SecurityError/NotSupportedError in PWA),
          // or if performRefresh itself throws an error, we need to handle it.
          if (lockError.name === 'AbortError' || lockError.name === 'SecurityError' || lockError.name === 'NotSupportedError') {
            console.warn('Web Locks API timed out or failed in PWA context. Falling back to unprotected refresh.', lockError);
            return await performRefresh();
          }
          // Otherwise, it's a real error from performRefresh (e.g. network error, 401), so throw it up
          throw lockError;
        }
      } else {
        return await performRefresh();
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
