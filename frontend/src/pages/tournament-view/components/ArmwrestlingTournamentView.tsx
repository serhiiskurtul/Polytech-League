import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import {
  ArmwrestlingEventResponse,
  ArmwrestlingMatchResponse
} from '@/entities/sport/model/armwrestling.types';
import { RegistrationStatus } from '@/entities/tournament/model/tournament.types';
import { PlayerDto, Gender } from '@/entities/player/model/player.types';
import { createPlayer, searchPlayers } from '@/entities/player/api/players.api';
import { searchUsers } from '@/entities/user/api/users.api';
import {
  createArmwrestlingMatch,
  updateArmwrestlingMatch,
  updateArmwrestlingEvent,
  addParticipantToArmwrestlingEvent,
  removeParticipantFromArmwrestlingEvent,
  deleteArmwrestlingMatch,
  approveArmwrestlingParticipant,
  rejectArmwrestlingParticipant
} from '@/entities/sport/api/armwrestling.api';
import ArmwrestlingBracket from './ArmwrestlingBracket';
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
  group?: string | null;
  rating?: number;
};

const ArmwrestlingTournamentView: React.FC<ArmwrestlingTournamentViewProps> = ({
  details,
  isAdmin,
  onRefresh
}) => {
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
    patronymic: '',
    group: '',
    userId: null as number | null,
    existingPlayerId: null as number | null,
    gender: 'None' as Gender
  });

  // Новий гравець зі швидкого додавання в поєдинок, для якого ще треба вказати групу
  const [quickAdd, setQuickAdd] = useState<{
    firstName: string;
    lastName: string;
    patronymic: string;
    group: string;
    playerNum: 1 | 2;
  } | null>(null);

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [useStrictFiltering, setUseStrictFiltering] = useState(true);
  const [roundNumber, setRoundNumber] = useState(1);
  const [stageType, setStageType] = useState('Раунд');

  useEffect(() => {
    // Kept empty, just for logic preservation
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
          const players = await searchPlayers(searchQuery);
          const playerResults: SearchResult[] = players
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
  }, [searchQuery]);

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
      if (selectId?.p1) setNewMatch(prev => ({ ...prev, player1Id: selectId.p1! }));
      if (selectId?.p2) setNewMatch(prev => ({ ...prev, player2Id: selectId.p2! }));
  };

  const handleQuickAddPlayer = async (fullName: string, playerNum: 1 | 2) => {
    const parts = fullName.trim().split(/\s+/);
    let firstName = 'Гравець';
    let lastName = 'Новий';
    let patronymic = '';

    if (parts.length >= 3) {
      firstName = parts[0];
      patronymic = parts[1];
      lastName = parts.slice(2).join(' ');
    } else {
      firstName = parts[0] || 'Гравець';
      lastName = parts.slice(1).join(' ') || 'Новий';
    }

    // 1. Check if ANY player with this name already exists in DB
    const searchResults = await searchPlayers(fullName);
    const existing = searchResults.find(p =>
      p.firstName.toLowerCase() === firstName.toLowerCase() &&
      p.lastName.toLowerCase() === lastName.toLowerCase()
    );

    if (existing) {
      await addQuickPlayerToEvent(existing.id!, existing.group || '', playerNum);
      return;
    }

    // Нового гравця без групи не створюємо: інакше він потрапляє в рейтинг під групою «Гість»
    setQuickAdd({ firstName, lastName, patronymic, group: '', playerNum });
  };

  const addQuickPlayerToEvent = async (playerId: number, group: string, playerNum: 1 | 2) => {
    try {
      const isAlreadyInEvent = details.participants?.some((p: any) => (p.playerId || p.id) === playerId);
      if (!isAlreadyInEvent) {
        await addParticipantToArmwrestlingEvent(details.id, { playerId, weight: 0, rank: 0, status: RegistrationStatus.Approved, groupName: group });
      }

      await loadPlayers(playerNum === 1 ? { p1: playerId } : { p2: playerId });
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при додаванні гравця');
    }
  };

  const handleConfirmQuickAdd = async () => {
    if (!quickAdd) return;
    const group = quickAdd.group.trim();
    if (!quickAdd.firstName.trim() || !quickAdd.lastName.trim() || !group) {
      toast.error("Вкажіть прізвище, ім'я та групу!");
      return;
    }

    let gender: Gender = 'None';
    if (details.category === 'Men') gender = 'Male';
    else if (details.category === 'Women') gender = 'Female';

    try {
      const created = await createPlayer({
        firstName: quickAdd.firstName.trim(),
        lastName: quickAdd.lastName.trim(),
        patronymic: quickAdd.patronymic.trim() || null,
        group,
        city: 'Київ',
        gender
      });
      await addQuickPlayerToEvent(created.id!, group, quickAdd.playerNum);
      setQuickAdd(null);
    } catch (err) {
      console.error(err);
      toast.error('Помилка при створенні гравця');
    }
  };

  const handleCreateMatch = async () => {
    if (!newMatch.player1Id || !newMatch.player2Id) {
      toast.error('Оберіть обох учасників!');
      return;
    }
    if (newMatch.player1Id === newMatch.player2Id) {
      toast.error('Учасник не може боротися сам із собою!');
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
      toast.error('Помилка при створенні матчу');
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
      toast.error('Помилка при збереженні результату');
    }
  };

  const handleDeleteMatch = async (matchId: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей поєдинок?')) return;

    try {
      await deleteArmwrestlingMatch(matchId);
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при видаленні матчу');
    }
  };

  const currentParticipants = (details.participants || []).map(p => {
    const groupToShow = p.groupName || 'Учасник';
    
    return {
      id: p.playerId,
      userId: p.userId,
      firstName: p.playerName?.split(' ')[0] || '',
      lastName: p.playerName?.split(' ').slice(1).join(' ') || '',
      group: groupToShow
    } as PlayerDto;
  });

  const matchParticipants = details.matches?.reduce((acc, m) => {
    if (m.player1 && !acc.find(p => p.id === m.player1?.id)) acc.push(m.player1);
    if (m.player2 && !acc.find(p => p.id === m.player2?.id)) acc.push(m.player2);
    return acc;
  }, [] as PlayerDto[]) || [];

  const displayParticipants = currentParticipants.length > 0 ? currentParticipants : matchParticipants;

  const STAGE_ORDER = [
    '1/16 Фіналу', '1/8 Фіналу', 'Чвертьфінал', 'Півфінал', 'Фінал',
    'Lower Раунд 1', 'Lower Раунд 2', 'Lower Раунд 3', 'Lower Півфінал', 'Lower Фінал', 'Grand Final'
  ];

  const getEligiblePlayers = (stage: string) => {
    if (!useStrictFiltering) return displayParticipants;
    
    if (!details.matches || details.matches.length === 0) {
      return (stage === 'Раунд 1' || stage === '1/16 Фіналу') ? displayParticipants : [];
    }

    const idx = STAGE_ORDER.indexOf(stage);
    if (idx < 0 && !stage.startsWith('Раунд')) return displayParticipants;

    // Logic for Single Elimination / Upper Bracket stages
    const standardStages = ['1/16 Фіналу', '1/8 Фіналу', 'Чвертьфінал', 'Півфінал', 'Фінал'];
    const sIdx = standardStages.indexOf(stage);

    if (sIdx > 0) {
      const prevStage = standardStages[sIdx - 1];
      const winners = details.matches
        .filter(m => m.stage === prevStage && m.winnerId)
        .map(m => m.winnerId!);
      return displayParticipants.filter(p => winners.includes(p.id!));
    }

    // Logic for sequential rounds (Round 1 -> Round 2 ...)
    const roundMatch = stage.match(/^Раунд (\d+)$/);
    if (roundMatch) {
      const rNum = parseInt(roundMatch[1]);
      if (rNum > 1) {
        const prevRound = `Раунд ${rNum - 1}`;
        const winners = details.matches
          .filter(m => m.stage === prevRound && m.winnerId)
          .map(m => m.winnerId!);
        return displayParticipants.filter(p => winners.includes(p.id!));
      }
      return displayParticipants;
    }

    // Logic for Lower Bracket stages
    const lowerStages = ['Lower Раунд 1', 'Lower Раунд 2', 'Lower Раунд 3', 'Lower Півфінал', 'Lower Фінал'];
    const lIdx = lowerStages.indexOf(stage);
    if (lIdx > 0) {
      const prevStage = lowerStages[lIdx - 1];
      const winners = details.matches
        .filter(m => m.stage === prevStage && m.winnerId)
        .map(m => m.winnerId!);
      return displayParticipants.filter(p => winners.includes(p.id!));
    }

    // For Lower Round 1 or Grand Final, allow everyone for now to avoid complexity of loser-tracking
    if (stage === 'Lower Раунд 1' || stage === 'Grand Final') {
      return displayParticipants;
    }

    return displayParticipants;
  };

  const eligiblePlayers = getEligiblePlayers(newMatch.stage);

  const handleSaveWinners = async () => {
    try {
      await updateArmwrestlingEvent(details.id, {
        ...details,
        firstPlaceId: winners.firstPlaceId || null,
        secondPlaceId: winners.secondPlaceId || null,
        thirdPlaceId: winners.thirdPlaceId || null
      });
      toast.success('Призерів успішно збережено!');
      setShowSetWinners(false);
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при збереженні призерів');
    }
  };

  const handleAddParticipant = async () => {
    if (!newParticipant.firstName || !newParticipant.lastName || !newParticipant.group) {
      toast.error("Заповніть всі поля!");
      return;
    }

    try {
      let playerId = newParticipant.existingPlayerId;

      if (!playerId) {
        // Check if player is already in the tournament (by userId)
        if (newParticipant.userId && details.participants?.some((p: any) => (p.userId === newParticipant.userId))) {
          toast.error("Цей користувач вже бере участь у змаганні!");
          return;
        }

        const created = await createPlayer({
          firstName: newParticipant.firstName,
          lastName: newParticipant.lastName,
          patronymic: newParticipant.patronymic || null,
          group: newParticipant.group,
          userId: newParticipant.userId,
          city: 'Київ',
          gender: newParticipant.gender
        });
        playerId = created.id!;
      } else {
        // Check if player is already in the tournament
        const isAlreadyInEvent = details.participants?.some((p: any) => (p.playerId || p.id) === playerId);
        if (isAlreadyInEvent) {
          toast.error("Цей гравець вже бере участь у змаганні!");
          return;
        }
      }

      // Add link to event
      await addParticipantToArmwrestlingEvent(details.id, { playerId: playerId!, weight: 0, rank: 0, status: RegistrationStatus.Approved, groupName: newParticipant.group });
      toast.success('Учасника успішно додано до змагання!');

      setShowAddPlayer(false);
      setNewParticipant({ firstName: '', lastName: '', patronymic: '', group: '', userId: null, existingPlayerId: null, gender: 'None' });
      loadPlayers();
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при додаванні гравця');
    }
  };

  const handleRemoveParticipant = async (playerId: number, playerName: string) => {
    if (!window.confirm(`Видалити учасника ${playerName} зі змагання?`)) return;
    try {
      await removeParticipantFromArmwrestlingEvent(details.id, playerId);
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при видаленні учасника');
    }
  };

  const handleApprove = async (playerId: number, playerName: string) => {
    try {
      await approveArmwrestlingParticipant(details.id, playerId);
      toast.success("Учасника схвалено!");
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error("Помилка при схваленні");
    }
  };

  const handleReject = async (playerId: number, playerName: string) => {
    if (!window.confirm(`Відхилити участь ${playerName}?`)) return;
    try {
      await rejectArmwrestlingParticipant(details.id, playerId);
      toast.success("Учасника відхилено");
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error("Помилка при відхиленні");
    }
  };

  return (
    <div className={styles.armwrestlingView}>
      {/* Category Header */}
      <div className={styles.armwCategoryNav}>
        <div className={`${styles.categoryBtn} ${styles.active}`}>
          <i className="fas fa-dumbbell"></i> {details.category === 'Men' ? 'Чоловіки' : 'Жінки'} • {details.hand === 'Left' ? 'Ліва рука' : 'Права рука'} • {details.weightClass?.replace('Under', '-').replace('Plus', '+')}
        </div>
        <div className={styles.categoryInfo}>
          <span className={styles.badge}>
            {details.level === 'CollegeGroup' ? 'Міжгруповий рівень' :
             details.level === 'Regional' ? 'Обласний рівень' :
             details.level === 'National' ? 'Національний рівень' : details.level}
          </span>
          {isAdmin && (
            <button
              type="button"
              className={styles.actionBtn}
              onClick={() => setShowSetWinners(!showSetWinners)}
            >
              <i className="fas fa-medal"></i> <span>Встановити призерів</span>
            </button>
          )}
        </div>
      </div>


      {/* Admin Panel for Winners */}
      {isAdmin && showSetWinners && (
        <div className={styles.adminForm} style={{ background: '#fffbeb', padding: '24px', borderRadius: '16px', marginBottom: '32px', border: '1px solid #fcd34d' }}>
          <h4 style={{ marginTop: 0, marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '10px' }}>
            <i className="fas fa-trophy" style={{ color: '#f59e0b' }}></i> Визначити призерів змагання
          </h4>
          <div className={styles.winnerSelectionGrid}>
            <div className={styles.formGroup}>
              <label>1 Місце (Золото)</label>
              <select
                value={winners.firstPlaceId}
                onChange={(e) => setWinners({ ...winners, firstPlaceId: Number(e.target.value) })}
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
                onChange={(e) => setWinners({ ...winners, secondPlaceId: Number(e.target.value) })}
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
                onChange={(e) => setWinners({ ...winners, thirdPlaceId: Number(e.target.value) })}
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

      {/* Layout: Bracket at top, Participants/Referees at bottom */}
      <div className={styles.armwrestlingLayout}>
        <div className={styles.mainContent} style={{ width: '100%', maxWidth: '100%' }}>
          {isAdmin && (
            <div className={styles.adminForm} style={{ background: '#f8fafc', padding: '28px', borderRadius: '20px', marginBottom: '32px', border: '1px solid #e2e8f0', boxShadow: '0 10px 15px -3px rgba(0,0,0,0.05)' }}>
              <h4 style={{ marginTop: 0, marginBottom: '24px', fontSize: '1.1rem', color: '#1e293b', display: 'flex', alignItems: 'center', gap: '10px' }}>
                <i className="fas fa-plus-circle" style={{ color: '#3b82f6' }}></i> Додати новий поєдинок
              </h4>
              <div style={{ display: 'grid', gridTemplateColumns: '2fr auto 2fr 1.5fr 80px auto', gap: '20px', alignItems: 'flex-end' }}>
                <div>
                  <PlayerSearchInput 
                    label="Гравець 1"
                    players={eligiblePlayers}
                    selectedId={newMatch.player1Id}
                    onSelect={(id) => setNewMatch({ ...newMatch, player1Id: id })}
                    onQuickAdd={(name) => handleQuickAddPlayer(name, 1)}
                    placeholder="Оберіть..."
                  />
                </div>

                <div style={{ paddingBottom: '12px', fontWeight: '800', color: '#94a3b8', fontSize: '12px' }}>VS</div>

                <div>
                  <PlayerSearchInput 
                    label="Гравець 2"
                    players={eligiblePlayers}
                    selectedId={newMatch.player2Id}
                    onSelect={(id) => setNewMatch({ ...newMatch, player2Id: id })}
                    onQuickAdd={(name) => handleQuickAddPlayer(name, 2)}
                    placeholder="Оберіть..."
                  />
                </div>

                <div style={{ display: 'flex', gap: '8px' }}>
                  <div style={{ flex: 2 }}>
                    <label style={{ display: 'block', fontSize: '13px', fontWeight: 600, color: '#64748b', marginBottom: '8px' }}>Етап</label>
                    <select
                      value={stageType}
                      onChange={(e) => {
                        const val = e.target.value;
                        setStageType(val);
                        if (val === 'Раунд') {
                          setNewMatch({ ...newMatch, stage: `Раунд ${roundNumber}` });
                        } else {
                          setNewMatch({ ...newMatch, stage: val });
                        }
                      }}
                      style={{ width: '100%', padding: '10px', borderRadius: '10px', border: '1px solid #e2e8f0', background: 'white', height: '42px', fontSize: '14px', color: '#1e293b' }}
                    >
                      <option value="Раунд">Раунд...</option>
                      <option value="1/16 Фіналу">1/16 Фіналу</option>
                      <option value="1/8 Фіналу">1/8 Фіналу</option>
                      <option value="Чвертьфінал">Чвертьфінал</option>
                      <option value="Півфінал">Півфінал</option>
                      <option value="Фінал">Фінал</option>
                      {details.tournamentType === 'DoubleElimination' && (
                        <>
                          <option value="Lower Раунд 1">Lower Раунд 1</option>
                          <option value="Lower Раунд 2">Lower Раунд 2</option>
                          <option value="Lower Раунд 3">Lower Раунд 3</option>
                          <option value="Lower Півфінал">Lower Півфінал</option>
                          <option value="Lower Фінал">Lower Фінал</option>
                          <option value="Grand Final">Grand Final</option>
                        </>
                      )}
                    </select>
                  </div>
                  {stageType === 'Раунд' && (
                    <div style={{ width: '60px' }}>
                      <label style={{ display: 'block', fontSize: '13px', fontWeight: 600, color: '#64748b', marginBottom: '8px' }}>№</label>
                      <input
                        type="number"
                        min="1"
                        value={roundNumber}
                        onFocus={(e) => e.target.select()}
                        onChange={(e) => {
                          const num = Number(e.target.value);
                          setRoundNumber(num);
                          setNewMatch({ ...newMatch, stage: `Раунд ${num}` });
                        }}
                        style={{ width: '100%', padding: '10px', borderRadius: '10px', border: '1px solid #e2e8f0', height: '42px', fontSize: '14px', color: '#1e293b' }}
                      />
                    </div>
                  )}
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center' }}>
                  <label style={{ fontSize: '11px', fontWeight: 600, color: '#94a3b8', marginBottom: '4px', whiteSpace: 'nowrap', display: 'flex', alignItems: 'center', gap: '4px' }}>
                    Фільтрація
                    <div className={styles.tooltip}>
                      <i className="fas fa-question-circle" style={{ cursor: 'help', color: '#cbd5e1' }}></i>
                      <span className={styles.tooltipText}>
                        Увімкнено: лише переможці попереднього етапу. <br/>
                        Вимкнено: будь-які учасники (дозволяє перестрибувати раунди).
                      </span>
                    </div>
                  </label>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <label className={styles.switch}>
                      <input 
                        type="checkbox" 
                        checked={useStrictFiltering} 
                        onChange={(e) => setUseStrictFiltering(e.target.checked)}
                      />
                      <span className={styles.slider}></span>
                    </label>
                    <span style={{ fontSize: '11px', color: useStrictFiltering ? '#3b82f6' : '#64748b', fontWeight: '600' }}>
                      {useStrictFiltering ? 'Увімк' : 'Вимк'}
                    </span>
                  </div>
                </div>

                <div style={{ width: '80px' }}>
                  <label style={{ display: 'block', fontSize: '13px', fontWeight: 600, color: '#64748b', marginBottom: '8px' }}>Стіл</label>
                  <input
                    type="number"
                    value={newMatch.tableNumber}
                    onFocus={(e) => e.target.select()}
                    onChange={(e) => setNewMatch({ ...newMatch, tableNumber: Number(e.target.value) })}
                    style={{ width: '100%', padding: '10px', borderRadius: '10px', border: '1px solid #e2e8f0', height: '42px', fontSize: '14px', color: '#1e293b' }}
                  />
                </div>

                <button
                  type="button"
                  className={styles.saveBtn}
                  onClick={handleCreateMatch}
                  style={{ height: '42px', padding: '0 24px', background: '#3b82f6', color: 'white', border: 'none', borderRadius: '10px', fontWeight: '700', cursor: 'pointer', transition: 'all 0.2s', fontSize: '14px' }}
                >
                  Створити
                </button>
              </div>

              {quickAdd && (
                <div style={{ marginTop: '20px', padding: '16px', background: '#eff6ff', border: '1px solid #bfdbfe', borderRadius: '12px' }}>
                  <div style={{ fontSize: '14px', fontWeight: 600, color: '#1e293b', marginBottom: '12px' }}>
                    Новий гравець для «Гравець {quickAdd.playerNum}»: вкажіть групу
                  </div>
                  <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))', gap: '8px', marginBottom: '12px' }}>
                    <input
                      placeholder="Прізвище"
                      value={quickAdd.lastName}
                      onChange={(e) => setQuickAdd({ ...quickAdd, lastName: e.target.value })}
                      style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                    <input
                      placeholder="Ім'я"
                      value={quickAdd.firstName}
                      onChange={(e) => setQuickAdd({ ...quickAdd, firstName: e.target.value })}
                      style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                    <input
                      placeholder="По батькові"
                      value={quickAdd.patronymic}
                      onChange={(e) => setQuickAdd({ ...quickAdd, patronymic: e.target.value })}
                      style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                    <input
                      autoFocus
                      placeholder="Група (напр. 211)"
                      value={quickAdd.group}
                      onChange={(e) => setQuickAdd({ ...quickAdd, group: e.target.value })}
                      onKeyDown={(e) => { if (e.key === 'Enter') handleConfirmQuickAdd(); }}
                      style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                  </div>
                  <div style={{ display: 'flex', gap: '8px' }}>
                    <button type="button" className={styles.saveBtn} onClick={handleConfirmQuickAdd}>
                      Створити та додати
                    </button>
                    <button type="button" className={styles.cancelBtn} onClick={() => setQuickAdd(null)}>
                      Скасувати
                    </button>
                  </div>
                </div>
              )}
            </div>
          )}

          <div className={styles.sectionHeader}>
            <h2><i className="fas fa-sitemap"></i> Сітка змагань</h2>
          </div>

          <ArmwrestlingBracket
            matches={details.matches || []}
            tournamentType={details.tournamentType}
            isAdmin={isAdmin}
            onSetWinner={handleSetWinner}
            onDeleteMatch={handleDeleteMatch}
          />
        </div>

        <div className={styles.sideInfoRow} style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(400px, 1fr))',
          gap: '32px',
          marginTop: '32px'
        }}>
          {/* Participants Simplified List */}
          <div className={styles.sideSection} style={{ background: 'white', padding: '24px', borderRadius: '20px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)', border: '1px solid #f1f5f9' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '15px' }}>
              <h3 style={{ margin: 0 }}><i className="fas fa-users"></i> Учасники {displayParticipants.length > 0 && `(${displayParticipants.length})`}</h3>
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

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '8px', marginBottom: '8px' }}>
                    <input
                      placeholder="Прізвище"
                      value={newParticipant.lastName}
                      onChange={(e) => setNewParticipant({ ...newParticipant, lastName: e.target.value })}
                      style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                    <input
                      placeholder="Ім'я"
                      value={newParticipant.firstName}
                      onChange={(e) => setNewParticipant({ ...newParticipant, firstName: e.target.value })}
                      style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                    <input
                      placeholder="По батькові"
                      value={newParticipant.patronymic}
                      onChange={(e) => setNewParticipant({ ...newParticipant, patronymic: e.target.value })}
                      style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                    />
                  </div>
                <input
                  placeholder="Група (напр. 211)"
                  value={newParticipant.group}
                  onChange={(e) => setNewParticipant({ ...newParticipant, group: e.target.value })}
                  style={{ width: '100%', padding: '8px', marginBottom: '12px', borderRadius: '6px', border: '1px solid #ccc' }}
                />
                <div style={{ marginBottom: '12px' }}>
                  <select
                    value={newParticipant.gender}
                    onChange={(e) => setNewParticipant({ ...newParticipant, gender: e.target.value as Gender })}
                    style={{ width: '100%', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                  >
                    <option value="None">Оберіть стать...</option>
                    <option value="Male">Чоловік</option>
                    <option value="Female">Жінка</option>
                  </select>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button type="button" className={styles.saveBtn} style={{ flex: 1 }} onClick={handleAddParticipant}>
                    {newParticipant.existingPlayerId ? 'Додати існуючого' : 'Створити та додати'}
                  </button>
                  {(newParticipant.userId || newParticipant.existingPlayerId) && (
                    <button
                      type="button"
                      onClick={() => setNewParticipant({ firstName: '', lastName: '', patronymic: '', group: '', userId: null, existingPlayerId: null, gender: 'None' })}
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
                <div key={player.id} className={styles.participantItem} style={{
                  position: 'relative',
                  display: 'flex',
                  alignItems: 'center',
                  padding: '12px',
                  background: 'white',
                  borderRadius: '12px',
                  marginBottom: '8px',
                  border: '1px solid #f1f5f9',
                  boxShadow: '0 1px 2px rgba(0,0,0,0.05)'
                }}>
                  <div className={styles.playerAvatar} style={{
                    width: '40px',
                    height: '40px',
                    borderRadius: '50%',
                    background: '#f1f5f9',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontWeight: 'bold',
                    color: '#64748b',
                    marginRight: '12px'
                  }}>
                    {(player.firstName?.[0] || '')}{(player.lastName?.[0] || '')}
                  </div>
                  <div className={styles.playerInfo} style={{ flex: 1 }}>
                    <div className={styles.playerName} style={{ fontWeight: '600', color: '#1e293b' }}>
                      {player.id ? (
                        <Link to={`/players/${player.id}`} style={{ color: 'inherit', textDecoration: 'none' }}>
                          {player.firstName} {player.lastName}
                        </Link>
                      ) : (
                        `${player.firstName} ${player.lastName}`
                      )}
                    </div>
                    <div className={styles.playerDetails} style={{ fontSize: '12px', color: '#64748b', display: 'flex', alignItems: 'center', gap: '8px' }}>
                      {player.group || 'Гість'} {player.armwrestlingWeight ? `• ${player.armwrestlingWeight} кг` : ''}
                      {((details.participants || []).find(p => p.playerId === player.id))?.status !== RegistrationStatus.Approved && (
                        <span className={`${styles.statusBadge} ${styles[((details.participants || []).find(p => p.playerId === player.id))?.status?.toLowerCase() || 'pending']}`} style={{ fontSize: '10px', padding: '1px 5px', borderRadius: '4px', textTransform: 'uppercase', fontWeight: 'bold' }}>
                          {((details.participants || []).find(p => p.playerId === player.id))?.status === RegistrationStatus.Pending ? 'Очікує' : 'Відхилено'}
                        </span>
                      )}
                    </div>
                  </div>
                  {isAdmin && (
                    <div style={{ display: 'flex', gap: '4px' }}>
                      {((details.participants || []).find(p => p.playerId === player.id))?.status === RegistrationStatus.Pending && (
                        <>
                          <button
                            type="button"
                            onClick={() => handleApprove(player.id!, `${player.firstName} ${player.lastName}`)}
                            style={{ background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', width: '30px', height: '30px', cursor: 'pointer' }}
                            title="Схвалити"
                          >
                            <i className="fas fa-check"></i>
                          </button>
                          <button
                            type="button"
                            onClick={() => handleReject(player.id!, `${player.firstName} ${player.lastName}`)}
                            style={{ background: '#f59e0b', color: '#fff', border: 'none', borderRadius: '6px', width: '30px', height: '30px', cursor: 'pointer' }}
                            title="Відхилити"
                          >
                            <i className="fas fa-user-slash"></i>
                          </button>
                        </>
                      )}
                      <button
                        type="button"
                        onClick={() => handleRemoveParticipant(player.id!, `${player.firstName} ${player.lastName}`)}
                        style={{ position: 'absolute', right: 0, top: '50%', transform: 'translateY(-50%)', background: 'none', border: 'none', color: '#ef4444', cursor: 'pointer', padding: '8px' }}
                        title="Видалити учасника"
                      >
                        <i className="fas fa-user-minus"></i>
                      </button>
                    </div>
                  )}
                </div>
              ))}
              {displayParticipants.length === 0 && (
                <p className={styles.noData}>Учасників ще не додано до змагання</p>
              )}
            </div>
          </div>

          {/* Referees */}
          <div className={styles.sideSection} style={{ background: 'white', padding: '24px', borderRadius: '20px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)', border: '1px solid #f1f5f9', height: 'fit-content' }}>
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
