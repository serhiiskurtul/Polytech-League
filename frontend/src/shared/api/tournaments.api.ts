import api from './axiosInstance';
import { Tournament, PagedResponse, RecentMatch } from '../types/tournament.types';

export const getTournaments = async (
    page: number = 1, 
    pageSize: number = 10,
    status?: string,
    sport?: string
): Promise<PagedResponse<Tournament>> => {
    try {
        const response = await api.get<PagedResponse<Tournament>>('/tournaments', {
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

export const getRecentResults = async (count: number = 4, sport?: string): Promise<RecentMatch[]> => {
    try {
        const response = await api.get<RecentMatch[]>('/tournaments/recent-results', {
            params: { count, sport }
        });
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні останніх результатів:', error);
        throw error;
    }
};

export const getTournamentById = async (id: string): Promise<Tournament> => {
    try {
        const response = await api.get<Tournament>(`/tournaments/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Помилка при отриманні турніру з id ${id}:`, error);
        throw error;
    }
};

export interface TournamentStats {
    activeTournamentsCount: number;
    registeredPlayersCount: number;
    totalMatchesCount: number;
    seasonProgress: number;
    topSport: string;
}

export const getTournamentsStats = async (): Promise<TournamentStats> => {
    try {
        const response = await api.get<TournamentStats>('/tournaments/stats');
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні статистики турнірів:', error);
        throw error;
    }
};
