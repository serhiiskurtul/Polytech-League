import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from './player.types';

export interface Basketball3x3TeamResponse {
  id: number;
  name: string;
}

export interface Basketball3x3MatchResponse {
  id: number;
  eventId: number;
  
  homeTeamId: number;
  homeTeam?: Basketball3x3TeamResponse;
  
  awayTeamId: number;
  awayTeam?: Basketball3x3TeamResponse;
  
  homeTeamScore?: number;
  awayTeamScore?: number;
  
  winnerTeamId?: number | null;
  winnerTeam?: Basketball3x3TeamResponse;
  
  stage?: string;
  
  homeRoster?: PlayerDto[];
  awayRoster?: PlayerDto[];
}

export interface Basketball3x3EventResponse {
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
  
  matches: Basketball3x3MatchResponse[] | null;
  teams: Basketball3x3TeamResponse[] | null;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  
  firstPlaceTeam?: Basketball3x3TeamResponse | null;
  secondPlaceTeam?: Basketball3x3TeamResponse | null;
  thirdPlaceTeam?: Basketball3x3TeamResponse | null;
}

export interface Basketball3x3EventRequest {
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
}
