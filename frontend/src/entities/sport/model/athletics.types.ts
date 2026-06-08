export interface AthleticsEventResponse {
    id: number;
    title: string;
    sport: string;
    date: string;
    location: string;
    level: string;
    category: string;
    description?: string;
    mainPhotoUrl?: string;
    photos: any[];
    results: AthleticsResultDto[];
    participants: AthleticsEventParticipantDto[];
    isTeam: boolean;
    isModerated: boolean;
    isPublic: boolean;
    registrationStart?: string;
    registrationEnd?: string;
    endDate?: string;
}

export interface AthleticsEventRequest {
    name: string;
    date: string;
    location: string;
    level: string;
    category: string;
    description?: string;
    isTeam: boolean;
    isModerated: boolean;
    isPublic: boolean;
    registrationStart?: string;
    registrationEnd?: string;
    endDate?: string;
}

export interface AthleticsResultDto {
    id?: number;
    athleticsEventId?: number;
    disciplineName: string;
    playerId?: number;
    playerName?: string;
    teamName?: string;
    score?: string;
    rank: number;
}

export interface AthleticsEventParticipantDto {
    playerId: number;
    groupName?: string;
    playerName?: string;
    status?: string;
    collegeId?: number;
}
