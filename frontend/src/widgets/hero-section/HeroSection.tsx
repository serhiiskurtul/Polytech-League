import { useEffect, useState } from 'react';
import styles from './HeroSection.module.css';
import { getTournamentsStats, TournamentStats } from '@/shared/api/tournaments.api';

const HeroSection = () => {
  const [stats, setStats] = useState<TournamentStats | null>(null);

  useEffect(() => {
    getTournamentsStats()
      .then(data => setStats(data))
      .catch(err => console.error(err));
  }, []);

  return (
    <section className={styles.heroSection}>
      <div className={styles.heroCard}>
        <h1 className={styles.heroTitle}>Спортивна платформа ЧПФК</h1>
        <p className={styles.heroDescription}>
          Приєднуйтесь до турнірів, стежте за результатами матчів та ведіть статистику
        </p>
        
        <div className={styles.heroStats}>
          <div className={styles.stat}>
            <div className={styles.statValue}>{stats?.activeTournamentsCount ?? '...'}</div>
            <div className={styles.statLabel}>Активні турніри</div>
          </div>
          <div className={styles.stat}>
            <div className={styles.statValue}>{stats?.registeredPlayersCount ?? '...'}</div>
            <div className={styles.statLabel}>Зареєстровані гравці</div>
          </div>
          <div className={styles.stat}>
            <div className={styles.statValue}>9</div>
            <div className={styles.statLabel}>Спортивних дисциплін</div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;
