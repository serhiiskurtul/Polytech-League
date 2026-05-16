import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './RecentMatches.module.css';
import { getRecentResults } from '@/shared/api/tournaments.api';
import { RecentMatch } from '@/shared/types/tournament.types';

const RecentMatches = () => {
  const navigate = useNavigate();
  const [matches, setMatches] = useState<RecentMatch[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedSport, setSelectedSport] = useState('Всі');

  const sportSlugMap: Record<string, string> = {
    'Всі': 'all',
    'Футбол': 'football',
    'Баскетбол': 'basketball-3x3',
    'Волейбол': 'volleyball',
    'Настільний теніс': 'table-tennis',
    'Шахи': 'chess',
    'Шашки': 'checkers',
    'Пляжний волейбол': 'beach-volleyball',
    'Бадмінтон': 'badminton',
    'Армрестлінг': 'armwrestling'
  };

  const reverseSportMap: Record<string, string> = {
    'football': 'Футбол',
    'basketball-3x3': 'Баскетбол',
    'volleyball': 'Волейбол',
    'table-tennis': 'Настільний теніс',
    'chess': 'Шахи',
    'checkers': 'Шашки',
    'beach-volleyball': 'Пляжний волейбол',
    'badminton': 'Бадмінтон',
    'armwrestling': 'Армрестлінг'
  };

  const sports = Object.keys(sportSlugMap);

  useEffect(() => {
    const fetchMatches = async () => {
      try {
        setLoading(true);
        const slug = sportSlugMap[selectedSport];
        const data = await getRecentResults(4, slug === 'all' ? undefined : slug);
        setMatches(data);
      } catch (error) {
        console.error('Помилка при завантаженні результатів:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchMatches();
  }, [selectedSport]);

  const formatDate = (dateString: string) => {
    const matchDate = dateString.split(',')[0];
    
    if (matchDate === 'Сьогодні') return <span className={styles.today}>Сьогодні</span>;
    if (matchDate === 'Вчора') return <span className={styles.yesterday}>Вчора</span>;
    return <span>{dateString}</span>;
  };

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Футбол': 'fa-futbol',
      'Баскетбол': 'fa-basketball-ball',
      'Волейбол': 'fa-volleyball-ball',
      'Настільний теніс': 'fa-table-tennis',
      'Шахи': 'fa-chess',
      'Бадмінтон': 'fa-table-tennis',
      'Армрестлінг': 'fa-hand-fist'
    };
    return icons[sport] || 'fa-trophy';
  };

  const getSportEmoji = (sport: string) => {
    const icons: Record<string, string> = {
      'Футбол': 'fas fa-futbol',
      'Баскетбол': 'fas fa-basketball-ball',
      'Волейбол': 'fas fa-volleyball-ball',
      'Настільний теніс': 'fas fa-table-tennis',
      'Шахи': 'fas fa-chess',
      'Шашки': 'fas fa-chess-board',
      'Пляжний волейбол': 'fas fa-umbrella-beach',
      'Бадмінтон': 'fas fa-feather-alt',
      'Армрестлінг': 'fas fa-fist-raised',
      'Всі': 'fas fa-filter'
    };
    const iconClass = icons[sport] || 'fas fa-trophy';
    return <i className={iconClass}></i>;
  };

  const getSportIconClass = (sportSlug: string) => {
    const icons: Record<string, string> = {
      'football': 'fa-futbol',
      'basketball-3x3': 'fa-basketball-ball',
      'volleyball': 'fa-volleyball-ball',
      'table-tennis': 'fa-table-tennis',
      'chess': 'fa-chess',
      'checkers': 'fa-chess-board',
      'beach-volleyball': 'fa-umbrella-beach',
      'badminton': 'fa-feather-alt',
      'armwrestling': 'fa-fist-raised'
    };
    return icons[sportSlug] || 'fa-trophy';
  };

  return (
    <section className={styles.recentMatches}>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <div className={styles.headerLeft}>
            <h2 className={styles.cardTitle}>
              <i className={`fas fa-history ${styles.cardIcon}`}></i>
              Останні результати
            </h2>
            <div className={`${styles.badge} ${styles.badgePrimary}`}>
              {loading ? '...' : matches.length} матчів
            </div>
          </div>
          
          <div className={styles.sportFilters}>
            {sports.map(sport => (
              <button
                key={sport}
                className={`${styles.sportFilter} ${selectedSport === sport ? styles.active : ''}`}
                onClick={(e) => {
                  e.stopPropagation();
                  setSelectedSport(sport);
                }}
              >
                {getSportEmoji(sport)}
                {sport}
              </button>
            ))}
          </div>
        </div>

        <div className={styles.matchesList}>
          {loading ? (
            [...Array(4)].map((_, i) => (
              <div key={i} className={`${styles.matchCard} ${styles.skeleton}`}>
                <div className={styles.skeletonHeader}></div>
                <div className={styles.skeletonContent}></div>
              </div>
            ))
          ) : matches.length > 0 ? (
            matches.map(match => (
              <div 
                key={match.id} 
                className={styles.matchCard}
                onClick={() => navigate(`/tournaments/${match.tournamentId}`)}
              >
                <div className={styles.matchHeader}>
                  <div className={styles.matchSport}>
                    <i className={`fas ${getSportIconClass(match.sport)} ${styles.sportIcon}`}></i>
                    {reverseSportMap[match.sport] || match.sport}
                  </div>
                  <div className={`${styles.statusBadge} ${match.status === 'Completed' ? styles.statusCompleted : styles.statusLive}`}>
                    <i className={`fas ${match.status === 'Completed' ? 'fa-check-circle' : 'fa-clock'}`}></i>
                    {match.status === 'Completed' ? 'Завершено' : match.status}
                  </div>
                </div>
                
                <div className={styles.matchContent}>
                  <div className={styles.teamsContainer}>
                    <div className={styles.teamInfo}>
                      <div className={styles.teamLogo} style={{ backgroundColor: 'var(--primary)' }}>
                        {match.team1Name.substring(0, 2).toUpperCase()}
                      </div>
                      <span className={styles.teamName}>{match.team1Name}</span>
                    </div>
                    
                    <div className={styles.scoreContainer}>
                      <div className={`${styles.score} ${Number(match.score1) > Number(match.score2) ? styles.winner : ''}`}>
                        {match.score1}
                      </div>
                      <div className={styles.scoreDivider}>:</div>
                      <div className={`${styles.score} ${Number(match.score2) > Number(match.score1) ? styles.winner : ''}`}>
                        {match.score2}
                      </div>
                    </div>
                    
                    <div className={styles.teamInfo} style={{ flexDirection: 'row-reverse' }}>
                      <div className={styles.teamLogo} style={{ backgroundColor: 'var(--accent)' }}>
                        {match.team2Name.substring(0, 2).toUpperCase()}
                      </div>
                      <span className={styles.teamName} style={{ textAlign: 'right' }}>{match.team2Name}</span>
                    </div>
                  </div>

                  <div className={styles.matchMeta}>
                    <div className={styles.metaItem}>
                      <i className="far fa-calendar-alt"></i>
                      {formatDate(match.date)}
                    </div>
                    <div className={styles.metaItem}>
                      <i className="fas fa-map-marker-alt"></i>
                      {match.location}
                    </div>
                  </div>
                  
                  <div className={styles.tournamentIndicator}>
                    <i className="fas fa-trophy"></i>
                    {match.tournamentName}
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className={styles.noMatches}>Поки немає результатів для цієї дисципліни</div>
          )}
        </div>

      </div>
    </section>
  );
};

export default RecentMatches;
