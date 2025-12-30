import React from 'react';

const TournamentCard = ({ tournament, viewMode, onRegister }) => {
  // Іконки для видів спорту
  const sportIcons = {
    'football': 'fas fa-futbol',
    'basketball': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'chess': 'fas fa-chess',
    'table-tennis': 'fas fa-table-tennis',
    'badminton': 'fas fa-feather-alt',
    'armwrestling': 'fas fa-hand-fist'
  };

  // Кольори для статусів
  const statusColors = {
    'ongoing': '#10b981',
    'upcoming': '#f59e0b',
    'completed': '#64748b'
  };

  // Назви статусів
  const statusLabels = {
    'ongoing': 'Триває',
    'upcoming': 'Скоро',
    'completed': 'Завершено'
  };

  // Назви видів спорту
  const sportNames = {
    'football': 'Футзал',
    'basketball': 'Баскетбол',
    'volleyball': 'Волейбол',
    'chess': 'Шахи',
    'table-tennis': 'Настільний теніс',
    'badminton': 'Бадмінтон',
    'armwrestling': 'Армреслінг'
  };

  // Форматування дати
  const formatDate = (dateString) => {
    if (dateString.includes(' - ')) {
      const [start, end] = dateString.split(' - ');
      return `${start} → ${end}`;
    }
    return dateString;
  };

  // Картка в режимі сітки
  if (viewMode === 'grid') {
    return (
      <div className={`tournament-card grid ${tournament.featured ? 'featured' : ''}`}>
        {tournament.featured && (
          <div className="featured-badge">
            <i className="fas fa-star"></i> Рекомендовано
          </div>
        )}

        <div className="tournament-header">
          <div className="tournament-sport">
            <i className={sportIcons[tournament.sport]}></i>
            <span>{sportNames[tournament.sport]}</span>
          </div>
          <div 
            className="tournament-status"
            style={{ backgroundColor: `${statusColors[tournament.status]}20`, color: statusColors[tournament.status] }}
          >
            {statusLabels[tournament.status]}
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
              style={{ 
                width: tournament.status === 'ongoing' ? '60%' : 
                       tournament.status === 'completed' ? '100%' : '10%' 
              }}
            ></div>
          </div>
        </div>

        <div className="tournament-footer">
          <div className="registration-info">
            {tournament.registered ? (
              <span className="registered-badge">
                <i className="fas fa-check-circle"></i> Ви зареєстровані
              </span>
            ) : (
              <span className="registration-deadline">
                Реєстрація до: {tournament.registrationEnd}
              </span>
            )}
          </div>
          
          <div className="tournament-actions">
            <button 
              className={`btn ${tournament.registered ? 'btn-secondary' : 'btn-primary'}`}
              onClick={() => onRegister(tournament.id)}
            >
              {tournament.registered ? 'Скасувати' : 'Зареєструватись'}
            </button>
            <button className="btn btn-outline">
              <i className="fas fa-eye"></i>
            </button>
          </div>
        </div>
      </div>
    );
  }

  // Картка в режимі списку
  return (
    <div className="tournament-card list">
      <div className="list-main">
        <div className="list-sport">
          <i className={sportIcons[tournament.sport]}></i>
        </div>
        
        <div className="list-content">
          <div className="list-header">
            <h3 className="tournament-title">{tournament.title}</h3>
            <div className="list-meta">
              <span className="tournament-sport-name">{sportNames[tournament.sport]}</span>
              <span className="tournament-location">
                <i className="fas fa-map-marker-alt"></i> {tournament.location}
              </span>
            </div>
          </div>
          
          <p className="tournament-description">{tournament.description}</p>
          
          <div className="list-details">
            <div className="detail-item">
              <i className="far fa-calendar"></i>
              <span>{formatDate(tournament.date)}</span>
            </div>
            <div className="detail-item">
              <i className="fas fa-users"></i>
              <span>{tournament.participants} учасників</span>
            </div>
            <div 
              className="tournament-status"
              style={{ backgroundColor: `${statusColors[tournament.status]}20`, color: statusColors[tournament.status] }}
            >
              {statusLabels[tournament.status]}
            </div>
          </div>
        </div>
      </div>

      <div className="list-sidebar">
        <div className="list-progress">
          <div className="progress-label">{tournament.currentRound}</div>
          <div className="progress-bar">
            <div 
              className="progress-fill"
              style={{ 
                width: tournament.status === 'ongoing' ? '60%' : 
                       tournament.status === 'completed' ? '100%' : '10%' 
              }}
            ></div>
          </div>
        </div>
        
        <div className="list-actions">
          {tournament.registered ? (
            <span className="registered-badge">
              <i className="fas fa-check-circle"></i> Зареєстровано
            </span>
          ) : (
            <span className="registration-deadline">
              До {tournament.registrationEnd}
            </span>
          )}
          
          <div className="action-buttons">
            <button 
              className={`btn ${tournament.registered ? 'btn-secondary' : 'btn-primary'}`}
              onClick={() => onRegister(tournament.id)}
            >
              {tournament.registered ? 'Скасувати' : 'Реєстрація'}
            </button>
            <button className="btn btn-outline">
              Деталі
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TournamentCard;