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

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол', 'Шахи', 'Настільний теніс'];
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

  const togglePlayerDetails = (playerId: number) => {
    setExpandedPlayer(expandedPlayer === playerId ? null : playerId);
  };

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Футзал': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Шахи': '♟️',
      'Настільний теніс': '🏓'
    };
    return icons[sport] || '🏆';
  };

  return (
    <section className={styles.topPlayers}>
      <div className={styles.header}>
        <div className={styles.titleSection}>
          <h2 className={styles.title}>
            <i className="fas fa-crown"></i>
            Рейтинг гравців
          </h2>
          <div className={styles.filters}>
            <div className={styles.sportFilter}>
              {sports.map(sport => (
                <button
                  key={sport}
                  className={`${styles.filterBtn} ${selectedSport === sport ? styles.active : ''}`}
                  onClick={() => setSelectedSport(sport)}
                >
                  <span className={styles.sportIconShort}>{getSportIcon(sport)}</span>
                  <span className={styles.sportNameFull}>{sport}</span>
                </button>
              ))}
            </div>
            
            <div className={styles.timeFilter}>
              {timeFilters.map(filter => (
                <button
                  key={filter}
                  className={`${styles.timeBtn} ${timeFilter === filter ? styles.active : ''}`}
                  onClick={() => setTimeFilter(filter)}
                >
                  {filter}
                </button>
              ))}
            </div>
          </div>
        </div>
      </div>

      <div className={styles.playersList}>
        {filteredPlayers.map((player) => (
          <div 
            key={player.id} 
            className={`${styles.playerCard} ${expandedPlayer === player.id ? styles.expanded : ''} ${player.rank <= 3 ? styles[`top${player.rank}`] : ''}`}
            onClick={() => togglePlayerDetails(player.id)}
          >
            <div className={styles.playerMain}>
              <div className={styles.rank}>{player.rank}</div>
              <div className={styles.playerInfo}>
                <div className={styles.avatar}>{player.initials}</div>
                <div className={styles.nameSection}>
                  <div className={styles.name}>{player.name}</div>
                  <div className={styles.team}>{player.team} • {player.sport}</div>
                </div>
              </div>
              <div className={styles.statsSummary}>
                <div className={styles.rating}>
                  {player.rating}
                  <span className={player.trend === 'up' ? styles.up : styles.down}>
                    {player.ratingChange}
                  </span>
                </div>
                <div className={styles.winRate}>
                  <div className={styles.winRateLabel}>Win Rate</div>
                  <div className={styles.winRateValue}>{player.winRate}</div>
                </div>
              </div>
              <div className={styles.expandIcon}>
                <i className={`fas fa-chevron-${expandedPlayer === player.id ? 'up' : 'down'}`}></i>
              </div>
            </div>

            {expandedPlayer === player.id && (
              <div className={styles.playerDetails}>
                <div className={styles.detailsGrid}>
                  <div className={styles.detailBlock}>
                    <div className={styles.blockTitle}>Досягнення</div>
                    <div className={styles.achievements}>
                      {player.achievements.map((achievement, i) => (
                        <div key={i} className={styles.achievementItem}>
                          <i className="fas fa-medal"></i>
                          {achievement}
                        </div>
                      ))}
                    </div>
                  </div>
                  <div className={styles.detailBlock}>
                    <div className={styles.blockTitle}>Статистика {timeFilter}</div>
                    <div className={styles.statsGrid}>
                      {player.sport === 'Футзал' && (
                        <>
                          <div className={styles.statItem}>
                            <span className={styles.statLabel}>Голи</span>
                            <span className={styles.statValue}>{player.goals}</span>
                          </div>
                          <div className={styles.statItem}>
                            <span className={styles.statLabel}>Паси</span>
                            <span className={styles.statValue}>{player.assists}</span>
                          </div>
                        </>
                      )}
                      {player.sport === 'Шахи' && (
                        <>
                          <div className={styles.statItem}>
                            <span className={styles.statLabel}>Перемоги</span>
                            <span className={styles.statValue}>{player.wins}</span>
                          </div>
                          <div className={styles.statItem}>
                            <span className={styles.statLabel}>Нічиї</span>
                            <span className={styles.statValue}>{player.draws}</span>
                          </div>
                        </>
                      )}
                      {/* Додайте інші види спорту за аналогією */}
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </section>
  );
};

export default TopPlayers;
