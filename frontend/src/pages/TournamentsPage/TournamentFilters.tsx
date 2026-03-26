interface Filter {
  id: string;
  label: string;
  icon: string;
  count?: number;
  color?: string;
}

interface Sport {
  id: string;
  label: string;
  icon: string;
}

interface TournamentFiltersProps {
  activeStatus: string;
  activeSport: string;
  onStatusChange: (id: string) => void;
  onSportChange: (id: string) => void;
}

const TournamentFilters = ({ activeStatus, activeSport, onStatusChange, onSportChange }: TournamentFiltersProps) => {
  const filters: Filter[] = [
    { id: 'all', label: 'Всі турніри', icon: 'fas fa-trophy', count: 0 },
    { id: 'ongoing', label: 'Тривають', icon: 'fas fa-play-circle', color: '#10b981' },
    { id: 'upcoming', label: 'Майбутні', icon: 'fas fa-clock', color: '#f59e0b' },
    { id: 'completed', label: 'Завершені', icon: 'fas fa-check-circle', color: '#64748b' }
  ];

  const sports: Sport[] = [
    { id: 'futsal', label: 'Футзал', icon: 'fas fa-futbol' },
    { id: 'basketball-3x3', label: 'Баскетбол 3х3', icon: 'fas fa-basketball-ball' },
    { id: 'volleyball', label: 'Волейбол', icon: 'fas fa-volleyball-ball' },
    { id: 'chess', label: 'Шахи', icon: 'fas fa-chess' },
    { id: 'checkers', label: 'Шашки', icon: 'fas fa-chess-board' },
    { id: 'beach-volleyball', label: 'Пляжний волейбол', icon: 'fas fa-umbrella-beach' },
    { id: 'badminton', label: 'Бадмінтон', icon: 'fas fa-feather-alt' },
    { id: 'armwrestling', label: 'Армреслінг', icon: 'fas fa-fist-raised' }
  ];

  return (
    <div className="tournament-filters">
      <div className="filter-section">
        <h4 className="filter-section-title">Статус турнірів</h4>
        <div className="filter-buttons">
          {filters.map(filter => (
            <button
              key={filter.id}
              className={`filter-btn ${activeStatus === filter.id ? 'active' : ''}`}
              onClick={() => onStatusChange(filter.id)}
              style={filter.color ? { '--filter-color': filter.color } as React.CSSProperties : {}}
            >
              <i className={filter.icon}></i>
              {filter.label}
            </button>
          ))}
        </div>
      </div>

      <div className="filter-section">
        <h4 className="filter-section-title">Види спорту</h4>
        <div className="sport-filters">
          {sports.map(sport => (
            <button
              key={sport.id}
              className={`sport-filter-btn ${activeSport === sport.id ? 'active' : ''}`}
              onClick={() => onSportChange(sport.id)}
            >
              <i className={sport.icon}></i>
              {sport.label}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default TournamentFilters;
