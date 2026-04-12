import React, { useState, useEffect } from 'react';
import { 
  ArmwrestlingEventResponse, 
  ArmwrestlingMatchResponse 
} from '@/shared/types/armwrestling.types';
import { PlayerDto } from '@/shared/types/player.types';
import { getAllPlayers, createPlayer } from '@/shared/api';
import { 
  createArmwrestlingMatch,
  updateArmwrestlingMatch,
  updateArmwrestlingEvent
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
    group: ''
  });

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

    try {
      const created = await createPlayer({
        firstName,
        lastName,
        group: 'Гість',
        city: 'Київ'
      });
      await loadPlayers(playerNum === 1 ? { p1: created.id } : { p2: created.id });
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
        await createPlayer({
            ...newParticipant,
            city: 'Київ'
        });
        alert('Гравець доданий у базу!');
        setShowAddPlayer(false);
        loadPlayers();
    } catch (err) {
        console.error(err);
        alert('Помилка при додаванні гравця');
    }
  };

  // Get unique participants list from matches
  const currentParticipants = details.matches?.reduce((acc, m) => {
    if (m.player1 && !acc.find(p => p.id === m.player1?.id)) acc.push(m.player1);
    if (m.player2 && !acc.find(p => p.id === m.player2?.id)) acc.push(m.player2);
    return acc;
  }, [] as PlayerDto[]) || [];

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
                  {currentParticipants.map(p => <option key={`gold-${p.id}`} value={p.id}>{p.firstName} {p.lastName}</option>)}
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
                  {currentParticipants.map(p => <option key={`silver-${p.id}`} value={p.id}>{p.firstName} {p.lastName}</option>)}
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
                  {currentParticipants.map(p => <option key={`bronze-${p.id}`} value={p.id}>{p.firstName} {p.lastName}</option>)}
                </select>
              </div>
           </div>
           <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
             <button className={styles.cancelBtn} onClick={() => setShowSetWinners(false)}>Скасувати</button>
             <button className={styles.saveBtn} onClick={handleSaveWinners}>Зберегти призерів</button>
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
                    onClick={() => setShowAddPlayer(!showAddPlayer)}
                    style={{ background: 'none', border: 'none', color: '#3b82f6', cursor: 'pointer', fontSize: '14px' }}
                >
                    {showAddPlayer ? 'Закрити' : '+ Додати'}
                </button>
              )}
            </div>

            {showAddPlayer && (
              <div style={{ background: '#f9fafb', padding: '15px', borderRadius: '8px', marginBottom: '15px', border: '1px solid #e5e7eb' }}>
                <input 
                    placeholder="Ім'я" 
                    value={newParticipant.firstName} 
                    onChange={(e) => setNewParticipant({...newParticipant, firstName: e.target.value})}
                    style={{ width: '100%', padding: '8px', marginBottom: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                />
                <input 
                    placeholder="Прізвище" 
                    value={newParticipant.lastName} 
                    onChange={(e) => setNewParticipant({...newParticipant, lastName: e.target.value})}
                    style={{ width: '100%', padding: '8px', marginBottom: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                />
                <input 
                    placeholder="Група" 
                    value={newParticipant.group} 
                    onChange={(e) => setNewParticipant({...newParticipant, group: e.target.value})}
                    style={{ width: '100%', padding: '8px', marginBottom: '12px', borderRadius: '6px', border: '1px solid #ccc' }}
                />
                <button className={styles.saveBtn} style={{ width: '100%' }} onClick={handleAddParticipant}>Зберегти гравця</button>
              </div>
            )}

            <div className={styles.participantsList}>
              {currentParticipants.map(player => (
                <div key={player.id} className={styles.participantItem}>
                  <div className={styles.playerAvatar}>
                    {player.firstName[0]}{player.lastName[0]}
                  </div>
                  <div className={styles.playerInfo}>
                    <div className={styles.playerName}>{player.firstName} {player.lastName}</div>
                    <div className={styles.playerDetails}>{player.group} • Допущений</div>
                  </div>
                </div>
              ))}
              {currentParticipants.length === 0 && (
                <p className={styles.noData}>Учасників ще не додано до поєдинків</p>
              )}
            </div>
          </div>

          {/* Referees */}
          <div className={styles.sideSection}>
            <h3><i className="fas fa-user-shield"></i> Суддівська колегія</h3>
            <div className={styles.refereesBoard}>
              <div className={styles.headReferee}>
                <strong>Головний суддя:</strong> {details.headRefereeName || 'Не призначено'}
              </div>
              <p className={styles.refereeNote}>
                До складу колегії входять {details.refereeIds?.length || 0} сертифікованих суддів.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ArmwrestlingTournamentView;
