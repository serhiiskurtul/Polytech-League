import api from './axiosInstance';
import { 
    CheckersEventResponse, 
    CheckersEventRequest, 
    CheckersEventParticipantDto, 
    CheckersGameDto 
} from '../types/checkers.types';

export const getCheckersEventById = async (id: number | string): Promise<CheckersEventResponse> => {
  const response = await api.get<CheckersEventResponse>(`/checkers-events/${id}`);
  return response.data;
};

export const updateCheckersEvent = async (id: number | string, data: CheckersEventRequest): Promise<void> => {
  await api.put(`/checkers-events/${id}`, data);
};

export const addParticipantToCheckersEvent = async (id: number | string, data: CheckersEventParticipantDto): Promise<void> => {
  await api.post(`/checkers-events/${id}/participants`, data);
};

export const removeParticipantFromCheckersEvent = async (id: number | string, playerId: number): Promise<void> => {
  await api.delete(`/checkers-events/${id}/participants/${playerId}`);
};

export const updateParticipantInCheckersEvent = async (id: number | string, playerId: number, data: CheckersEventParticipantDto): Promise<void> => {
  await api.put(`/checkers-events/${id}/participants/${playerId}`, data);
};

export const createCheckersGame = async (data: CheckersGameDto): Promise<void> => {
  await api.post(`/checkers-games`, data);
};

export const uploadCheckersPhoto = async (id: number | string, file: File): Promise<{ url: string }> => {
    const formData = new FormData();
    formData.append('photo', file);
    const response = await api.post<{ url: string }>(`/checkers-events/${id}/photos`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
};

export const deleteCheckersPhoto = async (id: number | string, photoId: number): Promise<void> => {
  await api.delete(`/checkers-events/${id}/photos/${photoId}`);
};

export const createCheckersTeam = async (data: any): Promise<any> => {
    const response = await api.post(`/checkers-teams`, data);
    return response.data;
};

export const updateCheckersTeam = async (id: number, data: any): Promise<void> => {
    await api.put(`/checkers-teams/${id}`, data);
};

export const updateCheckersGame = async (gameId: number | string, data: CheckersGameDto): Promise<void> => {
    await api.put(`/checkers-games/${gameId}`, data);
};

export const deleteCheckersGame = async (gameId: number | string, tournamentId: number | string): Promise<void> => {
    await api.delete(`/checkers-games/${gameId}?tournamentId=${tournamentId}`);
};
