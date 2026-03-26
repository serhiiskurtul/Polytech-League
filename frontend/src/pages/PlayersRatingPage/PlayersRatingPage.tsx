import { useState, useEffect } from 'react';
import styles from './PlayersRatingPage.module.css';

interface Player {
  id: number;
  name: string;
  avatar: string;
  sport: string;
  team: string;
  rating: number;
  games: number;
  wins: number;
  winRate: number;
  gender: 'Ч' | 'Ж';
  rank: number;
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
  const [filters, setFilters] = useState<Filters>({
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
    const sportsList = ['Футзал', 'Баскетбол', 'Волейбол', 'Шахи', 'Настільний теніс'];
    const teamsList = ['ІТ-31', 'ЕК-22', 'ПР-11', 'МК-41', 'ФІ-21', 'ЕП-32'];
    const names = [
      'Артем Іванов', 'Марія Семенова', 'Влад Ковальчук', 'Катерина Шевченко', 
      'Олексій Петренко', 'Юлія Мороз', 'Дмитро Соколов', 'Анна Бойко',
      'Сергій Бондар', 'Олена Кравченко', 'Ігор Лисенко', 'Тетяна Руденко'
    ];

    return Array.from({ length: 30 }, (_, i): Player => {
      const wins = Math.floor(Math.random() * 20);
      const games = wins + Math.floor(Math.random() * 10);
      return {
        id: i + 1,
        name: names[i % names.length],
        avatar: names[i % names.length].split(' ').map(n => n[0]).join(''),
        sport: sportsList[Math.floor(Math.random() * sportsList.length)],
        team: teamsList[Math.floor(Math.random() * teamsList.length)],
        rating: 1200 + Math.floor(Math.random() * 1000),
        games,
        wins,
        winRate: games > 0 ? Math.round((wins / games) * 100) : 0,
        gender: Math.random() > 0.4 ? 'Ч' : 'Ж',
        rank: 0
      };
    }).sort((a, b) => b.rating - a.rating).map((p, i) => ({ ...p, rank: i + 1 }));
  };

  const handleFilterChange = (key: keyof Filters, value: any) => {
    setFilters(prev => ({ ...prev, [key]: value }));
  };

  return (
    <div className={styles.ratingContainer}>
      <div className={styles.header}>
        <h1 className={styles.title}>
          <i className="fas fa-chart-line"></i>
          Рейтинг гравців
        </h1>
        <p className={styles.subtitle}>Кращі спортсмени коледжу за результатами змагань</p>
      </div>

      <div className={styles.filtersSection}>
        <div className={styles.searchBar}>
          <i className="fas fa-search"></i>
          <input 
            type="text" 
            placeholder="Пошук гравця або команди..." 
            value={filters.search}
            onChange={(e) => handleFilterChange('search', e.target.value)}
          />
        </div>

        <div className={styles.filterControls}>
          <div className={styles.filterGroup}>
            <label>Вид спорту:</label>
            <select 
              value={filters.sport}
              onChange={(e) => handleFilterChange('sport', e.target.value)}
            >
              {sports.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>

          <div className={styles.filterGroup}>
            <label>Стать:</label>
            <select 
              value={filters.gender}
              onChange={(e) => handleFilterChange('gender', e.target.value)}
            >
              {genders.map(g => <option key={g} value={g}>{g}</option>)}
            </select>
          </div>

          <div className={styles.filterGroup}>
            <label>Сортувати за:</label>
            <select 
              value={filters.sortBy}
              onChange={(e) => handleFilterChange('sortBy', e.target.value as keyof Player)}
            >
              {sortOptions.map(opt => <option key={opt.value} value={opt.value}>{opt.label}</option>)}
            </select>
          </div>

          <button 
            className={styles.sortOrderBtn}
            onClick={() => handleFilterChange('sortOrder', filters.sortOrder === 'asc' ? 'desc' : 'asc')}
          >
            <i className={`fas fa-sort-amount-${filters.sortOrder === 'asc' ? 'up' : 'down'}`}></i>
          </button>
        </div>
      </div>

      <div className={styles.ratingTable}>
        {loading ? (
          <div className={styles.loader}>Завантаження...</div>
        ) : (
          <div className={styles.tableWrapper}>
            <table>
              <thead>
                <tr>
                  <th className={styles.rankCol}>#</th>
                  <th>Гравець</th>
                  <th>Команда</th>
                  <th>Вид спорту</th>
                  <th className={styles.statsCol}>Ігри</th>
                  <th className={styles.statsCol}>Перемоги</th>
                  <th className={styles.statsCol}>%</th>
                  <th className={styles.ratingCol}>Рейтинг</th>
                </tr>
              </thead>
              <tbody>
                {filteredPlayers.map((player) => (
                  <tr key={player.id} className={player.rank <= 3 ? styles[`topRow${player.rank}`] : ''}>
                    <td className={styles.rankCell}>
                      {player.rank <= 3 ? (
                        <i className={`fas fa-crown ${styles.crownIcon}`}></i>
                      ) : player.rank}
                    </td>
                    <td className={styles.playerCell}>
                      <div className={styles.playerInfo}>
                        <div className={styles.avatar}>{player.avatar}</div>
                        <span>{player.name}</span>
                      </div>
                    </td>
                    <td>{player.team}</td>
                    <td>{player.sport}</td>
                    <td className={styles.statsCell}>{player.games}</td>
                    <td className={styles.statsCell}>{player.wins}</td>
                    <td className={styles.statsCell}>{player.winRate}%</td>
                    <td className={styles.ratingCell}>
                      <span className={styles.ratingBadge}>{player.rating}</span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {filteredPlayers.length === 0 && (
              <div className={styles.noResults}>Гравців не знайдено</div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default PlayersRatingPage;
