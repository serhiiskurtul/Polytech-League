import api from "./axiosInstance";
import { 
  VolleyballEventResponse, 
  VolleyballMatchResponse 
} from "../types/volleyball.types";

export const getVolleyballEventById = async (id: string | number): Promise<VolleyballEventResponse> => {
  const response = await api.get<VolleyballEventResponse>(`/volleyball-events/${id}`);
  return response.data;
};

export const updateVolleyballEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/volleyball-events/${id}`, data);
};

export const updateVolleyballMatch = async (id: number, data: any): Promise<void> => {
  await api.put(`/volleyball-matches/${id}`, data);
};

export const createVolleyballMatch = async (data: any): Promise<VolleyballMatchResponse> => {
  const response = await api.post<VolleyballMatchResponse>(`/volleyball-matches`, data);
  return response.data;
};

export const createVolleyballTeam = async (data: any): Promise<any> => {
  const response = await api.post(`/volleyball-teams`, data);
  return response.data;
};

export const updateVolleyballTeam = async (id: number, data: any): Promise<void> => {
  await api.put(`/volleyball-teams/${id}`, data);
};

export const registerForVolleyballEvent = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/volleyball-events/${id}/register?playerId=${playerId}`);
};

export const deleteVolleyballMatch = async (id: number): Promise<void> => {
  await api.delete(`/volleyball-matches/${id}`);
};
