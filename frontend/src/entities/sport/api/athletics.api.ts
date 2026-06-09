import api from '@/shared/api/axiosInstance';
import { 
    AthleticsEventResponse, 
    AthleticsEventRequest, 
    AthleticsEventParticipantDto, 
    AthleticsResultDto 
} from '@/entities/sport/model/athletics.types';

export const getAthleticsEventById = async (id: number | string): Promise<AthleticsEventResponse> => {
  const response = await api.get<AthleticsEventResponse>(`/athletics-events/${id}`);
  return response.data;
};

export const updateAthleticsEvent = async (id: number | string, data: AthleticsEventRequest): Promise<void> => {
  await api.put(`/athletics-events/${id}`, data);
};

export const addParticipantToAthleticsEvent = async (id: number | string, data: AthleticsEventParticipantDto): Promise<void> => {
  await api.post(`/athletics-events/${id}/participants`, data);
};

export const removeParticipantFromAthleticsEvent = async (id: number | string, playerId: number): Promise<void> => {
  await api.delete(`/athletics-events/${id}/participants/${playerId}`);
};

export const addAthleticsResult = async (id: number | string, data: AthleticsResultDto): Promise<void> => {
  await api.post(`/athletics-events/${id}/results`, data);
};

export const updateAthleticsResult = async (id: number | string, resultId: number | string, data: AthleticsResultDto): Promise<void> => {
  await api.put(`/athletics-events/${id}/results/${resultId}`, data);
};

export const deleteAthleticsResult = async (id: number | string, resultId: number | string): Promise<void> => {
  await api.delete(`/athletics-events/${id}/results/${resultId}`);
};
