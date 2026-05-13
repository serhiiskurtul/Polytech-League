import { useState } from 'react';
import styles from './TopPlayers.module.css';

interface Player {
  id: number;
  rank: number;
  name: string;
  initials: string;
  sport: string;
  rating: number;
  ratingChange: string;
  wins?: number;
  losses?: number;
  draws?: number;
  winRate: string;
  achievements: string[];
  team: string;
  trend: 'up' | 'down';
  goals?: number;
  assists?: number;
  matches?: number;
  setsWon?: number;
  setsLost?: number;
  points?: number;
  rebounds?: number;
  aces?: number;
  spikes?: number;
  blocks?: number;
}

const TopPlayers = () => {
  const [selectedSport, setSelectedSport] = useState('Всі');
  const [timeFilter, setTimeFilter] = useState('Тиждень');
  const [expandedPlayer, setExpandedPlayer] = useState<number | null>(null);

  const sports = ['Всі', 'Футбол', 'Баскетбол', 'Волейбол', 'Шахи', 'Настільний теніс'];
  const timeFilters = ['Тиждень', 'Місяць', 'Сезон', 'Всі часи'];

  const players: Player[] = [
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
      sport: 'Футбол',
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
      achievements: ['Фіналістка змагання першості'],
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

  const togglePlayerDetails = (playerId: number) => {
    setExpandedPlayer(expandedPlayer === playerId ? null : playerId);
  };

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Футбол': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Шахи': '♟️',
      'Настільний теніс': '🏓'
    };
    return icons[sport] || '🏆';
  };

  return (
    <section className={styles.topPlayers}>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <div className={styles.headerTop}>
            <h2 className={styles.cardTitle}>
              <i className={`fas fa-crown ${styles.cardIcon}`}></i>
              Рейтинг гравців
            </h2>
            <div className={`${styles.badge} ${styles.badgePrimary}`}>
              <i className="fas fa-users"></i>
              {players.length} гравців
            </div>
          </div>
          
          <div className={styles.filtersContainer}>
            <div className={styles.sportFilters}>
              {sports.map(sport => (
                <button
                  key={sport}
                  className={`${styles.sportFilter} ${selectedSport === sport ? styles.active : ''}`}
                  onClick={() => setSelectedSport(sport)}
                >
                  <span style={{ marginRight: '8px' }}>
                    {sport === 'Всі' ? '👥' : getSportIcon(sport)}
                  </span>
                  {sport}
                </button>
              ))}
            </div>
            
            <div className={styles.timeFilters}>
              {timeFilters.map(filter => (
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
            <div className={styles.sportColumn}>Дисципліна</div>
            <div className={styles.ratingColumn}>Рейтинг</div>
            <div className={styles.statsColumn}>Статистика</div>
            <div className={styles.winRateColumn}>Win Rate</div>
            <div className={styles.actionsColumn}></div>
          </div>
        </div>

        <div className={styles.playersList}>
          {filteredPlayers.map((player) => (
            <div 
              key={player.id} 
              className={`${styles.playerCard} ${expandedPlayer === player.id ? styles.expanded : ''}`}
              onClick={() => togglePlayerDetails(player.id)}
            >
              <div className={styles.playerRow}>
                <div className={styles.rankColumn}>
                  <div className={styles.rankBadge}>
                    {player.rank}
                    <div className={player.trend === 'up' ? styles.trendUp : styles.trendDown}>
                      <i className={`fas fa-caret-${player.trend === 'up' ? 'up' : 'down'}`}></i>
                    </div>
                  </div>
                </div>

                <div className={styles.playerColumn}>
                  <div className={styles.playerInfo}>
                    <div className={styles.playerAvatar} style={{ background: player.rank <= 3 ? 'linear-gradient(135deg, var(--warning), #f59e0b)' : 'linear-gradient(135deg, var(--primary), var(--primary-dark))' }}>
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
                    <span className={styles.sportIcon}>{getSportIcon(player.sport)}</span>
                    {player.sport}
                  </div>
                </div>

                <div className={styles.ratingColumn}>
                  <div className={styles.ratingValue}>
                    {player.rating}
                    <div className={`${styles.ratingChange} ${player.trend === 'up' ? styles.positive : styles.negative}`}>
                      {player.ratingChange}
                    </div>
                  </div>
                </div>

                <div className={styles.statsColumn}>
                  <div className={styles.statsText}>
                    {player.sport === 'Футбол' && `${player.goals} голів • ${player.assists} пасів`}
                    {player.sport === 'Шахи' && `${player.wins}W • ${player.losses}L • ${player.draws}D`}
                    {player.sport === 'Настільний теніс' && `${player.setsWon}S+ • ${player.setsLost}S-`}
                    {player.sport === 'Баскетбол' && `${player.points} pts • ${player.rebounds} reb`}
                    {player.sport === 'Волейбол' && `${player.aces} aces • ${player.blocks} blk`}
                  </div>
                </div>

                <div className={styles.winRateColumn}>
                  <div className={styles.winRateContainer}>
                    <span className={styles.winRateValue}>{player.winRate}</span>
                  </div>
                </div>

                <div className={styles.actionsColumn}>
                  <button className={styles.detailsButton}>
                    <i className={`fas fa-chevron-${expandedPlayer === player.id ? 'up' : 'down'}`}></i>
                  </button>
                </div>
              </div>

              {expandedPlayer === player.id && (
                <div className={styles.expandedDetails}>
                  <div className={styles.playerExpandedContent}>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '40px' }}>
                      <div className={styles.achievementsSection}>
                        <h4><i className="fas fa-medal"></i> Досягнення</h4>
                        <div className={styles.achievementsList}>
                          {player.achievements.map((achievement, i) => (
                            <div key={i} className={styles.achievement}>
                              <i className="fas fa-award"></i>
                              {achievement}
                            </div>
                          ))}
                        </div>
                      </div>
                      
                      <div className={styles.statsSection}>
                        <h4 style={{ display: 'flex', alignItems: 'center', gap: '10px', margin: '0 0 12px 0', fontSize: '16px', color: 'var(--text)' }}>
                          <i className="fas fa-chart-line" style={{ color: 'var(--primary)' }}></i>
                          Детальна статистика
                        </h4>
                        <div className={styles.statsGrid}>
                          {player.sport === 'Футбол' && (
                            <>
                              <div className={styles.statCard}>
                                <div className={styles.statValue}>{player.goals}</div>
                                <div className={styles.statLabel}>Голи</div>
                              </div>
                              <div className={styles.statCard}>
                                <div className={styles.statValue}>{player.assists}</div>
                                <div className={styles.statLabel}>Паси</div>
                              </div>
                              <div className={styles.statCard}>
                                <div className={styles.statValue}>{player.matches}</div>
                                <div className={styles.statLabel}>Матчі</div>
                              </div>
                            </>
                          )}
                          {player.sport === 'Шахи' && (
                            <>
                              <div className={styles.statCard}>
                                <div className={styles.statValue}>{player.wins}</div>
                                <div className={styles.statLabel}>Перемоги</div>
                              </div>
                              <div className={styles.statCard}>
                                <div className={styles.statValue}>{player.losses}</div>
                                <div className={styles.statLabel}>Поразки</div>
                              </div>
                              <div className={styles.statCard}>
                                <div className={styles.statValue}>{player.draws}</div>
                                <div className={styles.statLabel}>Нічиї</div>
                              </div>
                            </>
                          )}
                        </div>
                      </div>
                    </div>

                    <div className={styles.playerActions}>
                      <button className={`${styles.actionButton} ${styles.profileButton}`}>
                        <i className="fas fa-user"></i>
                        Профіль гравця
                      </button>
                      <button className={`${styles.actionButton} ${styles.compareButton}`} style={{ background: 'var(--primary-light)', color: 'var(--primary)' }}>
                        <i className="fas fa-exchange-alt"></i>
                        Порівняти
                      </button>
                    </div>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default TopPlayers;
