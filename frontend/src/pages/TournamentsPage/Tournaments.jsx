import React, { useState, useEffect } from 'react';
import TournamentCard from './TournamentCard';
import TournamentFilters from './TournamentFilters';
import TournamentStats from './TournamentStats';
import { Link } from 'react-router-dom';
import './Tournaments.css';

const Tournaments = () => {
  const [tournaments, setTournaments] = useState([]);
  const [filteredTournaments, setFilteredTournaments] = useState([]);
  const [activeFilter, setActiveFilter] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('date');
  const [viewMode, setViewMode] = useState('grid');

  // Ініціалізація даних турнірів
  useEffect(() => {
    const tournamentsData = [
      {
        id: 1,
        title: 'Чемпіонат коледжу з футзалу 2025/26',
        sport: 'football',
        status: 'ongoing',
        date: '15.12.2025 - 20.01.2026',
        location: 'Спортивний зал №1',
        description: 'Щорічний чемпіонат коледжу з футзалу. Участь беруть 16 команд з різних факультетів.',
        participants: 16,
        teams: 16,
        prize: 'Кубок декана та грошовий приз',
        organizer: 'Спортвідділ',
        registrationEnd: '10.12.2025',
        currentRound: 'Груповий етап',
        registered: true,
        featured: true
      },
      {
        id: 2,
        title: 'Кубок декана: баскетбол',
        sport: 'basketball',
        status: 'upcoming',
        date: '18.12.2025 - 22.12.2025',
        location: 'Спортивний зал №2',
        description: 'Відкритий кубок з баскетболу. Формат: 5х5, чоловіки та жінки окремо.',
        participants: 48,
        teams: 12,
        prize: 'Медалі та сертифікати',
        organizer: 'Факультет інформаційних технологій',
        registrationEnd: '15.12.2025',
        currentRound: 'Реєстрація',
        registered: false,
        featured: true
      },
      {
        id: 3,
        title: 'Волейбольний турнір пам\'яті проф. Іванова',
        sport: 'volleyball',
        status: 'ongoing',
        date: '10.12.2025 - 28.12.2025',
        location: 'Спортивний зал №1',
        description: 'Традиційний турнір пам\'яті видатного викладача. Змішані команди 6х6.',
        participants: 72,
        teams: 12,
        prize: 'Перехідний кубок',
        organizer: 'Асоціація випускників',
        registrationEnd: '05.12.2025',
        currentRound: 'Чвертьфінали',
        registered: true,
        featured: false
      },
      {
        id: 4,
        title: 'Шаховий чемпіонат коледжу',
        sport: 'chess',
        status: 'upcoming',
        date: '20.12.2025',
        location: 'Бібліотека, зал №4',
        description: 'Індивідуальний чемпіонат з шахів за швейцарською системою. 9 турів.',
        participants: 32,
        teams: 0,
        prize: 'Грошовий приз + рейтинг Elo',
        organizer: 'Шаховий клуб',
        registrationEnd: '18.12.2025',
        currentRound: 'Реєстрація',
        registered: false,
        featured: false
      },
      {
        id: 5,
        title: 'Турнір з настільного тенісу',
        sport: 'table-tennis',
        status: 'upcoming',
        date: '18.12.2025',
        location: 'Спортивний зал №3',
        description: 'Відкритий турнір з настільного тенісу. Одиночний та парний розряди.',
        participants: 24,
        teams: 0,
        prize: 'Подарункові сертифікати',
        organizer: 'Студентська рада',
        registrationEnd: '16.12.2025',
        currentRound: 'Реєстрація',
        registered: true,
        featured: false
      },
      {
        id: 6,
        title: 'Новорічний турнір з бадмінтону',
        sport: 'badminton',
        status: 'upcoming',
        date: '27.12.2025',
        location: 'Спортивний зал №3',
        description: 'Новорічний турнір з бадмінтону. Одиночний та змішаний парний розряди.',
        participants: 20,
        teams: 0,
        prize: 'Новорічні подарунки',
        organizer: 'Спортклуб',
        registrationEnd: '25.12.2025',
        currentRound: 'Реєстрація',
        registered: false,
        featured: false
      },
      {
        id: 7,
        title: 'Турнір з армреслінгу',
        sport: 'armwrestling',
        status: 'completed',
        date: '05.12.2025',
        location: 'Актовий зал',
        description: 'Турнір з армреслінгу серед студентів. Вагові категорії: до 70кг, до 85кг, понад 85кг.',
        participants: 18,
        teams: 0,
        prize: 'Медалі та сертифікати',
        organizer: 'Спортвідділ',
        registrationEnd: '01.12.2025',
        currentRound: 'Завершено',
        registered: false,
        featured: false
      },
      {
        id: 8,
        title: 'Футзал: Кубок весни',
        sport: 'football',
        status: 'upcoming',
        date: '15.01.2026 - 30.01.2026',
        location: 'Спортивний зал №1',
        description: 'Традиційний кубок весни з футзалу. Формат: груповий етап + плей-оф.',
        participants: 64,
        teams: 16,
        prize: 'Кубок та медалі',
        organizer: 'Студентська рада',
        registrationEnd: '10.01.2026',
        currentRound: 'Анонсовано',
        registered: false,
        featured: true
      }
    ];

    setTournaments(tournamentsData);
    setFilteredTournaments(tournamentsData);
  }, []);

  // Фільтрація та сортування турнірів
  useEffect(() => {
    let result = [...tournaments];

    // Фільтрація за статусом
    if (activeFilter !== 'all') {
      result = result.filter(tournament => tournament.status === activeFilter);
    }

    // Фільтрація за пошуком
    if (searchQuery.trim() !== '') {
      const query = searchQuery.toLowerCase();
      result = result.filter(tournament => 
        tournament.title.toLowerCase().includes(query) ||
        tournament.description.toLowerCase().includes(query) ||
        tournament.sport.toLowerCase().includes(query)
      );
    }

    // Сортування
    result.sort((a, b) => {
      switch (sortBy) {
        case 'date':
          return new Date(a.date.split(' - ')[0].split('.').reverse().join('-')) - 
                 new Date(b.date.split(' - ')[0].split('.').reverse().join('-'));
        case 'participants':
          return b.participants - a.participants;
        case 'title':
          return a.title.localeCompare(b.title);
        default:
          return 0;
      }
    });

    setFilteredTournaments(result);
  }, [tournaments, activeFilter, searchQuery, sortBy]);

  // Обробка зміни фільтра
  const handleFilterChange = (filter) => {
    setActiveFilter(filter);
  };

  // Обробка пошуку
  const handleSearch = (e) => {
    setSearchQuery(e.target.value);
  };

  // Обробка зміни сортування
  const handleSortChange = (e) => {
    setSortBy(e.target.value);
  };

  // Обробка зміни режиму перегляду
  const handleViewModeChange = (mode) => {
    setViewMode(mode);
  };

  // Реєстрація на турнір
  const handleRegister = (tournamentId) => {
    setTournaments(prev => prev.map(tournament => 
      tournament.id === tournamentId 
        ? { ...tournament, registered: !tournament.registered } 
        : tournament
    ));
  };

  // Отримати статистику
  const getStats = () => {
    const ongoing = tournaments.filter(t => t.status === 'ongoing').length;
    const upcoming = tournaments.filter(t => t.status === 'upcoming').length;
    const completed = tournaments.filter(t => t.status === 'completed').length;
    const totalParticipants = tournaments.reduce((sum, t) => sum + t.participants, 0);
    const registeredCount = tournaments.filter(t => t.registered).length;

    return {
      total: tournaments.length,
      ongoing,
      upcoming,
      completed,
      totalParticipants,
      registeredCount
    };
  };

  return (
    <div className="tournaments-page">
      {/* Заголовок сторінки */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Турніри</h1>
          <p className="page-subtitle">Усі спортивні змагання та турніри коледжу</p>
        </div>
        
        <button className="btn btn-primary create-tournament-btn">
          <Link to="/create-tournament">
            <i className="fas fa-plus-circle"></i> Створити турнір
          </Link> 
        </button>
      </div>

      {/* Статистика */}
      <TournamentStats stats={getStats()} />

      {/* Панель управління */}
      <div className="tournaments-controls">
        <div className="search-container">
          <i className="fas fa-search"></i>
          <input
            type="text"
            placeholder="Пошук турнірів..."
            value={searchQuery}
            onChange={handleSearch}
            className="search-input"
          />
        </div>

        <div className="controls-right">
          <select 
            value={sortBy} 
            onChange={handleSortChange}
            className="sort-select"
          >
            <option value="date">За датою</option>
            <option value="participants">За кількістю учасників</option>
            <option value="title">За назвою</option>
          </select>

          <div className="view-toggle">
            <button 
              className={`view-btn ${viewMode === 'grid' ? 'active' : ''}`}
              onClick={() => handleViewModeChange('grid')}
              title="Сітка"
            >
              <i className="fas fa-th-large"></i>
            </button>
            <button 
              className={`view-btn ${viewMode === 'list' ? 'active' : ''}`}
              onClick={() => handleViewModeChange('list')}
              title="Список"
            >
              <i className="fas fa-list"></i>
            </button>
          </div>
        </div>
      </div>

      {/* Фільтри */}
      <TournamentFilters 
        activeFilter={activeFilter}
        onFilterChange={handleFilterChange}
      />

      {/* Список турнірів */}
      <div className={`tournaments-container ${viewMode}`}>
        {filteredTournaments.length === 0 ? (
          <div className="no-results">
            <i className="fas fa-trophy"></i>
            <h3>Турніри не знайдені</h3>
            <p>Спробуйте змінити параметри пошуку або фільтрації</p>
          </div>
        ) : (
          filteredTournaments.map(tournament => (
            <TournamentCard
              key={tournament.id}
              tournament={tournament}
              viewMode={viewMode}
              onRegister={handleRegister}
            />
          ))
        )}
      </div>

      {/* Пагінація (якщо потрібно) */}
      {filteredTournaments.length > 0 && (
        <div className="pagination">
          <button className="pagination-btn" disabled>
            <i className="fas fa-chevron-left"></i>
          </button>
          <span className="pagination-info">1-{filteredTournaments.length} з {tournaments.length}</span>
          <button className="pagination-btn">
            <i className="fas fa-chevron-right"></i>
          </button>
        </div>
      )}
    </div>
  );
};

export default Tournaments;