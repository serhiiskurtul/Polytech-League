export type Gender = 'Male' | 'Female' | 'None';

export interface PlayerDto {
    id?: number;
    userId?: number | null;
    firstName: string;
    patronymic?: string | null;
    lastName: string;
    gender: Gender;
    birthDate?: string | null;
    group?: string | null;
    isGuest?: boolean;
    enrollmentYear?: number;
    specialtyCode?: number;
    groupNumber?: number;
    studentId?: string | null;
    city: string;
    studyYears?: string | null;
    bio?: string | null;
    instagramUrl?: string | null;
    photoUrl?: string | null;
    previousNames?: string | null;
    isFounder?: boolean;
    automaticBadges?: Array<{
        type: string;
        label: string;
        seasonName?: string;
        icon?: string;
    }>;

    // Chess
    chessRating?: number | null;
    chessFideId?: string | null;
    chessTitle?: string | null;
    hasChessFourStreak?: boolean;
    chessTotalPoints?: number | null;
    chessTotalPossiblePoints?: number | null;
    isChessUndefeatedFourSeasons?: boolean;

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
    footballPosition?: string | null;
    footballShirtNumber?: number | null;
    footballTeamId?: number | null;

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
    yellowCardsCount?: number;
    redCardsCount?: number;
    performanceScore?: number;
    globalRank?: number;
    sports?: string[];
    sportStats?: {
        sportType: string;
        performanceScore: number;
        matchesPlayed: number;
        winsCount: number;
        goalsCount: number;
        yellowCardsCount?: number;
        redCardsCount?: number;
        rank: number;
    }[];
    pointsBreakdown?: PointEntryDto[];
    competitionsCount?: number;
    isExternal?: boolean;
    hasPendingMergeRequest?: boolean;
}


export interface PointEntryDto {
    description: string;
    points: number;
    category: string;
    sportType?: string;
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
