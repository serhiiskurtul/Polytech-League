import api from './axiosInstance';
import { ChessEventResponse, ChessPlayerDto, ChessEventParticipantDto, ChessGameDto } from '../types/chess.types';

export const getChessEventById = async (id: number | string): Promise<ChessEventResponse> => {
    try {
        const response = await api.get<ChessEventResponse>(`/chess-events/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Помилка при отриманні шахового турніру з id ${id}:`, error);
        throw error;
    }
};

export const createChessPlayer = async (data: ChessPlayerDto): Promise<ChessPlayerDto> => {
    const res = await api.post<ChessPlayerDto>('/chess-players', data);
    return res.data;
};

export const getAllChessPlayers = async (): Promise<ChessPlayerDto[]> => {
    try {
        const response = await api.get<ChessPlayerDto[]>('/chess-players');
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні всіх шахових гравців:', error);
        throw error;
    }
};

export const addParticipantToEvent = async (eventId: number | string, data: ChessEventParticipantDto): Promise<void> => {
    await api.post(`/chess-events/${eventId}/participants`, data);
};

export const removeParticipantFromEvent = async (eventId: number | string, playerId: number | string): Promise<void> => {
    await api.delete(`/chess-events/${eventId}/participants/${playerId}`);
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
