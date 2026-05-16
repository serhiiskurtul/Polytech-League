import api from './axiosInstance';
import { Tournament, PagedResponse, RecentMatch, Champion } from '../types/tournament.types';

export const getTournaments = async (
    page: number = 1, 
    pageSize: number = 10,
    status?: string,
    sport?: string,
    level?: string
): Promise<PagedResponse<Tournament>> => {
    try {
        const response = await api.get<PagedResponse<Tournament>>('/tournaments', {
            params: { 
                page, 
                pageSize,
                status: status !== 'all' ? status : undefined,
                sport: sport !== 'all' ? sport : undefined,
                level: level !== 'all' ? level : undefined
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

export const getChampions = async (sport?: string, gender?: number, weightClass?: string): Promise<Champion[]> => {
    try {
        const response = await api.get<Champion[]>('/tournaments/champions', {
            params: { sport, gender, weightClass: weightClass !== 'all' ? weightClass : undefined }
        });
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні чемпіонів:', error);
        throw error;
    }
};

export const createHallOfFameEntry = async (data: { sport: string; seasonName: string; teamName: string; file?: File | null }) => {
    try {
        const formData = new FormData();
        formData.append('sport', data.sport);
        formData.append('seasonName', data.seasonName);
        formData.append('teamName', data.teamName);
        if (data.file) {
            formData.append('file', data.file);
        }
        const response = await api.post('/halloffame', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
        return response.data;
    } catch (error) {
        console.error('Помилка при створенні запису зали слави:', error);
        throw error;
    }
};
