import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from './player.types';

export interface FutsalTeamResponse {
  id: number;
  name: string;
  players?: PlayerDto[];
  captainId?: number | null;
  goalkeeperId?: number | null;
  points?: number;
}

export interface FutsalGoal {
  id: number;
  matchId: number;
  playerId: number;
  player?: PlayerDto;
  goalsCount: number;
}

export interface FutsalGoalRequest {
  playerId: number;
  goalsCount: number;
}

export interface FutsalMatchResponse {
  id: number;
  eventId: number;
  
  homeTeamId: number;
  homeTeam?: FutsalTeamResponse;
  
  awayTeamId: number;
  awayTeam?: FutsalTeamResponse;
  
  homeTeamScore?: number;
  awayTeamScore?: number;
  
  winnerTeamId?: number | null;
  winnerTeam?: FutsalTeamResponse;
  
  stage?: string;
  
  homeRoster?: PlayerDto[];
  awayRoster?: PlayerDto[];
  homeGoalkeeperId?: number | null;
  awayGoalkeeperId?: number | null;
  homeCaptainId?: number | null;
  awayCaptainId?: number | null;
  goals?: FutsalGoal[];
}

export interface FutsalEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  
  matches: FutsalMatchResponse[] | null;
  teams: FutsalTeamResponse[] | null;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  
  firstPlaceTeam?: FutsalTeamResponse | null;
  secondPlaceTeam?: FutsalTeamResponse | null;
  thirdPlaceTeam?: FutsalTeamResponse | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
}

export interface FutsalEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
}
