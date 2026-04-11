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
}

export interface ChessPlayerDto {
    id?: number;
    userId?: number | null;
    firstName: string;
    lastName: string;
    group: string;
    city: string;
    ratingELO: number;
    fideId?: string | null;
    title?: string | null;
}

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
