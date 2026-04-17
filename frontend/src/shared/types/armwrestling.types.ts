import { PlayerDto } from './player.types';
import { EventPhoto } from './chess.types';
import { CompetitionLevel } from './chess.types';

export interface ArmwrestlingEventParticipantResponse {
  id: number;
  userId?: number | null;
  tournamentId: number;
  playerId: number;
  playerName: string;
  weight: number;
  rank: number;
}

export interface ArmwrestlingEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  level: CompetitionLevel;
  description: string;
  participantsCount: number;
  participants: ArmwrestlingEventParticipantResponse[];
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
}

export interface ArmwrestlingEventRequest {
  name: string;
  date: string;
  location: string;
  level: CompetitionLevel;
  description: string;
  participantsCount: number;
  firstPlaceId?: number | null;
  secondPlaceId?: number | null;
  thirdPlaceId?: number | null;
}

export interface ArmwrestlingEventParticipantDto {
  playerId: number;
  weight: number;
  rank: number;
}
