import React, { useState, useEffect } from 'react';
import TournamentCard from './TournamentCard';
import TournamentFilters from './TournamentFilters';
import TournamentStats from './TournamentStats';
import { Link } from 'react-router-dom';
import { getTournaments } from '../../shared/api/tournaments.api';
import './Tournaments.css';


const Tournaments = () => {
  const [tournaments, setTournaments] = useState([]);
  const [filteredTournaments, setFilteredTournaments] = useState([]);
  const [activeStatus, setActiveStatus] = useState('all');
  const [activeSport, setActiveSport] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('date');
  const [viewMode, setViewMode] = useState('grid');

  // Ініціалізація даних турнірів
  useEffect(() => {
    const fetchTournaments = async () => {
      try {
        const data = await getTournaments();
        if (data && data.length > 0) {
          setTournaments(data);
          setFilteredTournaments(data);
        } else {
          setTournaments([]);
          setFilteredTournaments([]);
        }
      } catch (error) {
        console.error("Failed to fetch tournaments:", error);
      }
    };

    fetchTournaments();
  }, []);


  // Фільтрація та сортування турнірів
  useEffect(() => {
    let result = [...tournaments];

    // Фільтрація за статусом
    if (activeStatus !== 'all') {
      result = result.filter(tournament => tournament.status === activeStatus);
    }

    // Фільтрація за видом спорту
    if (activeSport !== 'all') {
      result = result.filter(tournament => tournament.sport === activeSport);
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
  }, [tournaments, activeStatus, activeSport, searchQuery, sortBy]);

  // Обробка зміни статусу
  const handleStatusChange = (status) => {
    setActiveStatus(status);
  };

  // Обробка зміни виду спорту
  const handleSportChange = (sport) => {
    setActiveSport(prev => prev === sport ? 'all' : sport);
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
        activeStatus={activeStatus}
        activeSport={activeSport}
        onStatusChange={handleStatusChange}
        onSportChange={handleSportChange}
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