import api from './axiosInstance';
import { Tournament, PagedResponse } from '../types/tournament.types';

export const getTournaments = async (
    page: number = 1, 
    pageSize: number = 10,
    status?: string,
    sport?: string
): Promise<PagedResponse<Tournament>> => {
    try {
        const response = await api.get<PagedResponse<Tournament>>('/api/tournaments', {
            params: { 
                page, 
                pageSize,
                status: status !== 'all' ? status : undefined,
                sport: sport !== 'all' ? sport : undefined
            }
        });
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні турнірів:', error);
        throw error;
    }
};

export const getTournamentById = async (id: string): Promise<Tournament> => {
    try {
        const response = await api.get<Tournament>(`/api/tournaments/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Помилка при отриманні турніру з id ${id}:`, error);
        throw error;
    }
};
