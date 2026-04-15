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
