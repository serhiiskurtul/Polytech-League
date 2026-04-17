import { PlayerDto } from './player.types';
import { EventPhoto } from './chess.types';
import { CompetitionLevel } from './chess.types';

export interface TableTennisSetDto {
  id: number;
  setNumber: number;
  player1Score: number;
  player2Score: number;
}

export interface TableTennisMatchResponse {
  id: number;
  tournamentId: number;
  player1Id: number;
  player2Id: number;
  winnerId?: number | null;
  stage?: string | null;
  tableNumber: number;
  setsToWin: number;
  sets?: TableTennisSetDto[] | null;
}

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
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  participantsCount: number;
  participants: TableTennisEventParticipantResponse[];
  matches: TableTennisMatchResponse[] | null;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
}

export interface TableTennisEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  participantsCount: number;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  firstPlaceId?: number | null;
  secondPlaceId?: number | null;
  thirdPlaceId?: number | null;
}

export interface TableTennisEventParticipantDto {
  playerId: number;
  points: number;
  rank: number;
}
