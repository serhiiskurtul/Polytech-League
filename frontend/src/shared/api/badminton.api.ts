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

export const addParticipantToBadmintonEvent = async (eventId: string | number, data: any): Promise<void> => {
  await api.post(`/badminton-events/${eventId}/participants`, data);
};

export const removeParticipantFromBadmintonEvent = async (eventId: string | number, playerId: number): Promise<void> => {
  await api.delete(`/badminton-events/${eventId}/participants/${playerId}`);
};

export const updateBadmintonParticipant = async (eventId: string | number, playerId: number, data: any): Promise<void> => {
  await api.put(`/badminton-events/${eventId}/participants/${playerId}`, data);
};

export const registerForBadmintonEvent = async (eventId: string | number, playerId: number): Promise<void> => {
  await api.post(`/badminton-events/${eventId}/register?playerId=${playerId}`);
};

export const approveBadmintonParticipant = async (eventId: string | number, playerId: number): Promise<void> => {
  await api.post(`/badminton-events/${eventId}/participants/${playerId}/approve`);
};

export const rejectBadmintonParticipant = async (eventId: string | number, playerId: number): Promise<void> => {
  await api.post(`/badminton-events/${eventId}/participants/${playerId}/reject`);
};

export const deleteBadmintonTeam = async (id: number): Promise<void> => {
  await api.delete(`/badminton-teams/${id}`);
};
  
