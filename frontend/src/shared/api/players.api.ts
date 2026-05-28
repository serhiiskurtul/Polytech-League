import api from './axiosInstance';
import { PlayerDto } from '../types/player.types';

export const getAllPlayers = async (): Promise<PlayerDto[]> => {
    const response = await api.get<PlayerDto[]>('/players');
    return response.data;
};

export const getPlayerById = async (id: number | string): Promise<PlayerDto> => {
    const response = await api.get<PlayerDto>(`/players/${id}`);
    return response.data;
};

export const getProfile = async (): Promise<PlayerDto> => {
    const response = await api.get<PlayerDto>('/players/profile');
    return response.data;
};

export const createPlayer = async (data: PlayerDto): Promise<PlayerDto> => {
    const response = await api.post<PlayerDto>('/players', data);
    return response.data;
};

export const updatePlayer = async (id: number | string, data: PlayerDto): Promise<void> => {
    await api.put(`/players/${id}`, data);
};

export const updateProfile = async (data: PlayerDto): Promise<PlayerDto> => {
    const response = await api.put<PlayerDto>('/players/profile', data);
    return response.data;
};



export const deletePlayer = async (id: number | string): Promise<void> => {
    await api.delete(`/players/${id}`);
};

export const searchPlayers = async (query: string): Promise<PlayerDto[]> => {
    const response = await api.get<PlayerDto[]>(`/players/search?query=${query}`);
    return response.data;
};

export const uploadAvatar = async (file: File): Promise<string> => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post<{ photoUrl: string }>('/players/profile/avatar', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data.photoUrl;
};

export const getPlayersRankings = async (seasonId?: number | string): Promise<PlayerDto[]> => {
    const url = seasonId ? `/players/rankings?seasonId=${seasonId}` : '/players/rankings';
    const response = await api.get<PlayerDto[]>(url);
    return response.data;
};

// Profile Merging
export const checkArchivalMatches = async (firstName: string, lastName: string, group: string): Promise<PlayerDto[]> => {
    const response = await api.get<PlayerDto[]>(`/players/check-archival?firstName=${firstName}&lastName=${lastName}&group=${group}`);
    return response.data;
};

export const createMergeRequest = async (targetPlayerId: number): Promise<void> => {
    await api.post(`/players/merge-request?targetPlayerId=${targetPlayerId}`);
};

export const getPendingMergeRequests = async (): Promise<any[]> => {
    const response = await api.get<any[]>('/players/merge-requests');
    return response.data;
};

export const approveMergeRequest = async (id: number): Promise<void> => {
    await api.post(`/players/merge-requests/${id}/approve`);
};

export const rejectMergeRequest = async (id: number, comment: string): Promise<void> => {
    await api.post(`/players/merge-requests/${id}/reject?comment=${comment}`);
};

export const ignoreMergeRequest = async (targetPlayerId: number): Promise<void> => {
    await api.post(`/players/merge-requests/ignore?targetPlayerId=${targetPlayerId}`);
};

export const getAllMergeRequests = async (): Promise<any[]> => {
    const response = await api.get<any[]>('/players/merge-requests/all');
    return response.data;
};

export const getHeadToHead = async (p1: number | string, p2: number | string): Promise<any> => {
    const response = await api.get(`/players/head-to-head?p1=${p1}&p2=${p2}`);
    return response.data;
};

export const mergePlayersManually = async (sourcePlayerId: number, targetPlayerId: number): Promise<void> => {
    await api.post(`/players/merge-manual?sourcePlayerId=${sourcePlayerId}&targetPlayerId=${targetPlayerId}`);
};

