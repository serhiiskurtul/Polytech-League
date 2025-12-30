import React from 'react';

const TournamentFilters = ({ activeFilter, onFilterChange }) => {
  const filters = [
    { id: 'all', label: 'Всі турніри', icon: 'fas fa-trophy', count: 0 },
    { id: 'ongoing', label: 'Тривають', icon: 'fas fa-play-circle', color: '#10b981' },
    { id: 'upcoming', label: 'Майбутні', icon: 'fas fa-clock', color: '#f59e0b' },
    { id: 'completed', label: 'Завершені', icon: 'fas fa-check-circle', color: '#64748b' }
  ];

  const sports = [
    { id: 'football', label: 'Футзал', icon: 'fas fa-futbol' },
    { id: 'basketball', label: 'Баскетбол', icon: 'fas fa-basketball-ball' },
    { id: 'volleyball', label: 'Волейбол', icon: 'fas fa-volleyball-ball' },
    { id: 'chess', label: 'Шахи', icon: 'fas fa-chess' },
    { id: 'table-tennis', label: 'Настільний теніс', icon: 'fas fa-table-tennis' }
  ];

  return (
    <div className="tournament-filters">
      <div className="filter-section">
        <h4 className="filter-section-title">Статус турнірів</h4>
        <div className="filter-buttons">
          {filters.map(filter => (
            <button
              key={filter.id}
              className={`filter-btn ${activeFilter === filter.id ? 'active' : ''}`}
              onClick={() => onFilterChange(filter.id)}
              style={filter.color ? { '--filter-color': filter.color } : {}}
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
              className="sport-filter-btn"
              onClick={() => onFilterChange(sport.id)}
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