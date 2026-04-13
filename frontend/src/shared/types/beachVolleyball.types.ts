import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from './player.types';

export interface BeachVolleyballTeamResponse {
  id: number;
  name: string;
}

export interface BeachVolleyballSetDto {
  id: number;
  setNumber: number;
  homeScore: number;
  awayScore: number;
}

export interface BeachVolleyballMatchResponse {
  id: number;
  eventId: number;
  
  homeTeamId: number;
  homeTeam?: BeachVolleyballTeamResponse;
  
  awayTeamId: number;
  awayTeam?: BeachVolleyballTeamResponse;
  
  homeSetsWon: number;
  awaySetsWon: number;
  
  winnerTeamId?: number | null;
  winnerTeam?: BeachVolleyballTeamResponse;
  
  stage?: string;
  sets?: BeachVolleyballSetDto[];
  
  homeRoster?: PlayerDto[];
  awayRoster?: PlayerDto[];
}

export interface BeachVolleyballEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  
  setsToWin: number;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  
  matches: BeachVolleyballMatchResponse[] | null;
  teams: BeachVolleyballTeamResponse[] | null;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  
  firstPlaceTeam?: BeachVolleyballTeamResponse | null;
  secondPlaceTeam?: BeachVolleyballTeamResponse | null;
  thirdPlaceTeam?: BeachVolleyballTeamResponse | null;
}

export interface BeachVolleyballEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  
  setsToWin: number;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
}
