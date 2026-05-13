import api from "./axiosInstance";
import { 
  footballEventResponse, 
  footballMatchResponse 
} from "../types/football.types";

export const getfootballEventById = async (id: string | number): Promise<footballEventResponse> => {
  const response = await api.get<footballEventResponse>(`/football-events/${id}`);
  return response.data;
};

export const updatefootballEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/football-events/${id}`, data);
};

export const updatefootballMatch = async (id: number, data: any): Promise<void> => {
  await api.put(`/football-matches/${id}`, data);
};

export const createfootballMatch = async (data: any): Promise<footballMatchResponse> => {
  const response = await api.post<footballMatchResponse>(`/football-matches`, data);
  return response.data;
};

export const createfootballTeam = async (data: any): Promise<any> => {
  const response = await api.post(`/football-teams`, data);
  return response.data;
};

export const deletefootballMatch = async (id: number): Promise<void> => {
  await api.delete(`/football-matches/${id}`);
};

export const updatefootballTeam = async (id: number, data: any): Promise<void> => {
  await api.put(`/football-teams/${id}`, data);
};

export const registerForfootballEvent = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/football-events/${id}/register?playerId=${playerId}`);
};

export const approvefootballParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/football-events/${id}/participants/${playerId}/approve`);
};

export const rejectfootballParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/football-events/${id}/participants/${playerId}/reject`);
};

export const removefootballParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.delete(`/football-events/${id}/participants/${playerId}`);
};

export const deletefootballTeam = async (id: number): Promise<void> => {
  await api.delete(`/football-teams/${id}`);
};
