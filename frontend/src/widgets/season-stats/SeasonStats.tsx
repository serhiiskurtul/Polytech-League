import { useState, useEffect } from 'react';
import styles from './SeasonStats.module.css';
import { getTournamentsStats } from '@/shared/api/tournaments.api';

interface Stats {
  totalMatches: number;
  activeTournaments: number;
  registeredPlayers: number;
  progress: number;
  topSport: string;
}

const SeasonStats= () => {
  const [stats, setStats] = useState<Stats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        setLoading(true);
        const data = await getTournamentsStats();
        setStats({
          totalMatches: data.totalMatchesCount,
          activeTournaments: data.activeTournamentsCount,
          registeredPlayers: data.registeredPlayersCount,
          progress: data.seasonProgress,
          topSport: data.topSport === 'Футзал' ? '⚽' : '🏆'
        });
      } catch (error) {
        console.error('Помилка при завантаженні статистики:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  if (loading || !stats) {
    return (
      <div className={`${styles.seasonStatsCompact} ${styles.skeleton}`}>
        <div className={styles.skeletonHeader}></div>
        <div className={styles.skeletonStats}></div>
        <div className={styles.skeletonProgress}></div>
      </div>
    );
  }

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
          <div className={styles.statLabel}>Змагань</div>
        </div>
        <div className={styles.statItem}>
          <div className={styles.statValue}>{stats.registeredPlayers}</div>
          <div className={styles.statLabel}>Гравців</div>
        </div>
      </div>

      <div className={styles.progressSection}>
        <div className={styles.progressInfo}>
          <span>Прогрес: {stats.progress}%</span>
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
