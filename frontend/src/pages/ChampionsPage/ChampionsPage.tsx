import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { getChampions } from '@/shared/api/tournaments.api';
import { Champion } from '@/shared/types/tournament.types';
import styles from './ChampionsPage.module.css';

const ChampionsPage: React.FC = () => {
  const { sport } = useParams<{ sport: string }>();
  const [champions, setChampions] = useState<Champion[]>([]);
  const [selectedGender, setSelectedGender] = useState<'all' | 'male' | 'female'>('all');
  const [loading, setLoading] = useState(true);

  const getSportTitle = (sportType: string) => {
    switch (sportType) {
      case 'chess': return 'шахів';
      case 'checkers': return 'шашок';
      case 'table-tennis': return 'настільного тенісу';
      case 'armwrestling': return 'армреслінгу';
      case 'badminton': return 'бадмінтону';
      case 'futsal': return 'футзалу';
      case 'basketball-3x3': return 'баскетболу 3x3';
      case 'volleyball': return 'волейболу';
      default: return '';
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
        const data = await getChampions(sport === 'all' ? undefined : sport);
        setChampions(data);
      } catch (error) {
        console.error('Error fetching champions:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchChampions();
    setSelectedGender('all');
  }, [sport]);

  const filteredChampions = champions.filter(champion => {
    if ((sport === 'chess' || sport === 'checkers') && selectedGender !== 'all') {
      const name = champion.playerName.toLowerCase();
      // Heuristic: check for patronymic suffixes in the full name string
      const isFemale = name.includes('івна') || name.includes('ївна');
      const isMale = name.includes('вич');
      
      if (selectedGender === 'female') return isFemale;
      if (selectedGender === 'male') return isMale;
    }
    return true;
  });

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
          <Link to="/hall-of-fame/armwrestling" className={`${styles.filterBtn} ${sport === 'armwrestling' ? styles.active : ''}`}>Армреслінг</Link>
        </div>

        {(sport === 'chess' || sport === 'checkers') && (
          <div className={styles.genderFilters}>
            <button 
              className={`${styles.genderBtn} ${selectedGender === 'all' ? styles.activeGender : ''}`}
              onClick={() => setSelectedGender('all')}
            >
              Всі
            </button>
            <button 
              className={`${styles.genderBtn} ${selectedGender === 'male' ? styles.activeGender : ''}`}
              onClick={() => setSelectedGender('male')}
            >
              Чоловіки
            </button>
            <button 
              className={`${styles.genderBtn} ${selectedGender === 'female' ? styles.activeGender : ''}`}
              onClick={() => setSelectedGender('female')}
            >
              Жінки
            </button>
          </div>
        )}

        {filteredChampions.length === 0 ? (
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
                    src={champion.playerPhotoUrl || '/images/placeholders/user.png'}
                    alt={champion.playerName}
                    className={styles.playerPhoto}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = '/images/placeholders/user.png';
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
