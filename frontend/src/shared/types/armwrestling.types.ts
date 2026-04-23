import { PlayerDto } from './player.types';
import { EventPhoto } from './chess.types';
import { CompetitionLevel } from './chess.types';

export type ArmwrestlingHand = "Left" | "Right" | "Both";
export type ArmwrestlingWeightClass = "U70" | "U80" | "U90" | "U100" | "Over100" | "Absolute";
export type ArmwrestlingCategory = "Men" | "Women" | "Juniors" | "Veterans";
export type ArmwrestlingTournamentType = "SingleElimination" | "DoubleElimination";

export interface ArmwrestlingMatchResponse {
  id: number;
  eventId: number;
  player1Id: number;
  player1?: PlayerDto;
  player2Id: number;
  player2?: PlayerDto;
  winnerId?: number | null;
  stage?: string | null;
  tableNumber: number;
}

export interface ArmwrestlingEventParticipantResponse {
  id: number;
  userId?: number | null;
  tournamentId: number;
  playerId: number;
  playerName: string;
  weight: number;
  rank: number;
}

export interface ArmwrestlingEventResponse {
  id: number;
  name: string;
  date: string;
  location: string;
  photos: EventPhoto[] | null;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  hand: ArmwrestlingHand;
  weightClass: ArmwrestlingWeightClass;
  category: ArmwrestlingCategory;
  participantsCount: number;
  participants: ArmwrestlingEventParticipantResponse[];
  matches: ArmwrestlingMatchResponse[] | null;
  tournamentType: ArmwrestlingTournamentType;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  timeControl?: string;
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
}

export interface ArmwrestlingEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  hand: ArmwrestlingHand;
  weightClass: ArmwrestlingWeightClass;
  category: ArmwrestlingCategory;
  participantsCount: number;
  tournamentType: ArmwrestlingTournamentType;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  referees?: string;
  schedule?: string;
  timeControl?: string;
  firstPlaceId?: number | null;
  secondPlaceId?: number | null;
  thirdPlaceId?: number | null;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
}

export interface ArmwrestlingEventParticipantDto {
  playerId: number;
  weight: number;
  rank: number;
}
