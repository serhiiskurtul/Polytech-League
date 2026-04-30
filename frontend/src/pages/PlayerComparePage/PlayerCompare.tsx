import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import styles from './PlayerComparePage.module.css';
import { getPlayerById, getPlayersRankings } from '../../shared/api/players.api';
import { getSeasons } from '../../shared/api/seasonRating.api';
import { PlayerDto } from '../../shared/types/player.types';
import { Season } from '../../shared/types/seasonRating.types';
import { getPhotoUrl } from '../../shared/utils/photo.utils';

export const PlayerComparePage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const p1Id = searchParams.get('p1');
  const p2Id = searchParams.get('p2');

  const [player1, setPlayer1] = useState<PlayerDto | null>(null);
  const [player2, setPlayer2] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  const [seasons, setSeasons] = useState<Season[]>([]);
  const [selectedSeasonId1, setSelectedSeasonId1] = useState<number | null>(null);
  const [selectedSeasonId2, setSelectedSeasonId2] = useState<number | null>(null);
  const [loadingSeasons, setLoadingSeasons] = useState(true);

  // Initial fetch of seasons
  useEffect(() => {
    const fetchSeasons = async () => {
      try {
        const data = await getSeasons();
        setSeasons(data);
        const current = data.find(s => s.isCurrent);
        if (current) {
          setSelectedSeasonId1(current.id);
          setSelectedSeasonId2(current.id);
        } else if (data.length > 0) {
          setSelectedSeasonId1(data[0].id);
          setSelectedSeasonId2(data[0].id);
        }
      } catch (err) {
        console.error('Failed to fetch seasons:', err);
      } finally {
        setLoadingSeasons(false);
      }
    };
    fetchSeasons();
  }, []);

  useEffect(() => {
    const fetchPlayers = async () => {
      if (!p1Id) {
        setError('Необхідно вибрати хоча б одного гравця для порівняння.');
        setLoading(false);
        return;
      }

      setLoading(true);
      try {
        const findAndEnrich = async (id: string, seasonId: number | null) => {
          const rankings = await getPlayersRankings(seasonId || undefined);
          const detail = await getPlayerById(id);
          const rankingInfo = rankings.find(p => p.id === Number(id));
          
          if (rankingInfo) {
            return {
              ...detail,
              performanceScore: rankingInfo.performanceScore,
              matchesPlayed: rankingInfo.matchesPlayed,
              winsCount: rankingInfo.winsCount,
              goalsScored: rankingInfo.goalsScored,
              competitionsCount: rankingInfo.competitionsCount,
              globalRank: rankingInfo.globalRank
            };
          }
          return {
            ...detail,
            performanceScore: 0,
            matchesPlayed: 0,
            winsCount: 0,
            goalsScored: 0,
            competitionsCount: 0,
            globalRank: undefined
          };
        };

        const data1 = await findAndEnrich(p1Id, selectedSeasonId1);
        setPlayer1(data1);
        
        if (p2Id) {
          const data2 = await findAndEnrich(p2Id, selectedSeasonId2);
          setPlayer2(data2);
        }
      } catch (err) {
        console.error('Error fetching players for comparison:', err);
        setError('Не вдалося завантажити дані гравців.');
      } finally {
        setLoading(false);
      }
    };

    if (!loadingSeasons) {
      fetchPlayers();
    }
  }, [p1Id, p2Id, selectedSeasonId1, selectedSeasonId2, loadingSeasons]);


  const calculateWinRate = (player: PlayerDto | null) => {
    if (!player || !player.matchesPlayed) return 0;
    return Math.round(((player.winsCount || 0) / player.matchesPlayed) * 100);
  };

  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://polytech-league-production.up.railway.app';

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.spinner}></div>
        <p>Підготовка порівняння...</p>
      </div>
    );
  }

  if (error || !player1) {
    return (
      <div className={styles.errorContainer}>
        <i className="fa-solid fa-triangle-exclamation"></i>
        <h2>Помилка</h2>
        <p>{error}</p>
        <button onClick={() => navigate('/ratings')} className={styles.backBtn}>
          До рейтингів
        </button>
      </div>
    );
  }


  return (
    <div className={styles.page}>
      <div className={styles.container}>
        <header className={styles.header}>
          <button onClick={() => navigate(-1)} className={styles.backBtnSmall}>
            <i className="fa-solid fa-arrow-left"></i> Назад
          </button>
          <h1 className={styles.title}>Порівняння гравців</h1>
          <div style={{ width: '100px' }}></div> {/* Spacer for symmetry */}
        </header>

        <div className={styles.comparisonGrid}>
          {/* Player Cards */}
          <div className={styles.playersRow}>
            <div className={styles.playerColumn}>
              <SeasonSelector 
                seasons={seasons} 
                selectedId={selectedSeasonId1} 
                onChange={setSelectedSeasonId1} 
              />
              <PlayerCard 
                player={player1} 
                color="#3b82f6" 
                selectedSeason={seasons.find(s => s.id === selectedSeasonId1)} 
              />
            </div>

            <div className={styles.vsCircle}>VS</div>

            <div className={styles.playerColumn}>
              {player2 ? (
                <>
                  <SeasonSelector 
                    seasons={seasons} 
                    selectedId={selectedSeasonId2} 
                    onChange={setSelectedSeasonId2} 
                  />
                  <PlayerCard 
                    player={player2} 
                    color="#ef4444" 
                    selectedSeason={seasons.find(s => s.id === selectedSeasonId2)} 
                  />
                </>
              ) : (
                <div className={styles.emptyPlayerSlot}>
                  <p>Виберіть другого гравця в рейтингах</p>
                  <button onClick={() => navigate(`/ratings?p1=${p1Id}`)} className={styles.addPlayerBtn}>
                    Додати гравця
                  </button>
                </div>
              )}
            </div>
          </div>


          {/* Metrics Table */}
          <div className={styles.statsSection}>
            <div className={styles.sectionHeader}>
              <i className="fa-solid fa-table-list"></i>
              <h2>Порівняльні метрики</h2>
            </div>
            <div className={styles.statsTable}>
              <StatRow 
                label="Загальний рейтинг" 
                val1={player1.performanceScore} 
                val2={player2?.performanceScore}
                suffix=" очок"
              />
              <StatRow 
                label="Зіграно матчів" 
                val1={player1.matchesPlayed} 
                val2={player2?.matchesPlayed}
                suffix=" ігор"
              />
              <StatRow 
                label="Кількість перемог" 
                val1={player1.winsCount} 
                val2={player2?.winsCount}
                suffix=" перемог"
              />
              <StatRow 
                label="Відсоток перемог" 
                val1={calculateWinRate(player1)} 
                val2={calculateWinRate(player2)}
                suffix="%"
                isBetterHigh={true}
              />
               <StatRow 
                 label="Забито голів" 
                 val1={player1.goalsScored} 
                 val2={player2?.goalsScored}
               />
               <StatRow 
                 label="Участь у змаганнях" 
                 val1={player1.competitionsCount} 
                 val2={player2?.competitionsCount}
                 suffix=" турнірів"
               />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const SeasonSelector = ({ seasons, selectedId, onChange }: { 
  seasons: Season[], 
  selectedId: number | null, 
  onChange: (id: number | null) => void 
}) => (
  <div className={styles.seasonSelector}>
    <label>Сезон</label>
    <div className={styles.selectWrapper}>
      <select 
        value={selectedId || ''} 
        onChange={(e) => onChange(Number(e.target.value) || null)}
        className={styles.seasonSelect}
      >
        <option value="">За весь час</option>
        {seasons.map(s => (
          <option key={s.id} value={s.id}>
            {s.name} {s.isCurrent ? '★' : ''}
          </option>
        ))}
      </select>
      <i className="fa-solid fa-chevron-down"></i>
    </div>
  </div>
);

const PlayerCard = ({ player, color, selectedSeason }: { 
  player: PlayerDto, 
  color: string,
  selectedSeason?: Season 
}) => {
  const photoSrc = getPhotoUrl(player.photoUrl);
  
  // Logic to determine status relative to selected season
  const getStatus = () => {
    if (!selectedSeason || !player.enrollmentYear) return null;
    
    // Extract year from season name (e.g., "2025-2026" -> 2025)
    const seasonStartYear = parseInt(selectedSeason.name.split('-')[0]);
    
    if (seasonStartYear < player.enrollmentYear) {
      return { text: 'Ще не вступив', type: 'future' };
    }
    
    // Assume 4 years of Bachelor degree
    if (seasonStartYear > player.enrollmentYear + 4) {
      return { text: 'Випускник', type: 'past' };
    }
    
    return null;
  };

  const status = getStatus();

  return (
    <div className={`${styles.playerCard} ${status ? styles.inactiveCard : ''}`} style={{ '--accent-color': color } as React.CSSProperties}>
      <div className={styles.avatarWrapper}>
        {photoSrc ? (
          <img src={photoSrc} alt="" className={styles.avatarImg} />
        ) : (
          <div className={styles.avatarPlaceholder}>
            {player.firstName[0]}{player.lastName[0]}
          </div>
        )}
        {status && <div className={styles.statusBadge}>{status.text}</div>}
      </div>
      <div className={styles.cardInfo}>
        <h3 className={styles.cardName}>{player.firstName} {player.lastName}</h3>
        <p className={styles.cardGroup}>{player.group}</p>
        {player.faculty && (
          <div className={styles.cardRank}>
            <i className="fa-solid fa-award"></i> {player.faculty}
          </div>
        )}
      </div>
    </div>
  );
};

const StatRow = ({ label, val1, val2, suffix = '', isBetterHigh = true }: { 
  label: string, 
  val1: number | undefined, 
  val2: number | undefined,
  suffix?: string,
  isBetterHigh?: boolean
}) => {
  const v1 = val1 || 0;
  const v2 = val2 || 0;
  
  const is1Better = isBetterHigh ? v1 > v2 : v1 < v2;
  const is2Better = isBetterHigh ? v2 > v1 : v2 < v1;

  return (
    <div className={styles.statRow}>
      <div className={`${styles.statValue} ${is1Better ? styles.betterValue : ''}`}>
        {v1}{suffix}
      </div>
      <div className={styles.statLabel}>{label}</div>
      <div className={`${styles.statValue} ${is2Better ? styles.betterValue : ''}`}>
        {v2 !== undefined ? `${v2}${suffix}` : '—'}
      </div>
    </div>
  );
};
