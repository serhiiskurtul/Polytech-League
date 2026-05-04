import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import styles from './PlayerComparePage.module.css';
import { getPlayerById, getPlayersRankings, getHeadToHead } from '../../shared/api/players.api';
import { getSeasons } from '../../shared/api/seasonRating.api';
import { PlayerDto } from '../../shared/types/player.types';
import { Season } from '../../shared/types/seasonRating.types';
import { getPhotoUrl } from '../../shared/utils/photo.utils';
import { 
  PieChart, 
  Pie, 
  Cell, 
  ResponsiveContainer, 
  Tooltip, 
  Legend 
} from 'recharts';

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
  
  const [headToHead, setHeadToHead] = useState<any>(null);
  const [loadingH2H, setLoadingH2H] = useState(false);

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

  // Fetch Head-to-Head
  useEffect(() => {
    const fetchH2H = async () => {
      if (!p1Id || !p2Id) return;
      setLoadingH2H(true);
      try {
        const data = await getHeadToHead(p1Id, p2Id);
        setHeadToHead(data);
      } catch (err) {
        console.error('Failed to fetch head-to-head:', err);
      } finally {
        setLoadingH2H(false);
      }
    };

    fetchH2H();
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
          <div style={{ width: '100px' }}></div> {/* Spacer for symmetry */}
        </header>

        <div className={styles.comparisonGrid}>
          {/* Player Cards */}
          <div className={styles.playersRow}>
            <div className={styles.playerColumn}>
              <SeasonSelector 
                seasons={seasons} 
                player={player1}
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
                    player={player2}
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
               {/* Only show goals if both played Futsal */}
               {player1.sportStats?.some(s => s.sportType === 'futsal') && 
                player2?.sportStats?.some(s => s.sportType === 'futsal') && (
                 <StatRow 
                   label="Забито голів" 
                   val1={player1.goalsScored} 
                   val2={player2?.goalsScored}
                 />
               )}
               <StatRow 
                 label="Участь у змаганнях" 
                 val1={player1.competitionsCount} 
                 val2={player2?.competitionsCount}
                 suffix=" турнірів"
               />
            </div>
          </div>

          {/* New Sections: Distribution & Head-to-Head */}
          <div className={styles.extraStatsRow}>
            {/* Points Distribution */}
            <div className={styles.statsSection}>
              <div className={styles.sectionHeader}>
                <i className="fa-solid fa-chart-pie"></i>
                <h2>Розподіл очок</h2>
              </div>
              <div className={styles.distributionContainer}>
                <div className={styles.distributionChart}>
                  <p className={styles.chartLabel}>{player1.firstName}</p>
                  <PointsDistributionChart player={player1} />
                </div>
                {player2 && (
                  <div className={styles.distributionChart}>
                    <p className={styles.chartLabel}>{player2.firstName}</p>
                    <PointsDistributionChart player={player2} />
                  </div>
                )}
              </div>
            </div>

            {/* Head to Head */}
            <div className={styles.statsSection}>
              <div className={styles.sectionHeader}>
                <i className="fa-solid fa-handshake"></i>
                <h2>Особисті зустрічі</h2>
              </div>
              <HeadToHeadSection 
                data={headToHead} 
                loading={loadingH2H} 
                p1Name={player1.firstName} 
                p2Name={player2?.firstName || 'Гравець 2'} 
              />
            </div>
          </div>

          {/* Chess Comparison (if applicable and BOTH played) */}
          {player1.sportStats?.some(s => s.sportType === 'chess') && 
           player2?.sportStats?.some(s => s.sportType === 'chess') && (
            <div className={styles.statsSection}>
              <div className={styles.sectionHeader}>
                <i className="fa-solid fa-chess"></i>
                <h2>Шахова аналітика</h2>
              </div>
              <div className={styles.statsTable}>
                <StatRow 
                  label="ELO Рейтинг" 
                  val1={player1.chessRating || 0} 
                  val2={player2?.chessRating || 0}
                  isBetterHigh={true}
                />
                <StatRow 
                  label="Титул" 
                  val1={player1.chessTitle ? 1 : 0} 
                  val2={player2?.chessTitle ? 1 : 0}
                  isBetterHigh={true}
                  customVal1={player1.chessTitle || '—'}
                  customVal2={player2?.chessTitle || '—'}
                />
                <StatRow 
                  label="Набрано очок (міжгр.)" 
                  val1={player1.chessTotalPoints || 0} 
                  val2={player2?.chessTotalPoints || 0}
                />
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

const SeasonSelector = ({ seasons, player, selectedId, onChange }: { 
  seasons: Season[], 
  player: PlayerDto | null,
  selectedId: number | null, 
  onChange: (id: number | null) => void 
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  const dropdownRef = React.useRef<HTMLDivElement>(null);

  // Close dropdown when clicking outside
  React.useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Filter seasons where the player had at least one achievement
  const filteredSeasons = seasons.filter(s => {
    if (!player || !player.achievements) return true;
    
    const start = new Date(s.startDate);
    const end = new Date(s.endDate);
    
    return player.achievements.some(a => {
      const achDate = new Date(a.date);
      return achDate >= start && achDate <= end;
    });
  });

  const selectedSeason = seasons.find(s => s.id === selectedId);

  return (
    <div className={styles.seasonSelector} ref={dropdownRef}>
      <label>Сезон</label>
      <div className={styles.customSelect} onClick={() => setIsOpen(!isOpen)}>
        <span className={styles.currentValue}>
          {selectedSeason ? selectedSeason.name : 'За весь час'}
        </span>
        <i className={`fa-solid fa-chevron-down ${isOpen ? styles.iconOpen : ''}`}></i>
        
        {isOpen && (
          <div className={styles.dropdownList}>
            <div 
              className={`${styles.dropdownOption} ${!selectedId ? styles.selectedOption : ''}`}
              onClick={() => { onChange(null); setIsOpen(false); }}
            >
              За весь час
            </div>
            {filteredSeasons.map(s => (
              <div 
                key={s.id} 
                className={`${styles.dropdownOption} ${selectedId === s.id ? styles.selectedOption : ''}`}
                onClick={() => { onChange(s.id); setIsOpen(false); }}
              >
                {s.name}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

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

  const getMedalCounts = () => {
    if (!player.achievements) return { gold: 0, silver: 0, bronze: 0 };
    
    // Filter achievements by season if selected
    let filtered = player.achievements.filter(a => a.result !== 'Спеціальне досягнення');
    
    if (selectedSeason) {
      const yearStr = selectedSeason.name.match(/\d{4}/)?.[0];
      if (yearStr) {
        filtered = filtered.filter(a => a.date.includes(yearStr));
      }
    }

    return {
      gold: filtered.filter(a => a.rank === 1).length,
      silver: filtered.filter(a => a.rank === 2).length,
      bronze: filtered.filter(a => a.rank === 3).length
    };
  };

  const medals = getMedalCounts();

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
        
        <div className={styles.medalsRow}>
          {medals.gold > 0 && <span className={styles.medalItem} title="Золото"><i className="fa-solid fa-medal" style={{color: '#ffd700'}}></i> {medals.gold}</span>}
          {medals.silver > 0 && <span className={styles.medalItem} title="Срібло"><i className="fa-solid fa-medal" style={{color: '#c0c0c0'}}></i> {medals.silver}</span>}
          {medals.bronze > 0 && <span className={styles.medalItem} title="Бронза"><i className="fa-solid fa-medal" style={{color: '#cd7f32'}}></i> {medals.bronze}</span>}
        </div>

        {player.studyYears && (
          <div className={styles.cardRank}>
            <i className="fa-solid fa-calendar-alt"></i> {player.studyYears}
          </div>
        )}
      </div>
    </div>
  );
};

const PointsDistributionChart = ({ player }: { player: PlayerDto }) => {
  const data = React.useMemo(() => {
    if (!player.sportStats || player.sportStats.length === 0) return [];
    return player.sportStats.map(s => ({
      name: translateSport(s.sportType),
      value: s.performanceScore
    })).filter(s => s.value > 0);
  }, [player]);

  const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'];

  if (data.length === 0) {
    return <div className={styles.noDataChart}>Немає балів</div>;
  }

  return (
    <div style={{ width: '100%', height: 260 }}>
      <ResponsiveContainer>
        <PieChart>
          <Pie
            data={data}
            cx="50%"
            cy="50%"
            innerRadius={40}
            outerRadius={70}
            paddingAngle={5}
            dataKey="value"
          >
            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
            ))}
          </Pie>
          <Tooltip 
            contentStyle={{ borderRadius: '12px', border: 'none', boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}
            formatter={(value: any) => [`${value} очок`, 'Бали']}
          />
          <Legend iconType="circle" />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
};

const translateSport = (sport: string) => {
  const translations: Record<string, string> = {
    chess: 'Шахи',
    checkers: 'Шашки',
    futsal: 'Футзал',
    volleyball: 'Волейбол',
    basketball_3x3: 'Баскетбол 3х3',
    table_tennis: 'Настільний теніс',
    armwrestling: 'Армрестлінг',
    badminton: 'Бадмінтон',
    beach_volleyball: 'Пляжний волейбол'
  };
  return translations[sport] || sport;
};

const HeadToHeadSection = ({ data, loading, p1Name, p2Name }: { data: any, loading: boolean, p1Name: string, p2Name: string }) => {
  const navigate = useNavigate();
  if (loading) return <div className={styles.h2hLoading}><div className={styles.spinnerSmall}></div> Пошук матчів...</div>;
  if (!data || !data.matches || data.matches.length === 0) {
    return <div className={styles.noH2H}>Гравці ще не зустрічалися в офіційних матчах</div>;
  }

  const getTournamentUrl = (sport: string, id: number) => {
    const map: Record<string, string> = {
      chess: 'chess',
      checkers: 'checkers',
      futsal: 'futsal',
      volleyball: 'volleyball',
      basketball_3x3: 'basketball',
      table_tennis: 'table-tennis',
      armwrestling: 'armwrestling',
      badminton: 'badminton',
      beach_volleyball: 'beach-volleyball'
    };
    return `/tournaments/${map[sport] || sport}/${id}`;
  };

  const translateResult = (res: string) => {
    if (res === 'P1 Won') return p1Name;
    if (res === 'P2 Won') return p2Name;
    if (res === 'Draw') return 'Нічия';
    return res;
  };

  return (
    <div className={styles.h2hContainer}>
      <div className={styles.h2hSummary}>
        <div className={styles.h2hScoreBlock}>
          <span className={styles.h2hPlayerName}>{p1Name}</span>
          <span className={styles.h2hScore}>{data.player1Wins}</span>
        </div>
        <div className={styles.h2hDivider}>
          <span className={styles.h2hDraws}>{data.draws} нічиїх</span>
        </div>
        <div className={styles.h2hScoreBlock}>
          <span className={styles.h2hPlayerName}>{p2Name}</span>
          <span className={styles.h2hScore}>{data.player2Wins}</span>
        </div>
      </div>

      <div className={styles.h2hMatchesList}>
        {data.matches.map((m: any, idx: number) => (
          <div 
            key={idx} 
            className={styles.h2hMatchItem}
            onClick={() => navigate(getTournamentUrl(m.sportType, m.tournamentId))}
            title="Перейти до турніру"
            style={{ cursor: 'pointer' }}
          >
            <div className={styles.h2hMatchSport}>
              <i className={`fa-solid ${getSportIcon(m.sportType)}`}></i>
            </div>
            <div className={styles.h2hMatchInfo}>
              <div className={styles.h2hMatchName}>{m.tournamentName}</div>
              <div className={styles.h2hMatchDate}>{new Date(m.date).toLocaleDateString()}</div>
            </div>
            <div className={`${styles.h2hMatchResult} ${m.result === 'P1 Won' ? styles.resP1 : m.result === 'P2 Won' ? styles.resP2 : ''}`}>
              {m.score}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

const getSportIcon = (sport: string) => {
  const icons: Record<string, string> = {
    chess: 'fa-chess',
    checkers: 'fa-chess-board',
    futsal: 'fa-soccer-ball',
    volleyball: 'fa-volleyball',
    basketball_3x3: 'fa-basketball',
    table_tennis: 'fa-table-tennis-paddle-ball',
    armwrestling: 'fa-handshake',
    badminton: 'fa-shuttlecock',
    beach_volleyball: 'fa-umbrella-beach'
  };
  return icons[sport] || 'fa-trophy';
};

const StatRow = ({ label, val1, val2, suffix = '', isBetterHigh = true, customVal1, customVal2 }: { 
  label: string, 
  val1: number | undefined, 
  val2: number | undefined,
  suffix?: string,
  isBetterHigh?: boolean,
  customVal1?: string,
  customVal2?: string
}) => {
  const v1 = val1 || 0;
  const v2 = val2 || 0;
  
  const is1Better = isBetterHigh ? v1 > v2 : v1 < v2;
  const is2Better = isBetterHigh ? v2 > v1 : v2 < v1;

  return (
    <div className={styles.statRow}>
      <div className={`${styles.statValue} ${is1Better ? styles.betterValue : ''}`}>
        {customVal1 || (v1 + suffix)}
      </div>
      <div className={styles.statLabel}>{label}</div>
      <div className={`${styles.statValue} ${is2Better ? styles.betterValue : ''}`}>
        {customVal2 || (v2 !== undefined ? `${v2}${suffix}` : '—')}
      </div>
    </div>
  );
};
