import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import styles from './PlayerComparePage.module.css';
import { getPlayerById, getPlayersRankings } from '../../shared/api/players.api';
import { PlayerDto } from '../../shared/types/player.types';

export const PlayerComparePage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const p1Id = searchParams.get('p1');
  const p2Id = searchParams.get('p2');

  const [player1, setPlayer1] = useState<PlayerDto | null>(null);
  const [player2, setPlayer2] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPlayers = async () => {
      if (!p1Id) {
        setError('Необхідно вибрати хоча б одного гравця для порівняння.');
        setLoading(false);
        return;
      }

      setLoading(true);
      try {
        // Fetch rankings to get performance score (calculated leaderboard data)
        const rankings = await getPlayersRankings();
        
        const findAndEnrich = async (id: string) => {
          const detail = await getPlayerById(id);
          const rankingInfo = rankings.find(p => p.id === Number(id));
          if (rankingInfo) {
            detail.performanceScore = rankingInfo.performanceScore;
          }
          return detail;
        };

        const data1 = await findAndEnrich(p1Id);
        setPlayer1(data1);
        
        if (p2Id) {
          const data2 = await findAndEnrich(p2Id);
          setPlayer2(data2);
        }
      } catch (err) {
        console.error('Error fetching players for comparison:', err);
        setError('Не вдалося завантажити дані гравців.');
      } finally {
        setLoading(false);
      }
    };

    fetchPlayers();
  }, [p1Id, p2Id]);


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
        </header>

        <div className={styles.comparisonGrid}>
          {/* Player Cards */}
          <div className={styles.playersRow}>
            <PlayerCard player={player1} baseUrl={baseUrl} color="#3b82f6" />
            <div className={styles.vsCircle}>VS</div>
            {player2 ? (
              <PlayerCard player={player2} baseUrl={baseUrl} color="#ef4444" />
            ) : (
              <div className={styles.emptyPlayerSlot}>
                <p>Виберіть другого гравця в рейтингах</p>
                <button onClick={() => navigate(`/ratings?p1=${p1Id}`)} className={styles.addPlayerBtn}>
                  Додати гравця
                </button>
              </div>
            )}
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
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const PlayerCard = ({ player, baseUrl, color }: { player: PlayerDto, baseUrl: string, color: string }) => {
  const photoSrc = player.photoUrl
    ? (player.photoUrl.startsWith('http') ? player.photoUrl : `${baseUrl}${player.photoUrl}`)
    : null;

  return (
    <div className={styles.playerCard} style={{ '--accent-color': color } as React.CSSProperties}>
      <div className={styles.avatarWrapper}>
        {photoSrc ? (
          <img src={photoSrc} alt="" className={styles.avatarImg} />
        ) : (
          <div className={styles.avatarPlaceholder}>
            {player.firstName[0]}{player.lastName[0]}
          </div>
        )}
      </div>
      <div className={styles.cardInfo}>
        <h3 className={styles.cardName}>{player.firstName} {player.lastName}</h3>
        <p className={styles.cardGroup}>{player.group}</p>
        <div className={styles.cardRank}>
          <i className="fa-solid fa-award"></i> {player.faculty || 'Студент'}
        </div>
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
