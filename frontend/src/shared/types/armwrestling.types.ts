import { EventPhoto, CompetitionLevel } from './chess.types';
import { PlayerDto } from './player.types';

export type ArmwrestlingHand = 'Left' | 'Right';

export type ArmwrestlingCategory = 'Men' | 'Women';

export type ArmwrestlingWeightClass = 
  | 'Under60kg' 
  | 'Under70kg' 
  | 'Under80kg' 
  | 'Under90kg' 
  | 'Plus90kg';

export interface ArmwrestlingMatchResponse {
  id: number;
  eventId: number;
  player1Id: number;
  player1?: PlayerDto;
  player2Id: number;
  player2?: PlayerDto;
  winnerId?: number | null;
  stage?: string; // Round of 16, Quarterfinal, etc.
  tableNumber: number;
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
  timeControl?: string;
  participantsCount?: number;
  hand: ArmwrestlingHand;
  weightClass: ArmwrestlingWeightClass;
  category: ArmwrestlingCategory;
  matches: ArmwrestlingMatchResponse[] | null;
  players: any[]; // Using any[] to match the unified PlayerDto or participant response structure
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  schedule?: string;
}

export interface ArmwrestlingEventRequest {
  name: string;
  date: string;
  location: string;
  refereeIds: number[];
  level: CompetitionLevel;
  description: string;
  timeControl?: string;
  participantsCount?: number;
  hand: ArmwrestlingHand;
  weightClass: ArmwrestlingWeightClass;
  category: ArmwrestlingCategory;
  rules?: string;
  prizes?: string;
  headRefereeName?: string;
  schedule?: string;
}
