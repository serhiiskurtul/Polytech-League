import api from '@/shared/api/axiosInstance';
import { 
  footballEventResponse, 
  footballMatchResponse 
} from '@/entities/sport/model/football.types';

export const getFootballEventById = async (id: string | number): Promise<footballEventResponse> => {
  const response = await api.get<footballEventResponse>(`/football-events/${id}`);
  return response.data;
};

export const updateFootballEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/football-events/${id}`, data);
};

export const updateFootballMatch = async (id: number, data: any): Promise<void> => {
  await api.put(`/football-matches/${id}`, data);
};

export const createFootballMatch = async (data: any): Promise<footballMatchResponse> => {
  const response = await api.post<footballMatchResponse>(`/football-matches`, data);
  return response.data;
};

export const createFootballTeam = async (data: any): Promise<any> => {
  const response = await api.post(`/football-teams`, data);
  return response.data;
};

export const deleteFootballMatch = async (id: number): Promise<void> => {
  await api.delete(`/football-matches/${id}`);
};

export const updateFootballTeam = async (id: number, data: any): Promise<void> => {
  await api.put(`/football-teams/${id}`, data);
};

export const registerForFootballEvent = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/football-events/${id}/register?playerId=${playerId}`);
};

export const approveFootballParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/football-events/${id}/participants/${playerId}/approve`);
};

export const rejectFootballParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/football-events/${id}/participants/${playerId}/reject`);
};

export const removeFootballParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.delete(`/football-events/${id}/participants/${playerId}`);
};

export const deleteFootballTeam = async (id: number): Promise<void> => {
  await api.delete(`/football-teams/${id}`);
};
