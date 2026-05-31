import api from '@/shared/api/axiosInstance';

export interface UserResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: number | string;
  isBlocked: boolean;
  isDeleted: boolean;
  createdAt: string;
  isEmailConfirmed: boolean;
  lastVisitAt?: string;
}

export interface PagedResponse<T> {
  data: T[];
  totalCount: number;
}

export const getUsers = async (
  page = 1, 
  pageSize = 10, 
  search = '', 
  orderBy?: string, 
  sortDirection?: 'Asc' | 'Desc'
): Promise<PagedResponse<UserResponse>> => {
  const response = await api.get<PagedResponse<UserResponse>>('/users', {
    params: {
      Page: page,
      PageSize: pageSize,
      Name: search || undefined,
      OrderBy: orderBy,
      SortDirection: sortDirection,
    }
  });
  return response.data;
};

export const searchUsers = async (query: string): Promise<UserResponse[]> => {
  const response = await api.get<PagedResponse<UserResponse>>('/users', {
    params: {
      Name: query,
    }
  });
  return response.data.data;
};

export const blockUser = async (id: number): Promise<void> => {
  await api.post(`/users/block/${id}`);
};

export const unblockUser = async (id: number): Promise<void> => {
  await api.post(`/users/unblock/${id}`);
};

export const deleteUser = async (id: number): Promise<void> => {
  await api.delete(`/users/hard-delete/${id}`);
};
