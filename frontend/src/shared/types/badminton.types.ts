import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from './player.types';

export type BadmintonTournamentType = 'Singles' | 'Doubles';

export interface BadmintonTeamDto {
  id: number;
  name?: string;
  player1Id: number;
  player1?: PlayerDto;
  player2Id: number;
  player2?: PlayerDto;
}

export interface BadmintonSetResponse {
  id: number;
  setNumber: number;
  player1Score: number;
  player2Score: number;
}

export interface BadmintonMatchResponse {
  id: number;
  
  // Singles
  player1Id?: number | null;
  player1?: PlayerDto;
  player2Id?: number | null;
  player2?: PlayerDto;
  
  // Doubles / Team Sports
  team1Id?: number | null;
  team1?: BadmintonTeamDto;
  team2Id?: number | null;
  team2?: BadmintonTeamDto;
  
  // Home/Away mapping for TeamSportsTournamentView compatibility
  homeTeamId?: number | null;
  homeTeam?: BadmintonTeamDto;
  awayTeamId?: number | null;
  awayTeam?: BadmintonTeamDto;
  
  // Scores
  homeScore?: number | null;
  awayScore?: number | null;
  
  winnerPlayerId?: number | null;
  winnerPlayer?: PlayerDto;
  winnerTeamId?: number | null;
  winnerTeam?: BadmintonTeamDto;
  
  stage?: string;
  courtNumber: number;
  setsToWin: number;
  sets?: BadmintonSetResponse[];
}

export interface BadmintonEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  refereeIds: number[];
  level: CompetitionLevel;
  tournamentType: BadmintonTournamentType;
  description: string;
  
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
  
  firstPlaceTeam?: BadmintonTeamDto | null;
  secondPlaceTeam?: BadmintonTeamDto | null;
  thirdPlaceTeam?: BadmintonTeamDto | null;
  
  isTeam: boolean;
  teams: BadmintonTeamDto[] | null;
  matches: BadmintonMatchResponse[] | null;
  players: BadmintonEventParticipantDto[] | null;
}

export interface BadmintonEventParticipantDto {
  playerId: number;
  playerName?: string;
  points: number;
  rank: number;
  userId?: number;
}

export interface BadmintonEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  tournamentType: BadmintonTournamentType;
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
