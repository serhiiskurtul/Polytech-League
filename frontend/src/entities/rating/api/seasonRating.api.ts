import api from '@/shared/api/axiosInstance';
import { Season, SeasonRatingResponse } from '@/entities/rating/model/seasonRating.types';

export const getSeasons = async (): Promise<Season[]> => {
  const response = await api.get<Season[]>('/SeasonRating/seasons');
  return response.data;
};

export const getCurrentSeason = async (): Promise<Season> => {
  const response = await api.get<Season>('/SeasonRating/current');
  return response.data;
};

export const getSeasonRating = async (seasonId: number): Promise<SeasonRatingResponse> => {
  const response = await api.get<SeasonRatingResponse>(`/SeasonRating/${seasonId}`);
  return response.data;
};

export const getNationalRating = async (seasonId: number): Promise<SeasonRatingResponse> => {
  const response = await api.get<SeasonRatingResponse>(`/SeasonRating/national/${seasonId}`);
  return response.data;
};

export const updateNationalRating = async (seasonId: number, data: { entries: any[], activeSports: string[] }): Promise<void> => {
  await api.post(`/SeasonRating/national/${seasonId}`, data);
};

export const getRegionalRating = async (seasonId: number): Promise<SeasonRatingResponse> => {
  const response = await api.get<SeasonRatingResponse>(`/SeasonRating/regional/${seasonId}`);
  return response.data;
};

export const updateRegionalRating = async (seasonId: number, data: { entries: any[], activeSports: string[] }): Promise<void> => {
  await api.post(`/SeasonRating/regional/${seasonId}`, data);
};

// Admin methods
export const createSeason = async (season: Partial<Season>): Promise<Season> => {
  const response = await api.post<Season>('/SeasonRating/seasons', season);
  return response.data;
};

export const updateSeason = async (id: number, season: Partial<Season>): Promise<Season> => {
  const response = await api.put<Season>(`/SeasonRating/seasons/${id}`, season);
  return response.data;
};

export const deleteSeason = async (id: number): Promise<void> => {
  await api.delete(`/SeasonRating/seasons/${id}`);
};
