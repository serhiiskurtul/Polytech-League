export interface PlayerDto {
    id?: number;
    userId?: number | null;
    firstName: string;
    patronymic?: string | null;
    lastName: string;
    birthDate?: string | null;
    group: string;
    studentId?: string | null;
    city: string;
    faculty?: string | null;
    bio?: string | null;
    instagramUrl?: string | null;
    photoUrl?: string | null;

    // Chess
    chessRating?: number | null;
    chessFideId?: string | null;
    chessTitle?: string | null;

    // Checkers
    checkersRating?: number | null;

    // Table Tennis
    tableTennisRating?: number | null;
    tableTennisHand?: 'Left' | 'Right' | null;

    // Armwrestling
    armwrestlingWeight?: number | null;
    armwrestlingPreferredWeightClass?: string | null;
    armwrestlingWeightClasses?: string[];

    // Team Sports
    futsalPosition?: string | null;
    futsalShirtNumber?: number | null;
    futsalTeamId?: number | null;

    volleyballPosition?: string | null;
    volleyballShirtNumber?: number | null;
    volleyballTeamId?: number | null;

    basketballShirtNumber?: number | null;
    basketballTeamId?: number | null;

    beachVolleyballShirtNumber?: number | null;
    beachVolleyballTeamId?: number | null;

    achievements?: AchievementDto[];
    matchesPlayed?: number;
    winsCount?: number;
    goalsScored?: number;
    performanceScore?: number;
    globalRank?: number;
    sports?: string[];
    sportStats?: {
        sportType: string;
        performanceScore: number;
        matchesPlayed: number;
        winsCount: number;
        goalsCount: number;
        rank: number;
    }[];
    pointsBreakdown?: PointEntryDto[];
    isExternal?: boolean;
}


export interface PointEntryDto {
    description: string;
    points: number;
    category: string;
}

export interface AchievementDto {
    tournamentName: string;
    sportType: string;
    date: string;
    rank: number;
    result: string;
    tournamentId?: number;
    isCaptain?: boolean;
}
