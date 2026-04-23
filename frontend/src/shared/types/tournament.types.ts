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
