export interface Player {
  id: number;
  rank: number;
  name: string;
  initials: string;
  sport: string;
  rating: number;
  ratingChange: string;
  team: string;
  trend: 'up' | 'down';
}
