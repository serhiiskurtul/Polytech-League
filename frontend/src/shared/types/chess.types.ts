import { PlayerDto } from './player.types';

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
  points: number;
  buchholz: number;
  sonnebornBerger: number;
  rating: number;
  rank: number;
  fideId?: string | null;
  title?: string | null;
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
  participantsCount: number;
  system: ChessSystem;
  games: ChessGameResponse[];
  players: ChessEventParticipantResponse[];
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  schedule?: string;
}

export interface ChessEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  timeControl: string;
  participantsCount: number;
  system: ChessSystem;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  schedule?: string;
  firstPlaceId?: number | null;
  secondPlaceId?: number | null;
  thirdPlaceId?: number | null;
}

// ChessPlayerDto removed in favor of unified PlayerDto

export interface ChessEventParticipantDto {
    playerId: number;
    points: number;
    rank: number;
}

export interface ChessGameDto {
    tournamentId: number;
    whitePlayerId: number | null;
    blackPlayerId: number | null;
    result: GameResult;
    round: number;
    board: number;
}
