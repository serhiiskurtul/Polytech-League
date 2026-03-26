import { useState, useEffect } from 'react';
import TournamentCard from './TournamentCard';
import TournamentFilters from './TournamentFilters';
import TournamentStats from './TournamentStats';
import { getTournaments } from '../../shared/api/tournaments.api';
import './Tournaments.css';
import { Tournament } from '../../shared/types/tournament.types';

// Extend the base Tournament type with UI-specific fields if needed, 
// or use the base type if it's sufficient.
// Based on the code, it seems the UI expects more fields like 'title', 'sport', 'participants', etc.
// I'll define a local interface that matches the usage in Tournaments.jsx

interface TournamentUI extends Tournament {
  title: string;
  sport: string;
  participants: number;
  featured?: boolean;
  currentRound?: string;
  progress?: number;
  date: string; // The UI expects a date string like "15.04.2025 - 20.04.2025"
}

const Tournaments = () => {
  const [tournaments, setTournaments] = useState<TournamentUI[]>([]);
  const [filteredTournaments, setFilteredTournaments] = useState<TournamentUI[]>([]);
  const [activeStatus, setActiveStatus] = useState('all');
  const [activeSport, setActiveSport] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState('date');
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');

  // Ініціалізація даних турнірів
  useEffect(() => {
    const fetchTournaments = async () => {
      try {
        const data = await getTournaments() as any[]; // Cast to any for now to allow mapping
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
        (tournament.description?.toLowerCase().includes(query)) ||
        tournament.sport.toLowerCase().includes(query)
      );
    }

    // Сортування
    result.sort((a, b) => {
      switch (sortBy) {
        case 'date':
          return new Date(a.date.split(' - ')[0].split('.').reverse().join('-')).getTime() -
            new Date(b.date.split(' - ')[0].split('.').reverse().join('-')).getTime();
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
  const handleStatusChange = (status: string) => {
    setActiveStatus(status);
  };

  // Обробка зміни виду спорту
  const handleSportChange = (sport: string) => {
    setActiveSport(prev => prev === sport ? 'all' : sport);
  };

  // Обробка пошуку
  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(e.target.value);
  };

  // Обробка зміни сортування
  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSortBy(e.target.value);
  };

  const stats = {
    total: tournaments.length,
    ongoing: tournaments.filter(t => t.status === 'ongoing').length,
    upcoming: tournaments.filter(t => t.status === 'upcoming').length,
    totalParticipants: tournaments.reduce((acc, t) => acc + t.participants, 0),
    registeredCount: 0 // Mock value
  };

  return (
    <div className="tournaments-container">
      <div className="tournaments-header">
        <div className="header-content">
          <h1>Турніри</h1>
          <p>Беріть участь у змаганнях та перемагайте</p>
        </div>
      </div>

      <TournamentStats stats={stats} />

      <div className="tournaments-main">
        <aside className="tournaments-sidebar">
          <TournamentFilters 
            activeStatus={activeStatus}
            activeSport={activeSport}
            onStatusChange={handleStatusChange}
            onSportChange={handleSportChange}
          />
        </aside>

        <main className="tournaments-content">
          <div className="content-toolbar">
            <div className="search-box">
              <i className="fas fa-search"></i>
              <input 
                type="text" 
                placeholder="Пошук за назвою або описом..." 
                value={searchQuery}
                onChange={handleSearch}
              />
            </div>

            <div className="toolbar-actions">
              <div className="sort-box">
                <label>Сортувати:</label>
                <select value={sortBy} onChange={handleSortChange}>
                  <option value="date">За датою</option>
                  <option value="participants">За учасниками</option>
                  <option value="title">За назвою</option>
                </select>
              </div>

              <div className="view-toggle">
                <button 
                  className={`toggle-btn ${viewMode === 'grid' ? 'active' : ''}`}
                  onClick={() => setViewMode('grid')}
                  title="Сітка"
                >
                  <i className="fas fa-th-large"></i>
                </button>
                <button 
                  className={`toggle-btn ${viewMode === 'list' ? 'active' : ''}`}
                  onClick={() => setViewMode('list')}
                  title="Список"
                >
                  <i className="fas fa-list"></i>
                </button>
              </div>
            </div>
          </div>

          <div className={`tournaments-${viewMode}`}>
            {filteredTournaments.length > 0 ? (
              filteredTournaments.map(tournament => (
                <TournamentCard 
                  key={tournament.id} 
                  tournament={tournament} 
                  viewMode={viewMode}
                  onRegister={() => alert(`Реєстрація на турнір "${tournament.title}"`)}
                />
              ))
            ) : (
              <div className="no-results">
                <i className="fas fa-search"></i>
                <p>Нічого не знайдено за вашим запитом</p>
                <button onClick={() => {
                  setActiveStatus('all');
                  setActiveSport('all');
                  setSearchQuery('');
                }}>
                  Скинути всі фільтри
                </button>
              </div>
            )}
          </div>
        </main>
      </div>
    </div>
  );
};

export default Tournaments;
