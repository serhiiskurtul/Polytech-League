import api from "./axiosInstance";
import { 
  BadmintonEventResponse, 
  BadmintonMatchResponse 
} from "../types/badminton.types";

export const getBadmintonEventById = async (id: string | number): Promise<BadmintonEventResponse> => {
  const response = await api.get<BadmintonEventResponse>(`/badminton-events/${id}`);
  return response.data;
};

export const updateBadmintonEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/badminton-events/${id}`, data);
};

export const updateBadmintonMatch = async (id: number, data: any): Promise<void> => {
  await api.put(`/badminton-matches/${id}`, data);
};

export const createBadmintonMatch = async (data: any): Promise<BadmintonMatchResponse> => {
  const response = await api.post<BadmintonMatchResponse>(`/badminton-matches`, data);
  return response.data;
};

export const deleteBadmintonMatch = async (id: number): Promise<void> => {
  await api.delete(`/badminton-matches/${id}`);
};

export const createBadmintonTeam = async (data: any): Promise<any> => {
  const response = await api.post(`/badminton-teams`, data);
  return response.data;
};

export const updateBadmintonTeam = async (id: number, data: any): Promise<void> => {
  await api.put(`/badminton-teams/${id}`, data);
};
