export interface PlayerDto {
    id?: number;
    userId?: number | null;
    firstName: string;
    lastName: string;
    group: string;
    city: string;

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
}
