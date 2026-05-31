import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from '@/entities/player/model/player.types';

export interface VolleyballTeamResponse {
  id: number;
  name: string; // Group name like "221", "541"
}

export interface VolleyballSetDto {
  id: number;
  setNumber: number;
  homeScore: number;
  awayScore: number;
}

export interface VolleyballMatchResponse {
  id: number;
  eventId: number;
  
  homeTeamId: number;
  homeTeam?: VolleyballTeamResponse;
  
  awayTeamId: number;
  awayTeam?: VolleyballTeamResponse;
  
  homeSetsWon: number;
  awaySetsWon: number;
  setsToWin: number;
  
  winnerTeamId?: number | null;
  winnerTeam?: VolleyballTeamResponse;
  
  stage?: string;
  sets?: VolleyballSetDto[];
  
  homeRoster?: PlayerDto[];
  awayRoster?: PlayerDto[];
}

export interface VolleyballEventResponse {
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
  
  matches: VolleyballMatchResponse[] | null;
  teams: VolleyballTeamResponse[] | null;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  
  firstPlaceTeam?: VolleyballTeamResponse | null;
  secondPlaceTeam?: VolleyballTeamResponse | null;
  thirdPlaceTeam?: VolleyballTeamResponse | null;
  mvpId?: number | null;
  mvpName?: string | null;
}


export interface VolleyballEventRequest {
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
  mvpId?: number | null;
}

