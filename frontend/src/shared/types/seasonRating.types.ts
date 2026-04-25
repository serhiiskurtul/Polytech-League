export interface Season {
  id: number;
  name: string;
  isCurrent: boolean;
  startDate: string;
  endDate: string;
}

export interface SportResult {
  points: number;
  rank: number;
}

export interface GroupRatingRow {
  groupName: string;
  sportResults: Record<string, SportResult>;
  totalPoints: number;
  totalRank: number;
}

export interface SeasonRatingResponse {
  seasonId: number;
  seasonName: string;
  sports: string[];
  rows: GroupRatingRow[];
}
