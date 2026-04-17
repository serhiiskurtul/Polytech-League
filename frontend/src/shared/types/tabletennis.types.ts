import { PlayerDto } from './player.types';
import { EventPhoto } from './chess.types';
import { CompetitionLevel } from './chess.types';

export interface TableTennisEventParticipantResponse {
  id: number;
  userId?: number | null;
  tournamentId: number;
  playerId: number;
  playerName: string;
  points: number;
  rank: number;
}

export interface TableTennisEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  level: CompetitionLevel;
  description: string;
  participantsCount: number;
  participants: TableTennisEventParticipantResponse[];
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
}

export interface TableTennisEventRequest {
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

export interface TableTennisEventParticipantDto {
  playerId: number;
  points: number;
  rank: number;
}
