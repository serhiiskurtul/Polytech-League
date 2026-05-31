import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from '@/entities/player/model/player.types';

export interface footballTeamResponse {
  id: number;
  name: string;
  players?: PlayerDto[];
  captainId?: number | null;
  goalkeeperId?: number | null;
  points?: number;
}

export interface footballGoal {
  id: number;
  matchId: number;
  playerId: number;
  player?: PlayerDto;
  goalsCount: number;
}

export interface footballGoalRequest {
  playerId: number;
  goalsCount: number;
}

export interface footballCard {
  id: number;
  matchId: number;
  playerId: number;
  player?: PlayerDto;
  type: 'Yellow' | 'Red';
}

export interface footballCardRequest {
  playerId: number;
}

export interface footballMatchResponse {
  id: number;
  eventId: number;
  
  homeTeamId: number;
  homeTeam?: footballTeamResponse;
  
  awayTeamId: number;
  awayTeam?: footballTeamResponse;
  
  homeTeamScore?: number;
  awayTeamScore?: number;
  homePenaltyScore?: number;
  awayPenaltyScore?: number;
  
  winnerTeamId?: number | null;
  winnerTeam?: footballTeamResponse;
  
  stage?: string;
  
  homeRoster?: PlayerDto[];
  awayRoster?: PlayerDto[];
  homeGoalkeeperId?: number | null;
  awayGoalkeeperId?: number | null;
  homeCaptainId?: number | null;
  awayCaptainId?: number | null;
  goals?: footballGoal[];
  yellowCards?: footballCard[];
  redCards?: footballCard[];
}

export interface footballEventResponse {
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
  
  matches: footballMatchResponse[] | null;
  teams: footballTeamResponse[] | null;
  
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  
  firstPlaceTeam?: footballTeamResponse | null;
  secondPlaceTeam?: footballTeamResponse | null;
  thirdPlaceTeam?: footballTeamResponse | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
  mvpId?: number | null;
  mvpName?: string | null;
}


export interface footballEventRequest {
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
  mvpId?: number | null;
}

