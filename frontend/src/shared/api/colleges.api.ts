import api from './axiosInstance';

export interface College {
  id: number;
  name: string;
  shortName: string;
  logoUrl?: string;
  city?: string;
}

export const getColleges = async (): Promise<College[]> => {
  const response = await api.get<College[]>('/Colleges');
  return response.data;
};

export const getCollegeById = async (id: number): Promise<College> => {
  const response = await api.get<College>(`/Colleges/${id}`);
  return response.data;
};

export const createCollege = async (college: Partial<College>): Promise<College> => {
  const response = await api.post<College>('/Colleges', college);
  return response.data;
};

export const updateCollege = async (id: number, college: Partial<College>): Promise<College> => {
  const response = await api.put<College>(`/Colleges/${id}`, college);
  return response.data;
};

export const deleteCollege = async (id: number): Promise<void> => {
  await api.delete(`/Colleges/${id}`);
};

export const uploadCollegeLogo = async (id: number, file: File): Promise<string> => {
  const formData = new FormData();
  formData.append('file', file);
  const response = await api.post<{ url: string }>(`/Colleges/${id}/upload-logo`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
  return response.data.url;
};
