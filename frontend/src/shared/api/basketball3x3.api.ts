import api from "./axiosInstance";
import { 
  Basketball3x3EventResponse, 
  Basketball3x3MatchResponse 
} from "../types/basketball3x3.types";

export const getBasketball3x3EventById = async (id: string | number): Promise<Basketball3x3EventResponse> => {
  const response = await api.get<Basketball3x3EventResponse>(`/basketball3x3-events/${id}`);
  return response.data;
};

export const updateBasketball3x3Event = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/basketball3x3-events/${id}`, data);
};

export const updateBasketball3x3Match = async (id: number, data: any): Promise<void> => {
  await api.put(`/basketball3x3-matches/${id}`, data);
};

export const createBasketball3x3Match = async (data: any): Promise<Basketball3x3MatchResponse> => {
  const response = await api.post<Basketball3x3MatchResponse>(`/basketball3x3-matches`, data);
  return response.data;
};

export const createBasketball3x3Team = async (data: any): Promise<any> => {
  const response = await api.post(`/basketball3x3-teams`, data);
  return response.data;
};

export const deleteBasketball3x3Match = async (id: number): Promise<void> => {
  await api.delete(`/basketball3x3-matches/${id}`);
};

export const updateBasketball3x3Team = async (id: number, data: any): Promise<void> => {
  await api.put(`/basketball3x3-teams/${id}`, data);
};

export const registerForBasketball3x3Event = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/basketball3x3-events/${id}/register?playerId=${playerId}`);
};

export const approveBasketball3x3Participant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/basketball3x3-events/${id}/participants/${playerId}/approve`);
};

export const rejectBasketball3x3Participant = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/basketball3x3-events/${id}/participants/${playerId}/reject`);
};

export const removeBasketball3x3Participant = async (id: string | number, playerId: number): Promise<void> => {
  await api.delete(`/basketball3x3-events/${id}/participants/${playerId}`);
};

export const deleteBasketball3x3Team = async (id: number): Promise<void> => {
  await api.delete(`/basketball3x3-teams/${id}`);
};
