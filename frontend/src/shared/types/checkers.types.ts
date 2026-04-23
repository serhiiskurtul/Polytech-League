import { PlayerDto } from './player.types';

export interface CheckersTeamResponse {
  id: number;
  name: string;
  players?: PlayerDto[];
}
import { EventPhoto, CompetitionLevel, GameResult } from './chess.types';

export type CheckersSystem = "Swiss" | "RoundRobin" | "Knockout";
export type CheckersVariant = "Russian" | "International" | "English";

export interface CheckersEventParticipantResponse {
  id: number;
  userId?: number | null;
  checkersEventId: number;
  playerId: number;
  playerName: string;
  points: number;
  rating: number;
  rank: number;
}

export interface CheckersGameResponse {
  id: number;
  checkersEventId: number;
  round: number;
  whitePlayerId: number | null;
  whitePlayerName: string | null;
  blackPlayerId: number | null;
  blackPlayerName: string | null;
  result: GameResult;
  whiteTeamId?: number | null;
  blackTeamId?: number | null;
  whiteTeamScore?: number | null;
  blackTeamScore?: number | null;
}

export interface CheckersEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  timeControl: string;
  participantsCount: number;
  system: CheckersSystem;
  variant: CheckersVariant;
  games: CheckersGameResponse[];
  participants: CheckersEventParticipantResponse[];
  teams: CheckersTeamResponse[] | null;
  isTeam: boolean;
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
}

export interface CheckersEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  timeControl: string;
  participantsCount: number;
  system: CheckersSystem;
  variant: CheckersVariant;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  isTeam: boolean;
  firstPlaceId?: number | null;
  secondPlaceId?: number | null;
  thirdPlaceId?: number | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
}

export interface CheckersEventParticipantDto {
    playerId: number;
    points: number;
    rank: number;
}

export interface CheckersGameDto {
    tournamentId: number;
    whitePlayerId: number | null;
    blackPlayerId: number | null;
    result: GameResult;
    round: number;
    whiteTeamId?: number | null;
    blackTeamId?: number | null;
    whiteTeamScore?: number | null;
    blackTeamScore?: number | null;
}
