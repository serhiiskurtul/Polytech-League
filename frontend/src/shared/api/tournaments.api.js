import api from './axiosInstance';

export const getTournaments = async () => {
    try {
        const response = await api.get('/api/tournaments');
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні турнірів:', error);
        throw error;
    }
};
