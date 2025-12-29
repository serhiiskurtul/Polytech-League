import React from 'react';
import styles from './SeasonStats.module.css';

const SeasonStats= () => {
  const stats = {
    totalMatches: 156,
    activeTournaments: 8,
    registeredPlayers: 324,
    progress: 68,
    topSport: '⚽'
  };

  return (
    <div className={styles.seasonStatsCompact}>
      <div className={styles.header}>
        <h4 className={styles.title}>
          <i className="fas fa-chart-pie"></i>
          Сезон 2025/2026
        </h4>
      </div>

      <div className={styles.statsRow}>
        <div className={styles.statItem}>
          <div className={styles.statValue}>{stats.totalMatches}</div>
          <div className={styles.statLabel}>Матчів</div>
        </div>
        <div className={styles.statItem}>
          <div className={styles.statValue}>{stats.activeTournaments}</div>
          <div className={styles.statLabel}>Турнірів</div>
        </div>
        <div className={styles.statItem}>
          <div className={styles.statValue}>{stats.registeredPlayers}</div>
          <div className={styles.statLabel}>Гравців</div>
        </div>
      </div>

      <div className={styles.progressSection}>
        <div className={styles.progressInfo}>
          <span>Прогрес: {stats.progress}%</span>
          <span>Топ: {stats.topSport}</span>
        </div>
        <div className={styles.progressBar}>
          <div 
            className={styles.progressFill}
            style={{ width: `${stats.progress}%` }}
          ></div>
        </div>
      </div>
    </div>
  );
};

export default SeasonStats;