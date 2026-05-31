export interface Tournament {
  id: string;
  title: string;
  description?: string;
  sport: string;
  date: string;
  location?: string;
  status: 'upcoming' | 'ongoing' | 'completed';
  participants: number;
  teams: number;
  featured?: boolean;
  isTeam?: boolean;
  time?: string;
  rawDate?: string;
  currentRound?: string;
  registrationStart?: string;
  registrationEnd?: string;
  endDate?: string;
  isModerated?: boolean;
  level: string;
  category: string;
}

export interface RecentMatch {
  id: number;
  sport: string;
  team1Name: string;
  team2Name: string;
  score1?: string;
  score2?: string;
  status: string;
  date: string;
  location: string;
  tournamentName: string;
  tournamentId: number;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export enum RegistrationStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export enum ModerationStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export type TournamentCategory = 'Men' | 'Women' | 'Mixed';

export interface Champion {
  playerId: number;
  playerName: string;
  playerPhotoUrl?: string;
  seasonName: string;
  tournamentId: number;
  tournamentName: string;
  sportType: string;
  date: string;
  points: number;
  gender: 'Male' | 'Female' | 'None';
  weightClass?: string;
}
