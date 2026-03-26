import { useState, useEffect } from 'react';
import styles from './PlayersRatingPage.module.css';

interface Player {
  id: number;
  name: string;
  avatar: string;
  sport: string;
  sportIcon: string;
  team: string;
  rating: number;
  ratingChange: number;
  games: number;
  wins: number;
  points: number;
  years: string;
  gender: 'Ч' | 'Ж';
  rank: number;
  trend: 'up' | 'down' | 'stable';
}

interface Filters {
  sport: string;
  gender: string;
  search: string;
  sortBy: keyof Player;
  sortOrder: 'asc' | 'desc';
}

const PlayersRatingPage = () => {
  const [players, setPlayers] = useState<Player[]>([]);
  const [filteredPlayers, setFilteredPlayers] = useState<Player[]>([]);
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    totalPlayers: 50,
    totalSports: 8,
    maxRating: 2192
  });
  
  const [filters, setFilters] = useState<Filters>({
    sport: 'Всі',
    gender: 'Всі',
    search: '',
    sortBy: 'rating',
    sortOrder: 'desc'
  });

  const sports = [
    { id: 'all', name: 'Всі', icon: 'fa-filter' },
    { id: 'futsal', name: 'Футзал', icon: 'fa-futbol' },
    { id: 'basketball', name: 'Баскетбол', icon: 'fa-basketball-ball' },
    { id: 'volleyball', name: 'Волейбол', icon: 'fa-volleyball-ball' },
    { id: 'chess', name: 'Шахи', icon: 'fa-chess' },
    { id: 'checkers', name: 'Шашки', icon: 'fa-chess-board' },
    { id: 'table_tennis', name: 'Настільний теніс', icon: 'fa-table-tennis' },
    { id: 'badminton', name: 'Бадмінтон', icon: 'fa-feather-alt' },
    { id: 'armwrestling', name: 'Армреслінг', icon: 'fa-fist-raised' }
  ];

  const genders = [
    { id: 'all', name: 'Всі', icon: 'fa-users' },
    { id: 'men', name: 'Чоловіки', icon: '👨' },
    { id: 'women', name: 'Жінки', icon: '👩' }
  ];

  const sortOptions = [
    { value: 'rating', label: 'Рейтинг' },
    { value: 'games', label: 'Кількість ігор' },
    { value: 'wins', label: 'Перемоги' },
    { value: 'years', label: 'Роки навчання' },
    { value: 'name', label: 'Ім\'я' }
  ];

  // Імітація завантаження даних
  useEffect(() => {
    const mockPlayers = generateMockPlayers();
    setPlayers(mockPlayers);
    setFilteredPlayers(mockPlayers);
    
    if (mockPlayers.length > 0) {
      setStats({
        totalPlayers: mockPlayers.length,
        totalSports: new Set(mockPlayers.map(p => p.sport)).size,
        maxRating: Math.max(...mockPlayers.map(p => p.rating))
      });
    }
    
    setLoading(false);
  }, []);

  // Фільтрація та сортування
  useEffect(() => {
    let result = [...players];

    // Фільтрація за видом спорту
    if (filters.sport !== 'Всі') {
      result = result.filter(player => player.sport === filters.sport);
    }

    // Фільтрація за статтю
    if (filters.gender !== 'Всі') {
      const genderFilter = filters.gender === 'Чоловіки' ? 'Ч' : 'Ж';
      result = result.filter(player => player.gender === genderFilter);
    }

    // Пошук за ім'ям або команді
    if (filters.search) {
      const searchLower = filters.search.toLowerCase();
      result = result.filter(player => 
        player.name.toLowerCase().includes(searchLower) ||
        player.team.toLowerCase().includes(searchLower)
      );
    }

    // Сортування
    result.sort((a, b) => {
      const aValue = a[filters.sortBy];
      const bValue = b[filters.sortBy];
      
      if (typeof aValue === 'string' && typeof bValue === 'string') {
        return filters.sortOrder === 'asc' 
          ? aValue.localeCompare(bValue)
          : bValue.localeCompare(aValue);
      }

      if (typeof aValue === 'number' && typeof bValue === 'number') {
        return filters.sortOrder === 'asc' 
          ? aValue - bValue
          : bValue - aValue;
      }
      
      return 0;
    });

    setFilteredPlayers(result);
  }, [filters, players]);

  const generateMockPlayers = (): Player[] => {
    const sportsData = [
      { name: 'Футзал', icon: '⚽' },
      { name: 'Баскетбол', icon: '🏀' },
      { name: 'Волейбол', icon: '🏐' },
      { name: 'Шахи', icon: '♟️' },
      { name: 'Шашки', icon: '♟️' },
      { name: 'Настільний теніс', icon: '🏓' },
      { name: 'Бадмінтон', icon: '🏸' },
      { name: 'Армреслінг', icon: '💪' }
    ];
    const teamsList = ['ІТ-31', 'ЕК-22', 'ПР-11', 'МК-41', 'ФІ-21', 'ЕП-32', 'МК-31', 'ЕК-41'];
    const names = [
      'Артем Іванов', 'Марія Семенова', 'Влад Ковальчук', 'Катерина Шевченко', 
      'Олексій Петренко', 'Юлія Мороз', 'Дмитро Соколов', 'Анна Бойко',
      'Сергій Бондар', 'Олена Кравченко', 'Ігор Лисенко', 'Тетяна Руденко',
      'Поліщук Вікторія', 'Бондаренко Дмитро', 'Шевчук Павло'
    ];

    const mockData = Array.from({ length: 50 }, (_, i): Player => {
      const wins = Math.floor(Math.random() * 20) + 5;
      const games = wins + Math.floor(Math.random() * 15);
      const sportObj = sportsData[i % sportsData.length];
      const rating = 1200 + Math.floor(Math.random() * 1000);
      const ratingChange = Math.floor(Math.random() * 40) - 15;
      
      const startYear = 2022 + Math.floor(Math.random() * 3);
      const endYear = startYear + 4;
      const years = i % 5 === 0 ? `${startYear} - тепер` : `${startYear} - ${endYear}`;

      return {
        id: i + 1,
        name: i < names.length ? names[i] : names[i % names.length] + ' ' + (Math.floor(i / names.length) + 1),
        avatar: names[i % names.length].split(' ').map(n => n[0]).join(''),
        sport: sportObj.name,
        sportIcon: sportObj.icon,
        team: teamsList[Math.floor(Math.random() * teamsList.length)],
        rating: rating,
        ratingChange: ratingChange,
        games,
        wins,
        points: wins * 10 + Math.floor(Math.random() * 50),
        years,
        gender: Math.random() > 0.4 ? 'Ч' : 'Ж',
        rank: 0,
        trend: ratingChange > 0 ? 'up' : (ratingChange < 0 ? 'down' : 'stable')
      };
    });

    return mockData.sort((a, b) => b.rating - a.rating).map((p, i) => ({ ...p, rank: i + 1 }));
  };

  const handleFilterChange = (key: keyof Filters, value: any) => {
    setFilters(prev => ({ ...prev, [key]: value }));
  };

  const resetFilters = () => {
    setFilters({
      sport: 'Всі',
      gender: 'Всі',
      search: '',
      sortBy: 'rating',
      sortOrder: 'desc'
    });
  };

  return (
    <div className={styles.page}>
      <div className={styles.container}>
        <div className={styles.main}>
          {/* Hero Section */}
          <div className={styles.heroSection}>
            <h1 className={styles.heroTitle}>
              <i className="fa-solid fa-chart-line"></i>
              Рейтинг гравців
            </h1>
            <p className={styles.heroDescription}>
              Детальна статистика та рейтинг всіх гравців платформи. Фільтруйте, сортуйте та аналізуйте результати.
            </p>
            
            <div className={styles.statsOverview}>
              <div className={styles.statCard}>
                <div className={styles.statValue}>{stats.totalPlayers}</div>
                <div className={styles.statLabel}>ГРАВЦІВ У СИСТЕМІ</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statValue}>{stats.totalSports}</div>
                <div className={styles.statLabel}>ВИДІВ СПОРТУ</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statValue}>{stats.maxRating}</div>
                <div className={styles.statLabel}>МАКСИМАЛЬНИЙ РЕЙТИНГ</div>
              </div>
            </div>
          </div>

          {/* Filters Section */}
          <div className={styles.filtersSection}>
            <div className={styles.filtersCard}>
              <div className={styles.filtersHeader}>
                <h3 className={styles.filtersTitle}>
                  <i className="fa-solid fa-filter"></i> Фільтри та сортування
                </h3>
                <div className={styles.actions}>
                  <button className={styles.resetButton} onClick={resetFilters}>
                    <i className="fa-solid fa-redo"></i> Скинути фільтри
                  </button>
                  <button className={styles.exportButton}>
                    <i className="fa-solid fa-file-csv"></i> Експорт CSV
                  </button>
                </div>
              </div>

              <div className={styles.filtersContent}>
                <div className={styles.searchSide}>
                  <label className={styles.filterLabel}>
                    <i className="fa-solid fa-search"></i> Пошук гравця
                  </label>
                  <div className={styles.searchContainer}>
                    <i className="fa-solid fa-search"></i>
                    <input 
                      type="text" 
                      className={styles.searchInput}
                      placeholder="Введіть ім'я або команду..." 
                      value={filters.search}
                      onChange={(e) => handleFilterChange('search', e.target.value)}
                    />
                  </div>
                </div>

                <div className={styles.sportsSide}>
                  <label className={styles.filterLabel}>
                    <i className="fa-solid fa-running"></i> Вид спорту
                  </label>
                  <div className={styles.chipsContainer}>
                    {sports.map(s => (
                      <button 
                        key={s.id}
                        className={`${styles.chip} ${filters.sport === s.name ? styles.active : ''}`}
                        onClick={() => handleFilterChange('sport', s.name)}
                      >
                        <i className={`fa-solid ${s.icon}`}></i>
                        {s.name}
                      </button>
                    ))}
                  </div>
                </div>
              </div>

              <div className={styles.bottomFilters}>
                <div className={styles.genderChips}>
                  {genders.map(g => (
                    <button
                      key={g.id}
                      className={`${styles.chip} ${filters.gender === g.name ? styles.active : ''}`}
                      onClick={() => handleFilterChange('gender', g.name)}
                    >
                      {g.icon.includes('fa-') ? <i className={`fa-solid ${g.icon}`}></i> : <span>{g.icon}</span>}
                      {g.name}
                    </button>
                  ))}
                </div>

                <div className={styles.sortChips}>
                  {sortOptions.map(opt => (
                    <button
                      key={opt.value}
                      className={`${styles.sortChip} ${filters.sortBy === opt.value ? styles.active : ''}`}
                      onClick={() => handleFilterChange('sortBy', opt.value)}
                    >
                      {opt.label}
                      {filters.sortBy === opt.value && <i className="fa-solid fa-chevron-down"></i>}
                    </button>
                  ))}
                </div>
              </div>
            </div>
            
            <div className={styles.resultsSummary}>
              Знайдено: <strong>{filteredPlayers.length}</strong> гравців
            </div>
          </div>

          {/* Rating Table Section */}
          <div className={styles.ratingSection}>
            {loading ? (
              <div className={styles.loadingContainer}>
                <div className={styles.spinner}></div>
                <p>Завантаження рейтингу...</p>
              </div>
            ) : (
              <div className={styles.ratingTable}>
                <div className={styles.tableHeader}>
                  <div className={styles.headerCell}>#</div>
                  <div className={styles.headerCell}>Гравець</div>
                  <div className={styles.headerCell}>Спорт</div>
                  <div className={styles.headerCell}>Рейтинг</div>
                  <div className={styles.headerCell}>Статистика</div>
                  <div className={`${styles.headerCell} ${styles.centered}`}>Роки навчання</div>
                  <div className={`${styles.headerCell} ${styles.centered}`}>Дії</div>
                </div>
                
                <div className={styles.tableBody}>
                  {filteredPlayers.map((player) => (
                    <div key={player.id} className={styles.tableRow}>
                      <div className={styles.cell}>
                        <div className={`${styles.rankCircle} ${player.rank <= 3 ? styles.topRank : ''}`}>
                          {player.rank}
                          <i className={`fa-solid fa-arrow-${player.trend === 'up' ? 'up' : 'down'} ${styles.trendIcon} ${styles[player.trend]}`}></i>
                        </div>
                      </div>
                      
                      <div className={styles.cell}>
                        <div className={styles.playerInfo}>
                          <div 
                            className={styles.playerAvatar}
                            style={{ background: `linear-gradient(135deg, hsl(${(player.id * 137.5) % 360}, 70%, 50%), hsl(${(player.id * 137.5 + 40) % 360}, 70%, 40%))` }}
                          >
                            {player.avatar}
                          </div>
                          <div className={styles.playerDetails}>
                            <div className={styles.playerName}>{player.name}</div>
                            <div className={styles.playerSubInfo}>
                              <span className={styles.genderIcon} title={player.gender === 'Ч' ? 'Чоловік' : 'Жінка'}>
                                {player.gender === 'Ч' ? '👨' : '👩'}
                              </span>
                              <span className={styles.teamText}>{player.team}</span>
                            </div>
                          </div>
                        </div>
                      </div>

                      <div className={styles.cell}>
                        <div className={styles.sportBadge} title={player.sport}>
                          {player.sportIcon}
                        </div>
                      </div>

                      <div className={styles.cell}>
                        <div className={styles.ratingBox}>
                          <div className={styles.ratingLarge}>{player.rating}</div>
                          <div className={`${styles.ratingBadge} ${player.ratingChange >= 0 ? styles.positive : styles.negative}`}>
                            {player.ratingChange >= 0 ? `+${player.ratingChange}` : player.ratingChange}
                          </div>
                        </div>
                      </div>

                      <div className={styles.cell}>
                        <div className={styles.statsColumn}>
                          <div className={styles.statsLine}>
                            <i className="fa-regular fa-play-circle"></i> {player.games} ігор
                          </div>
                          <div className={styles.statsLine}>
                            <i className="fa-solid fa-trophy"></i> {player.wins} перемог
                          </div>
                          <div className={styles.statsLine}>
                            <i className="fa-solid fa-star"></i> {player.points} очок
                          </div>
                        </div>
                      </div>

                      <div className={`${styles.cell} ${styles.centered}`}>
                        <div className={styles.yearsBadge}>{player.years}</div>
                      </div>

                      <div className={`${styles.cell} ${styles.centered}`}>
                        <div className={styles.actionsGroup}>
                          <button className={styles.actionBtn} title="Профіль">
                            <i className="fa-regular fa-user"></i>
                          </button>
                          <button className={styles.actionBtn} title="Статистика">
                            <i className="fa-solid fa-chart-bar"></i>
                          </button>
                          <button className={styles.actionBtn} title="Порівняти">
                            <i className="fa-solid fa-balance-scale"></i>
                          </button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
                
                {filteredPlayers.length === 0 && (
                  <div className={styles.noResults}>Гравців не знайдено</div>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PlayersRatingPage;
