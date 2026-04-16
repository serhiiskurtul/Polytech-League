import api from "./axiosInstance";
import { 
  ArmwrestlingEventResponse, 
  ArmwrestlingMatchResponse 
} from "../types/armwrestling.types";

/**
 * Fetch detailed armwrestling event by ID
 * Includes category, hand, weight class and list of matches
 */
export const getArmwrestlingEventById = async (id: string | number): Promise<ArmwrestlingEventResponse> => {
  const response = await api.get<ArmwrestlingEventResponse>(`/armwrestling-events/${id}`);
  return response.data;
};

/**
 * Fetch all matches for a specific armwrestling event
 */
export const getArmwrestlingMatches = async (eventId: string | number): Promise<ArmwrestlingMatchResponse[]> => {
  const response = await api.get<ArmwrestlingMatchResponse[]>(`/armwrestling-matches`, {
    params: { eventId }
  });
  return response.data;
};

/**
 * Update a match (e.g., set winner)
 */
export const updateArmwrestlingMatch = async (matchId: number, data: Partial<ArmwrestlingMatchResponse>): Promise<void> => {
  await api.put(`/armwrestling-matches/${matchId}`, data);
};

/**
 * Update an armwrestling event (e.g., set winners, rules, prizes)
 */
export const updateArmwrestlingEvent = async (id: string | number, data: any): Promise<void> => {
  await api.put(`/armwrestling-events/${id}`, data);
};

/**
 * Create a new match for an armwrestling event
 */
export const createArmwrestlingMatch = async (data: Partial<ArmwrestlingMatchResponse>): Promise<ArmwrestlingMatchResponse> => {
  const response = await api.post<ArmwrestlingMatchResponse>(`/armwrestling-matches`, data);
  return response.data;
};

/**
 * Add a participant to an armwrestling event
 */
export const addParticipantToArmwrestlingEvent = async (eventId: string | number, playerId: number): Promise<void> => {
    await api.post(`/armwrestling-events/${eventId}/participants`, playerId, {
        headers: {
            'Content-Type': 'application/json'
        }
    });
};

/**
 * Remove a participant from an armwrestling event
 */
export const removeParticipantFromArmwrestlingEvent = async (eventId: string | number, playerId: number): Promise<void> => {
    await api.delete(`/armwrestling-events/${eventId}/participants/${playerId}`);
};

