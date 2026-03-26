import { useState } from 'react';
import styles from './TopPlayersCompact.module.css';

interface Player {
  id: number;
  rank: number;
  name: string;
  initials: string;
  sport: string;
  rating: number;
  ratingChange: string;
  team: string;
  trend: 'up' | 'down';
}

const TopPlayersCompact = () => {
  const [selectedSport, setSelectedSport] = useState('Всі');

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол'];

  const players: Player[] = [
    {
      id: 1,
      rank: 1,
      name: 'Артем Іванов',
      initials: 'АІ',
      sport: 'Шахи',
      rating: 2184,
      ratingChange: '+24',
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
      team: 'ПР-21',
      trend: 'up'
    }
  ];

  const filteredPlayers = selectedSport === 'Всі' 
    ? players.slice(0, 3) // Показуємо тільки топ-3 для компактного вигляду
    : players.filter(player => player.sport === selectedSport).slice(0, 3);

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Футзал': 'fas fa-futbol',
      'Баскетбол': 'fas fa-basketball-ball',
      'Волейбол': 'fas fa-volleyball-ball',
      'Шахи': 'fas fa-chess',
      'Настільний теніс': 'fas fa-table-tennis'
    };
    return icons[sport] || 'fas fa-trophy';
  };

  const getTrendIcon = (trend: 'up' | 'down') => {
    if (trend === 'up') return 'fas fa-caret-up';
    if (trend === 'down') return 'fas fa-caret-down';
    return 'fas fa-minus';
  };

  return (
    <section className={styles.topPlayersCompact}>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <h2 className={styles.cardTitle}>
            <i className={`fas fa-crown ${styles.cardIcon}`}></i>
            ТОП гравців
          </h2>
          
          <div className={styles.sportSelector}>
            {sports.map((sport) => (
              <button
                key={sport}
                className={`${styles.sportButton} ${selectedSport === sport ? styles.active : ''}`}
                onClick={() => setSelectedSport(sport)}
                title={sport}
              >
                {sport === 'Всі' ? <i className="fas fa-users"></i> : <i className={getSportIcon(sport)}></i>}
              </button>
            ))}
          </div>
        </div>

        <div className={styles.playersList}>
          {filteredPlayers.map((player) => (
            <div key={player.id} className={styles.playerItem}>
              <div className={styles.playerRank}>
                <div className={`${styles.rankNumber} ${player.rank <= 3 ? styles.topRank : ''}`}>
                  {player.rank}
                </div>
              </div>
              <div className={styles.playerAvatar}>{player.initials}</div>
              <div className={styles.playerInfo}>
                <div className={styles.playerName}>{player.name}</div>
                <div className={styles.playerTeam}>{player.team}</div>
              </div>
              <div className={styles.playerRating}>
                <div className={styles.ratingValue}>
                  {player.rating}
                  <span className={styles.ratingIcon}>
                    <i className={getSportIcon(player.sport)}></i>
                  </span>
                </div>
                <div className={`${styles.ratingChange} ${player.trend === 'up' ? styles.positive : styles.negative}`}>
                  <i className={getTrendIcon(player.trend)}></i> {player.ratingChange}
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className={styles.seeMoreContainer}>
          <button className={styles.seeMoreButton}>
            Переглянути весь рейтинг
            <i className="fas fa-arrow-right"></i>
          </button>
        </div>
      </div>
    </section>
  );
};

export default TopPlayersCompact;
