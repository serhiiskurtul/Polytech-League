import React from 'react';

const TournamentStats = ({ stats }) => {
  return (
    <div className="tournament-stats">
      <div className="stat-card">
        <div className="stat-icon" style={{ backgroundColor: 'rgba(59, 130, 246, 0.1)' }}>
          <i className="fas fa-trophy" style={{ color: '#3b82f6' }}></i>
        </div>
        <div className="stat-content">
          <div className="stat-value">{stats.total}</div>
          <div className="stat-label">Всього турнірів</div>
        </div>
      </div>

      <div className="stat-card">
        <div className="stat-icon" style={{ backgroundColor: 'rgba(16, 185, 129, 0.1)' }}>
          <i className="fas fa-play-circle" style={{ color: '#10b981' }}></i>
        </div>
        <div className="stat-content">
          <div className="stat-value">{stats.ongoing}</div>
          <div className="stat-label">Тривають</div>
        </div>
      </div>

      <div className="stat-card">
        <div className="stat-icon" style={{ backgroundColor: 'rgba(245, 158, 11, 0.1)' }}>
          <i className="fas fa-clock" style={{ color: '#f59e0b' }}></i>
        </div>
        <div className="stat-content">
          <div className="stat-value">{stats.upcoming}</div>
          <div className="stat-label">Майбутні</div>
        </div>
      </div>

      <div className="stat-card">
        <div className="stat-icon" style={{ backgroundColor: 'rgba(244, 63, 94, 0.1)' }}>
          <i className="fas fa-users" style={{ color: '#f43f5e' }}></i>
        </div>
        <div className="stat-content">
          <div className="stat-value">{stats.totalParticipants}</div>
          <div className="stat-label">Учасників</div>
        </div>
      </div>

      <div className="stat-card">
        <div className="stat-icon" style={{ backgroundColor: 'rgba(139, 92, 246, 0.1)' }}>
          <i className="fas fa-user-check" style={{ color: '#8b5cf6' }}></i>
        </div>
        <div className="stat-content">
          <div className="stat-value">{stats.registeredCount}</div>
          <div className="stat-label">Ваших реєстрацій</div>
        </div>
      </div>
    </div>
  );
};

export default TournamentStats;