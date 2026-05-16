import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { getChampions } from '@/shared/api/tournaments.api';
import { Champion } from '@/shared/types/tournament.types';
import styles from './ChampionsPage.module.css';

const ChampionsPage: React.FC = () => {
  const { sport } = useParams<{ sport: string }>();
  const [champions, setChampions] = useState<Champion[]>([]);
  const [loading, setLoading] = useState(true);
  const [gender, setGender] = useState<number | undefined>(undefined);
  const [weightClass, setWeightClass] = useState<string | undefined>(undefined);

  const getSportTitle = (sportType: string) => {
    switch (sportType) {
      case 'chess': return 'шахів';
      case 'checkers': return 'шашок';
      case 'table-tennis': return 'настільного тенісу';
      case 'armwrestling': return 'армрестлінгу';
      case 'badminton': return 'бадмінтону';
      case 'football': return 'футболу';
      case 'basketball-3x3': return 'баскетболу 3x3';
      case 'volleyball': return 'волейболу';
      default: return '';
    }
  };

  const getWeightClassLabel = (wc: string) => {
    switch (wc) {
      case 'Under60kg': return 'до 60 кг';
      case 'Under70kg': return 'до 70 кг';
      case 'Under80kg': return 'до 80 кг';
      case 'Under90kg': return 'до 90 кг';
      case 'Plus90kg': return 'понад 90 кг';
      default: return wc;
    }
  };

  const getSportIcon = (sportType: string) => {
    switch (sportType) {
      case 'chess': return 'fa-chess-king';
      case 'checkers': return 'fa-chess-board';
      case 'table-tennis': return 'fa-table-tennis-paddle-ball';
      case 'armwrestling': return 'fa-handshake-angle';
      case 'badminton': return 'fa-shuttlecock';
      default: return 'fa-trophy';
    }
  };

  useEffect(() => {
    const fetchChampions = async () => {
      setLoading(true);
      try {
        const data = await getChampions(sport === 'all' ? undefined : sport, gender, weightClass);
        setChampions(data);
      } catch (error) {
        console.error('Error fetching champions:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchChampions();
  }, [sport, gender, weightClass]);

  // Reset filters when sport changes
  useEffect(() => {
    setGender(undefined);
    setWeightClass(undefined);
  }, [sport]);

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
          <i className={`fas ${getSportIcon(sport || 'all')}`} style={{ fontSize: '3.5rem', color: '#fbbf24', marginBottom: '1.5rem', filter: 'drop-shadow(0 0 10px rgba(251, 191, 36, 0.5))' }}></i>
          <h1 className={styles.title}>Зала слави {getSportTitle(sport || 'all')}</h1>
          <p className={styles.subtitle}>Чемпіони міжгрупових змагань</p>
        </div>
      </header>

      <main className={styles.content}>
        <div className={styles.sportFilters}>
          <Link to="/hall-of-fame/chess" className={`${styles.filterBtn} ${sport === 'chess' ? styles.active : ''}`}>Шахи</Link>
          <Link to="/hall-of-fame/checkers" className={`${styles.filterBtn} ${sport === 'checkers' ? styles.active : ''}`}>Шашки</Link>
          <Link to="/hall-of-fame/table-tennis" className={`${styles.filterBtn} ${sport === 'table-tennis' ? styles.active : ''}`}>Теніс</Link>
          <Link to="/hall-of-fame/armwrestling" className={`${styles.filterBtn} ${sport === 'armwrestling' ? styles.active : ''}`}>Армрестлінг</Link>
        </div>

        {(sport === 'chess' || sport === 'checkers') && (
          <div className={styles.genderFilters}>
            <button 
              className={`${styles.genderBtn} ${gender === undefined ? styles.activeGender : ''}`}
              onClick={() => setGender(undefined)}
            >
              Усі
            </button>
            <button 
              className={`${styles.genderBtn} ${gender === 1 ? styles.activeGender : ''}`}
              onClick={() => setGender(1)}
            >
              Чоловіки
            </button>
            <button 
              className={`${styles.genderBtn} ${gender === 2 ? styles.activeGender : ''}`}
              onClick={() => setGender(2)}
            >
              Жінки
            </button>
          </div>
        )}

        {sport === 'armwrestling' && (
          <div className={styles.genderFilters}>
            <button 
              className={`${styles.genderBtn} ${weightClass === undefined ? styles.activeGender : ''}`}
              onClick={() => setWeightClass(undefined)}
            >
              Усі категорії
            </button>
            <button 
              className={`${styles.genderBtn} ${weightClass === 'Under60kg' ? styles.activeGender : ''}`}
              onClick={() => setWeightClass('Under60kg')}
            >
              до 60 кг
            </button>
            <button 
              className={`${styles.genderBtn} ${weightClass === 'Under70kg' ? styles.activeGender : ''}`}
              onClick={() => setWeightClass('Under70kg')}
            >
              до 70 кг
            </button>
            <button 
              className={`${styles.genderBtn} ${weightClass === 'Under80kg' ? styles.activeGender : ''}`}
              onClick={() => setWeightClass('Under80kg')}
            >
              до 80 кг
            </button>
            <button 
              className={`${styles.genderBtn} ${weightClass === 'Under90kg' ? styles.activeGender : ''}`}
              onClick={() => setWeightClass('Under90kg')}
            >
              до 90 кг
            </button>
            <button 
              className={`${styles.genderBtn} ${weightClass === 'Plus90kg' ? styles.activeGender : ''}`}
              onClick={() => setWeightClass('Plus90kg')}
            >
              понад 90 кг
            </button>
          </div>
        )}

        {champions.length === 0 ? (
          <div className={styles.emptyState}>
            <i className="fas fa-award" style={{ fontSize: '4rem', opacity: 0.1, marginBottom: '1.5rem' }}></i>
            <p>Чемпіонів у цій категорії поки не визначено</p>
          </div>
        ) : (
          <div className={styles.championsGrid}>
            {champions.map((champion) => (
              <Link
                key={`${champion.playerId}-${champion.tournamentId}`}
                to={`/tournaments/${champion.tournamentId}`}
                className={styles.championCard}
              >
                <div className={styles.yearBadge}>{champion.seasonName}</div>
                <div className={styles.photoWrapper}>
                  <img
                    src={champion.playerPhotoUrl || '/champion-placeholder.png'}
                    alt={champion.playerName}
                    className={styles.playerPhoto}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = '/champion-placeholder.png';
                    }}
                  />
                  <div className={styles.medal}>
                    <i className="fas fa-medal"></i>
                  </div>
                </div>
                <div className={styles.info}>
                  <Link
                    to={`/players/${champion.playerId}`}
                    className={styles.playerName}
                    onClick={(e) => e.stopPropagation()}
                  >
                    {champion.playerName}
                  </Link>
                  <div className={styles.tournamentName}>
                    <i className="fas fa-calendar-check"></i> {champion.tournamentName}
                  </div>
                  {champion.weightClass && (
                    <div className={styles.points} style={{ color: '#fbbf24', marginTop: '0.25rem' }}>
                      <i className="fas fa-weight-hanging"></i> {getWeightClassLabel(champion.weightClass)}
                    </div>
                  )}
                  {champion.points > 0 && (
                    <div className={styles.points}>
                      <strong>{champion.points}</strong> очок
                    </div>
                  )}
                </div>
              </Link>
            ))}
          </div>
        )}
      </main>
    </div>
  );
};

export default ChampionsPage;
