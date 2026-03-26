import api from './axiosInstance';
import { Tournament, PagedResponse } from '../types/tournament.types';

export const getTournaments = async (page: number = 1, pageSize: number = 10): Promise<PagedResponse<Tournament>> => {
    try {
        const response = await api.get<PagedResponse<Tournament>>('/api/tournaments', {
            params: { page, pageSize }
        });
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні турнірів:', error);
        throw error;
    }
};
