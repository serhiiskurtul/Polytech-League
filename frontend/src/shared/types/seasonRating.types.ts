export interface Season {
  id: number;
  name: string;
  isCurrent: boolean;
  startDate: string;
  endDate: string;
  spartakiadNumber?: number;
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
  spartakiadNumber?: number;
  sports: string[];
  rows: GroupRatingRow[];
}

