import api from "./axiosInstance";
import { 
  BeachVolleyballEventResponse, 
  BeachVolleyballMatchResponse 
} from "../types/beachVolleyball.types";

export const getBeachVolleyballEventById = async (id: string | number): Promise<BeachVolleyballEventResponse> => {
  const response = await api.get<BeachVolleyballEventResponse>(`/beach-volleyball-events/${id}`);
  return response.data;
};

export const updateBeachVolleyballEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/beach-volleyball-events/${id}`, data);
};

export const updateBeachVolleyballMatch = async (id: number, data: any): Promise<void> => {
  await api.put(`/beach-volleyball-matches/${id}`, data);
};

export const createBeachVolleyballMatch = async (data: any): Promise<BeachVolleyballMatchResponse> => {
  const response = await api.post<BeachVolleyballMatchResponse>(`/beach-volleyball-matches`, data);
  return response.data;
};

export const createBeachVolleyballTeam = async (data: any): Promise<any> => {
  const response = await api.post(`/beach-volleyball-teams`, data);
  return response.data;
};

export const deleteBeachVolleyballMatch = async (id: number): Promise<void> => {
  await api.delete(`/beach-volleyball-matches/${id}`);
};

export const updateBeachVolleyballTeam = async (id: number, data: any): Promise<void> => {
  await api.put(`/beach-volleyball-teams/${id}`, data);
};

export const registerForBeachVolleyballEvent = async (id: string | number, playerId: number): Promise<void> => {
  await api.post(`/beach-volleyball-events/${id}/register?playerId=${playerId}`);
};
