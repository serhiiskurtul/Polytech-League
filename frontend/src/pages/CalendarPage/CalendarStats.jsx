import React from 'react';

const CalendarStats = ({ stats }) => {
  return (
    <div className="calendar-stats">
      <div className="stat-card">
        <div className="stat-value">{stats.totalEvents}</div>
        <div className="stat-label">Подій у цьому місяці</div>
      </div>
      <div className="stat-card">
        <div className="stat-value">{stats.footballEvents}</div>
        <div className="stat-label">Матчі з футзалу</div>
      </div>
      <div className="stat-card">
        <div className="stat-value">{stats.basketballEvents}</div>
        <div className="stat-label">Матчі з баскетболу</div>
      </div>
      <div className="stat-card">
        <div className="stat-value">{stats.daysWithEvents}</div>
        <div className="stat-label">Днів з подіями</div>
      </div>
    </div>
  );
};

export default CalendarStats;