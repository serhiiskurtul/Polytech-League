import api from './axiosInstance';
import { TableTennisEventResponse, TableTennisEventParticipantDto } from '@/shared/types/tabletennis.types';

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

export const removeParticipantFromTableTennisEvent = async (eventId: string | number, playerId: number) => {
  const response = await api.delete(`/table-tennis-events/${eventId}/participants/${playerId}`);
  return response.data;
};

export const updateTableTennisParticipant = async (eventId: string | number, playerId: number, dto: TableTennisEventParticipantDto) => {
  const response = await api.put(`/table-tennis-events/${eventId}/participants/${playerId}`, dto);
  return response.data;
};
