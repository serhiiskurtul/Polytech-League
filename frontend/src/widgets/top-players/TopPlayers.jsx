import React, { useState } from 'react';
import styles from './TopPlayers.module.css';

const TopPlayers = () => {
  const [selectedSport, setSelectedSport] = useState('Всі');
  const [timeFilter, setTimeFilter] = useState('Тиждень');
  const [expandedPlayer, setExpandedPlayer] = useState(null);

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол', 'Шахи', 'Настільний теніс'];
  const timeFilters = ['Тиждень', 'Місяць', 'Сезон', 'Всі часи'];

  const players = [
    {
      id: 1,
      rank: 1,
      name: 'Артем Іванов',
      initials: 'АІ',
      sport: 'Шахи',
      rating: 2184,
      ratingChange: '+24',
      wins: 12,
      losses: 2,
      draws: 3,
      winRate: '85%',
      achievements: ['Чемпіон коледжу 2024', 'Кращий гравець місяця'],
      team: 'ІТ-31',
      trend: 'up'
    },
    {
      id: 2,
      rank: 2,
      name: 'Влад Ковальчук',
      initials: 'ВК',
      sport: 'Футзал',
      rating: 1850,
      ratingChange: '+18',
      goals: 21,
      assists: 8,
      matches: 15,
      winRate: '73%',
      achievements: ['Кращий бомбардир сезону', 'MVP фіналу'],
      team: 'ФІ-22',
      trend: 'up'
    },
    {
      id: 3,
      rank: 3,
      name: 'Марія Семенова',
      initials: 'МС',
      sport: 'Настільний теніс',
      rating: 1790,
      ratingChange: '+12',
      wins: 10,
      losses: 4,
      setsWon: 32,
      setsLost: 18,
      winRate: '71%',
      achievements: ['Фіналістка турніру першості'],
      team: 'ЕК-41',
      trend: 'up'
    },
    {
      id: 4,
      rank: 4,
      name: 'Олексій Петренко',
      initials: 'ОП',
      sport: 'Баскетбол',
      rating: 1720,
      ratingChange: '-5',
      points: 185,
      rebounds: 42,
      assists: 38,
      winRate: '65%',
      achievements: ['Кращий асистент сезону'],
      team: 'МК-31',
      trend: 'down'
    },
    {
      id: 5,
      rank: 5,
      name: 'Катерина Шевченко',
      initials: 'КШ',
      sport: 'Волейбол',
      rating: 1680,
      ratingChange: '+32',
      aces: 18,
      spikes: 45,
      blocks: 22,
      winRate: '80%',
      achievements: ['Кращий подавач сезону'],
      team: 'ПР-21',
      trend: 'up'
    }
  ];

  const filteredPlayers = selectedSport === 'Всі' 
    ? players 
    : players.filter(player => player.sport === selectedSport);

  const togglePlayerDetails = (playerId) => {
    setExpandedPlayer(expandedPlayer === playerId ? null : playerId);
  };

  const getSportIcon = (sport) => {
    const icons = {
      'Футзал': 'fa-futbol',
      'Баскетбол': 'fa-basketball-ball',
      'Волейбол': 'fa-volleyball-ball',
      'Шахи': 'fa-chess',
      'Настільний теніс': 'fa-table-tennis',
      'Бадмінтон': 'fa-table-tennis',
      'Армреслінг': 'fa-hand-fist'
    };
    return icons[sport] || 'fa-trophy';
  };

  const getPlayerStats = (player) => {
    switch (player.sport) {
      case 'Футзал':
        return `⚽ ${player.goals} голів • 🎯 ${player.assists} асистів • ${player.matches} матчів`;
      case 'Баскетбол':
        return `🏀 ${player.points} очок • 📊 ${player.rebounds} підбирань • 🎯 ${player.assists} асистів`;
      case 'Волейбол':
        return `🏐 ${player.aces} ейсів • 💥 ${player.spikes} атак • 🛡️ ${player.blocks} блоків`;
      case 'Шахи':
        return `♟️ ${player.wins} перемог • ⚖️ ${player.draws} нічиїх • ❌ ${player.losses} поразок`;
      case 'Настільний теніс':
        return `🏓 ${player.wins} перемог • ${player.setsWon}:${player.setsLost} сети • ${player.winRate} перемог`;
      default:
        return `${player.wins} перемог • ${player.winRate} перемог`;
    }
  };

  const handlePlayerClick = (playerId) => {
    console.log(`Перейти до профілю гравця ${playerId}`);
    // Тут можна додати перехід на сторінку профілю гравця
  };

  const handleCompareClick = () => {
    console.log('Порівняти гравців');
    // Тут можна додати логіку для порівняння гравців
  };

  return (
    <section className={styles.topPlayers}>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <div className={styles.headerTop}>
            <h2 className={styles.cardTitle}>
              <i className={`fas fa-crown ${styles.cardIcon}`}></i>
              ТОП гравців
            </h2>
            <div className={`${styles.badge} ${styles.badgePrimary}`}>
              <i className="fas fa-trophy"></i>
              Рейтингова система
            </div>
          </div>

          <div className={styles.filtersContainer}>
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

            <div className={styles.timeFilters}>
              {timeFilters.map((filter) => (
                <button
                  key={filter}
                  className={`${styles.timeFilter} ${timeFilter === filter ? styles.active : ''}`}
                  onClick={() => setTimeFilter(filter)}
                >
                  {filter}
                </button>
              ))}
            </div>
          </div>
        </div>

        <div className={styles.leaderboardHeader}>
          <div className={styles.headerRow}>
            <div className={styles.rankColumn}>#</div>
            <div className={styles.playerColumn}>Гравець</div>
            <div className={styles.sportColumn}>Спорт</div>
            <div className={styles.ratingColumn}>Рейтинг</div>
            <div className={styles.statsColumn}>Статистика</div>
            <div className={styles.winRateColumn}>% перемог</div>
            <div className={styles.actionsColumn}></div>
          </div>
        </div>

        <div className={styles.playersList}>
          {filteredPlayers.map((player) => (
            <div 
              key={player.id} 
              className={`${styles.playerCard} ${expandedPlayer === player.id ? styles.expanded : ''}`}
              onClick={() => handlePlayerClick(player.id)}
            >
              <div className={styles.playerRow}>
                <div className={styles.rankColumn}>
                  <div className={styles.rankBadge}>
                    {player.rank}
                    {player.trend === 'up' && <i className={`fas fa-arrow-up ${styles.trendUp}`}></i>}
                    {player.trend === 'down' && <i className={`fas fa-arrow-down ${styles.trendDown}`}></i>}
                  </div>
                </div>

                <div className={styles.playerColumn}>
                  <div className={styles.playerInfo}>
                    <div 
                      className={styles.playerAvatar}
                      style={{ 
                        background: `linear-gradient(135deg, ${player.trend === 'up' ? 'var(--success)' : 'var(--accent)'}, var(--primary))`
                      }}
                    >
                      {player.initials}
                    </div>
                    <div className={styles.playerDetails}>
                      <div className={styles.playerName}>{player.name}</div>
                      <div className={styles.playerTeam}>
                        <i className="fas fa-users"></i>
                        {player.team}
                      </div>
                    </div>
                  </div>
                </div>

                <div className={styles.sportColumn}>
                  <div className={styles.sportBadge}>
                    <i className={`fas ${getSportIcon(player.sport)} ${styles.sportIcon}`}></i>
                    {player.sport}
                  </div>
                </div>

                <div className={styles.ratingColumn}>
                  <div className={styles.ratingValue}>
                    {player.rating}
                    <span className={`${styles.ratingChange} ${player.ratingChange.startsWith('+') ? styles.positive : styles.negative}`}>
                      {player.ratingChange}
                    </span>
                  </div>
                </div>

                <div className={styles.statsColumn}>
                  <div className={styles.statsText}>
                    {getPlayerStats(player)}
                  </div>
                </div>

                <div className={styles.winRateColumn}>
                  <div className={styles.winRateContainer}>
                    <div className={styles.winRateValue}>{player.winRate}</div>
                    <div className={styles.winRateBar}>
                      <div 
                        className={styles.winRateFill}
                        style={{ width: player.winRate }}
                      ></div>
                    </div>
                  </div>
                </div>

                <div className={styles.actionsColumn}>
                  <button 
                    className={styles.detailsButton}
                    onClick={(e) => {
                      e.stopPropagation();
                      togglePlayerDetails(player.id);
                    }}
                  >
                    <i className={`fas ${expandedPlayer === player.id ? 'fa-chevron-up' : 'fa-chevron-down'}`}></i>
                  </button>
                </div>
              </div>

              {expandedPlayer === player.id && (
                <div className={styles.expandedDetails}>
                  <div className={styles.playerExpandedContent}>
                    <div className={styles.achievementsSection}>
                      <h4><i className="fas fa-trophy"></i> Досягнення</h4>
                      <div className={styles.achievementsList}>
                        {player.achievements.map((achievement, index) => (
                          <div key={index} className={styles.achievement}>
                            <i className="fas fa-award"></i>
                            {achievement}
                          </div>
                        ))}
                      </div>
                    </div>

                    <div className={styles.statsGrid}>
                      <div className={styles.statCard}>
                        <div className={styles.statValue}>{player.trend === 'up' ? '↑' : '↓'}</div>
                        <div className={styles.statLabel}>Тренд</div>
                      </div>
                      <div className={styles.statCard}>
                        <div className={styles.statValue}>{player.ratingChange}</div>
                        <div className={styles.statLabel}>Зміна рейтингу</div>
                      </div>
                      <div className={styles.statCard}>
                        <div className={styles.statValue}>{player.winRate}</div>
                        <div className={styles.statLabel}>Відсоток перемог</div>
                      </div>
                    </div>

                    <div className={styles.playerActions}>
                      <button className={`${styles.actionButton} ${styles.profileButton}`}>
                        <i className="fas fa-user"></i>
                        Профіль гравця
                      </button>
                      <button className={`${styles.actionButton} ${styles.statsButton}`}>
                        <i className="fas fa-chart-line"></i>
                        Детальна статистика
                      </button>
                      <button className={`${styles.actionButton} ${styles.compareButton}`}>
                        <i className="fas fa-balance-scale"></i>
                        Порівняти
                      </button>
                    </div>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>

        <div className={styles.viewAllContainer}>
          <button className={styles.viewAllButton} onClick={handleCompareClick}>
            <i className="fas fa-balance-scale"></i>
            Порівняти гравців
          </button>
          <button className={styles.fullRatingButton}>
            <i className="fas fa-list-ol"></i>
            Повний рейтинг
          </button>
        </div>
      </div>
    </section>
  );
};

export default TopPlayers;