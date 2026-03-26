interface Stats {
  totalEvents: number;
  daysWithEvents: number;
}

interface CalendarStatsProps {
  stats: Stats;
}

const CalendarStats = ({ stats }: CalendarStatsProps) => {
  return (
    <div className="calendar-stats">
      <div className="stat-card">
        <div className="stat-icon">
          <i className="fas fa-calendar-check"></i>
        </div>
        <div className="stat-info">
          <div className="stat-value">{stats.totalEvents}</div>
          <div className="stat-label">Подій у місяці</div>
        </div>
      </div>
      <div className="stat-card">
        <div className="stat-icon">
          <i className="fas fa-calendar-day"></i>
        </div>
        <div className="stat-info">
          <div className="stat-value">{stats.daysWithEvents}</div>
          <div className="stat-label">Днів з подіями</div>
        </div>
      </div>
    </div>
  );
};

export default CalendarStats;
