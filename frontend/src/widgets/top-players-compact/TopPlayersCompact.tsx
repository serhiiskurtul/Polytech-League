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
    { label: 'Футбол', slug: 'football' },
    { label: 'Баскетбол', slug: 'basketball_3x3' },
    { label: 'Волейбол', slug: 'volleyball' },
    { label: 'Шахи', slug: 'chess' },
    { label: 'Шашки', slug: 'checkers' },
    { label: 'Настільний теніс', slug: 'table_tennis' },
    { label: 'Пляжний волейбол', slug: 'beach_volleyball' },
    { label: 'Бадмінтон', slug: 'badminton' },
    { label: 'Армрестлінг', slug: 'armwrestling' }
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
      filtered = players
        .filter(p => p.sportStats?.some(s => s.sportType.toLowerCase() === slug.toLowerCase()))
        .sort((a, b) => {
          const aSport = a.sportStats?.find(s => s.sportType.toLowerCase() === slug.toLowerCase());
          const bSport = b.sportStats?.find(s => s.sportType.toLowerCase() === slug.toLowerCase());
          return (bSport?.performanceScore || 0) - (aSport?.performanceScore || 0);
        });
    }
    return filtered.slice(0, 5);
  };

  const getSportIcon = (sportSlug: string) => {
    const icons: Record<string, string> = {
      'football': 'fas fa-futbol',
      'basketball_3x3': 'fas fa-basketball-ball',
      'volleyball': 'fas fa-volleyball-ball',
      'chess': 'fas fa-chess',
      'table_tennis': 'fas fa-table-tennis',
      'badminton': 'fas fa-feather-alt',
      'armwrestling': 'fas fa-fist-raised',
      'checkers': 'fas fa-chess-board',
      'beach_volleyball': 'fas fa-umbrella-beach'
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
            ТОП гравців сезону
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
                  {getPhotoUrl(player.photoUrl) ? (
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
                    {selectedSport === 'Всі' 
                      ? player.performanceScore 
                      : player.sportStats?.find(s => s.sportType.toLowerCase() === sports.find(sp => sp.label === selectedSport)?.slug.toLowerCase())?.performanceScore || 0}
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
