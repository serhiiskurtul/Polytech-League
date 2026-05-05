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
  activeLevel: string;
  onStatusChange: (id: string) => void;
  onSportChange: (id: string) => void;
  onLevelChange: (id: string) => void;
}


const TournamentFilters = ({ activeStatus, activeSport, activeLevel, onStatusChange, onSportChange, onLevelChange }: TournamentFiltersProps) => {

  const filters: Filter[] = [
    { id: 'all', label: 'Всі змагання', icon: 'fas fa-trophy' },
    { id: 'ongoing', label: 'Тривають', icon: 'fas fa-play-circle', color: '#10b981' },
    { id: 'upcoming', label: 'Майбутні', icon: 'fas fa-clock',color: '#f59e0b' },
    { id: 'completed', label: 'Завершені', icon: 'fas fa-check-circle', color: '#64748b' }
  ];

  const sports: Sport[] = [
    { id: 'futsal', label: 'Футзал', icon: 'fas fa-futbol' },
    { id: 'basketball-3x3', label: 'Баскетбол 3х3', icon: 'fas fa-basketball-ball' },
    { id: 'volleyball', label: 'Волейбол', icon: 'fas fa-volleyball-ball' },
    { id: 'chess', label: 'Шахи', icon: 'fas fa-chess' },
    { id: 'checkers', label: 'Шашки', icon: 'fas fa-chess-board' },
    { id: 'table-tennis', label: 'Настільний теніс', icon: 'fas fa-table-tennis' },
    { id: 'beach-volleyball', label: 'Пляжний волейбол', icon: 'fas fa-umbrella-beach' },
    { id: 'badminton', label: 'Бадмінтон', icon: 'fas fa-feather-alt' },
    { id: 'armwrestling', label: 'Армреслінг', icon: 'fas fa-fist-raised' },
    { id: 'athletics', label: 'Легка атлетика', icon: 'fas fa-running' }
  ];

  const levels = [
    { id: 'all', label: 'Всі категорії', icon: 'fas fa-globe' },
    { id: 'CollegeGroup', label: 'Міжгрупові', icon: 'fas fa-users' },
    { id: 'Regional', label: 'Обласні', icon: 'fas fa-map-marked-alt' },
    { id: 'National', label: 'Всеукраїнські', icon: 'fas fa-flag' }
  ];


  return (
    <div className="tournament-filters">
      <div className="filter-section">
        <h3 className="filter-title">Статус</h3>
        <div className="filter-group">
          {filters.map(filter => (
            <button
              key={filter.id}
              className={`filter-btn ${activeStatus === filter.id ? 'active' : ''}`}
              onClick={() => onStatusChange(filter.id)}
            >
              <div className="filter-btn-content">
                <i className={filter.icon}></i>
                <span>{filter.label}</span>
              </div>
              {filter.count !== undefined && (
                <span className="filter-count">{filter.count}</span>
              )}
            </button>
          ))}
        </div>
      </div>

      <div className="filter-section">
        <h3 className="filter-title">Вид спорту</h3>
        <div className="filter-group">
          <button 
            className={`filter-btn ${activeSport === 'all' ? 'active' : ''}`}
            onClick={() => onSportChange('all')}
          >
            <div className="filter-btn-content">
              <i className="fas fa-filter"></i>
              <span>Всі види</span>
            </div>
          </button>
          {sports.map(sport => (
            <button
              key={sport.id}
              className={`filter-btn ${activeSport === sport.id ? 'active' : ''}`}
              onClick={() => onSportChange(sport.id)}
            >
              <div className="filter-btn-content">
                <i className={sport.icon}></i>
                <span>{sport.label}</span>
              </div>
            </button>
          ))}
        </div>
      </div>

      <div className="filter-section">
        <h3 className="filter-title">Категорія</h3>
        <div className="filter-group">
          {levels.map(level => (
            <button
              key={level.id}
              className={`filter-btn ${activeLevel === level.id ? 'active' : ''}`}
              onClick={() => onLevelChange(level.id)}
            >
              <div className="filter-btn-content">
                <i className={level.icon}></i>
                <span>{level.label}</span>
              </div>
            </button>
          ))}
        </div>
      </div>
    </div>

  );
};

export default TournamentFilters;
