import { PlayerDto } from '@/entities/player/model/player.types';
import { RegistrationStatus, TournamentCategory } from '@/entities/tournament/model/tournament.types';

export interface ChessTeamResponse {
  id: number;
  name: string;
  players?: PlayerDto[];
}

export interface EventPhoto {
  id: number;
  url: string;
}

export type CompetitionLevel = "CollegeGroup" | "Regional" | "National";
export type ChessSystem = "Swiss" | "RoundRobin" | "Knockout" | "MacMahon" | "Scheveningen" | "Match";
export type GameResult = "WhiteWin" | "BlackWin" | "Draw" | "NotPlayed";
export type PiecesColor = "White" | "Black" | "NotDefined";

export interface ChessEventParticipantResponse {
  id: number;
  userId?: number | null;
  chessEventId: number;
  playerId: number;
  playerName: string;
  firstName?: string;
  lastName?: string;
  points: number;
  buchholz: number;
  sonnebornBerger: number;
  rating: number;
  rank: number;
  fideId?: string | null;
  title?: string | null;
  groupName?: string | null;
  status: RegistrationStatus;
  isGuest?: boolean;
  boardRank?: number | null;
}

export interface ChessGameResponse {
  id: number;
  chessEventId: number;
  round: number;
  board: number;
  whitePlayerId: number | null;
  whiteUserId?: number | null;
  whitePlayerName: string | null;
  blackPlayerId: number | null;
  blackUserId?: number | null;
  blackPlayerName: string | null;
  result: GameResult;
  whiteColor: PiecesColor;
  blackColor: PiecesColor;
  whiteTeamId?: number | null;
  blackTeamId?: number | null;
  whiteTeamScore?: number | null;
  blackTeamScore?: number | null;
  pgn?: string;
}

export interface ChessEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  timeControl: string;
  isModerated: boolean;
  isPublic: boolean;
  participantsCount: number;
  system: ChessSystem;
  games: ChessGameResponse[];
  participants: ChessEventParticipantResponse[];
  teams: ChessTeamResponse[] | null;
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
  seasonId?: number | null;
  category?: TournamentCategory;
}

export interface ChessEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  timeControl: string;
  isModerated: boolean;
  isPublic: boolean;
  participantsCount: number;
  system: ChessSystem;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  isTeam: boolean;
  firstPlaceId?: number | null;
  secondPlaceId?: number | null;
  thirdPlaceId?: number | null;
  firstPlaceTeamId?: number | null;
  secondPlaceTeamId?: number | null;
  thirdPlaceTeamId?: number | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
  seasonId?: number | null;
  category?: TournamentCategory;
}

// ChessPlayerDto removed in favor of unified PlayerDto

export interface ChessEventParticipantDto {
    playerId: number;
    firstName?: string;
    lastName?: string;
    points: number;
    rank: number;
    groupName?: string;
    rating?: number;
    status: RegistrationStatus;
    isGuest?: boolean;
    boardRank?: number;
}

export interface ChessGameDto {
    tournamentId: number;
    whitePlayerId: number | null;
    blackPlayerId: number | null;
    result: GameResult;
    round: number;
    board: number;
    whiteTeamId?: number | null;
    blackTeamId?: number | null;
    whiteTeamScore?: number | null;
    blackTeamScore?: number | null;
    pgn?: string;
}

export interface ChessChampionDto {
  playerId: number;
  playerName: string;
  playerPhotoUrl?: string;
  seasonName: string;
  tournamentName: string;
  date: string;
  points: number;
}
