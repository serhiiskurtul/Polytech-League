import { useState, useEffect, useRef } from 'react';
import { useNavigate, useSearchParams, useLocation } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthContext';
import TournamentCard from './TournamentCard';
import TournamentFilters from './TournamentFilters';
import TournamentStats from './TournamentStats';
import { getTournaments } from '@/shared/api';
import './Tournaments.css';
import { Tournament } from '@/shared/types/tournament.types';

// Extend the base Tournament type with UI-specific fields if needed, 
// or use the base type if it's sufficient.
// Based on the code, it seems the UI expects more fields like 'title', 'sport', 'participants', etc.
// I'll define a local interface that matches the usage in Tournaments.jsx

interface TournamentUI extends Tournament {
  progress?: number;
  currentRound?: string;
}

const Tournaments = () => {
  const location = useLocation();
  const containerRef = useRef<HTMLDivElement>(null);
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const sportFromUrl = searchParams.get('sport');
  const { user } = useAuth();
  
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';
  
  const [tournaments, setTournaments] = useState<TournamentUI[]>([]);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 0
  });
  const [activeStatus, setActiveStatus] = useState('all');
  const [activeSport, setActiveSport] = useState(sportFromUrl || 'all');
  const [activeLevel, setActiveLevel] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');

  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [isLoading, setIsLoading] = useState(false);

  // Фільтрація та сортування турнірів (тепер працює над отриманими даними)
  // В ідеалі це має бути на бекенді, але поки що залишимо так для сумісності з існуючим кодом
  const getFilteredTournaments = () => {
    let result = [...tournaments];

    // Фільтрація за пошуком
    if (searchQuery.trim() !== '') {
      const query = searchQuery.toLowerCase();
      result = result.filter(tournament =>
        tournament.title.toLowerCase().includes(query) ||
        (tournament.description?.toLowerCase().includes(query)) ||
        tournament.sport.toLowerCase().includes(query)
      );
    }

    // Сортування (видалено за запитом користувача)
    /*
    result.sort((a, b) => {
      switch (sortBy) {
        case 'date':
          return new Date(b.date.split('.').reverse().join('-')).getTime() -
            new Date(a.date.split('.').reverse().join('-')).getTime();
        case 'participants':
          return b.participants - a.participants;
        case 'title':
          return a.title.localeCompare(b.title);
        default:
          return 0;
      }
    });
    */

    return result;
  };

  const filteredTournaments = getFilteredTournaments();

  // Обробка зміни статусу
  const handleStatusChange = (status: string) => {
    setActiveStatus(status);
    setPagination(prev => ({ ...prev, page: 1 }));
  };

  // Обробка зміни виду спорту
  const handleSportChange = (sport: string) => {
    setActiveSport(prev => prev === sport ? 'all' : sport);
    setPagination(prev => ({ ...prev, page: 1 }));
  };

  const handleLevelChange = (level: string) => {
    setActiveLevel(level);
    setPagination(prev => ({ ...prev, page: 1 }));
  };


  // Обробка пошуку
  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(e.target.value);
    setPagination(prev => ({ ...prev, page: 1 }));
  };



  // Ініціалізація даних турнірів
  const fetchTournamentsData = async (page: number) => {
    setIsLoading(true);
    try {
      const response = await getTournaments(page, pagination.pageSize, activeStatus, activeSport, activeLevel);
      setTournaments(response.items as TournamentUI[]);

      setPagination(prev => ({
        ...prev,
        page: response.page,
        totalCount: response.totalCount,
        totalPages: response.totalPages
      }));
    } catch (error) {
      console.error("Failed to fetch tournaments:", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (sportFromUrl) {
      setActiveSport(sportFromUrl);
    } else {
      setActiveSport('all');
    }
    setPagination(prev => ({ ...prev, page: 1 }));
  }, [sportFromUrl]);

  useEffect(() => {
    fetchTournamentsData(pagination.page);
  }, [pagination.page, activeStatus, activeSport, activeLevel]);

  const prevPageRef = useRef(pagination.page);
  useEffect(() => {
    if (prevPageRef.current !== pagination.page) {
      if (containerRef.current) {
        containerRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
      prevPageRef.current = pagination.page;
    }
  }, [pagination.page]);

  useEffect(() => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }, [location.pathname, location.search]);


  const handleResetFilters = () => {
    setActiveStatus('all');
    setActiveSport('all');
    setActiveLevel('all');
    setSearchQuery('');

    setPagination(prev => ({ ...prev, page: 1 }));
  };

  // Обробка перемикання сторінок
  const handlePageChange = (newPage: number) => {
    if (newPage >= 1 && newPage <= pagination.totalPages) {
      setPagination(prev => ({ ...prev, page: newPage }));
    }
  };

  const stats = {
    total: pagination.totalCount,
    ongoing: tournaments.filter(t => t.status === 'ongoing').length, // Це лише для поточної сторінки, в ідеалі теж з бекенду
    upcoming: tournaments.filter(t => t.status === 'upcoming').length,
    totalParticipants: tournaments.reduce((acc, t) => acc + t.participants, 0)
  };

  const handleRegister = (id: string) => {
    alert(`Реєстрація на змагання з ID: ${id}`);
  };

  const handleDetails = (id: string) => {
    navigate(`/tournaments/${id}`);
  };

  return (
    <div className="tournaments-page">
      <header className="page-header">
        <div className="header-left">
          <h1 className="page-title">Змагання</h1>
          <p className="page-subtitle">Відкривайте нові можливості та змагайтесь за першість</p>
        </div>
        {isAdmin && (
          <button className="btn btn-primary create-tournament-btn" onClick={() => navigate('/create-tournament')}>
            <i className="fas fa-plus"></i> Створити
          </button>
        )}
      </header>

      <TournamentStats stats={stats} isAdmin={isAdmin} />


      <TournamentFilters 
        activeStatus={activeStatus}
        activeSport={activeSport}
        activeLevel={activeLevel}
        onStatusChange={handleStatusChange}
        onSportChange={handleSportChange}
        onLevelChange={handleLevelChange}
      />


      <div className="tournaments-controls" ref={containerRef}>
        <div className="search-container">
          <i className="fas fa-search search-icon"></i>
          <input 
            type="text" 
            className="search-input" 
            placeholder="Пошук за назвою або описом..." 
            value={searchQuery}
            onChange={handleSearch}
          />
        </div>

        <div className="controls-right">
          {(activeStatus !== 'all' || activeSport !== 'all' || activeLevel !== 'all' || searchQuery !== '') && (
            <button className="btn btn-outline reset-filters-btn" onClick={handleResetFilters}>

              <i className="fas fa-times"></i> Скинути
            </button>
          )}
          


          <div className="view-toggle">
            <button 
              className={`view-btn ${viewMode === 'grid' ? 'active' : ''}`}
              onClick={() => setViewMode('grid')}
              title="Сітка"
            >
              <i className="fas fa-th-large"></i>
            </button>
            <button 
              className={`view-btn ${viewMode === 'list' ? 'active' : ''}`}
              onClick={() => setViewMode('list')}
              title="Список"
            >
              <i className="fas fa-list"></i>
            </button>
          </div>
        </div>
      </div>

      <div className={`tournaments-container ${viewMode}`}>
        {filteredTournaments.length > 0 ? (
          filteredTournaments.map(tournament => (
            <TournamentCard 
              key={tournament.id} 
              tournament={tournament} 
              viewMode={viewMode}
              onRegister={() => handleRegister(tournament.id)}
              onDetails={() => handleDetails(tournament.id)}
            />
          ))
        ) : (
          <div className="no-results">
            <i className="fas fa-search"></i>
            <h3>Нічого не знайдено</h3>
            <p>Спробуйте змінити параметри пошуку або фільтри</p>
            <button className="btn btn-primary" onClick={handleResetFilters}>
              Скинути всі фільтри
            </button>
          </div>
        )}
      </div>

      {pagination.totalPages > 1 && (
        <div className="pagination">
          <button 
            className="pagination-btn"
            disabled={pagination.page === 1}
            onClick={() => handlePageChange(pagination.page - 1)}
            title="Попередня"
          >
            <i className="fas fa-chevron-left"></i>
          </button>
          
          <div className="pagination-info">
            Сторінка {pagination.page} з {pagination.totalPages}
          </div>
          
          <button 
            className="pagination-btn"
            disabled={pagination.page === pagination.totalPages}
            onClick={() => handlePageChange(pagination.page + 1)}
            title="Наступна"
          >
            <i className="fas fa-chevron-right"></i>
          </button>
        </div>
      )}
    </div>
  );
};

export default Tournaments;
