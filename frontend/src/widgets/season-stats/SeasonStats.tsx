import { useState, useEffect } from 'react';
import styles from './SeasonStats.module.css';
import { getTournamentsStats } from '@/entities/tournament/api/tournaments.api';
import { getCurrentSeason } from '@/entities/rating/api/seasonRating.api';

interface Stats {
  totalMatches: number;
  activeTournaments: number;
  registeredPlayers: number;
  progress: number;
}

const SeasonStats= () => {
  const [stats, setStats] = useState<Stats | null>(null);
  const [seasonName, setSeasonName] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        setLoading(true);
        const [data, season] = await Promise.all([
          getTournamentsStats(),
          getCurrentSeason().catch(() => null),
        ]);
        setSeasonName(season?.name ?? null);
        setStats({
          totalMatches: data.totalMatchesCount,
          activeTournaments: data.activeTournamentsCount,
          registeredPlayers: data.registeredPlayersCount,
          progress: data.seasonProgress,
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
          {seasonName ? `Сезон ${seasonName}` : 'Сезон'}
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
