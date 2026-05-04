import { PlayerDto } from "./player.types";

export interface AthleticsEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  description?: string;
  mainPhotoUrl?: string;
  level: string;
  category: string;
  isTeam: boolean;
  isModerated: boolean;
  isPublic: boolean;
  seasonId?: number;
  results: AthleticsResultResponse[];
  participants: AthleticsParticipantResponse[];
  rules?: string;
  prizes?: string;
  schedule?: string;
}

export interface AthleticsResultResponse {
  id: number;
  disciplineName: string;
  playerId?: number;
  playerName?: string;
  teamName?: string;
  score?: string;
  rank: number;
}

export interface AthleticsParticipantResponse {
  playerId: number;
  playerName: string;
  groupName?: string;
}
