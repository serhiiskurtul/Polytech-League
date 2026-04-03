import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getTournamentById } from '../../shared/api/tournaments.api';
import { getChessEventById, createChessPlayer, addParticipantToEvent, createChessGame, getAllChessPlayers } from '../../shared/api/chess.api';
import { Tournament } from '../../shared/types/tournament.types';
import { ChessEventResponse, ChessPlayerDto, ChessGameDto, GameResult } from '../../shared/types/chess.types';
import styles from './TournamentViewPage.module.css';

const TournamentViewPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [tournament, setTournament] = useState<Tournament | null>(null);
  const [chessDetails, setChessDetails] = useState<ChessEventResponse | null>(null);
  const [allChessPlayers, setAllChessPlayers] = useState<ChessPlayerDto[]>([]);
  const [activeTab, setActiveTab] = useState<'standings' | 'games' | 'participants' | 'schedule'>('standings');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Admin access mocked (e.g. true for demonstration, in reality use global Auth context)
  const [isAdmin, setIsAdmin] = useState(true);

  // Admin Form States
  const [newPlayer, setNewPlayer] = useState({ firstName: '', lastName: '', group: '', city: 'Київ', ratingELO: 1500, fideId: '', title: '' });
  const [selectedExistingPlayerId, setSelectedExistingPlayerId] = useState<string>('');
  const [newGame, setNewGame] = useState({ whitePlayerId: '', blackPlayerId: '', result: 'NotPlayed', round: 1 });

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

  const handleAddPlayer = async () => {
    if (!newPlayer.firstName || !newPlayer.lastName || !newPlayer.group) {
        alert("Заповніть ім'я, прізвище та групу користувача!");
        return;
    }
    try {
      const { ...playerData } = newPlayer;
      const playerPayload: ChessPlayerDto = { 
        ...playerData,
        fideId: newPlayer.fideId.trim() || null,
        title: newPlayer.title.trim() || null
      };
      const created = await createChessPlayer(playerPayload);
      if (created.id) {
         // Create participant binding
         await addParticipantToEvent(id!, { playerId: created.id, points: 0, rank: 0 });
         alert("Гравець успішно створений та доданий до турніру!");
         setNewPlayer({ firstName: '', lastName: '', group: '', city: 'Київ', ratingELO: 1500, fideId: '', title: '' });
         fetchTournament(); // Refresh
      }
    } catch (err) {
      console.error(err);
      alert("Помилка при додаванні учасника");
    }
  };

  const handleAddExistingPlayer = async () => {
    if (!selectedExistingPlayerId) {
      alert("Оберіть гравця зі списку!");
      return;
    }
    
    // Check if player is already in the tournament
    const isAlreadyParticipant = chessDetails?.players.some(p => p.playerId === Number(selectedExistingPlayerId));
    if (isAlreadyParticipant) {
      alert("Цей гравець вже бере участь у турнірі!");
      return;
    }

    try {
      await addParticipantToEvent(id!, { 
        playerId: Number(selectedExistingPlayerId), 
        points: 0, 
        rank: 0 
      });
      alert("Гравець успішно доданий до турніру!");
      setSelectedExistingPlayerId('');
      fetchTournament();
    } catch (err) {
      console.error(err);
      alert("Помилка при додаванні існуючого учасника");
    }
  };

  const handleAddGame = async () => {
    if (!newGame.whitePlayerId && !newGame.blackPlayerId) {
        alert("Супротивники не обрані!");
        return;
    }
    try {
      const payload: ChessGameDto = {
        tournamentId: Number(id),
        whitePlayerId: newGame.whitePlayerId ? Number(newGame.whitePlayerId) : null,
        blackPlayerId: newGame.blackPlayerId ? Number(newGame.blackPlayerId) : null,
        result: newGame.result as GameResult,
        round: Number(newGame.round) || 1,
        board: 1 // default UI mock
      };
      await createChessGame(payload);
      alert("Матч успішно доданий!");
      setNewGame({ ...newGame, result: 'NotPlayed' });
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

  const sortedPlayers = chessDetails?.players?.sort((a, b) => b.points - a.points || b.buchholz - a.buchholz) || [];
  
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

  const badgeObj = getLevelBadgeParams(chessDetails?.level);
  const playedGames = chessDetails?.games?.filter(g => g.result !== 'NotPlayed').length || 0;
  const totalRounds = chessDetails?.games?.reduce((acc, game) => Math.max(acc, game.roundNumber), 0) || 0;

  return (
    <div className={styles.tournamentViewPage}>
      <div className={styles.container}>
        <button className={styles.backButton} onClick={() => navigate('/tournaments')}>
          <i className="fas fa-arrow-left"></i> Назад
        </button>

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
                <span>{tournament.participants} учасників</span>
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
            <i className="fas fa-chess-queen"></i>
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
              <p>
                🥇 1 місце - Кубок та медаль<br/>
                🥈 2 місце - Медаль<br/>
                🥉 3 місце - Медаль<br/>
                Спеціальні призи від організаторів
              </p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-gavel"></i> Правила</h3>
              <p>
                • {getSystemLabel(chessDetails?.system)}<br/>
                • Контроль часу: {chessDetails?.timeControl || 'Не вказано'}<br/>
                • Заборонені підказки та мобільні телефони<br/>
                • Рішення судді остаточні
              </p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-users"></i> Судді</h3>
              <div className={styles.refereeList}>
                <div className={styles.refereeItem}>
                  <div className={styles.refereeAvatar}>
                    <i className="fas fa-user"></i>
                  </div>
                  <div>
                    <strong>Головний суддя</strong><br/>
                    <small>Призначено організатором</small>
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
                <div className={styles.statValue}>{tournament.participants || '-'}</div>
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
                        </tr>
                      </thead>
                      <tbody>
                        {sortedPlayers.length > 0 ? sortedPlayers.map((p, idx) => (
                          <tr key={p.id}>
                            <td>{idx + 1}</td>
                            <td>
                              <strong>{p.playerName}</strong>
                            </td>
                            <td>{p.rating}</td>
                            <td><strong>{p.points}</strong></td>
                            <td>{p.buchholz}</td>
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
                         <input type="number" min="1" placeholder="Номер туру" value={newGame.round} onChange={(e) => setNewGame({...newGame, round: parseInt(e.target.value) || 1})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                         
                         <select value={newGame.whitePlayerId} onChange={(e) => setNewGame({...newGame, whitePlayerId: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                           <option value="">-- Білі --</option>
                           {sortedPlayers.map(p => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                         </select>
                         
                         <strong style={{ margin: '0 8px' }}> - </strong>

                         <select value={newGame.blackPlayerId} onChange={(e) => setNewGame({...newGame, blackPlayerId: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                           <option value="">-- Чорні --</option>
                           {sortedPlayers.map(p => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                         </select>

                         <select value={newGame.result} onChange={(e) => setNewGame({...newGame, result: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                           <option value="NotPlayed">Очікується (*-*)</option>
                           <option value="WhiteWin">Перемога Білих (1-0)</option>
                           <option value="BlackWin">Перемога Чорних (0-1)</option>
                           <option value="Draw">Нічия (½-½)</option>
                         </select>
                         
                         <button onClick={handleAddGame} style={{ padding: '8px 16px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}>Зберегти</button>
                       </div>
                     </div>
                  )}

                  <div className={styles.tableWrapper}>
                    <table className={styles.dataTable}>
                      <thead>
                        <tr>
                          <th>Тур</th>
                          <th>Дошка</th>
                          <th>Білі</th>
                          <th>Чорні</th>
                          <th>Результат</th>
                        </tr>
                      </thead>
                      <tbody>
                        {chessDetails?.games && chessDetails.games.length > 0 ? (
                          [...chessDetails.games].sort((a,b) => a.roundNumber - b.roundNumber).map((game, idx) => (
                            <tr key={game.id}>
                              <td>{game.roundNumber}</td>
                              <td>{idx + 1}</td>
                              <td>{game.whitePlayerName || '—'}</td>
                              <td>{game.blackPlayerName || '—'}</td>
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
                            </tr>
                          ))
                        ) : (
                          <tr><td colSpan={5}>Матчі ще не сформовані</td></tr>
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
                      <div style={{ marginBottom: '24px', borderBottom: '1px solid #ddd', paddingBottom: '16px' }}>
                        <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>[Admin] Додати Існуючого Гравця</h3>
                        <div style={{ display: 'flex', gap: '16px', alignItems: 'center' }}>
                          <select 
                            value={selectedExistingPlayerId} 
                            onChange={(e) => setSelectedExistingPlayerId(e.target.value)}
                            style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', flex: 1 }}
                          >
                            <option value="">-- Оберіть гравця зі списку --</option>
                            {allChessPlayers
                              .filter(ap => !chessDetails?.players.some(p => p.playerId === ap.id))
                              .map(player => (
                                <option key={player.id} value={player.id}>
                                  {player.firstName} {player.lastName} ({player.group}) - ELO: {player.ratingELO}
                                </option>
                              ))
                            }
                          </select>
                          <button 
                            onClick={handleAddExistingPlayer}
                            style={{ padding: '8px 24px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
                          >
                            Додати до турніру
                          </button>
                        </div>
                      </div>

                      <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>[Admin] Створити та Зареєструвати Нового Гравця</h3>
                      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginBottom: '16px' }}>
                        <input type="text" placeholder="Ім'я" value={newPlayer.firstName} onChange={(e) => setNewPlayer({...newPlayer, firstName: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                        <input type="text" placeholder="Прізвище" value={newPlayer.lastName} onChange={(e) => setNewPlayer({...newPlayer, lastName: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                        <input type="text" placeholder="Група (напр. КН-31)" value={newPlayer.group} onChange={(e) => setNewPlayer({...newPlayer, group: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                        <input type="number" placeholder="Рейтинг ELO" value={newPlayer.ratingELO} onChange={(e) => setNewPlayer({...newPlayer, ratingELO: parseInt(e.target.value) || 1500})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                        <input type="text" placeholder="FIDE ID (опціонально)" value={newPlayer.fideId} onChange={(e) => setNewPlayer({...newPlayer, fideId: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                        <input type="text" placeholder="Звання (напр. CM, FM, IM, GM) (опціонально)" value={newPlayer.title} onChange={(e) => setNewPlayer({...newPlayer, title: e.target.value})} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}/>
                      </div>
                      <button onClick={handleAddPlayer} style={{ padding: '10px 24px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}>Створити та Додати</button>
                    </div>
                  )}

                  <div className={styles.tableWrapper}>
                    <table className={styles.dataTable}>
                      <thead>
                        <tr>
                          <th>№</th>
                          <th>Учасник</th>
                          <th>Рейтинг ELO</th>
                        </tr>
                      </thead>
                      <tbody>
                        {sortedPlayers.length > 0 ? sortedPlayers.map((p, idx) => (
                          <tr key={p.id}>
                            <td>{idx + 1}</td>
                            <td>{p.playerName}</td>
                            <td>{p.rating}</td>
                          </tr>
                        )) : (
                          <tr><td colSpan={3}>Учасники не знайдені</td></tr>
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
                      <br/>
                      <p>
                        <strong>Примірний розклад:</strong><br/>
                        <strong>10:00</strong> - Реєстрація учасників<br/>
                        <strong>11:00</strong> - Жеребкування та відкриття<br/>
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
    </div>
  );
};

export default TournamentViewPage;
