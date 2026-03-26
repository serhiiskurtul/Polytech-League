import api from './axiosInstance';
import { Tournament } from '../types/tournament.types';

export const getTournaments = async (): Promise<Tournament[]> => {
    try {
        const response = await api.get<Tournament[]>('/api/tournaments');
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні турнірів:', error);
        throw error;
    }
};
