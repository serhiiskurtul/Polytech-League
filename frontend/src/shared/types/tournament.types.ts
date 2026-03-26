export interface Tournament {
  id: string;
  name: string;
  description?: string;
  sportType: string;
  startDate: string;
  endDate?: string;
  location?: string;
  status: 'upcoming' | 'ongoing' | 'completed';
  // Add other fields as per your backend TournamentDto
}
