import api from './axiosInstance';
import { ChessEventResponse, ChessEventParticipantDto, ChessGameDto, ChessEventRequest } from '@/shared/types/chess.types';

export const getChessEventById = async (id: number | string): Promise<ChessEventResponse> => {
    try {
        const response = await api.get<ChessEventResponse>(`/chess-events/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Помилка при отриманні шахового турніру з id ${id}:`, error);
        throw error;
    }
};

export const updateChessEvent = async (id: number | string, data: ChessEventRequest): Promise<void> => {
    await api.put(`/chess-events/${id}`, data);
};

export const uploadTournamentPhoto = async (id: number | string, file: File): Promise<{ url: string }> => {
    const formData = new FormData();
    formData.append('photo', file);
    const response = await api.post<{ url: string }>(`/chess-events/${id}/photos`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data;
};

export const deleteTournamentPhoto = async (eventId: number | string, photoId: number | string): Promise<void> => {
    await api.delete(`/chess-events/${eventId}/photos/${photoId}`);
};

// Player-related methods removed in favor of unified players.api.ts

export const addParticipantToEvent = async (eventId: number | string, data: ChessEventParticipantDto): Promise<void> => {
    await api.post(`/chess-events/${eventId}/participants`, data);
};

export const removeParticipantFromEvent = async (eventId: number | string, playerId: number | string): Promise<void> => {
    await api.delete(`/chess-events/${eventId}/participants/${playerId}`);
};

export const updateParticipantInEvent = async (eventId: number | string, playerId: number | string, data: ChessEventParticipantDto): Promise<void> => {
    await api.put(`/chess-events/${eventId}/participants/${playerId}`, data);
};

export const createChessGame = async (data: ChessGameDto): Promise<void> => {
    await api.post('/chess-games', data);
};

export const deleteChessGame = async (gameId: number | string, tournamentId: number | string): Promise<void> => {
    await api.delete(`/chess-games/${gameId}?tournamentId=${tournamentId}`);
};

export const updateChessGame = async (gameId: number | string, data: ChessGameDto): Promise<void> => {
    await api.put(`/chess-games/${gameId}`, data);
};

export const createChessTeam = async (data: any): Promise<any> => {
    const response = await api.post(`/chess-teams`, data);
    return response.data;
};

export const updateChessTeam = async (id: number, data: any): Promise<void> => {
    await api.put(`/chess-teams/${id}`, data);
};

export const importDataFromUrl = async (eventId: number | string, url: string): Promise<void> => {
    await api.post(`/chess-events/${eventId}/import?url=${encodeURIComponent(url)}`);
};
export const importDataFromFile = async (eventId: number | string, file: File): Promise<void> => {
    const formData = new FormData();
    formData.append('file', file);
    await api.post(`/chess-events/${eventId}/import-file`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
};

export const registerForChessEvent = async (eventId: number | string, playerId: number): Promise<void> => {
    await api.post(`/chess-events/${eventId}/register?playerId=${playerId}`);
};

export const approveChessParticipant = async (eventId: number | string, playerId: number): Promise<void> => {
    await api.post(`/chess-events/${eventId}/participants/${playerId}/approve`);
};

export const rejectChessParticipant = async (eventId: number | string, playerId: number): Promise<void> => {
    await api.post(`/chess-events/${eventId}/participants/${playerId}/reject`);
};

export const deleteChessTeam = async (id: number): Promise<void> => {
    await api.delete(`/chess-teams/${id}`);
};
