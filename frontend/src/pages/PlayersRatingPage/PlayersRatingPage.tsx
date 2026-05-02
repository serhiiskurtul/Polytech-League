import { useState, useEffect, useMemo } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import styles from './PlayersRatingPage.module.css';
import { getPhotoUrl } from '../../shared/utils/photo.utils';
import { getPlayersRankings } from '../../shared/api/players.api';
import { getSeasons } from '../../shared/api/seasonRating.api';
import { PlayerDto } from '../../shared/types/player.types';
import { Season } from '../../shared/types/seasonRating.types';

const PlayersRatingPage = () => {
  const [searchParams] = useSearchParams();
  const p1Id = searchParams.get('p1');
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState('Загальний');
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedWeightClass, setSelectedWeightClass] = useState<string>('all');
  const [sortBy, setSortBy] = useState<'performance' | 'goals'>('performance');
  const [selectedPlayers, setSelectedPlayers] = useState<PlayerDto[]>([]);
  
  const [seasons, setSeasons] = useState<Season[]>([]);
  const [selectedSeasonId, setSelectedSeasonId] = useState<string>('all');
  const [selectedGender, setSelectedGender] = useState<'all' | 'male' | 'female'>('all');

  useEffect(() => {
    if (players.length > 0 && p1Id && selectedPlayers.length === 0) {
      const playerToPreselect = players.find(p => p.id === Number(p1Id));
      if (playerToPreselect) {
        setSelectedPlayers([playerToPreselect]);
      }
    }
  }, [players, p1Id]);

  const togglePlayerSelection = (player: PlayerDto) => {
    setSelectedPlayers(prev => {
      const isSelected = prev.find(p => p.id === player.id);
      if (isSelected) {
        return prev.filter(p => p.id !== player.id);
      }
      if (prev.length >= 2) {
        return [prev[1], player];
      }
      return [...prev, player];
    });
  };

  useEffect(() => {
    const fetchSeasons = async () => {
      try {
        const data = await getSeasons();
        setSeasons(data);
        const current = data.find(s => s.isCurrent);
        if (current) setSelectedSeasonId(current.id.toString());
      } catch (err) {
        console.error('Error fetching seasons:', err);
      }
    };
    fetchSeasons();
  }, []);

  useEffect(() => {
    const fetchRankings = async () => {
      setLoading(true);
      setError(null);
      try {
        const sid = selectedSeasonId === 'all' ? undefined : selectedSeasonId;
        const data = await getPlayersRankings(sid);
        setPlayers(data);
      } catch (err: any) {
        console.error('Error fetching rankings:', err);
        setError('Не вдалося завантажити рейтинги. Будь ласка, спробуйте пізніше.');
      } finally {
        setLoading(false);
      }
    };
    fetchRankings();
  }, [selectedSeasonId]);

  const sportsTabs = [
    { id: 'all', name: 'Загальний', icon: 'fa-globe', key: 'Загальний' },
    { id: 'chess', name: 'Шахи', icon: 'fa-chess', key: 'chess' },
    { id: 'checkers', name: 'Шашки', icon: 'fa-chess-board', key: 'checkers' },
    { id: 'tt', name: 'Настільний теніс', icon: 'fa-table-tennis-paddle-ball', key: 'table_tennis' },
    { id: 'futsal', name: 'Футзал', icon: 'fa-futbol', key: 'futsal' },
    { id: 'volleyball', name: 'Волейбол', icon: 'fa-volleyball-ball', key: 'volleyball' },
    { id: 'basketball', name: 'Баскетбол 3х3', icon: 'fa-basketball-ball', key: 'basketball_3x3' },
    { id: 'beach', name: 'Пляжний волейбол', icon: 'fa-umbrella-beach', key: 'beach_volleyball' },
    { id: 'arm', name: 'Армрестлинг', icon: 'fa-fist-raised', key: 'armwrestling' },
    { id: 'badm', name: 'Бадмінтон', icon: 'fa-feather-alt', key: 'badminton' }
  ];

  const armwrestlingWeightClasses = [
    { id: 'all', name: 'Всі категорії' },
    { id: 'Under60kg', name: 'До 60 кг' },
    { id: 'Under70kg', name: 'До 70 кг' },
    { id: 'Under80kg', name: 'До 80 кг' },
    { id: 'Under90kg', name: 'До 90 кг' },
    { id: 'Plus90kg', name: 'Понад 90 кг' }
  ];

  // Reset filters when changing tab
  useEffect(() => {
    setSelectedWeightClass('all');
    setSortBy('performance');
    setSelectedGender('all');
  }, [activeTab]);

  const filteredPlayers = useMemo(() => {
    let result = [...players];

    // Filter by gender for Chess/Checkers
    if ((activeTab.toLowerCase() === 'chess' || activeTab.toLowerCase() === 'checkers') && selectedGender !== 'all') {
      result = result.filter(p => {
        // 1. Use explicit gender if set
        if (p.gender && p.gender !== 'None') {
          if (selectedGender === 'female') return p.gender === 'Female';
          if (selectedGender === 'male') return p.gender === 'Male';
        }

        // 2. Fallback to heuristic based on patronymic
        if (!p.patronymic) return false;
        const patr = p.patronymic.trim().toLowerCase();
        const isFemale = patr.endsWith('івна') || patr.endsWith('ївна');
        const isMale = patr.endsWith('вич');
        
        if (selectedGender === 'female') return isFemale;
        if (selectedGender === 'male') return isMale;
        return true;
      });
    }

    // Filter by tab
    if (activeTab !== 'Загальний') {
      result = result.filter(p => p.sports?.includes(activeTab) || p.sports?.some(s => s.toLowerCase() === activeTab.toLowerCase()));
      
      // Filter by weight class for armwrestling
      if (activeTab.toLowerCase() === 'armwrestling' && selectedWeightClass !== 'all') {
        result = result.filter(p => 
          p.armwrestlingPreferredWeightClass === selectedWeightClass || 
          p.armwrestlingWeightClasses?.includes(selectedWeightClass)
        );
      }
    }

    // Filter by search
    if (searchQuery) {
      const q = searchQuery.toLowerCase();
      result = result.filter(p =>
        p.firstName.toLowerCase().includes(q) ||
        p.lastName.toLowerCase().includes(q) ||
        (p.previousNames && p.previousNames.toLowerCase().includes(q))
      );
    }

    // Sorting
    return result.sort((a, b) => {
      if (activeTab.toLowerCase() === 'futsal' && sortBy === 'goals') {
        return (b.goalsScored || 0) - (a.goalsScored || 0);
      }
      
      if (activeTab !== 'Загальний') {
        const aSport = a.sportStats?.find(s => s.sportType.toLowerCase() === activeTab.toLowerCase());
        const bSport = b.sportStats?.find(s => s.sportType.toLowerCase() === activeTab.toLowerCase());
        return (bSport?.performanceScore || 0) - (aSport?.performanceScore || 0);
      }
      return (b.performanceScore || 0) - (a.performanceScore || 0);
    });
  }, [players, activeTab, searchQuery, selectedWeightClass, sortBy]);

  const getSportInfo = (sportKey: string) => {
    switch (sportKey.toLowerCase()) {
      case 'chess': return { icon: '♟️', color: '#8b4513' };
      case 'checkers': return { icon: '🏁', color: '#555' };
      case 'futsal': return { icon: '⚽', color: '#2c3e50' };
      case 'basketball_3x3': return { icon: '🏀', color: '#e67e22' };
      case 'volleyball': return { icon: '🏐', color: '#f1c40f' };
      case 'table_tennis': return { icon: '🏓', color: '#e74c3c' };
      case 'badminton': return { icon: '🏸', color: '#1abc9c' };
      case 'armwrestling': return { icon: '💪', color: '#7f8c8d' };
      case 'beach_volleyball': return { icon: '🏖️', color: '#3498db' };
      default: return { icon: '🏅', color: '#9b59b6' };
    }
  };

  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://polytech-league-production.up.railway.app';

  return (
    <div className={styles.page}>
      <div className={styles.container}>
        <div className={styles.main}>
          {/* Hero Section */}
          <div className={styles.heroSection}>
            <h1 className={styles.heroTitle}>
              <i className="fa-solid fa-ranking-star"></i>
              Рейтинги гравців
            </h1>
            <p className={styles.heroDescription}>
              Система рейтингів на основі результатів змагань (призові місця та рівень змагань).
            </p>
          </div>

          {/* Tabs Section */}
          <div className={styles.tabsContainer}>
            <div className={styles.tabsScroll}>
              {sportsTabs.map(tab => (
                <button
                  key={tab.id}
                  className={`${styles.tabItem} ${activeTab === tab.key ? styles.activeTab : ''}`}
                  onClick={() => setActiveTab(tab.key)}
                >
                  <i className={`fa-solid ${tab.icon}`}></i>
                  {tab.name}
                </button>
              ))}
            </div>
          </div>

          <div className={styles.filtersRow}>
            <div className={styles.searchBox}>
              <i className="fa-solid fa-search"></i>
              <input
                type="text"
                placeholder="Пошук за ім'ям..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
            
            {activeTab.toLowerCase() === 'armwrestling' && (
              <div className={styles.weightFilter}>
                <i className="fa-solid fa-weight-hanging"></i>
                <select 
                  value={selectedWeightClass} 
                  onChange={(e) => setSelectedWeightClass(e.target.value)}
                  className={styles.weightSelect}
                >
                  {armwrestlingWeightClasses.map(wc => (
                    <option key={wc.id} value={wc.id}>{wc.name}</option>
                  ))}
                </select>
              </div>
            )}

            {activeTab.toLowerCase() === 'futsal' && (
              <div className={styles.weightFilter}>
                <i className="fa-solid fa-sort"></i>
                <select 
                  value={sortBy} 
                  onChange={(e) => setSortBy(e.target.value as any)}
                  className={styles.weightSelect}
                >
                  <option value="performance">За рейтингом</option>
                  <option value="goals">За голами</option>
                </select>
              </div>
            )}

            <div className={styles.weightFilter}>
              <i className="fa-solid fa-calendar-days"></i>
              <select 
                value={selectedSeasonId} 
                onChange={(e) => setSelectedSeasonId(e.target.value)}
                className={styles.weightSelect}
              >
                <option value="all">За весь час</option>
                {seasons.map(s => (
                  <option key={s.id} value={s.id}>{s.name} {s.isCurrent ? '(Поточний)' : ''}</option>
                ))}
              </select>
            </div>

            {(activeTab.toLowerCase() === 'chess' || activeTab.toLowerCase() === 'checkers') && (
              <div className={styles.genderTabs}>
                <button 
                  className={`${styles.genderTab} ${selectedGender === 'all' ? styles.activeGenderTab : ''}`}
                  onClick={() => setSelectedGender('all')}
                >
                  Всі
                </button>
                <button 
                  className={`${styles.genderTab} ${selectedGender === 'male' ? styles.activeGenderTab : ''}`}
                  onClick={() => setSelectedGender('male')}
                >
                  Чоловіки
                </button>
                <button 
                  className={`${styles.genderTab} ${selectedGender === 'female' ? styles.activeGenderTab : ''}`}
                  onClick={() => setSelectedGender('female')}
                >
                  Жінки
                </button>
              </div>
            )}

            <div className={styles.resultsCount}>
              Знайдено: <strong>{filteredPlayers.length}</strong> гравців
            </div>
          </div>

          {/* Rating Table */}
          <div className={styles.ratingSection}>
            {loading ? (
              <div className={styles.loadingContainer}>
                <div className={styles.spinner}></div>
                <p>Завантаження даних...</p>
              </div>
            ) : error ? (
              <div className={styles.errorContainer}>
                <i className="fa-solid fa-triangle-exclamation"></i>
                <p>{error}</p>
                <button onClick={() => window.location.reload()} className={styles.retryBtn}>
                  Спробувати ще раз
                </button>
              </div>
            ) : (
              <div className={styles.ratingTable}>
                <div className={styles.tableHeader}>
                  <div className={styles.headerCell}>#</div>
                  <div className={styles.headerCell}>Гравець</div>
                  <div className={styles.headerCell}>Спорт</div>
                  <div className={styles.headerCell}>Очки</div>
                  <div className={styles.headerCell}>Статистика</div>
                  <div className={`${styles.headerCell} ${styles.centered}`}>Дії</div>
                </div>

                <div className={styles.tableBody}>
                  {filteredPlayers.map((player, index) => {
                    const photoSrc = getPhotoUrl(player.photoUrl);

                    return (
                      <div key={player.id} className={styles.tableRow}>
                        <div className={styles.cell}>
                          <div className={`${styles.rankCircle} ${index < 3 ? styles.topRank : ''}`}>
                            {index + 1}
                          </div>
                        </div>

                        <div className={styles.cell}>
                          <Link to={`/players/${player.id}`} className={styles.playerInfo}>
                            {photoSrc ? (
                              <img src={photoSrc} alt="" className={styles.playerAvatarImg} />
                            ) : (
                              <img 
                                src="https://cdn-icons-png.flaticon.com/512/149/149071.png" 
                                alt="Placeholder" 
                                className={styles.playerAvatarImg} 
                                style={{ opacity: 0.6, background: '#f1f5f9' }}
                              />
                            )}
                            <div className={styles.playerDetails}>
                              <div className={styles.playerName}>
                                {player.firstName} {player.lastName}
                                {player.previousNames && (
                                  <span style={{ fontSize: '11px', color: '#64748b', fontWeight: 'normal', marginLeft: '4px' }}>
                                    ({player.previousNames})
                                  </span>
                                )}
                              </div>
                              <div className={styles.playerSubInfo}>
                                <span className={styles.groupText}>{player.group || '—'}</span>
                              </div>
                            </div>
                          </Link>
                        </div>

                        <div className={styles.cell}>
                          <div className={styles.sportsList}>
                            {player.sports
                              ?.filter(s => activeTab === 'Загальний' || s.toLowerCase() === activeTab.toLowerCase())
                              .map(s => {
                                const info = getSportInfo(s);
                                return (
                                  <div key={s} className={styles.sportBadgeSmall} title={s} style={{ backgroundColor: info.color + '22', color: info.color }}>
                                    {info.icon}
                                  </div>
                                );
                              })}
                            {(!player.sports || player.sports.length === 0) && <span className={styles.emptyVal}>—</span>}
                          </div>
                        </div>

                        <div className={styles.cell}>
                          <div className={styles.ratingBox}>
                            <div className={styles.ratingLarge}>
                              {activeTab === 'Загальний' 
                                ? player.performanceScore 
                                : player.sportStats?.find(s => s.sportType.toLowerCase() === activeTab.toLowerCase())?.performanceScore || 0}
                            </div>
                            <div className={styles.pointsLabel}>очок</div>
                            
                            {player.pointsBreakdown && player.pointsBreakdown.length > 0 && (
                              <div className={styles.breakdownTrigger}>
                                <i className="fa-solid fa-circle-info"></i>
                                <div className={`${styles.breakdownTooltip} ${index < 3 ? styles.tooltipDown : ''}`}>
                                  <div className={styles.breakdownHeader}>Деталізація балів</div>
                                  <div className={styles.breakdownList}>
                                    {player.pointsBreakdown
                                      .filter(entry => activeTab === 'Загальний' || entry.sportType?.toLowerCase() === activeTab.toLowerCase())
                                      .map((entry, i) => (
                                      <div key={i} className={styles.breakdownItem}>
                                        <span className={styles.breakdownDesc}>{entry.description}</span>
                                        <span className={styles.breakdownPoints}>+{entry.points}</span>
                                      </div>
                                    ))}
                                  </div>
                                  <div className={styles.breakdownTotal}>
                                    Разом: {activeTab === 'Загальний' 
                                      ? player.performanceScore 
                                      : player.sportStats?.find(s => s.sportType.toLowerCase() === activeTab.toLowerCase())?.performanceScore || 0} очок
                                  </div>
                                </div>
                              </div>
                            )}
                          </div>
                        </div>

                        <div className={styles.cell}>
                          <div className={styles.statsMini}>
                            <div className={styles.statMiniItem}>
                              <span className={styles.statMiniVal}>
                                {activeTab === 'Загальний' 
                                  ? player.matchesPlayed 
                                  : player.sportStats?.find(s => s.sportType.toLowerCase() === activeTab.toLowerCase())?.matchesPlayed || 0}
                              </span> ігор
                            </div>
                            {(activeTab.toLowerCase() === 'futsal' || (activeTab === 'Загальний' && player.sports?.includes('futsal'))) && (
                              <div className={`${styles.statMiniItem} ${sortBy === 'goals' ? styles.highlightStat : ''}`}>
                                <span className={styles.statMiniVal}>{player.goalsScored || 0}</span> голів
                              </div>
                            )}
                            <div className={styles.statMiniItem}>
                              <span className={styles.statMiniVal}>
                                {activeTab === 'Загальний' 
                                  ? player.winsCount 
                                  : player.sportStats?.find(s => s.sportType.toLowerCase() === activeTab.toLowerCase())?.winsCount || 0}
                              </span> перемог
                            </div>
                          </div>
                        </div>

                         <div className={`${styles.cell} ${styles.centered}`}>
                          <div className={styles.actionsGroup}>
                            <Link to={`/players/${player.id}`} className={styles.actionBtn} title="Профіль">
                              <i className="fa-regular fa-user"></i>
                            </Link>
                            <button 
                              className={`${styles.actionBtn} ${selectedPlayers.find(p => p.id === player.id) ? styles.activeAction : ''}`}
                              onClick={() => togglePlayerSelection(player)}
                              title="Порівняти"
                            >
                              <i className="fa-solid fa-arrows-left-right"></i>
                            </button>
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>

                {filteredPlayers.length === 0 && (
                  <div className={styles.noResults}>Гравців не знайдено</div>
                )}
              </div>
            )}
          </div>

          {/* Scoring Formula Section */}
          <div className={styles.formulaSection}>
            <div className={styles.formulaHeader}>
              <i className="fa-solid fa-circle-info"></i>
              <h2>Як нараховуються очки?</h2>
            </div>
            <div className={styles.formulaGrid}>
              <div className={styles.formulaCard}>
                <div className={styles.formulaIcon} style={{ background: '#fef3c7', color: '#d97706' }}>
                  <i className="fa-solid fa-trophy"></i>
                </div>
                <h3>Призові місця</h3>
                <ul>
                  <li><span>🥇 1 місце:</span> 100 балів</li>
                  <li><span>🥈 2 місце:</span> 50 балів</li>
                  <li><span>🥉 3 місце:</span> 30 балів</li>
                </ul>
              </div>

              <div className={styles.formulaCard}>
                <div className={styles.formulaIcon} style={{ background: '#dcfce7', color: '#16a34a' }}>
                  <i className="fa-solid fa-layer-group"></i>
                </div>
                <h3>Рівень змагань</h3>
                <p>Бали за призові місця множаться на статус змагань:</p>
                <ul>
                  <li><span>Міжгрупові:</span> x1.0</li>
                  <li><span>Обласні:</span> x2.5</li>
                  <li><span>Всеукраїнські:</span> x5.0</li>
                </ul>
              </div>



              <div className={styles.formulaCard}>
                <div className={styles.formulaIcon} style={{ background: '#ffedd5', color: '#ea580c' }}>
                  <i className="fa-solid fa-futbol"></i>
                </div>
                <h3>Забиті голи (Футзал)</h3>
                <p>Додаткові бали за індивідуальну результативність:</p>
                <ul>
                  <li><span>Кожен забитий гол:</span> +5 балів</li>
                </ul>
              </div>

              <div className={styles.formulaCard}>
                <div className={styles.formulaIcon} style={{ background: '#f5f3ff', color: '#7c3aed' }}>
                  <i className="fa-solid fa-users"></i>
                </div>
                <h3>Участь у змаганнях</h3>
                <p>Бали за сам факт участі (тільки для міжгрупових змагань):</p>
                <ul>
                  <li><span>Кожен турнір:</span> +10 балів</li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      {/* Comparison Floating Bar */}
      {selectedPlayers.length > 0 && (
        <div className={styles.comparisonBar}>
          <div className={styles.comparisonBarContent}>
            <div className={styles.selectedInfo}>
              <span className={styles.compareCount}>{selectedPlayers.length}/2</span>
              <span>гравців вибрано</span>
            </div>
            <div className={styles.selectedPlayersList}>
              {selectedPlayers.map(p => (
                <div key={p.id} className={styles.selectedPlayerBadge}>
                  {p.firstName} {p.lastName[0]}.
                   <button onClick={() => togglePlayerSelection(p)} className={styles.removeBtn}>
                    <i className="fa-solid fa-xmark"></i>
                  </button>
                </div>
              ))}
            </div>
            <div className={styles.compareActions}>
              <button onClick={() => setSelectedPlayers([])} className={styles.clearBtn}>
                Очистити
              </button>
              <Link 
                to={`/compare?p1=${selectedPlayers[0]?.id}${selectedPlayers[1] ? `&p2=${selectedPlayers[1].id}` : ''}`}
                className={`${styles.compareBtn} ${selectedPlayers.length < 2 ? styles.disabled : ''}`}
              >
                Порівняти
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default PlayersRatingPage;
