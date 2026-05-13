import { useState, useEffect } from 'react';
import styles from './TopPlayersCompact.module.css';
import { getPlayersRankings } from '@/shared/api/players.api';
import { getCurrentSeason } from '@/shared/api/seasonRating.api';
import { PlayerDto } from '@/shared/types/player.types';
import { useNavigate } from 'react-router-dom';
import { BASE_URL } from '@/shared/api/axiosInstance';
import { getPhotoUrl } from '@/shared/utils/photo.utils';

const TopPlayersCompact = () => {
  const navigate = useNavigate();
  const [selectedSport, setSelectedSport] = useState('Всі');
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [loading, setLoading] = useState(true);

  const sports = [
    { label: 'Всі', slug: 'all' },
    { label: 'Футзал', slug: 'football' },
    { label: 'Баскетбол', slug: 'basketball-3x3' },
    { label: 'Волейбол', slug: 'volleyball' },
    { label: 'Шахи', slug: 'chess' },
    { label: 'Шашки', slug: 'checkers' },
    { label: 'Настільний теніс', slug: 'table-tennis' },
    { label: 'Пляжний волейбол', slug: 'beach-volleyball' },
    { label: 'Бадмінтон', slug: 'badminton' },
    { label: 'Армреслінг', slug: 'armwrestling' }
  ];

  useEffect(() => {
    const fetchRankings = async () => {
      try {
        setLoading(true);
        // First try to get current season to show seasonal rankings
        let seasonId = undefined;
        try {
          const currentSeason = await getCurrentSeason();
          if (currentSeason) seasonId = currentSeason.id;
        } catch (e) {
          console.error('Could not fetch current season, falling back to all-time', e);
        }
        
        const data = await getPlayersRankings(seasonId);
        setPlayers(data);
      } catch (error) {
        console.error('Помилка при завантаженні рейтингів:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchRankings();
  }, []);

  const getFilteredPlayers = () => {
    const sportObj = sports.find(s => s.label === selectedSport);
    const slug = sportObj?.slug || 'all';

    let filtered = players;
    if (slug !== 'all') {
      filtered = players.filter(p => p.sports?.includes(slug));
    }
    return filtered.slice(0, 5);
  };

  const getSportIcon = (sportSlug: string) => {
    const icons: Record<string, string> = {
      'football': 'fas fa-futbol',
      'football': 'fas fa-futbol',
      'basketball-3x3': 'fas fa-basketball-ball',
      'volleyball': 'fas fa-volleyball-ball',
      'chess': 'fas fa-chess',
      'table-tennis': 'fas fa-table-tennis',
      'badminton': 'fas fa-feather-alt',
      'armwrestling': 'fas fa-fist-raised',
      'checkers': 'fas fa-chess-board',
      'beach-volleyball': 'fas fa-umbrella-beach'
    };
    return icons[sportSlug] || 'fas fa-trophy';
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };


  const filteredPlayers = getFilteredPlayers();

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
                key={sport.label}
                className={`${styles.sportButton} ${selectedSport === sport.label ? styles.active : ''}`}
                onClick={() => setSelectedSport(sport.label)}
                title={sport.label}
              >
                {sport.label === 'Всі' ? <i className="fas fa-users"></i> : <i className={getSportIcon(sport.slug)}></i>}
              </button>
            ))}
          </div>
        </div>

        <div className={styles.playersList}>
          {loading ? (
             [...Array(5)].map((_, i) => (
              <div key={i} className={`${styles.playerItem} ${styles.skeleton}`}>
                <div className={styles.skeletonRank}></div>
                <div className={styles.skeletonAvatar}></div>
                <div className={styles.skeletonInfo}></div>
                <div className={styles.skeletonRating}></div>
              </div>
            ))
          ) : filteredPlayers.length > 0 ? (
            filteredPlayers.map((player, index) => (
              <div 
                key={player.id} 
                className={styles.playerItem}
                onClick={() => navigate(`/players/${player.id}`)}
                style={{ cursor: 'pointer' }}
              >
                <div className={styles.playerRank}>
                  <div className={`${styles.rankNumber} ${index < 3 ? styles.topRank : ''}`}>
                    {index + 1}
                  </div>
                </div>
                <div className={styles.playerAvatar}>
                  {player.photoUrl ? (
                    <img src={getPhotoUrl(player.photoUrl)!} alt={player.firstName} className={styles.avatarImage} />
                  ) : (
                    getInitials(player.firstName, player.lastName)
                  )}
                </div>
                <div className={styles.playerInfo}>
                  <div className={styles.playerName}>{player.firstName} {player.lastName}</div>
                  <div className={styles.playerTeam}>{player.group}</div>
                </div>
                <div className={styles.playerRating}>
                  <div className={styles.ratingValue}>
                    {player.performanceScore}
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className={styles.noPlayers}>Поки немає рейтингу для цієї дисципліни</div>
          )}
        </div>

        <div className={styles.seeMoreContainer}>
          <button className={styles.seeMoreButton} onClick={() => navigate('/ratings')}>
            Переглянути весь рейтинг
            <i className="fas fa-arrow-right"></i>
          </button>
        </div>
      </div>
    </section>
  );
};

export default TopPlayersCompact;
