import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000',
  withCredentials: true
});

api.interceptors.request.use(
  (config) => {
    return config;
  },
  (error) => Promise.reject(error)
);

// Опціонально — обробка 401
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      console.warn('Токен недійсний або користувач не авторизований');
      // можна додати: localStorage.removeItem('token'); window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;