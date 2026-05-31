import api from '@/shared/api/axiosInstance';
import { TableTennisEventResponse, TableTennisEventParticipantDto } from '@/entities/sport/model/tabletennis.types';

export const getTableTennisEventById = async (id: string | number): Promise<TableTennisEventResponse> => {
  const response = await api.get<TableTennisEventResponse>(`/table-tennis-events/${id}`);
  return response.data;
};

export const updateTableTennisEvent = async (id: string | number, data: any) => {
  const response = await api.put(`/table-tennis-events/${id}`, data);
  return response.data;
};

export const addParticipantToTableTennisEvent = async (eventId: string | number, dto: TableTennisEventParticipantDto) => {
  const response = await api.post(`/table-tennis-events/${eventId}/participants`, dto);
  return response.data;
};

export const removeTableTennisParticipant = async (eventId: string | number, playerId: number) => {
  const response = await api.delete(`/table-tennis-events/${eventId}/participants/${playerId}`);
  return response.data;
};

export const updateTableTennisParticipant = async (eventId: string | number, playerId: number, dto: TableTennisEventParticipantDto) => {
  const response = await api.put(`/table-tennis-events/${eventId}/participants/${playerId}`, dto);
  return response.data;
};

export const registerForTableTennisEvent = async (eventId: string | number, playerId: number) => {
  const response = await api.post(`/table-tennis-events/${eventId}/register?playerId=${playerId}`);
  return response.data;
};

export const approveTableTennisParticipant = async (eventId: string | number, playerId: number) => {
  const response = await api.post(`/table-tennis-events/${eventId}/participants/${playerId}/approve`);
  return response.data;
};

export const rejectTableTennisParticipant = async (eventId: string | number, playerId: number) => {
  const response = await api.post(`/table-tennis-events/${eventId}/participants/${playerId}/reject`);
  return response.data;
};
// Matches
export const createTableTennisMatch = async (data: any) => {
  const response = await api.post('/table-tennis-matches', data);
  return response.data;
};

export const updateTableTennisMatch = async (id: number, data: any) => {
  const response = await api.put(`/table-tennis-matches/${id}`, data);
  return response.data;
};

export const deleteTableTennisMatch = async (id: number) => {
  const response = await api.delete(`/table-tennis-matches/${id}`);
  return response.data;
};

// Teams
export const createTableTennisTeam = async (data: any) => {
  const response = await api.post('/table-tennis-teams', data);
  return response.data;
};

export const updateTableTennisTeam = async (id: number, data: any) => {
  const response = await api.put(`/table-tennis-teams/${id}`, data);
  return response.data;
};

export const deleteTableTennisTeam = async (id: number) => {
  const response = await api.delete(`/table-tennis-teams/${id}`);
  return response.data;
};
