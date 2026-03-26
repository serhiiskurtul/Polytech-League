import { useState } from 'react';
import styles from './RecentMatches.module.css';

interface Match {
  id: number;
  sport: string;
  team1: string;
  team2: string;
  score1: number;
  score2: number;
  status: string;
  date: string;
  location: string;
  winner: string;
  details: string;
  team1Color: string;
  team2Color: string;
}

const RecentMatches = () => {
  const [matches] = useState<Match[]>([
    {
      id: 1,
      sport: 'Футзал',
      team1: 'ІТ-31',
      team2: 'ЕК-22',
      score1: 6,
      score2: 4,
      status: 'Завершено',
      date: 'Сьогодні, 14:30',
      location: 'Спортзал №2',
      winner: 'ІТ-31',
      details: 'Чвертьфінал чемпіонату коледжу',
      team1Color: 'var(--primary)',
      team2Color: 'var(--accent)'
    },
    {
      id: 2,
      sport: 'Баскетбол',
      team1: 'МК-41',
      team2: 'ПР-23',
      score1: 42,
      score2: 38,
      status: 'Завершено',
      date: 'Сьогодні, 12:00',
      location: 'Спортзал №1',
      winner: 'МК-41',
      details: 'Груповий етап',
      team1Color: 'var(--success)',
      team2Color: 'var(--warning)'
    },
    {
      id: 3,
      sport: 'Волейбол',
      team1: 'ФІ-21',
      team2: 'ЕП-32',
      score1: 3,
      score2: 1,
      status: 'Завершено',
      date: 'Вчора, 16:45',
      location: 'Спортзал №3',
      winner: 'ФІ-21',
      details: 'Півфінал кубка декана',
      team1Color: '#8b5cf6',
      team2Color: '#f59e0b'
    },
    {
      id: 4,
      sport: 'Настільний теніс',
      team1: 'А.Іванов',
      team2: 'В.Коваль',
      score1: 3,
      score2: 2,
      status: 'Завершено',
      date: 'Вчора, 10:00',
      location: 'Бібліотека, 3 поверх',
      winner: 'А.Іванов',
      details: 'Чвертьфінал особистого заліку',
      team1Color: 'var(--primary)',
      team2Color: '#10b981'
    }
  ]);

  const [selectedSport, setSelectedSport] = useState('Всі');
  const [expandedMatch, setExpandedMatch] = useState<number | null>(null);

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол', 'Настільний теніс', 'Шахи'];

  const filteredMatches = selectedSport === 'Всі' 
    ? matches 
    : matches.filter(match => match.sport === selectedSport);

  const toggleMatchDetails = (matchId: number) => {
    setExpandedMatch(expandedMatch === matchId ? null : matchId);
  };

  const formatDate = (dateString: string) => {
    const matchDate = dateString.split(',')[0];
    
    if (matchDate === 'Сьогодні') return <span className={styles.today}>Сьогодні</span>;
    if (matchDate === 'Вчора') return <span className={styles.yesterday}>Вчора</span>;
    return <span>{dateString}</span>;
  };

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Футзал': 'fa-futbol',
      'Баскетбол': 'fa-basketball-ball',
      'Волейбол': 'fa-volleyball-ball',
      'Настільний теніс': 'fa-table-tennis',
      'Шахи': 'fa-chess',
      'Бадмінтон': 'fa-table-tennis',
      'Армреслінг': 'fa-hand-fist'
    };
    return icons[sport] || 'fa-trophy';
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
              {filteredMatches.length} матчів
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
                {sport}
              </button>
            ))}
          </div>
        </div>

        <div className={styles.matchesList}>
          {filteredMatches.length > 0 ? (
            filteredMatches.map(match => (
              <div 
                key={match.id} 
                className={`${styles.matchCard} ${expandedMatch === match.id ? styles.expanded : ''}`}
                onClick={() => toggleMatchDetails(match.id)}
              >
                <div className={styles.matchHeader}>
                  <div className={styles.matchSport}>
                    <i className={`fas ${getSportIcon(match.sport)} ${styles.sportIcon}`}></i>
                    {match.sport}
                  </div>
                  <div className={`${styles.statusBadge} ${match.status === 'Завершено' ? styles.statusCompleted : styles.statusLive}`}>
                    {match.status}
                  </div>
                </div>
                
                <div className={styles.matchContent}>
                  <div className={styles.teamsContainer}>
                    <div className={styles.teamInfo}>
                      <div className={styles.teamAvatar} style={{ backgroundColor: match.team1Color }}>
                        {match.team1.substring(0, 2).toUpperCase()}
                      </div>
                      <span className={styles.teamName}>{match.team1}</span>
                    </div>
                    
                    <div className={styles.scoreContainer}>
                      <div className={styles.scoreValue}>{match.score1}</div>
                      <div className={styles.scoreDivider}>:</div>
                      <div className={styles.scoreValue}>{match.score2}</div>
                    </div>
                    
                    <div className={styles.teamInfo} style={{ flexDirection: 'row-reverse' }}>
                      <div className={styles.teamAvatar} style={{ backgroundColor: match.team2Color }}>
                        {match.team2.substring(0, 2).toUpperCase()}
                      </div>
                      <span className={styles.teamName} style={{ textAlign: 'right' }}>{match.team2}</span>
                    </div>
                  </div>
                </div>

                <div className={styles.matchFooter}>
                  <div className={styles.matchDate}>
                    <i className="far fa-clock"></i>
                    {formatDate(match.date)}
                  </div>
                  <div className={styles.matchLocation}>
                    <i className="fas fa-map-marker-alt"></i>
                    {match.location}
                  </div>
                </div>

                {expandedMatch === match.id && (
                  <div className={styles.matchDetails}>
                    <div className={styles.detailsGrid}>
                      <div className={styles.detailItem}>
                        <div className={styles.detailLabel}>Турнір/Етап</div>
                        <div className={styles.detailValue}>{match.details}</div>
                      </div>
                      <div className={styles.winnerSection}>
                        <i className="fas fa-crown"></i>
                        <span>Переможець: <strong>{match.winner}</strong></span>
                      </div>
                    </div>
                  </div>
                )}
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
