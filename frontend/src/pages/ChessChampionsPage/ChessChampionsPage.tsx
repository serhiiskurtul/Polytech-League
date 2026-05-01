import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getChessChampions } from '@/shared/api/chess.api';
import { ChessChampionDto } from '@/shared/types/chess.types';
import styles from './ChessChampionsPage.module.css';

const ChessChampionsPage: React.FC = () => {
  const [champions, setChampions] = useState<ChessChampionDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchChampions = async () => {
      try {
        const data = await getChessChampions();
        setChampions(data);
      } catch (error) {
        console.error('Error fetching chess champions:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchChampions();
  }, []);

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
        <p>Завантаження зали слави...</p>
      </div>
    );
  }

  return (
    <div className={styles.pageContainer}>
      <header className={styles.header}>
        <div className={styles.headerContent}>
          <i className="fas fa-chess-king" style={{ fontSize: '3rem', color: '#f59e0b', marginBottom: '1rem' }}></i>
          <h1 className={styles.title}>Зала слави шахів</h1>
          <p className={styles.subtitle}>Чемпіони Polytech League міжгрупових змагань</p>
        </div>
      </header>

      <main className={styles.content}>
        {champions.length === 0 ? (
          <div className={styles.emptyState}>
            <i className="fas fa-trophy" style={{ fontSize: '4rem', opacity: 0.2 }}></i>
            <p>Чемпіонів поки не визначено</p>
          </div>
        ) : (
          <div className={styles.championsGrid}>
            {champions.map((champion, index) => (
              <div key={`${champion.playerId}-${champion.date}`} className={styles.championCard}>
                <div className={styles.yearBadge}>{champion.seasonName}</div>
                <div className={styles.photoWrapper}>
                  <img 
                    src={champion.playerPhotoUrl || '/images/placeholders/user.png'} 
                    alt={champion.playerName} 
                    className={styles.playerPhoto}
                  />
                  <div className={styles.medal}>
                    <i className="fas fa-medal"></i>
                  </div>
                </div>
                <div className={styles.info}>
                  <Link to={`/profile/${champion.playerId}`} className={styles.playerName}>
                    {champion.playerName}
                  </Link>
                  <div className={styles.tournamentName}>
                    <i className="fas fa-calendar-alt"></i> {champion.tournamentName}
                  </div>
                  <div className={styles.points}>
                    <strong>{champion.points}</strong> очок
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </main>

      <footer className={styles.footer}>
        <p>&copy; {new Date().getFullYear()} Polytech League. Всі права захищено.</p>
      </footer>
    </div>
  );
};

export default ChessChampionsPage;
