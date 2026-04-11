import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthContext';
import {
  getTournamentById,
  getChessEventById,
  createChessPlayer,
  addParticipantToEvent,
  removeParticipantFromEvent,
  updateParticipantInEvent,
  createChessGame,
  updateChessGame,
  updateChessEvent,
  uploadTournamentPhoto,
  deleteTournamentPhoto,
  getAllChessPlayers,
  searchUsers,
  UserResponse
} from '@/shared/api';
import { Tournament } from '@/shared/types/tournament.types';
import { 
  ChessEventResponse, 
  ChessPlayerDto, 
  ChessGameDto, 
  GameResult, 
  ChessGameResponse, 
  ChessEventParticipantResponse, 
  ChessEventParticipantDto, 
  ChessEventRequest 
} from '@/shared/types/chess.types';
import { BASE_URL } from '@/shared/api/axiosInstance';
import styles from './TournamentViewPage.module.css';

const TournamentViewPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [tournament, setTournament] = useState<Tournament | null>(null);
  const [chessDetails, setChessDetails] = useState<ChessEventResponse | null>(null);
  const [allChessPlayers, setAllChessPlayers] = useState<ChessPlayerDto[]>([]);
  const [activeTab, setActiveTab] = useState<'standings' | 'games' | 'participants' | 'schedule'>('standings');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Admin access check (0 = User, 1 = Admin)
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';

  // Search Results type
  type SearchResult = {
    type: 'player' | 'user';
    id: number;
    userId?: number | null;
    firstName: string;
    lastName: string;
    email?: string;
    group?: string;
    rating?: number;
  };

  // Admin Form States
  const [newPlayer, setNewPlayer] = useState({
    firstName: '',
    lastName: '',
    group: '',
    city: 'Київ',
    ratingELO: 1500,
    fideId: '',
    title: '',
    userId: null as number | null,
    existingPlayerId: null as number | null
  });
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [newGame, setNewGame] = useState({ whitePlayerId: '', blackPlayerId: '', result: 'WhiteWin', round: 1, board: 1 });

  const [editingGameId, setEditingGameId] = useState<number | null>(null);
  const [editingGameData, setEditingGameData] = useState<{ round: number; board: number; result: string; whitePlayerId: string; blackPlayerId: string }>({ round: 1, board: 1, result: 'WhiteWin', whitePlayerId: '', blackPlayerId: '' });

  const [editingParticipantId, setEditingParticipantId] = useState<number | null>(null);
  const [editingParticipantData, setEditingParticipantData] = useState<{ points: number; rank: number }>({ points: 0, rank: 0 });
  const [gameFilters, setGameFilters] = useState({ round: '', board: '', participant: '', result: '' });

  const [isEditingTournament, setIsEditingTournament] = useState(false);
  const [editTournamentData, setEditTournamentData] = useState<ChessEventRequest>({
    name: '',
    date: '',
    location: '',
    refereeIds: [],
    level: 'CollegeGroup',
    description: '',
    timeControl: '',
    participantsCount: 0,
    system: 'Swiss',
    rules: '',
    prizes: '',
    headRefereeName: ''
  });

  const [isUploading, setIsUploading] = useState(false);

  const handleEditTournament = () => {
    if (chessDetails) {
      // Use the actual backend date which is likely ISO string
      let dateValue = '';
      if (chessDetails.date) {
        try {
          dateValue = new Date(chessDetails.date).toISOString().split('T')[0];
        } catch (e) {
          dateValue = chessDetails.date.split(' ')[0];
        }
      }

      setEditTournamentData({
        name: tournament?.title || chessDetails.name,
        date: dateValue,
        location: tournament?.location || chessDetails.location,
        refereeIds: chessDetails.refereeIds || [],
        level: chessDetails.level || 'CollegeGroup',
        description: tournament?.description || chessDetails.description,
        timeControl: chessDetails.timeControl || '',
        participantsCount: chessDetails.participantsCount || 0,
        system: chessDetails.system || 'Swiss',
        rules: chessDetails.rules || '',
        prizes: chessDetails.prizes || '',
        headRefereeName: chessDetails.headRefereeName || ''
      });
      setIsEditingTournament(true);
    }
  };

  const handlePhotoUpload = async (e: React.ChangeEvent<HTMLInputElement> | React.DragEvent) => {
    let file: File | undefined;
    
    if ('files' in e.target && e.target.files) {
      file = e.target.files[0];
    } else if ('dataTransfer' in e && e.dataTransfer.files) {
        e.preventDefault();
      file = e.dataTransfer.files[0];
    }

    if (!file) return;

    try {
      setIsUploading(true);
      await uploadTournamentPhoto(id!, file);
      fetchTournament();
    } catch (err) {
      console.error(err);
      alert("Помилка при завантаженні фото");
    } finally {
      setIsUploading(false);
    }
  };

  const handleDeletePhoto = async (photoId: number) => {
    if (!window.confirm("Ви дійсно хочете видалити це фото?")) return;
    try {
      await deleteTournamentPhoto(id!, photoId);
      fetchTournament();
    } catch (err) {
      console.error(err);
      alert("Помилка при видаленні фото");
    }
  };

  const handleSaveTournament = async () => {
    try {
      if (!editTournamentData.name || !editTournamentData.date) {
        alert("Назва та дата обов'язкові!");
        return;
      }
      await updateChessEvent(id!, editTournamentData);
      alert("Дані турніру успішно оновлено!");
      setIsEditingTournament(false);
      fetchTournament();
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні турніру");
    }
  };

  const handleEditClick = (game: ChessGameResponse) => {
    setEditingGameId(game.id);
    setEditingGameData({
      round: game.round || 1,
      board: game.board || 1,
      result: game.result,
      whitePlayerId: game.whitePlayerId ? game.whitePlayerId.toString() : '',
      blackPlayerId: game.blackPlayerId ? game.blackPlayerId.toString() : ''
    });
  };

  const handleEditParticipantClick = (p: ChessEventParticipantResponse) => {
    setEditingParticipantId(p.playerId);
    setEditingParticipantData({ points: p.points, rank: p.rank });
  };

  const handleSaveEdit = async (gameId: number) => {
    if (!editingGameData.whitePlayerId || !editingGameData.blackPlayerId) {
      alert("Оберіть обох супротивників!");
      return;
    }
    if (editingGameData.whitePlayerId === editingGameData.blackPlayerId) {
      alert("Гравець не може грати сам проти себе!");
      return;
    }

    try {
      const payload: ChessGameDto = {
        tournamentId: Number(id),
        whitePlayerId: editingGameData.whitePlayerId ? Number(editingGameData.whitePlayerId) : null,
        blackPlayerId: editingGameData.blackPlayerId ? Number(editingGameData.blackPlayerId) : null,
        result: editingGameData.result as GameResult,
        round: editingGameData.round,
        board: 1 // default UI mock
      };
      await updateChessGame(gameId, payload);
      alert("Матч успішно оновлено!");
      setEditingGameId(null);
      fetchTournament();
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні матчу");
    }
  };

  const handleCancelEdit = () => {
    setEditingGameId(null);
  };

  const handleCancelParticipantEdit = () => {
    setEditingParticipantId(null);
  };

  const handleSaveParticipantEdit = async (playerId: number) => {
    try {
      const payload: ChessEventParticipantDto = {
        playerId,
        points: editingParticipantData.points,
        rank: editingParticipantData.rank,
      };
      await updateParticipantInEvent(id!, playerId, payload);
      alert("Дані учасника оновлено!");
      setEditingParticipantId(null);
      fetchTournament();
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні учасника");
    }
  };

  const handleRemoveParticipant = async (playerId: number, playerName: string) => {
    if (window.confirm(`Ви впевнені, що хочете видалити учасника "${playerName}" з турніру?`)) {
      try {
        await removeParticipantFromEvent(id!, playerId);
        alert("Учасника видалено з турніру!");
        fetchTournament();
      } catch (err) {
        console.error(err);
        alert("Помилка при видаленні учасника");
      }
    }
  };

  const fetchTournament = async () => {
    if (!id) return;
    setIsLoading(true);
    try {
      const data = await getTournamentById(id);
      setTournament(data);

      if (data.sport === 'chess') {
        try {
          const [chessData, playersData] = await Promise.all([
            getChessEventById(id),
            getAllChessPlayers()
          ]);
          setChessDetails(chessData);
          setAllChessPlayers(playersData);
        } catch (chessErr) {
          console.error('Failed to fetch detailed chess info:', chessErr);
        }
      }
    } catch (err) {
      console.error('Failed to fetch tournament:', err);
      setError('Не вдалося завантажити інформацію про турнір.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchTournament();
  }, [id]);

  useEffect(() => {
    const delayDebounceFn = setTimeout(async () => {
      if (searchQuery.length >= 2) {
        setIsSearching(true);
        try {
          // 1. Search in existing ChessPlayers (local search for now, as we have allChessPlayers)
          const playerResults: SearchResult[] = allChessPlayers
            .filter(p =>
              (p.firstName + ' ' + p.lastName).toLowerCase().includes(searchQuery.toLowerCase()) ||
              p.group?.toLowerCase().includes(searchQuery.toLowerCase())
            )
            .map(p => ({
              type: 'player',
              id: p.id!,
              userId: p.userId,
              firstName: p.firstName,
              lastName: p.lastName,
              group: p.group,
              rating: p.ratingELO
            }));

          // 2. Search in Users (remote search)
          const users = await searchUsers(searchQuery);

          // Filter out users who are already in playerResults (to avoid duplicates)
          const userResults: SearchResult[] = users
            .filter(u => !playerResults.some(pr => pr.userId === u.id))
            .map(u => ({
              type: 'user',
              id: u.id,
              userId: u.id,
              firstName: u.firstName,
              lastName: u.lastName,
              email: u.email
            }));

          setSearchResults([...playerResults, ...userResults]);
        } catch (err) {
          console.error('Search failed:', err);
        } finally {
          setIsSearching(false);
        }
      } else {
        setSearchResults([]);
      }
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [searchQuery, allChessPlayers]);

  const handleSelectResult = (result: SearchResult) => {
    if (result.type === 'player') {
      setNewPlayer({
        ...newPlayer,
        firstName: result.firstName,
        lastName: result.lastName,
        group: result.group || '',
        ratingELO: result.rating || 1500,
        userId: result.userId || null,
        existingPlayerId: result.id
      });
    } else {
      setNewPlayer({
        ...newPlayer,
        firstName: result.firstName,
        lastName: result.lastName,
        userId: result.userId || null,
        existingPlayerId: null
      });
    }
    setSearchResults([]);
    setSearchQuery('');
  };

  const handleAddPlayer = async () => {
    if (!newPlayer.firstName || !newPlayer.lastName || !newPlayer.group) {
      alert("Заповніть ім'я, прізвище та групу користувача!");
      return;
    }
    try {
      let playerId = newPlayer.existingPlayerId;

      if (!playerId) {
        // Check if player is already in the tournament (by userId)
        if (newPlayer.userId && chessDetails?.players.some(p => p.userId === newPlayer.userId)) {
          alert("Цей користувач вже бере участь у турнірі!");
          return;
        }

        const playerPayload: ChessPlayerDto = {
          firstName: newPlayer.firstName,
          lastName: newPlayer.lastName,
          group: newPlayer.group,
          city: newPlayer.city,
          ratingELO: newPlayer.ratingELO,
          userId: newPlayer.userId,
          fideId: newPlayer.fideId.trim() || null,
          title: newPlayer.title.trim() || null
        };
        const created = await createChessPlayer(playerPayload);
        playerId = created.id!;
      } else {
        // Check if player is already in the tournament
        const isAlreadyParticipant = chessDetails?.players.some(p => p.playerId === playerId);
        if (isAlreadyParticipant) {
          alert("Цей гравець вже бере участь у турнірі!");
          return;
        }
      }

      // Create participant binding
      await addParticipantToEvent(id!, { playerId: playerId, points: 0, rank: 0 });
      alert("Учасник успішно доданий до турніру!");
      setNewPlayer({ firstName: '', lastName: '', group: '', city: 'Київ', ratingELO: 1500, fideId: '', title: '', userId: null, existingPlayerId: null });
      fetchTournament(); // Refresh
    } catch (err) {
      console.error(err);
      alert("Помилка при додаванні учасника");
    }
  };

  const handleAddGame = async () => {
    if (!newGame.whitePlayerId || !newGame.blackPlayerId) {
      alert("Оберіть обох супротивників!");
      return;
    }
    if (newGame.whitePlayerId === newGame.blackPlayerId) {
      alert("Гравець не може грати сам проти себе!");
      return;
    }
    try {
      const payload: ChessGameDto = {
        tournamentId: Number(id),
        whitePlayerId: newGame.whitePlayerId ? Number(newGame.whitePlayerId) : null,
        blackPlayerId: newGame.blackPlayerId ? Number(newGame.blackPlayerId) : null,
        result: newGame.result as GameResult,
        round: Number(newGame.round) || 1,
        board: Number(newGame.board) || 1
      };
      await createChessGame(payload);
      alert("Матч успішно доданий!");
      setNewGame({ ...newGame, result: 'WhiteWin', board: newGame.board + 1 });
      fetchTournament(); // Refresh
    } catch (err) {
      console.error(err);
      alert("Помилка при додаванні матчу");
    }
  };

  if (isLoading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
        <p>Завантаження інформації про турнір...</p>
      </div>
    );
  }

  if (error || !tournament) {
    return (
      <div className={styles.errorContainer}>
        <h2>Помилка</h2>
        <p>{error || 'Турнір не знайдено.'}</p>
        <button className="btn btn-primary" onClick={() => navigate('/tournaments')}>
          Повернутися до списку
        </button>
      </div>
    );
  }

  const sortedPlayers = chessDetails?.players?.sort((a, b) => {
    if (b.points !== a.points) return b.points - a.points;
    if (a.rank || b.rank) return (a.rank || 999) - (b.rank || 999);
    return b.buchholz - a.buchholz;
  }) || [];

  const getLevelBadgeParams = (level?: string) => {
    switch (level) {
      case 'CollegeGroup': return { label: 'Міжгрупові (коледж)', className: styles.levelCollege };
      case 'Regional': return { label: 'Обласні', className: styles.levelRegional };
      case 'National': return { label: 'Всеукраїнські', className: styles.levelNational };
      default: return { label: level || 'Невідомо', className: styles.levelCollege };
    }
  };

  const getSystemLabel = (system?: string) => {
    switch (system) {
      case 'Swiss': return 'Швейцарська система';
      case 'RoundRobin': return 'Кругова система';
      case 'Knockout': return 'Олімпійська система';
      default: return system || 'Невідома система';
    }
  };

  const getImageUrl = (url: string) => {
    if (!url) return '';
    if (url.startsWith('http')) return url;
    return `${BASE_URL}${url}`;
  };

  const badgeObj = getLevelBadgeParams(chessDetails?.level);
  const playedGames = chessDetails?.games?.filter(g => g.result !== 'NotPlayed').length || 0;
  const totalRounds = chessDetails?.games?.reduce((acc, game) => Math.max(acc, game.round), 0) || 0;
  const actualParticipants = tournament.sport === 'chess' 
    ? (chessDetails?.players?.length || 0) 
    : (tournament.participants || 0);

  const filteredGames = chessDetails?.games ? [...chessDetails.games].filter(g => {
    if (gameFilters.round && g.round.toString() !== gameFilters.round) return false;
    if (gameFilters.board && g.board?.toString() !== gameFilters.board) return false;
    if (gameFilters.participant) {
      const q = gameFilters.participant.toLowerCase();
      if (!g.whitePlayerName?.toLowerCase().includes(q) && !g.blackPlayerName?.toLowerCase().includes(q)) {
        return false;
      }
    }
    if (gameFilters.result && g.result !== gameFilters.result) return false;
    return true;
  }).sort((a, b) => (a.round === b.round ? a.board - b.board : a.round - b.round)) : [];

  return (
    <div className={styles.tournamentViewPage}>
      <div className={styles.container}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
          <button className={styles.backButton} onClick={() => navigate('/tournaments')}>
            <i className="fas fa-arrow-left"></i> Назад
          </button>
          {isAdmin && (
            <button onClick={handleEditTournament} style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 20px', backgroundColor: '#3b82f6', color: '#fff', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' }}>
              <i className="fas fa-edit"></i> Редагувати турнір
            </button>
          )}
        </div>

        <div className={styles.headerContent}>
          <div>
            <div className={styles.tournamentBadges}>
              {chessDetails?.level && (
                <span className={`${styles.levelBadge} ${badgeObj.className}`}>
                  <i className="fas fa-graduation-cap"></i> {badgeObj.label}
                </span>
              )}
              {chessDetails?.system && (
                <span className={styles.systemBadge}>
                  <i className="fas fa-chess-board"></i> {getSystemLabel(chessDetails.system)}
                </span>
              )}
            </div>
            <h1 className={styles.tournamentName}>{tournament.title}</h1>
            <div className={styles.tournamentMeta}>
              <div className={styles.metaItem}>
                <i className="fas fa-calendar-alt"></i>
                <span>{tournament.date}</span>
              </div>
              <div className={styles.metaItem}>
                <i className="fas fa-map-marker-alt"></i>
                <span>{tournament.location}</span>
              </div>
              <div className={styles.metaItem}>
                <i className="fas fa-users"></i>
                <span>{actualParticipants} учасників</span>
              </div>
              {chessDetails?.timeControl && (
                <div className={styles.metaItem}>
                  <i className="fas fa-clock"></i>
                  <span>Контроль часу: {chessDetails.timeControl}</span>
                </div>
              )}
            </div>
          </div>
          <div className={styles.tournamentImage}>
            {chessDetails?.photos && chessDetails.photos.length > 0 ? (
              <img src={getImageUrl(chessDetails.photos[0].url)} alt={tournament.title} />
            ) : (
              <i className="fas fa-chess-queen"></i>
            )}
          </div>
        </div>

        <div className={styles.infoCard}>
          <h2><i className="fas fa-info-circle"></i> Про турнір</h2>
          <div className={styles.infoGrid}>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-align-left"></i> Опис</h3>
              <p>{tournament.description}</p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-trophy"></i> Призи</h3>
              <p style={{ whiteSpace: 'pre-line' }}>
                {chessDetails?.prizes || 'Призи будуть оголошені пізніше.'}
              </p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-gavel"></i> Правила</h3>
              <p style={{ whiteSpace: 'pre-line' }}>
                {chessDetails?.rules ? chessDetails.rules : (
                  <>
                    • {getSystemLabel(chessDetails?.system)}<br />
                    • Контроль часу: {chessDetails?.timeControl || 'Не вказано'}<br />
                    • Заборонені підказки та мобільні телефони<br />
                    • Рішення судді остаточні
                  </>
                )}
              </p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-users"></i> Судді</h3>
              <div className={styles.refereeList}>
                <div className={styles.refereeItem}>
                  <div className={styles.refereeAvatar}>
                    <i className="fas fa-user-tie"></i>
                  </div>
                  <div>
                    <strong>{chessDetails?.headRefereeName || 'Головний суддя'}</strong><br />
                    <small>{chessDetails?.headRefereeName ? 'Офіційний суддя турніру' : 'Призначається організатором'}</small>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {tournament.sport === 'chess' && (
          <>
            <div className={styles.statsGrid}>
              <div className={styles.statCard}>
                <div className={styles.statIcon}><i className="fas fa-chess-board"></i></div>
                <div className={styles.statValue}>{totalRounds || '-'}</div>
                <div className={styles.statLabel}>Всього турів</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statIcon}><i className="fas fa-users"></i></div>
                <div className={styles.statValue}>{actualParticipants}</div>
                <div className={styles.statLabel}>Учасників</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statIcon}><i className="fas fa-clock"></i></div>
                <div className={styles.statValue}>{chessDetails?.timeControl || '-'}</div>
                <div className={styles.statLabel}>Контроль часу</div>
              </div>
              <div className={styles.statCard}>
                <div className={styles.statIcon}><i className="fas fa-chart-line"></i></div>
                <div className={styles.statValue}>{playedGames}</div>
                <div className={styles.statLabel}>Зіграно партій</div>
              </div>
            </div>

            <div className={styles.tabsContainer}>
              <div className={styles.tabsHeader}>
                <button
                  className={`${styles.tabButton} ${activeTab === 'standings' ? styles.active : ''}`}
                  onClick={() => setActiveTab('standings')}
                >
                  <i className="fas fa-chart-simple"></i> Турнірна таблиця
                </button>
                <button
                  className={`${styles.tabButton} ${activeTab === 'games' ? styles.active : ''}`}
                  onClick={() => setActiveTab('games')}
                >
                  <i className="fas fa-chess-board"></i> Всі партії
                </button>
                <button
                  className={`${styles.tabButton} ${activeTab === 'participants' ? styles.active : ''}`}
                  onClick={() => setActiveTab('participants')}
                >
                  <i className="fas fa-users"></i> Учасники
                </button>
                <button
                  className={`${styles.tabButton} ${activeTab === 'schedule' ? styles.active : ''}`}
                  onClick={() => setActiveTab('schedule')}
                >
                  <i className="fas fa-calendar-week"></i> Розклад турів
                </button>
              </div>

              {activeTab === 'standings' && (
                <div className={styles.tabContent}>
                  <div className={styles.tableWrapper}>
                    <table className={styles.dataTable}>
                      <thead>
                        <tr>
                          <th>Місце</th>
                          <th>Учасник</th>
                          <th>Рейтинг</th>
                          <th>Очки</th>
                          <th>Бухгольц</th>
                          {isAdmin && <th>Дії</th>}
                        </tr>
                      </thead>
                      <tbody>
                        {sortedPlayers.length > 0 ? sortedPlayers.map((p, idx) => (
                          <tr key={p.id}>
                            {editingParticipantId === p.playerId ? (
                              <>
                                <td>
                                  <input type="number" value={editingParticipantData.rank} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, rank: parseInt(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                </td>
                                <td>
                                  {p.userId ? (
                                    <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                      {p.playerName}
                                    </Link>
                                  ) : (
                                    <span style={{ fontWeight: 'bold' }}>{p.playerName}</span>
                                  )}
                                </td>
                                <td>{p.rating}</td>
                                <td>
                                  <input type="number" step="0.5" value={editingParticipantData.points} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, points: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                </td>
                                <td>{p.buchholz}</td>
                                <td>
                                  <button onClick={() => handleSaveParticipantEdit(p.playerId)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }}><i className="fas fa-check"></i></button>
                                  <button onClick={handleCancelParticipantEdit} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}><i className="fas fa-times"></i></button>
                                </td>
                              </>
                            ) : (
                              <>
                                <td>{p.rank || idx + 1}</td>
                                <td>
                                  {p.userId ? (
                                    <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                      {p.playerName}
                                    </Link>
                                  ) : (
                                    <strong>{p.playerName}</strong>
                                  )}
                                </td>
                                <td>{p.rating}</td>
                                <td><strong>{p.points}</strong></td>
                                <td>{p.buchholz}</td>
                                {isAdmin && <td>
                                  <button onClick={() => handleEditParticipantClick(p)} style={{ padding: '4px 8px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }} title="Редагувати"><i className="fas fa-edit"></i></button>
                                  <button onClick={() => handleRemoveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Видалити"><i className="fas fa-trash"></i></button>
                                </td>}
                              </>
                            )}
                          </tr>
                        )) : (
                          <tr><td colSpan={6}>Ще немає даних</td></tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {activeTab === 'games' && (
                <div className={styles.tabContent}>
                  {isAdmin && (
                    <div style={{ background: '#f5f5f5', padding: '16px', borderRadius: '12px', marginBottom: '24px' }}>
                      <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>[Admin] Додати партію (результат)</h3>
                      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '16px', alignItems: 'center' }}>
                        <input type="number" min="1" placeholder="Тур" title="Номер туру" value={newGame.round} onChange={(e) => setNewGame({ ...newGame, round: parseInt(e.target.value) || 1 })} style={{ width: '60px', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        <input type="number" min="1" placeholder="Дошка" title="Номер дошки" value={newGame.board} onChange={(e) => setNewGame({ ...newGame, board: parseInt(e.target.value) || 1 })} style={{ width: '60px', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />

                        <select value={newGame.whitePlayerId} onChange={(e) => setNewGame({ ...newGame, whitePlayerId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                          <option value="">-- Білі --</option>
                          {sortedPlayers.map(p => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                        </select>

                        <strong style={{ margin: '0 8px' }}> - </strong>

                        <select value={newGame.blackPlayerId} onChange={(e) => setNewGame({ ...newGame, blackPlayerId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                          <option value="">-- Чорні --</option>
                          {sortedPlayers.map(p => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                        </select>

                        <select value={newGame.result} onChange={(e) => setNewGame({ ...newGame, result: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                          <option value="WhiteWin">Перемога Білих (1-0)</option>
                          <option value="BlackWin">Перемога Чорних (0-1)</option>
                          <option value="Draw">Нічия (½-½)</option>
                        </select>

                        <button onClick={handleAddGame} style={{ padding: '8px 16px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}>Зберегти</button>
                      </div>
                    </div>
                  )}

                  <div style={{ display: 'flex', gap: '16px', marginBottom: '16px', flexWrap: 'wrap', padding: '16px', background: '#f9fafb', borderRadius: '12px' }}>
                    <div style={{ display: 'flex', flexDirection: 'column' }}>
                      <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Тур</label>
                      <input type="number" placeholder="Усі" value={gameFilters.round} onChange={(e) => setGameFilters({ ...gameFilters, round: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', width: '80px' }} />
                    </div>
                    <div style={{ display: 'flex', flexDirection: 'column' }}>
                      <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Дошка</label>
                      <input type="number" placeholder="Усі" value={gameFilters.board} onChange={(e) => setGameFilters({ ...gameFilters, board: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', width: '80px' }} />
                    </div>
                    <div style={{ display: 'flex', flexDirection: 'column' }}>
                      <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Учасник (ім'я)</label>
                      <input type="text" placeholder="Пошук..." value={gameFilters.participant} onChange={(e) => setGameFilters({ ...gameFilters, participant: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', minWidth: '200px' }} />
                    </div>
                    <div style={{ display: 'flex', flexDirection: 'column' }}>
                      <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Результат</label>
                      <select value={gameFilters.result} onChange={(e) => setGameFilters({ ...gameFilters, result: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                        <option value="">Усі результати</option>
                        <option value="WhiteWin">Перемога Білих (1-0)</option>
                        <option value="BlackWin">Перемога Чорних (0-1)</option>
                        <option value="Draw">Нічия (½-½)</option>
                      </select>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'flex-end' }}>
                      <button 
                        onClick={() => setGameFilters({ round: '', board: '', participant: '', result: '' })} 
                        style={{ padding: '8px 16px', background: '#6b7280', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
                      >
                        Скинути
                      </button>
                    </div>
                  </div>

                  <div className={styles.tableWrapper}>
                    <table className={styles.dataTable}>
                      <thead>
                        <tr>
                          <th>Тур</th>
                          <th>Дошка</th>
                          <th>Білі</th>
                          <th>Чорні</th>
                          <th>Результат</th>
                          {isAdmin && <th>Дії</th>}
                        </tr>
                      </thead>
                      <tbody>
                        {filteredGames.length > 0 ? (
                          filteredGames.map((game, idx) => (
                            <tr key={game.id}>
                              {editingGameId === game.id ? (
                                <>
                                  <td>
                                    <input
                                      type="number"
                                      min="1"
                                      value={editingGameData.round}
                                      onChange={(e) => setEditingGameData({ ...editingGameData, round: parseInt(e.target.value) || 1 })}
                                      style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}
                                    />
                                  </td>
                                  <td>
                                    <input
                                      type="number"
                                      min="1"
                                      value={editingGameData.board}
                                      onChange={(e) => setEditingGameData({ ...editingGameData, board: parseInt(e.target.value) || 1 })}
                                      style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}
                                    />
                                  </td>
                                  <td>
                                    <select value={editingGameData.whitePlayerId} onChange={(e) => setEditingGameData({ ...editingGameData, whitePlayerId: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                      <option value="">-- Білі --</option>
                                      {sortedPlayers.map(p => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                                    </select>
                                  </td>
                                  <td>
                                    <select value={editingGameData.blackPlayerId} onChange={(e) => setEditingGameData({ ...editingGameData, blackPlayerId: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                      <option value="">-- Чорні --</option>
                                      {sortedPlayers.map(p => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                                    </select>
                                  </td>
                                  <td>
                                    <select value={editingGameData.result} onChange={(e) => setEditingGameData({ ...editingGameData, result: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                      <option value="WhiteWin">Перемога Білих (1-0)</option>
                                      <option value="BlackWin">Перемога Чорних (0-1)</option>
                                      <option value="Draw">Нічия (½-½)</option>
                                    </select>
                                  </td>
                                  <td>
                                    <button onClick={() => handleSaveEdit(game.id)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }}><i className="fas fa-check"></i></button>
                                    <button onClick={handleCancelEdit} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}><i className="fas fa-times"></i></button>
                                  </td>
                                </>
                              ) : (
                                <>
                                  <td>{game.round}</td>
                                  <td>{game.board || idx + 1}</td>
                                  <td>
                                    {game.whiteUserId ? (
                                      <Link to={`/profile/${game.whiteUserId}`} style={{ color: '#3b82f6', textDecoration: 'none' }}>
                                        {game.whitePlayerName}
                                      </Link>
                                    ) : (
                                      game.whitePlayerName || '—'
                                    )}
                                  </td>
                                  <td>
                                    {game.blackUserId ? (
                                      <Link to={`/profile/${game.blackUserId}`} style={{ color: '#3b82f6', textDecoration: 'none' }}>
                                        {game.blackPlayerName}
                                      </Link>
                                    ) : (
                                      game.blackPlayerName || '—'
                                    )}
                                  </td>
                                  <td>
                                    {game.result === 'WhiteWin' ? (
                                      <span className={`${styles.resultBadge} ${styles.resultWhiteWin}`}>
                                        <i className="fas fa-chess-king"></i> 1-0
                                      </span>
                                    ) : game.result === 'BlackWin' ? (
                                      <span className={`${styles.resultBadge} ${styles.resultBlackWin}`}>
                                        <i className="fas fa-chess-queen"></i> 0-1
                                      </span>
                                    ) : game.result === 'Draw' ? (
                                      <span className={`${styles.resultBadge} ${styles.resultDraw}`}>
                                        <i className="fas fa-handshake"></i> ½-½
                                      </span>
                                    ) : (
                                      <span className={`${styles.resultBadge} ${styles.resultNotPlayed}`}>
                                        <i className="fas fa-clock"></i> Очікується
                                      </span>
                                    )}
                                  </td>
                                  {isAdmin && (
                                    <td>
                                      <button onClick={() => handleEditClick(game)} style={{ padding: '4px 8px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
                                        <i className="fas fa-edit"></i>
                                      </button>
                                    </td>
                                  )}
                                </>
                              )}
                            </tr>
                          ))
                        ) : (
                          <tr><td colSpan={isAdmin ? 6 : 5} style={{ textAlign: 'center', padding: '24px', color: '#666' }}>Партії не знайдені за вказаними фільтрами</td></tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {activeTab === 'participants' && (
                <div className={styles.tabContent}>
                  {isAdmin && (
                    <div style={{ background: '#f5f5f5', padding: '24px', borderRadius: '12px', marginBottom: '24px' }}>
                      <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>[Admin] Додати учасника</h3>

                      <div style={{ marginBottom: '20px', position: 'relative' }}>
                        <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold' }}>Пошук гравця або користувача:</label>
                        <input
                          type="text"
                          placeholder="Введіть ім'я, прізвище або email для пошуку в базі..."
                          value={searchQuery}
                          onChange={(e) => setSearchQuery(e.target.value)}
                          style={{ width: '100%', padding: '10px', borderRadius: '6px', border: '1px solid #ccc' }}
                        />
                        {isSearching && <div style={{ marginTop: '8px' }}>Пошук...</div>}
                        {searchResults.length > 0 && (
                          <div style={{
                            position: 'absolute',
                            top: '100%',
                            left: 0,
                            right: 0,
                            background: '#fff',
                            border: '1px solid #ccc',
                            borderRadius: '6px',
                            zIndex: 10,
                            maxHeight: '200px',
                            overflowY: 'auto',
                            boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
                          }}>
                            {searchResults.map((result, idx) => (
                              <div
                                key={`${result.type}-${result.id}-${idx}`}
                                onClick={() => handleSelectResult(result)}
                                style={{ padding: '10px', cursor: 'pointer', borderBottom: '1px solid #eee' }}
                                onMouseOver={(e) => (e.currentTarget.style.background = '#f0f0f0')}
                                onMouseOut={(e) => (e.currentTarget.style.background = '#fff')}
                              >
                                <span style={{
                                  fontSize: '10px',
                                  background: result.type === 'player' ? '#10b981' : '#3b82f6',
                                  color: '#fff',
                                  padding: '2px 6px',
                                  borderRadius: '4px',
                                  marginRight: '8px',
                                  textTransform: 'uppercase'
                                }}>
                                  {result.type === 'player' ? 'Гравець' : 'Користувач'}
                                </span>
                                <strong>{result.firstName} {result.lastName}</strong>
                                {result.email && <span style={{ color: '#666', marginLeft: '8px' }}>({result.email})</span>}
                                {result.group && <span style={{ color: '#666', marginLeft: '8px' }}>[{result.group}]</span>}
                              </div>
                            ))}
                          </div>
                        )}
                      </div>

                      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginBottom: '16px' }}>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ marginBottom: '4px', fontSize: '12px' }}>Ім'я</label>
                          <input type="text" placeholder="Ім'я" value={newPlayer.firstName} onChange={(e) => setNewPlayer({ ...newPlayer, firstName: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ marginBottom: '4px', fontSize: '12px' }}>Прізвище</label>
                          <input type="text" placeholder="Прізвище" value={newPlayer.lastName} onChange={(e) => setNewPlayer({ ...newPlayer, lastName: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ marginBottom: '4px', fontSize: '12px' }}>Група (напр. КН-31)</label>
                          <input type="text" placeholder="Група (напр. КН-31)" value={newPlayer.group} onChange={(e) => setNewPlayer({ ...newPlayer, group: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ marginBottom: '4px', fontSize: '12px' }}>Рейтинг ELO</label>
                          <input type="number" placeholder="Рейтинг ELO" value={newPlayer.ratingELO} onChange={(e) => setNewPlayer({ ...newPlayer, ratingELO: parseInt(e.target.value) || 1500 })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ marginBottom: '4px', fontSize: '12px' }}>FIDE ID (опціонально)</label>
                          <input type="text" placeholder="FIDE ID (опціонально)" value={newPlayer.fideId} onChange={(e) => setNewPlayer({ ...newPlayer, fideId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ marginBottom: '4px', fontSize: '12px' }}>Звання (напр. CM, FM, IM, GM) (опціонально)</label>
                          <input type="text" placeholder="Звання (напр. CM, FM, IM, GM) (опціонально)" value={newPlayer.title} onChange={(e) => setNewPlayer({ ...newPlayer, title: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                        </div>
                      </div>
                      <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                        <button onClick={handleAddPlayer} style={{ padding: '10px 24px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}>
                          {newPlayer.existingPlayerId ? 'Додати існуючого гравця' : newPlayer.userId ? 'Створити профіль гравця та додати' : 'Створити гостя та додати'}
                        </button>
                        {(newPlayer.userId || newPlayer.existingPlayerId) && (
                          <button
                            onClick={() => setNewPlayer({ ...newPlayer, userId: null, existingPlayerId: null, firstName: '', lastName: '', group: '' })}
                            style={{ padding: '10px 24px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
                          >
                            Скинути вибір
                          </button>
                        )}
                      </div>
                    </div>
                  )}

                  <div className={styles.tableWrapper}>
                    <table className={styles.dataTable}>
                      <thead>
                        <tr>
                          <th>№</th>
                          <th>Учасник</th>
                          <th>Рейтинг ELO</th>
                          <th>Титул</th>
                          <th>FIDE ID</th>
                          {isAdmin && <th>Дії</th>}
                        </tr>
                      </thead>
                      <tbody>
                        {sortedPlayers.length > 0 ? sortedPlayers.map((p, idx) => (
                          <tr key={p.id}>
                            <td>{idx + 1}</td>
                            <td>
                              {p.userId ? (
                                <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                  {p.playerName}
                                </Link>
                              ) : (
                                p.playerName
                              )}
                            </td>
                            <td>{p.rating}</td>
                            <td>
                              {p.title ? (
                                <span className={styles.systemBadge} style={{ fontWeight: 'bold' }}>{p.title}</span>
                              ) : '—'}
                            </td>
                            <td>{p.fideId || '—'}</td>
                            {isAdmin && (
                              <td>
                                <button onClick={() => handleRemoveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Видалити">
                                  <i className="fas fa-trash"></i>
                                </button>
                              </td>
                            )}
                          </tr>
                        )) : (
                          <tr><td colSpan={isAdmin ? 6 : 5}>Учасники не знайдені</td></tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {activeTab === 'schedule' && (
                <div className={styles.tabContent}>
                  <div className={styles.infoGrid}>
                    <div className={styles.infoSection}>
                      <h3><i className="fas fa-calendar-day"></i> Розклад</h3>
                      <p>Турнір проводиться згідно із розкладом, вказаним організаторами під час реєстрації або під час відкриття.</p>
                      <br />
                      <p>
                        <strong>Примірний розклад:</strong><br />
                        <strong>10:00</strong> - Реєстрація учасників<br />
                        <strong>11:00</strong> - Жеребкування та відкриття<br />
                        <strong>Далі:</strong> за календарем турів
                      </p>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </>
        )}
      </div>

      {isEditingTournament && (
        <div className={styles.modalOverlay}>
          <div className={styles.modalContent}>
            <div className={styles.modalHeader}>
              <h2 style={{ margin: 0, fontSize: '1.5rem', display: 'flex', alignItems: 'center', gap: '12px' }}>
                <i className="fas fa-edit" style={{ color: '#3b82f6' }}></i>
                Редагувати турнір
              </h2>
              <button onClick={() => setIsEditingTournament(false)} className={styles.closeButton}>&times;</button>
            </div>
            <div className={styles.modalBody}>
              <div className={styles.editFormGrid}>
                <div className={styles.formGroup}>
                  <label>Назва турніру</label>
                  <input type="text" value={editTournamentData.name} onChange={(e) => setEditTournamentData({ ...editTournamentData, name: e.target.value })} />
                </div>
                <div className={styles.formGroup}>
                  <label>Дата проведення</label>
                  <input type="date" value={editTournamentData.date} onChange={(e) => setEditTournamentData({ ...editTournamentData, date: e.target.value })} />
                </div>
                <div className={styles.formGroup}>
                  <label>Локація</label>
                  <input type="text" value={editTournamentData.location} onChange={(e) => setEditTournamentData({ ...editTournamentData, location: e.target.value })} />
                </div>
                <div className={styles.formGroup}>
                  <label>Рівень змагань</label>
                  <select value={editTournamentData.level} onChange={(e) => setEditTournamentData({ ...editTournamentData, level: e.target.value as CompetitionLevel })}>
                    <option value="CollegeGroup">Міжгрупові (коледж)</option>
                    <option value="Regional">Обласні</option>
                    <option value="National">Всеукраїнські</option>
                  </select>
                </div>
                <div className={styles.formGroup}>
                  <label>Система проведення</label>
                  <select value={editTournamentData.system} onChange={(e) => setEditTournamentData({ ...editTournamentData, system: e.target.value as ChessSystem })}>
                    <option value="Swiss">Швейцарська система</option>
                    <option value="RoundRobin">Кругова система</option>
                    <option value="Knockout">Олімпійська система</option>
                  </select>
                </div>
                <div className={styles.formGroup}>
                  <label>Контроль часу</label>
                  <input type="text" placeholder="напр. 10хв + 5с" value={editTournamentData.timeControl} onChange={(e) => setEditTournamentData({ ...editTournamentData, timeControl: e.target.value })} />
                </div>
                <div className={styles.formGroup}>
                  <label>Головний суддя</label>
                  <input type="text" placeholder="ПІБ судді" value={editTournamentData.headRefereeName} onChange={(e) => setEditTournamentData({ ...editTournamentData, headRefereeName: e.target.value })} />
                </div>
                <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                  <label>Опис турніру</label>
                  <textarea rows={3} value={editTournamentData.description} onChange={(e) => setEditTournamentData({ ...editTournamentData, description: e.target.value })} />
                </div>
                <div className={styles.formGroup}>
                  <label>Правила</label>
                  <textarea rows={5} placeholder="Введіть правила турніру..." value={editTournamentData.rules} onChange={(e) => setEditTournamentData({ ...editTournamentData, rules: e.target.value })} />
                </div>
                <div className={styles.formGroup}>
                  <label>Призи</label>
                  <textarea rows={5} placeholder="Опишіть призовий фонд..." value={editTournamentData.prizes} onChange={(e) => setEditTournamentData({ ...editTournamentData, prizes: e.target.value })} />
                </div>

                <div className={styles.photoManagementSection} style={{ gridColumn: '1 / -1' }}>
                  <label><i className="fas fa-images"></i> Фотографії турніру</label>
                  
                  <div className={styles.photoUploadArea}
                       onDragOver={(e) => e.preventDefault()}
                       onDrop={handlePhotoUpload}>
                    {isUploading ? (
                      <div className={styles.uploadSpinner}><i className="fas fa-circle-notch fa-spin"></i> Завантаження...</div>
                    ) : (
                      <div className={styles.uploadPlaceholder}>
                        <i className="fas fa-cloud-upload-alt"></i>
                        <p>Перетягніть фото сюди або <span>виберіть файл</span></p>
                        <input type="file" accept="image/*" onChange={handlePhotoUpload} className={styles.fileInput} />
                      </div>
                    )}
                  </div>

                  {chessDetails?.photos && chessDetails.photos.length > 0 && (
                    <div className={styles.currentPhotosGrid}>
                      {chessDetails.photos.map(photo => (
                        <div key={photo.id} className={styles.photoPreviewCard}>
                          <img src={getImageUrl(photo.url)} alt="Tournament" />
                          <button onClick={() => handleDeletePhoto(photo.id)} className={styles.deletePhotoBtn}>
                            <i className="fas fa-times"></i>
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            </div>
            <div className={styles.modalFooter}>
              <button onClick={() => setIsEditingTournament(false)} className={styles.cancelBtn}>Скасувати</button>
              <button onClick={handleSaveTournament} className={styles.saveBtn}>Зберегти зміни</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TournamentViewPage;
