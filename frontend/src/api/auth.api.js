import api from "./axiosInstance";

export const login = async (email, password) => {
  const response = await api.post('/login', {
    email,
    password,
  });

  return response.data; // { accessToken, refreshToken }
};

export const logout = async (refreshToken) => {
  await api.post('/logout', {
    refreshToken,
  });
};
