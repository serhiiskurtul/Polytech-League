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
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
