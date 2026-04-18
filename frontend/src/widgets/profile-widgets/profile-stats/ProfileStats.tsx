import styles from './ProfileStats.module.css';

interface Stats {
  matchesPlayed: number;
  wins: number;
  eloRating: number;
  goalsScored: number;
}

interface ProfileStatsProps {
  stats: Stats;
}

const ProfileStats = ({ stats }: ProfileStatsProps) => {
  return (
    <section className={styles.card}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-chart-bar"></i> Статистика</h2>
      </div>
      
      <div className={styles.statsGrid}>
        <div className={styles.statCard}>
          <div className={styles.statValue}>{stats.matchesPlayed}</div>
          <div className={styles.statLabel}>Зіграних матчів</div>
        </div>
        <div className={styles.statCard}>
          <div className={styles.statValue}>{stats.wins}</div>
          <div className={styles.statLabel}>Перемог</div>
        </div>
        {stats.eloRating > 0 && (
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.eloRating}</div>
            <div className={styles.statLabel}>Рейтинг Elo (шахи)</div>
          </div>
        )}
        {stats.goalsScored > 0 && (
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.goalsScored}</div>
            <div className={styles.statLabel}>Забитих голів</div>
          </div>
        )}
      </div>
    </section>
  );
};

export default ProfileStats;
