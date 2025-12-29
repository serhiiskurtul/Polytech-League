import React, { useState, useEffect } from 'react';
import Header from '../../widgets/header/Header';
import Footer from '../../widgets/footer/Footer';
import styles from './PlayersRatingPage.module.css';

const PlayersRatingPage = () => {
  const [players, setPlayers] = useState([]);
  const [filteredPlayers, setFilteredPlayers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState({
    sport: 'Всі',
    gender: 'Всі',
    search: '',
    sortBy: 'rating',
    sortOrder: 'desc'
  });

  const sports = ['Всі', 'Футзал', 'Баскетбол', 'Волейбол', 'Шахи', 'Шашки', 'Настільний теніс', 'Бадмінтон', 'Армреслінг'];
  const genders = ['Всі', 'Чоловіки', 'Жінки'];
  const sortOptions = [
    { value: 'rating', label: 'Рейтинг' },
    { value: 'games', label: 'Кількість ігор' },
    { value: 'wins', label: 'Перемоги' },
    { value: 'winRate', label: '% перемог' },
    { value: 'name', label: 'Ім\'я' }
  ];

  // Імітація завантаження даних
  useEffect(() => {
    const mockPlayers = generateMockPlayers();
    setPlayers(mockPlayers);
    setFilteredPlayers(mockPlayers);
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
      let aValue, bValue;
      
      switch (filters.sortBy) {
        case 'rating':
          aValue = a.rating;
          bValue = b.rating;
          break;
        case 'games':
          aValue = a.games;
          bValue = b.games;
          break;
        case 'wins':
          aValue = a.wins;
          bValue = b.wins;
          break;
        case 'winRate':
          aValue = a.winRate;
          bValue = b.winRate;
          break;
        case 'name':
          aValue = a.name;
          bValue = b.name;
          break;
        default:
          aValue = a.rating;
          bValue = b.rating;
      }

      if (typeof aValue === 'string') {
        return filters.sortOrder === 'asc' 
          ? aValue.localeCompare(bValue)
          : bValue.localeCompare(aValue);
      }

      return filters.sortOrder === 'asc' 
        ? aValue - bValue
        : bValue - aValue;
    });

    setFilteredPlayers(result);
  }, [players, filters]);

  const handleFilterChange = (filterName, value) => {
    setFilters(prev => ({
      ...prev,
      [filterName]: value
    }));
  };

  const handleSortChange = (sortBy) => {
    if (filters.sortBy === sortBy) {
      setFilters(prev => ({
        ...prev,
        sortOrder: prev.sortOrder === 'asc' ? 'desc' : 'asc'
      }));
    } else {
      setFilters(prev => ({
        ...prev,
        sortBy,
        sortOrder: 'desc'
      }));
    }
  };

  const handleResetFilters = () => {
    setFilters({
      sport: 'Всі',
      gender: 'Всі',
      search: '',
      sortBy: 'rating',
      sortOrder: 'desc'
    });
  };

  const getSportIcon = (sport) => {
    const icons = {
      'Футзал': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Шахи': '♟️',
      'Шашки': '♟️',
      'Настільний теніс': '🏓',
      'Бадмінтон': '🏸',
      'Армреслінг': '💪'
    };
    return icons[sport] || '🏆';
  };

  const getSportStats = (player) => {
    switch (player.sport) {
      case 'Футзал':
        return `${player.goals} голів`;
      case 'Баскетбол':
        return `${player.points} очок`;
      case 'Волейбол':
        return `${player.aces} ейсів`;
      case 'Шахи':
        return `${player.wins} перемог`;
      case 'Шашки':
        return `${player.wins} перемог`;
      default:
        return `${player.wins} перемог`;
    }
  };

  const exportToCSV = () => {
    const headers = ['Місце', 'Гравець', 'Спорт', 'Команда', 'Рейтинг', 'Ігри', 'Перемоги', '% перемог'];
    const csvContent = [
      headers.join(','),
      ...filteredPlayers.map((player, index) => [
        index + 1,
        `"${player.name}"`,
        player.sport,
        player.team,
        player.rating,
        player.games,
        player.wins,
        player.winRate
      ].join(','))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `рейтинг_гравців_${new Date().toLocaleDateString('uk-UA')}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  };

  const generateMockPlayers = () => {
    const names = [
      'Іванов Артем', 'Ковальчук Влад', 'Семенова Марія', 'Петренко Олексій',
      'Шевченко Катерина', 'Бондаренко Дмитро', 'Мельник Анна', 'Коваленко Сергій',
      'Ткаченко Ольга', 'Кравченко Андрій', 'Олійник Наталія', 'Шевчук Павло',
      'Поліщук Вікторія', 'Савенко Максим', 'Гриценко Юлія'
    ];
    
    const teams = ['ІТ-31', 'ФІ-22', 'ЕК-41', 'МК-31', 'ПР-21', 'ЕП-32', 'МТ-23', 'ХБ-42'];
    const sportsList = ['Футзал', 'Баскетбол', 'Волейбол', 'Шахи', 'Шашки', 'Настільний теніс'];
    const gendersList = ['Ч', 'Ж'];

    return Array.from({ length: 50 }, (_, index) => {
      const sport = sportsList[Math.floor(Math.random() * sportsList.length)];
      const gender = gendersList[Math.floor(Math.random() * gendersList.length)];
      const name = names[Math.floor(Math.random() * names.length)];
      const games = 10 + Math.floor(Math.random() * 30);
      const wins = Math.floor(Math.random() * games);
      const winRate = Math.round((wins / games) * 100);

      return {
        id: index + 1,
        rank: index + 1,
        name,
        sport,
        gender,
        team: teams[Math.floor(Math.random() * teams.length)],
        rating: 1500 + Math.floor(Math.random() * 700),
        games,
        wins,
        winRate: `${winRate}%`,
        goals: sport === 'Футзал' ? Math.floor(Math.random() * 30) : 0,
        points: sport === 'Баскетбол' ? Math.floor(Math.random() * 200) : 0,
        aces: sport === 'Волейбол' ? Math.floor(Math.random() * 20) : 0,
        ratingChange: Math.random() > 0.5 ? `+${Math.floor(Math.random() * 50)}` : `-${Math.floor(Math.random() * 30)}`,
        trend: Math.random() > 0.3 ? 'up' : Math.random() > 0.5 ? 'down' : 'stable',
        achievements: Math.random() > 0.7 ? ['Кращий гравець місяця'] : []
      };
    });
  };

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.spinner}></div>
        <p>Завантаження рейтингу...</p>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <main className={styles.main}>
        <div className={styles.container}>
          {/* Герой секція */}
          <section className={styles.heroSection}>
            <h1 className={styles.heroTitle}>
              <i className="fas fa-chart-line"></i>
              Рейтинг гравців
            </h1>
            <p className={styles.heroDescription}>
              Детальна статистика та рейтинг всіх гравців платформи. Фільтруйте, сортуйте та аналізуйте результати.
            </p>
            
            <div className={styles.statsOverview}>
              <div className={styles.statCard}>
                <div className={styles.statValue}>{players.length}</div>
                <div className={styles.statLabel}>Гравців у системі</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statValue}>{sports.length - 1}</div>
                <div className={styles.statLabel}>Видів спорту</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statValue}>
                  {Math.max(...players.map(p => p.rating))}
                </div>
                <div className={styles.statLabel}>Максимальний рейтинг</div>
              </div>
            </div>
          </section>

          {/* Фільтри та управління */}
          <section className={styles.filtersSection}>
            <div className={styles.filtersCard}>
              <div className={styles.filtersHeader}>
                <h2 className={styles.filtersTitle}>
                  <i className="fas fa-filter"></i>
                  Фільтри та сортування
                </h2>
                <div className={styles.actions}>
                  <button 
                    className={styles.resetButton}
                    onClick={handleResetFilters}
                  >
                    <i className="fas fa-redo"></i>
                    Скинути фільтри
                  </button>
                  <button 
                    className={styles.exportButton}
                    onClick={exportToCSV}
                  >
                    <i className="fas fa-file-export"></i>
                    Експорт CSV
                  </button>
                </div>
              </div>

              <div className={styles.filtersGrid}>
                {/* Пошук */}
                <div className={styles.filterGroup}>
                  <label className={styles.filterLabel}>
                    <i className="fas fa-search"></i>
                    Пошук гравця
                  </label>
                  <div className={styles.searchContainer}>
                    <input
                      type="text"
                      value={filters.search}
                      onChange={(e) => handleFilterChange('search', e.target.value)}
                      placeholder="Введіть ім'я або команду..."
                      className={styles.searchInput}
                    />
                    <i className="fas fa-search"></i>
                  </div>
                </div>

                {/* Вид спорту */}
                <div className={styles.filterGroup}>
                  <label className={styles.filterLabel}>
                    <i className="fas fa-futbol"></i>
                    Вид спорту
                  </label>
                  <div className={styles.chipsContainer}>
                    {sports.map((sport) => (
                      <button
                        key={sport}
                        className={`${styles.chip} ${filters.sport === sport ? styles.active : ''}`}
                        onClick={() => handleFilterChange('sport', sport)}
                      >
                        {sport === 'Всі' ? (
                          <i className="fas fa-list"></i>
                        ) : (
                          <span className={styles.chipIcon}>{getSportIcon(sport)}</span>
                        )}
                        {sport}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Стать */}
                <div className={styles.filterGroup}>
                  <label className={styles.filterLabel}>
                    <i className="fas fa-user-friends"></i>
                    Стать
                  </label>
                  <div className={styles.chipsContainer}>
                    {genders.map((gender) => (
                      <button
                        key={gender}
                        className={`${styles.chip} ${filters.gender === gender ? styles.active : ''}`}
                        onClick={() => handleFilterChange('gender', gender)}
                      >
                        {gender === 'Чоловіки' ? '👨' : gender === 'Жінки' ? '👩' : '👥'}
                        {gender}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Сортування */}
                <div className={styles.filterGroup}>
                  <label className={styles.filterLabel}>
                    <i className="fas fa-sort-amount-down"></i>
                    Сортування
                  </label>
                  <div className={styles.sortButtons}>
                    {sortOptions.map((option) => (
                      <button
                        key={option.value}
                        className={`${styles.sortButton} ${filters.sortBy === option.value ? styles.active : ''}`}
                        onClick={() => handleSortChange(option.value)}
                      >
                        {option.label}
                        {filters.sortBy === option.value && (
                          <i className={`fas fa-sort-${filters.sortOrder === 'asc' ? 'up' : 'down'}`}></i>
                        )}
                      </button>
                    ))}
                  </div>
                </div>
              </div>

              <div className={styles.resultsInfo}>
                <span>
                  Знайдено: <strong>{filteredPlayers.length}</strong> гравців
                </span>
                <span className={styles.resultsFilters}>
                  {filters.sport !== 'Всі' && `Спорт: ${filters.sport}`}
                  {filters.gender !== 'Всі' && ` • Стать: ${filters.gender}`}
                  {filters.search && ` • Пошук: "${filters.search}"`}
                </span>
              </div>
            </div>
          </section>

          {/* Таблиця рейтингу */}
          <section className={styles.ratingSection}>
            <div className={styles.ratingTable}>
              <div className={styles.tableHeader}>
                <div className={styles.headerCell}>#</div>
                <div className={styles.headerCell}>Гравець</div>
                <div className={styles.headerCell}>Спорт</div>
                <div className={styles.headerCell}>Команда</div>
                <div className={styles.headerCell}>Рейтинг</div>
                <div className={styles.headerCell}>Статистика</div>
                <div className={styles.headerCell}>% перемог</div>
                <div className={styles.headerCell}>Дії</div>
              </div>

              <div className={styles.tableBody}>
                {filteredPlayers.map((player, index) => (
                  <div key={player.id} className={styles.tableRow}>
                    <div className={styles.cell}>
                      <div className={`${styles.rank} ${index < 3 ? styles.topRank : ''}`}>
                        {index + 1}
                        {player.trend === 'up' && <i className={`fas fa-arrow-up ${styles.trendUp}`}></i>}
                        {player.trend === 'down' && <i className={`fas fa-arrow-down ${styles.trendDown}`}></i>}
                      </div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.playerInfo}>
                        <div 
                          className={styles.playerAvatar}
                          style={{ 
                            background: `linear-gradient(135deg, ${
                              player.gender === 'Ч' ? 'var(--primary)' : 'var(--accent)'
                            }, ${player.trend === 'up' ? 'var(--success)' : 'var(--warning)'})`
                          }}
                        >
                          {player.name.split(' ').map(n => n[0]).join('')}
                        </div>
                        <div>
                          <div className={styles.playerName}>{player.name}</div>
                          <div className={styles.playerGender}>
                            {player.gender === 'Ч' ? '👨' : '👩'}
                          </div>
                        </div>
                      </div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.sportBadge}>
                        <span className={styles.sportIcon}>{getSportIcon(player.sport)}</span>
                        {player.sport}
                      </div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.teamName}>{player.team}</div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.ratingContainer}>
                        <div className={styles.ratingValue}>{player.rating}</div>
                        <div className={`${styles.ratingChange} ${player.ratingChange.startsWith('+') ? styles.positive : styles.negative}`}>
                          {player.ratingChange}
                        </div>
                      </div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.statsContainer}>
                        <div className={styles.statsItem}>
                          <i className="fas fa-gamepad"></i>
                          {player.games} ігор
                        </div>
                        <div className={styles.statsItem}>
                          <i className="fas fa-trophy"></i>
                          {player.wins} перемог
                        </div>
                        <div className={styles.statsItem}>
                          {getSportStats(player)}
                        </div>
                      </div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.winRateContainer}>
                        <div className={styles.winRateValue}>{player.winRate}</div>
                        <div className={styles.winRateBar}>
                          <div 
                            className={styles.winRateFill}
                            style={{ width: player.winRate }}
                          ></div>
                        </div>
                      </div>
                    </div>

                    <div className={styles.cell}>
                      <div className={styles.actionsContainer}>
                        <button className={styles.actionButton} title="Профіль">
                          <i className="fas fa-user"></i>
                        </button>
                        <button className={styles.actionButton} title="Статистика">
                          <i className="fas fa-chart-bar"></i>
                        </button>
                        <button className={styles.actionButton} title="Порівняти">
                          <i className="fas fa-balance-scale"></i>
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Пагінація */}
            {filteredPlayers.length > 20 && (
              <div className={styles.pagination}>
                <button className={styles.paginationButton} disabled>
                  <i className="fas fa-chevron-left"></i>
                </button>
                <button className={`${styles.paginationButton} ${styles.active}`}>1</button>
                <button className={styles.paginationButton}>2</button>
                <button className={styles.paginationButton}>3</button>
                <span className={styles.paginationDots}>...</span>
                <button className={styles.paginationButton}>5</button>
                <button className={styles.paginationButton}>
                  <i className="fas fa-chevron-right"></i>
                </button>
              </div>
            )}

            {/* Інформація про рейтингову систему */}
            <div className={styles.ratingInfo}>
              <h3 className={styles.infoTitle}>
                <i className="fas fa-info-circle"></i>
                Про рейтингову систему
              </h3>
              <div className={styles.infoGrid}>
                <div className={styles.infoCard}>
                  <div className={styles.infoIcon}>🏆</div>
                  <h4>Elo система</h4>
                  <p>Рейтинг розраховується за системою Elo, що враховує силу суперників.</p>
                </div>
                <div className={styles.infoCard}>
                  <div className={styles.infoIcon}>📈</div>
                  <h4>Динаміка змін</h4>
                  <p>Зміна рейтингу після кожної гри залежить від результату та рейтингу суперника.</p>
                </div>
                <div className={styles.infoCard}>
                  <div className={styles.infoIcon}>⚖️</div>
                  <h4>Об\'єктивність</h4>
                  <p>Рейтинг є об'єктивним показником сили гравця та оновлюється після кожної гри.</p>
                </div>
              </div>
            </div>
          </section>
        </div>
      </main>
    </div>
  );
};

export default PlayersRatingPage;