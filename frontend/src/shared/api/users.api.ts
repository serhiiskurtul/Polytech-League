import api from "./axiosInstance";

export interface UserResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: number;
}

export interface PagedResponse<T> {
  data: T[];
  totalCount: number;
}

export const searchUsers = async (query: string): Promise<UserResponse[]> => {
  const response = await api.get<PagedResponse<UserResponse>>('/users', {
    params: {
      'filter.FirstName': query,
      // 'filter.LastName': query, // API might not support multiple filters for one query easily
      // 'filter.Email': query
    }
  });
  return response.data.data;
};
