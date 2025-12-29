import React, { useState } from 'react';
import styles from './RecentMatches.module.css';

const RecentMatches = () => {
  const [matches] = useState([
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
  const [expandedMatch, setExpandedMatch] = useState(null);

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол', 'Настільний теніс', 'Шахи'];

  const filteredMatches = selectedSport === 'Всі' 
    ? matches 
    : matches.filter(match => match.sport === selectedSport);

  const toggleMatchDetails = (matchId) => {
    setExpandedMatch(expandedMatch === matchId ? null : matchId);
  };

  const formatDate = (dateString) => {
    const matchDate = dateString.split(',')[0];
    
    if (matchDate === 'Сьогодні') return <span className={styles.today}>Сьогодні</span>;
    if (matchDate === 'Вчора') return <span className={styles.yesterday}>Вчора</span>;
    return <span>{dateString}</span>;
  };

  const getSportIcon = (sport) => {
    const icons = {
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

  const handleShowMore = () => {
    // Логіка завантаження більше матчів
    console.log('Завантажити більше матчів');
    // Тут можна додати запит на сервер
  };

  const handleMatchClick = (matchId) => {
    console.log(`Перейти до матчу ${matchId}`);
    // Тут можна додати перехід на сторінку матчу
  };

  return (
    <section className={styles.recentMatches}>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <div className={styles.headerLeft}>
            <h2 className={styles.cardTitle}>
              <i className={`fas fa-history ${styles.cardIcon}`}></i>
              Останні матчі
            </h2>
            <div className={`${styles.badge} ${styles.badgePrimary}`}>
              {matches.length} матчів
            </div>
          </div>
          
          <div className={styles.sportFilters}>
            {sports.map((sport) => (
              <button
                key={sport}
                className={`${styles.sportFilter} ${selectedSport === sport ? styles.active : ''}`}
                onClick={() => setSelectedSport(sport)}
              >
                {sport === 'Всі' ? (
                  <i className="fas fa-list"></i>
                ) : (
                  <i className={`fas ${getSportIcon(sport)}`}></i>
                )}
                {sport}
              </button>
            ))}
          </div>
        </div>
        
        <div className={styles.matchesList}>
          {filteredMatches.map((match) => (
            <div 
              key={match.id} 
              className={`${styles.matchCard} ${expandedMatch === match.id ? styles.expanded : ''}`}
              onClick={() => handleMatchClick(match.id)}
            >
              <div className={styles.matchHeader}>
                <div className={styles.matchSport}>
                  <i className={`fas ${getSportIcon(match.sport)} ${styles.sportIcon}`}></i>
                  {match.sport}
                </div>
                <div className={`${styles.statusBadge} ${styles.statusCompleted}`}>
                  <i className="fas fa-check-circle"></i>
                  {match.status}
                </div>
              </div>
              
              <div className={styles.matchContent}>
                <div className={styles.teamsContainer}>
                  <div className={styles.teamInfo}>
                    <div 
                      className={styles.teamLogo} 
                      style={{ background: match.team1Color }}
                    >
                      {match.team1.split('-')[0] || match.team1.charAt(0)}
                    </div>
                    <div className={styles.teamName}>{match.team1}</div>
                  </div>
                  
                  <div className={styles.scoreContainer}>
                    <div className={`${styles.score} ${match.winner === match.team1 ? styles.winner : ''}`}>
                      {match.score1}
                    </div>
                    <div className={styles.scoreDivider}>:</div>
                    <div className={`${styles.score} ${match.winner === match.team2 ? styles.winner : ''}`}>
                      {match.score2}
                    </div>
                  </div>
                  
                  <div className={styles.teamInfo}>
                    <div className={styles.teamName}>{match.team2}</div>
                    <div 
                      className={styles.teamLogo} 
                      style={{ background: match.team2Color }}
                    >
                      {match.team2.split('-')[0] || match.team2.charAt(0)}
                    </div>
                  </div>
                </div>
                
                <div className={styles.matchMeta}>
                  <div className={styles.metaItem}>
                    <i className="far fa-calendar"></i>
                    {formatDate(match.date)}
                  </div>
                  <div className={styles.metaItem}>
                    <i className="fas fa-map-marker-alt"></i>
                    {match.location}
                  </div>
                </div>
                
                <div className={styles.matchDetails}>
                  <div className={styles.matchWinner}>
                    <i className="fas fa-trophy"></i>
                    <span>Переможець: <strong>{match.winner}</strong></span>
                  </div>
                  <div className={styles.matchDescription}>
                    {match.details}
                  </div>
                </div>
                
                <button 
                  className={styles.detailsButton}
                  onClick={(e) => {
                    e.stopPropagation();
                    toggleMatchDetails(match.id);
                  }}
                >
                  <i className={`fas ${expandedMatch === match.id ? 'fa-chevron-up' : 'fa-chevron-down'}`}></i>
                  {expandedMatch === match.id ? 'Менше' : 'Детальніше'}
                </button>
                
                {expandedMatch === match.id && (
                  <div className={styles.expandedDetails}>
                    <div className={styles.statistics}>
                      <h4>Статистика матчу:</h4>
                      <div className={styles.statsGrid}>
                        <div className={styles.statItem}>
                          <div className={styles.statLabel}>Удари по воротах</div>
                          <div className={styles.statValues}>
                            <span>18</span>
                            <span>12</span>
                          </div>
                        </div>
                        <div className={styles.statItem}>
                          <div className={styles.statLabel}>Порушення</div>
                          <div className={styles.statValues}>
                            <span>8</span>
                            <span>10</span>
                          </div>
                        </div>
                        <div className={styles.statItem}>
                          <div className={styles.statLabel}>Володіння м'ячем</div>
                          <div className={styles.statValues}>
                            <span>58%</span>
                            <span>42%</span>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <div className={styles.actions}>
                      <button className={`${styles.actionButton} ${styles.reportButton}`}>
                        <i className="fas fa-file-alt"></i>
                        Звіт матчу
                      </button>
                      <button className={`${styles.actionButton} ${styles.photosButton}`}>
                        <i className="fas fa-camera"></i>
                        Фото
                      </button>
                      <button className={`${styles.actionButton} ${styles.videoButton}`}>
                        <i className="fas fa-video"></i>
                        Відео
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
        
        <div className={styles.viewAllContainer}>
          <button className={styles.viewAllButton} onClick={handleShowMore}>
            <i className="fas fa-history"></i>
            Показати всі матчі
          </button>
        </div>
      </div>
    </section>
  );
};

export default RecentMatches;