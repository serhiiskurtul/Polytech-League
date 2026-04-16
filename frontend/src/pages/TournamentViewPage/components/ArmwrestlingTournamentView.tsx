import React, { useState, useEffect } from 'react';
import { 
  ArmwrestlingEventResponse, 
  ArmwrestlingMatchResponse 
} from '@/shared/types/armwrestling.types';
import { PlayerDto } from '@/shared/types/player.types';
import { getAllPlayers, createPlayer, searchUsers } from '@/shared/api';
import { 
  createArmwrestlingMatch,
  updateArmwrestlingMatch,
  updateArmwrestlingEvent,
  addParticipantToArmwrestlingEvent,
  removeParticipantFromArmwrestlingEvent
} from '@/shared/api/armwrestling.api';
import ArmwrestlingBracket from './ArmwrestlingBracket';
import TournamentPodium from './TournamentPodium';
import PlayerSearchInput from './PlayerSearchInput';
import styles from '../TournamentViewPage.module.css';

interface ArmwrestlingTournamentViewProps {
  details: ArmwrestlingEventResponse;
  isAdmin: boolean;
  onRefresh: () => void;
}

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

const ArmwrestlingTournamentView: React.FC<ArmwrestlingTournamentViewProps> = ({ 
  details, 
  isAdmin,
  onRefresh
}) => {
  const [allPlayers, setAllPlayers] = useState<PlayerDto[]>([]);
  const [showAddPlayer, setShowAddPlayer] = useState(false);
  const [showSetWinners, setShowSetWinners] = useState(false);
  
  const [winners, setWinners] = useState({
    firstPlaceId: details.firstPlace?.id || 0,
    secondPlaceId: details.secondPlace?.id || 0,
    thirdPlaceId: details.thirdPlace?.id || 0
  });

  const [newMatch, setNewMatch] = useState({
    player1Id: 0,
    player2Id: 0,
    stage: 'Раунд 1',
    tableNumber: 1
  });

  const [newParticipant, setNewParticipant] = useState({
    firstName: '',
    lastName: '',
    group: '',
    userId: null as number | null,
    existingPlayerId: null as number | null
  });

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);

  useEffect(() => {
    if (isAdmin) {
      loadPlayers();
    }
  }, [isAdmin]);

  // Sync winners state with props when details change
  useEffect(() => {
    setWinners({
      firstPlaceId: details.firstPlace?.id || 0,
      secondPlaceId: details.secondPlace?.id || 0,
      thirdPlaceId: details.thirdPlace?.id || 0
    });
  }, [details]);

  useEffect(() => {
    const delayDebounceFn = setTimeout(async () => {
      if (searchQuery.length >= 2) {
        setIsSearching(true);
        try {
          // 1. Search in existing Players
          const playerResults: SearchResult[] = allPlayers
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
              rating: p.performanceScore || 0
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
  }, [searchQuery, allPlayers]);

  const handleSelectResult = (result: SearchResult) => {
    setNewParticipant({
      ...newParticipant,
      firstName: result.firstName,
      lastName: result.lastName,
      group: result.group || '',
      userId: result.userId || null,
      existingPlayerId: result.type === 'player' ? result.id : null
    });
    setSearchResults([]);
    setSearchQuery('');
  };

  const loadPlayers = async (selectId?: { p1?: number, p2?: number }) => {
    try {
      const players = await getAllPlayers();
      setAllPlayers(players);
      if (selectId?.p1) setNewMatch(prev => ({ ...prev, player1Id: selectId.p1! }));
      if (selectId?.p2) setNewMatch(prev => ({ ...prev, player2Id: selectId.p2! }));
    } catch (err) {
      console.error('Failed to load players:', err);
    }
  };

  const handleQuickAddPlayer = async (fullName: string, playerNum: 1 | 2) => {
    const parts = fullName.trim().split(/\s+/);
    const firstName = parts[0] || 'Гравець';
    const lastName = parts.slice(1).join(' ') || 'Новий';

    // 1. Check if ANY player with this name already exists in DB
    const existing = allPlayers.find(p => 
      p.firstName.toLowerCase() === firstName.toLowerCase() && 
      p.lastName.toLowerCase() === lastName.toLowerCase()
    );

    try {
      let playerId: number;
      if (existing) {
        playerId = existing.id!;
      } else {
        const created = await createPlayer({
          firstName,
          lastName,
          group: 'Гість',
          city: 'Київ'
        });
        playerId = created.id!;
      }

      // 2. Add to event if not already there
      const isAlreadyInEvent = details.players?.some((p: any) => p.id === playerId);
      if (!isAlreadyInEvent) {
          await addParticipantToArmwrestlingEvent(details.id, playerId);
      }

      await loadPlayers(playerNum === 1 ? { p1: playerId } : { p2: playerId });
      onRefresh();
    } catch (err) {
      console.error(err);
    }
  };

  const handleCreateMatch = async () => {
    if (!newMatch.player1Id || !newMatch.player2Id) {
      alert('Оберіть обох учасників!');
      return;
    }
    if (newMatch.player1Id === newMatch.player2Id) {
      alert('Учасник не може боротися сам із собою!');
      return;
    }

    try {
      await createArmwrestlingMatch({
        eventId: details.id,
        player1Id: newMatch.player1Id,
        player2Id: newMatch.player2Id,
        stage: newMatch.stage,
        tableNumber: newMatch.tableNumber
      });
      // Reset only player IDs to allow rapid adding for same stage/table
      setNewMatch(prev => ({ ...prev, player1Id: 0, player2Id: 0 }));
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при створенні матчу');
    }
  };

  const handleSetWinner = async (matchId: number, winnerId: number) => {
    const match = details.matches?.find(m => m.id === matchId);
    if (!match) return;

    try {
      await updateArmwrestlingMatch(matchId, {
        ...match,
        winnerId
      });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при збереженні результату');
    }
  };

  const handleSaveWinners = async () => {
    try {
      await updateArmwrestlingEvent(details.id, {
        ...details,
        firstPlaceId: winners.firstPlaceId || null,
        secondPlaceId: winners.secondPlaceId || null,
        thirdPlaceId: winners.thirdPlaceId || null
      });
      alert('Призерів успішно збережено!');
      setShowSetWinners(false);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при збереженні призерів');
    }
  };

  const handleAddParticipant = async () => {
    if (!newParticipant.firstName || !newParticipant.lastName || !newParticipant.group) {
        alert("Заповніть всі поля!");
        return;
    }

    try {
        let playerId = newParticipant.existingPlayerId;

        if (!playerId) {
            // Check if player is already in the tournament (by userId)
            if (newParticipant.userId && details.players?.some((p: any) => p.userId === newParticipant.userId)) {
              alert("Цей користувач вже бере участь у змаганні!");
              return;
            }

            const created = await createPlayer({
                firstName: newParticipant.firstName,
                lastName: newParticipant.lastName,
                group: newParticipant.group,
                userId: newParticipant.userId,
                city: 'Київ'
            });
            playerId = created.id!;
            alert('Гравця створено та додано у базу!');
        } else {
            // Check if player is already in the tournament
            const isAlreadyInEvent = details.players?.some((p: any) => p.id === playerId);
            if (isAlreadyInEvent) {
                alert("Цей гравець вже бере участь у змаганні!");
                return;
            }
        }

        // Add link to event
        await addParticipantToArmwrestlingEvent(details.id, playerId);
        alert('Учасника успішно додано до змагання!');

        setShowAddPlayer(false);
        setNewParticipant({ firstName: '', lastName: '', group: '', userId: null, existingPlayerId: null });
        loadPlayers();
        onRefresh();
    } catch (err) {
        console.error(err);
        alert('Помилка при додаванні гравця');
    }
  };

  const handleRemoveParticipant = async (playerId: number, playerName: string) => {
    if (!window.confirm(`Видалити учасника ${playerName} з турніру?`)) return;
    try {
        await removeParticipantFromArmwrestlingEvent(details.id, playerId);
        onRefresh();
    } catch (err) {
        console.error(err);
        alert('Помилка при видаленні учасника');
    }
  };

  // Use participants from event details (linked Players)
  // The backend now returns players directly as PlayerDto[] in details.players
  const currentParticipants = (details.players || []) as PlayerDto[];
  
  // Fallback to matches only if players collection is truly empty
  const matchParticipants = details.matches?.reduce((acc, m) => {
    if (m.player1 && !acc.find(p => p.id === m.player1?.id)) acc.push(m.player1);
    if (m.player2 && !acc.find(p => p.id === m.player2?.id)) acc.push(m.player2);
    return acc;
  }, [] as PlayerDto[]) || [];

  const displayParticipants = currentParticipants.length > 0 ? currentParticipants : matchParticipants;

  return (
    <div className={styles.armwrestlingView}>
      {/* Category Header */}
      <div className={styles.armwCategoryNav}>
        <div className={`${styles.categoryBtn} ${styles.active}`}>
          <i className="fas fa-dumbbell"></i> {details.category === 'Men' ? 'Чоловіки' : 'Жінки'} • {details.hand === 'Left' ? 'Ліва рука' : 'Права рука'} • {details.weightClass?.replace('Under', '-').replace('Plus', '+')}
        </div>
        <div className={styles.categoryInfo}>
          <span className={styles.badge}>{details.level}</span>
          {isAdmin && (
            <button 
              type="button"
              className={styles.actionBtn} 
              onClick={() => setShowSetWinners(!showSetWinners)}
            >
              <i className="fas fa-medal"></i> Встановити призерів
            </button>
          )}
        </div>
      </div>

      {/* Podium Display */}
      {(details.firstPlace || details.secondPlace || details.thirdPlace) && !showSetWinners && (
        <TournamentPodium 
          firstPlace={details.firstPlace}
          secondPlace={details.secondPlace}
          thirdPlace={details.thirdPlace}
        />
      )}

      {/* Admin Panel for Winners */}
      {isAdmin && showSetWinners && (
        <div className={styles.adminForm} style={{ background: '#fffbeb', padding: '24px', borderRadius: '16px', marginBottom: '32px', border: '1px solid #fcd34d' }}>
           <h4 style={{ marginTop: 0, marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '10px' }}>
             <i className="fas fa-trophy" style={{ color: '#f59e0b' }}></i> Визначити призерів турніру
           </h4>
           <div className={styles.winnerSelectionGrid}>
              <div className={styles.formGroup}>
                <label>1 Місце (Золото)</label>
                <select 
                  value={winners.firstPlaceId} 
                  onChange={(e) => setWinners({...winners, firstPlaceId: Number(e.target.value)})}
                  style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0' }}
                >
                  <option key="default-gold" value={0}>Оберіть гравця...</option>
                  {displayParticipants.map(p => <option key={`gold-${p.id}`} value={p.id}>{p.firstName} {p.lastName}</option>)}
                </select>
              </div>
              <div className={styles.formGroup}>
                <label>2 Місце (Срібло)</label>
                <select 
                  value={winners.secondPlaceId} 
                  onChange={(e) => setWinners({...winners, secondPlaceId: Number(e.target.value)})}
                  style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0' }}
                >
                  <option key="default-silver" value={0}>Оберіть гравця...</option>
                  {displayParticipants.map(p => <option key={`silver-${p.id}`} value={p.id}>{p.firstName} {p.lastName}</option>)}
                </select>
              </div>
              <div className={styles.formGroup}>
                <label>3 Місце (Бронза)</label>
                <select 
                  value={winners.thirdPlaceId} 
                  onChange={(e) => setWinners({...winners, thirdPlaceId: Number(e.target.value)})}
                  style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0' }}
                >
                  <option key="default-bronze" value={0}>Оберіть гравця...</option>
                  {displayParticipants.map(p => <option key={`bronze-${p.id}`} value={p.id}>{p.firstName} {p.lastName}</option>)}
                </select>
              </div>
           </div>
           <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
             <button type="button" className={styles.cancelBtn} onClick={() => setShowSetWinners(false)}>Скасувати</button>
             <button type="button" className={styles.saveBtn} onClick={handleSaveWinners}>Зберегти призерів</button>
           </div>
        </div>
      )}

      {/* Main Grid: Bracket & Info */}
      <div className={styles.tournamentGrid}>
        <div className={styles.mainContent}>
          {isAdmin && (
            <div className={styles.adminForm} style={{ background: '#f8fafc', padding: '24px', borderRadius: '16px', marginBottom: '32px', border: '1px solid #e2e8f0', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)' }}>
              <h4 style={{ marginTop: 0, marginBottom: '20px', fontSize: '18px', display: 'flex', alignItems: 'center', gap: '10px' }}>
                <i className="fas fa-plus-circle" style={{ color: '#3b82f6' }}></i> Додати новий поєдинок
              </h4>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '24px', alignItems: 'flex-end' }}>
                <PlayerSearchInput 
                  label="Гравець 1"
                  players={allPlayers}
                  selectedId={newMatch.player1Id}
                  onSelect={(id) => setNewMatch({...newMatch, player1Id: id})}
                  onQuickAdd={(name) => handleQuickAddPlayer(name, 1)}
                />

                <div style={{ textAlign: 'center', paddingBottom: '12px', fontWeight: 'bold', color: '#94a3b8' }}>VS</div>

                <PlayerSearchInput 
                  label="Гравець 2"
                  players={allPlayers}
                  selectedId={newMatch.player2Id}
                  onSelect={(id) => setNewMatch({...newMatch, player2Id: id})}
                  onQuickAdd={(name) => handleQuickAddPlayer(name, 2)}
                />

                <div className={styles.formGroup}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: 600, color: '#64748b', marginBottom: '6px' }}>Етап турніру</label>
                  <select 
                    value={newMatch.stage} 
                    onChange={(e) => setNewMatch({...newMatch, stage: e.target.value})}
                    style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0', background: 'white' }}
                  >
                    <option value="Раунд 1">Раунд 1</option>
                    <option value="Раунд 2">Раунд 2</option>
                    <option value="Раунд 3">Раунд 3</option>
                    <option value="Раунд 4">Раунд 4</option>
                    <option value="Раунд 5">Раунд 5</option>
                    <option value="1/16 Фіналу">1/16 Фіналу</option>
                    <option value="1/8 Фіналу">1/8 Фіналу</option>
                    <option value="Чвертьфінал">Чвертьфінал</option>
                    <option value="Півфінал">Півфінал</option>
                    <option value="Фінал">Фінал</option>
                  </select>
                </div>

                <div className={styles.formGroup} style={{ width: '80px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: 600, color: '#64748b', marginBottom: '6px' }}>Стіл</label>
                  <input 
                    type="number" 
                    value={newMatch.tableNumber} 
                    onChange={(e) => setNewMatch({...newMatch, tableNumber: Number(e.target.value)})}
                    style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0' }}
                  />
                </div>

                <button 
                  type="button"
                  className={styles.saveBtn} 
                  onClick={handleCreateMatch}
                  style={{ height: '42px', background: '#3b82f6', color: 'white', border: 'none', borderRadius: '8px', fontWeight: '600', cursor: 'pointer' }}
                >
                  Створити
                </button>
              </div>
            </div>
          )}

          <div className={styles.sectionHeader}>
            <h2><i className="fas fa-sitemap"></i> Турнірна сітка</h2>
          </div>
          
          <ArmwrestlingBracket 
            matches={details.matches || []} 
            isAdmin={isAdmin}
            onSetWinner={handleSetWinner}
          />
        </div>

        <div className={styles.sidePanel}>
          {/* Participants Simplified List */}
          <div className={styles.sideSection}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '15px' }}>
              <h3 style={{ margin: 0 }}><i className="fas fa-users"></i> Учасники</h3>
              {isAdmin && (
                <button 
                    type="button"
                    onClick={() => {
                      setShowAddPlayer(!showAddPlayer);
                      setSearchQuery('');
                      setSearchResults([]);
                    }}
                    style={{ background: 'none', border: 'none', color: '#3b82f6', cursor: 'pointer', fontSize: '14px' }}
                >
                    {showAddPlayer ? 'Закрити' : '+ Додати'}
                </button>
              )}
            </div>

            {showAddPlayer && (
              <div style={{ background: '#f9fafb', padding: '15px', borderRadius: '12px', marginBottom: '15px', border: '1px solid #e5e7eb' }}>
                <div style={{ marginBottom: '15px', position: 'relative' }}>
                  <label style={{ display: 'block', fontSize: '12px', marginBottom: '6px', fontWeight: 'bold' }}>Швидкий пошук:</label>
                  <input
                    type="text"
                    placeholder="Ім'я, прізвище або email..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #ccc' }}
                  />
                  {isSearching && <div style={{ marginTop: '8px', fontSize: '12px' }}>Шукаємо...</div>}
                  {searchResults.length > 0 && (
                    <div style={{
                      position: 'absolute',
                      top: '100%',
                      left: 0,
                      right: 0,
                      background: '#fff',
                      border: '1px solid #ccc',
                      borderRadius: '8px',
                      zIndex: 100,
                      maxHeight: '200px',
                      overflowY: 'auto',
                      boxShadow: '0 4px 12px rgba(0,0,0,0.1)'
                    }}>
                      {searchResults.map((result, idx) => (
                        <div
                          key={`${result.type}-${result.id}-${idx}`}
                          onClick={() => handleSelectResult(result)}
                          style={{ padding: '10px', cursor: 'pointer', borderBottom: '1px solid #eee' }}
                        >
                          <span style={{
                            fontSize: '9px',
                            background: result.type === 'player' ? '#10b981' : '#3b82f6',
                            color: '#fff',
                            padding: '2px 5px',
                            borderRadius: '4px',
                            marginRight: '8px',
                            textTransform: 'uppercase'
                          }}>
                            {result.type === 'player' ? 'Гравець' : 'Юзер'}
                          </span>
                          <strong style={{ fontSize: '13px' }}>{result.firstName} {result.lastName}</strong>
                          {result.group && <span style={{ color: '#666', fontSize: '11px', marginLeft: '6px' }}>({result.group})</span>}
                        </div>
                      ))}
                    </div>
                  )}
                </div>

                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '8px', marginBottom: '8px' }}>
                  <input 
                      placeholder="Ім'я" 
                      value={newParticipant.firstName} 
                      onChange={(e) => setNewParticipant({...newParticipant, firstName: e.target.value})}
                      style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                  />
                  <input 
                      placeholder="Прізвище" 
                      value={newParticipant.lastName} 
                      onChange={(e) => setNewParticipant({...newParticipant, lastName: e.target.value})}
                      style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                  />
                </div>
                <input 
                    placeholder="Група (напр. КН-31)" 
                    value={newParticipant.group} 
                    onChange={(e) => setNewParticipant({...newParticipant, group: e.target.value})}
                    style={{ width: '100%', padding: '8px', marginBottom: '12px', borderRadius: '6px', border: '1px solid #ccc' }}
                />
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button type="button" className={styles.saveBtn} style={{ flex: 1 }} onClick={handleAddParticipant}>
                    {newParticipant.existingPlayerId ? 'Додати існуючого' : 'Створити та додати'}
                  </button>
                  {(newParticipant.userId || newParticipant.existingPlayerId) && (
                    <button 
                      type="button" 
                      onClick={() => setNewParticipant({ firstName: '', lastName: '', group: '', userId: null, existingPlayerId: null })}
                      style={{ padding: '8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
                    >
                      <i className="fas fa-times"></i>
                    </button>
                  )}
                </div>
              </div>
            )}

            <div className={styles.participantsList}>
              {displayParticipants.map(player => (
                <div key={player.id} className={styles.participantItem} style={{ position: 'relative' }}>
                  <div className={styles.playerAvatar}>
                    {player.firstName[0]}{player.lastName[0]}
                  </div>
                  <div className={styles.playerInfo}>
                    <div className={styles.playerName}>
                      {player.userId ? (
                        <a href={`/profile/${player.userId}`} style={{ color: 'inherit', textDecoration: 'none' }} target="_blank" rel="noreferrer">
                          {player.firstName} {player.lastName}
                        </a>
                      ) : (
                        `${player.firstName} ${player.lastName}`
                      )}
                    </div>
                    <div className={styles.playerDetails}>{player.group} • Допущений</div>
                  </div>
                  {isAdmin && (
                    <button 
                      type="button"
                      onClick={() => handleRemoveParticipant(player.id, `${player.firstName} ${player.lastName}`)}
                      style={{ position: 'absolute', right: 0, top: '50%', transform: 'translateY(-50%)', background: 'none', border: 'none', color: '#ef4444', cursor: 'pointer', padding: '8px' }}
                      title="Видалити учасника"
                    >
                      <i className="fas fa-user-minus"></i>
                    </button>
                  )}
                </div>
              ))}
              {displayParticipants.length === 0 && (
                <p className={styles.noData}>Учасників ще не додано до змагання</p>
              )}
            </div>
          </div>

          {/* Referees */}
          <div className={styles.sideSection}>
            <h3><i className="fas fa-user-shield"></i> Суддівська колегія</h3>
            <div className={styles.refereesBoard}>
              <div className={styles.headReferee}>
                <strong>Судді:</strong> {details.referees || details.headRefereeName || 'Не призначено'}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ArmwrestlingTournamentView;
