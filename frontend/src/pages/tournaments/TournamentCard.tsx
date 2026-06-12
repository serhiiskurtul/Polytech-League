import { BASE_URL } from '@/shared/api/axiosInstance';

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
  isTeam?: boolean;
  currentRound?: string;
  progress?: number;
  time?: string;
  rawDate?: string;
  level: string;
  category: string;
  weightClass?: string;
  mainPhotoUrl?: string;
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
    'football': 'fas fa-futbol',
    'basketball-3x3': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'chess': 'fas fa-chess',
    'checkers': 'fas fa-chess-board',
    'table-tennis': 'fas fa-table-tennis',
    'badminton': 'fas fa-feather-alt',
    'armwrestling': 'fas fa-fist-raised',
    'beach-volleyball': 'fas fa-umbrella-beach',
    'athletics': 'fas fa-running'
  };

  // Кольори для статусів
  const statusColors: Record<string, string> = {
    'ongoing': '#10b981',
    'upcoming': '#f59e0b',
    'registration': '#3b82f6',
    'completed': '#64748b'
  };

  // Назви статусів
  const statusLabels: Record<string, string> = {
    'ongoing': 'Триває',
    'upcoming': 'Заплановано',
    'registration': 'Реєстрація',
    'completed': 'Завершено'
  };

  // Назви видів спорту
  const sportNames: Record<string, string> = {
    'football': 'Футбол',
    'basketball-3x3': 'Баскетбол',
    'volleyball': 'Волейбол',
    'chess': 'Шахи',
    'checkers': 'Шашки',
    'table-tennis': 'Настільний теніс',
    'badminton': 'Бадмінтон',
    'armwrestling': 'Армрестлінг',
    'beach-volleyball': 'Пляжний волейбол',
    'athletics': 'Легка атлетика'
  };

  const levelLabels: Record<string, string> = {
    'CollegeGroup': 'Міжгруповий',
    'Regional': 'Обласний',
    'National': 'Всеукраїнський'
  };

  const categoryLabels: Record<string, string> = {
    'Men': 'Чоловіки',
    'Women': 'Жінки',
    'Mixed': 'Змішаний'
  };

  const weightClassLabels: Record<string, string> = {
    'Under60kg': 'До 60 кг',
    'Under70kg': 'До 70 кг',
    'Under80kg': 'До 80 кг',
    'Under90kg': 'До 90 кг',
    'Plus90kg': 'Понад 90 кг'
  };

  const getImageUrl = (url?: string) => {
    if (!url) return '';
    if (url.startsWith('http')) return url;
    return `${BASE_URL}${url}`;
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

        <div className="tournament-image-container">
          {tournament.mainPhotoUrl ? (
            <img src={getImageUrl(tournament.mainPhotoUrl)} alt={tournament.title} className="tournament-cover-image" loading="lazy" />
          ) : (
            <div className="tournament-cover-placeholder">
              <i className={sportIcons[tournament.sport] || 'fas fa-trophy'}></i>
            </div>
          )}
        </div>

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
        
        <div className="tournament-badges">
          <div className="badge-item badge-level">
            <i className="fas fa-layer-group"></i>
            <span>{levelLabels[tournament.level] || tournament.level}</span>
          </div>
          <div className="badge-item badge-category">
            <i className="fas fa-users"></i>
            <span>{categoryLabels[tournament.category] || tournament.category}</span>
          </div>
          {tournament.sport === 'armwrestling' && tournament.weightClass && (
            <div className="badge-item badge-category">
              <i className="fas fa-weight-hanging"></i>
              <span>{weightClassLabels[tournament.weightClass] || tournament.weightClass}</span>
            </div>
          )}
        </div>

        <p className="tournament-description">{tournament.description}</p>

        <div className="tournament-details">
          <div className="detail-item">
            <i className="far fa-calendar"></i>
            <span>
              {tournament.rawDate ? new Date(tournament.rawDate).toLocaleString('uk-UA', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
              }) : `${formatDate(tournament.date)} ${tournament.time || ''}`}
            </span>
          </div>
          <div className="detail-item">
            <i className="fas fa-map-marker-alt"></i>
            <span>{tournament.location}</span>
          </div>
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
        {tournament.mainPhotoUrl ? (
          <div className="list-image">
            <img src={getImageUrl(tournament.mainPhotoUrl)} alt={tournament.title} loading="lazy" />
          </div>
        ) : (
          <div className="list-sport">
            <i className={sportIcons[tournament.sport] || 'fas fa-trophy'}></i>
          </div>
        )}
        
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
              <span className="tournament-level-badge">
                 <i className="fas fa-layer-group"></i> {levelLabels[tournament.level] || tournament.level}
              </span>
              <span className="tournament-category-badge">
                 <i className="fas fa-users"></i> {categoryLabels[tournament.category] || tournament.category}
              </span>
              {tournament.sport === 'armwrestling' && tournament.weightClass && (
                <span className="tournament-category-badge">
                   <i className="fas fa-weight-hanging"></i> {weightClassLabels[tournament.weightClass] || tournament.weightClass}
                </span>
              )}
            </div>
          </div>
          
          <div className="list-details">
            <div className="detail-item">
              <i className="far fa-calendar"></i>
              <span>
                {tournament.rawDate ? new Date(tournament.rawDate).toLocaleString('uk-UA', {
                  day: '2-digit',
                  month: '2-digit',
                  year: 'numeric',
                  hour: '2-digit',
                  minute: '2-digit'
                }) : `${formatDate(tournament.date)} ${tournament.time || ''}`}
              </span>
            </div>
          </div>
        </div>
      </div>


    </div>
  );
};

export default TournamentCard;

