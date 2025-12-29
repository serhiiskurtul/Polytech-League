import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import styles from './TopPlayersCompact.module.css';

const TopPlayersCompact = () => {
  const [selectedSport, setSelectedSport] = useState('Всі');

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол'];

  const players = [
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

  const getSportIcon = (sport) => {
    const icons = {
      'Футзал': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Шахи': '♟️',
      'Настільний теніс': '🏓'
    };
    return icons[sport] || '🏆';
  };

  const getTrendIcon = (trend) => {
    if (trend === 'up') return '📈';
    if (trend === 'down') return '📉';
    return '➡️';
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
                {sport === 'Всі' ? '★' : getSportIcon(sport)}
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
                <div className={styles.trendIcon}>
                  {getTrendIcon(player.trend)}
                </div>
              </div>

              <div className={styles.playerAvatar}>
                {player.initials}
              </div>

              <div className={styles.playerInfo}>
                <div className={styles.playerName}>
                  {player.name}
                  <span className={styles.playerSport}>
                    {getSportIcon(player.sport)}
                  </span>
                </div>
                <div className={styles.playerTeam}>
                  {player.team}
                </div>
              </div>

              <div className={styles.playerRating}>
                <div className={styles.ratingValue}>
                  {player.rating}
                </div>
                <div className={`${styles.ratingChange} ${player.ratingChange.startsWith('+') ? styles.positive : styles.negative}`}>
                  {player.ratingChange}
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className={styles.seeMoreContainer}>
          <Link to="/ratings" className={styles.seeMoreButton}>
            <i className="fas fa-chart-line"></i>
            Детальний рейтинг
          </Link>
        </div>
      </div>
    </section>
  );
};

export default TopPlayersCompact;