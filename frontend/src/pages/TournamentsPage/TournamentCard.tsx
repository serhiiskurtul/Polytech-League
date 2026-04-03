interface TournamentUI {
  id: string;
  sport: string;
  status: string;
  title: string;
  description?: string;
  date: string;
  location?: string;
  participants: number;
  featured?: boolean;
  currentRound?: string;
  progress?: number;
}

interface TournamentCardProps {
  tournament: TournamentUI;
  viewMode: 'grid' | 'list';
  onRegister: (id: string) => void;
  onDetails: (id: string) => void;
}

const TournamentCard = ({ tournament, viewMode, onRegister, onDetails }: TournamentCardProps) => {
  // Іконки для видів спорту
  const sportIcons: Record<string, string> = {
    'futsal': 'fas fa-futbol',
    'basketball-3x3': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'chess': 'fas fa-chess',
    'checkers': 'fas fa-chess-board',
    'table-tennis': 'fas fa-table-tennis',
    'badminton': 'fas fa-feather-alt',
    'armwrestling': 'fas fa-fist-raised',
    'beach-volleyball': 'fas fa-umbrella-beach'
  };

  // Кольори для статусів
  const statusColors: Record<string, string> = {
    'ongoing': '#10b981',
    'upcoming': '#f59e0b',
    'completed': '#64748b'
  };

  // Назви статусів
  const statusLabels: Record<string, string> = {
    'ongoing': 'Триває',
    'upcoming': 'Скоро',
    'completed': 'Завершено'
  };

  // Назви видів спорту
  const sportNames: Record<string, string> = {
    'futsal': 'Футзал',
    'basketball-3x3': 'Баскетбол 3х3',
    'volleyball': 'Волейбол',
    'chess': 'Шахи',
    'checkers': 'Шашки',
    'table-tennis': 'Настільний теніс',
    'badminton': 'Бадмінтон',
    'armwrestling': 'Армреслінг',
    'beach-volleyball': 'Пляжний волейбол'
  };

  // Форматування дати
  const formatDate = (dateString: string) => {
    if (dateString.includes(' - ')) {
      const [start, end] = dateString.split(' - ');
      return `${start} → ${end}`;
    }
    return dateString;
  };

  // Картка в режимі сітки
  if (viewMode === 'grid') {
    return (
      <div 
        className={`tournament-card grid ${tournament.featured ? 'featured' : ''}`}
        onClick={() => onDetails(tournament.id)}
        style={{ cursor: 'pointer' }}
      >
        {tournament.featured && (
          <div className="featured-badge">
            <i className="fas fa-star"></i> Рекомендовано
          </div>
        )}

        <div className="tournament-header">
          <div className="tournament-sport">
            <i className={sportIcons[tournament.sport] || 'fas fa-trophy'}></i>
            <span>{sportNames[tournament.sport] || tournament.sport}</span>
          </div>
          <div 
            className="tournament-status"
            style={{ backgroundColor: `${statusColors[tournament.status]}20`, color: statusColors[tournament.status] }}
          >
            {statusLabels[tournament.status] || tournament.status}
          </div>
        </div>

        <h3 className="tournament-title">{tournament.title}</h3>
        
        <p className="tournament-description">{tournament.description}</p>

        <div className="tournament-details">
          <div className="detail-item">
            <i className="far fa-calendar"></i>
            <span>{formatDate(tournament.date)}</span>
          </div>
          <div className="detail-item">
            <i className="fas fa-map-marker-alt"></i>
            <span>{tournament.location}</span>
          </div>
          <div className="detail-item">
            <i className="fas fa-users"></i>
            <span>{tournament.participants} учасників</span>
          </div>
        </div>

        <div className="tournament-progress">
          <div className="progress-label">{tournament.currentRound}</div>
          <div className="progress-bar">
            <div 
              className="progress-fill"
              style={{ width: `${tournament.progress || 0}%` }}
            ></div>
          </div>
        </div>

        <div className="tournament-actions">
          <button 
            className="btn btn-primary" 
            onClick={(e) => { e.stopPropagation(); onRegister(tournament.id); }}
            disabled={tournament.status === 'completed'}
          >
            {tournament.status === 'completed' ? 'Завершено' : 'Зареєструватись'}
          </button>
          <button 
            className="btn btn-outline" 
            onClick={() => onDetails(tournament.id)}
          >
            Деталі
          </button>
        </div>
      </div>
    );
  }

  // Картка в режимі списку
  return (
    <div 
      className={`tournament-card list ${tournament.featured ? 'featured' : ''}`}
      onClick={() => onDetails(tournament.id)}
      style={{ cursor: 'pointer' }}
    >
      <div className="list-main">
        <div className="list-sport">
          <i className={sportIcons[tournament.sport] || 'fas fa-trophy'}></i>
        </div>
        
        <div className="list-content">
          <div className="list-header">
            <h3 className="tournament-title">{tournament.title}</h3>
            <div className="list-meta">
              <span className="tournament-sport-name">
                {sportNames[tournament.sport] || tournament.sport}
              </span>
              <span className="tournament-location">
                <i className="fas fa-map-marker-alt"></i> {tournament.location}
              </span>
            </div>
          </div>
          
          <div className="list-details">
            <div className="detail-item">
              <i className="far fa-calendar"></i>
              <span>{formatDate(tournament.date)}</span>
            </div>
            <div className="detail-item">
              <i className="fas fa-users"></i>
              <span>{tournament.participants} учасників</span>
            </div>
          </div>
        </div>
      </div>

      <div className="list-sidebar">
        <div className="list-progress">
          <div 
            className="tournament-status"
            style={{ backgroundColor: `${statusColors[tournament.status]}20`, color: statusColors[tournament.status], display: 'inline-block', marginBottom: '12px' }}
          >
            {statusLabels[tournament.status] || tournament.status}
          </div>
          <div className="progress-bar">
            <div 
              className="progress-fill"
              style={{ width: `${tournament.progress || 0}%` }}
            ></div>
          </div>
        </div>

        <div className="list-actions">
          <button 
            className="btn btn-primary" 
            onClick={(e) => { e.stopPropagation(); onRegister(tournament.id); }}
            disabled={tournament.status === 'completed'}
          >
            {tournament.status === 'completed' ? 'Завершено' : 'Реєстрація'}
          </button>
          <button 
            className="btn btn-outline"
            onClick={(e) => { e.stopPropagation(); onDetails(tournament.id); }}
          >
            Деталі
          </button>
        </div>
      </div>
    </div>
  );
};

export default TournamentCard;
