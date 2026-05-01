import api from "./axiosInstance";
import { 
  FutsalEventResponse, 
  FutsalMatchResponse 
} from "../types/futsal.types";

export const getFutsalEventById = async (id: string | number): Promise<FutsalEventResponse> => {
  const response = await api.get<FutsalEventResponse>(`/futsal-events/${id}`);
  return response.data;
};

export const updateFutsalEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/futsal-events/${id}`, data);
};

export const updateFutsalMatch = async (id: number, data: any): Promise<void> => {
  await api.put(`/futsal-matches/${id}`, data);
};

export const createFutsalMatch = async (data: any): Promise<FutsalMatchResponse> => {
  const response = await api.post<FutsalMatchResponse>(`/futsal-matches`, data);
  return response.data;
};

export const createFutsalTeam = async (data: any): Promise<any> => {
  const response = await api.post(`/futsal-teams`, data);
  return response.data;
};

export const deleteFutsalMatch = async (id: number): Promise<void> => {
  await api.delete(`/futsal-matches/${id}`);
};

export const updateFutsalTeam = async (id: number, data: any): Promise<void> => {
  await api.put(`/futsal-teams/${id}`, data);
};

export const registerForFutsalEvent = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/futsal-events/${id}/register?playerId=${playerId}`);
};

export const approveFutsalParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/futsal-events/${id}/participants/${playerId}/approve`);
};

export const rejectFutsalParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/futsal-events/${id}/participants/${playerId}/reject`);
};

export const removeFutsalParticipant = async (id: string | number, playerId: number): Promise<void> => {
  await api.delete(`/futsal-events/${id}/participants/${playerId}`);
};

export const deleteFutsalTeam = async (id: number): Promise<void> => {
  await api.delete(`/futsal-teams/${id}`);
};
