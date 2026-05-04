import React, { useState } from 'react';
import toast from 'react-hot-toast';
import { 
  AthleticsEventResponse, 
  AthleticsResultResponse 
} from '@/shared/types/athletics.types';
import api from '@/shared/api/axiosInstance';
import styles from '../TournamentViewPage.module.css';
import PlayerSearchInput from './PlayerSearchInput';
import { RegistrationStatus } from '@/shared/types/tournament.types';
import { PlayerDto, Gender } from '@/shared/types/player.types';
import { createPlayer, searchUsers, getAllPlayers } from '@/shared/api';

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

interface AthleticsTournamentViewProps {
  details: AthleticsEventResponse;
  allPlayers: any[];
  isAdmin: boolean;
  onRefresh: () => void;
}

const AthleticsTournamentView: React.FC<AthleticsTournamentViewProps> = ({
  details,
  allPlayers,
  isAdmin,
  onRefresh
}) => {
  const [showAddResult, setShowAddResult] = useState(false);
  const [showParticipants, setShowParticipants] = useState(false);
  const [showAddParticipant, setShowAddParticipant] = useState(false);
  
  const [newResult, setNewResult] = useState({
    disciplineName: '',
    playerId: undefined as number | undefined,
    teamName: '',
    score: '',
    rank: 1
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

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);

  const handleAddResult = async () => {
    if (!newResult.disciplineName) {
      toast.error('Введіть назву дисципліни');
      return;
    }

    try {
      await api.post(`/athletics-events/${details.id}/results`, newResult);
      toast.success('Результат успішно додано');
      setShowAddResult(false);
      setNewResult({
        disciplineName: '',
        playerId: undefined,
        teamName: '',
        score: '',
        rank: 1
      });
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при додаванні результату');
    }
  };

  const handleDeleteResult = async (resultId: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей результат?')) return;

    try {
      await api.delete(`/athletics-events/${details.id}/results/${resultId}`);
      toast.success('Результат видалено');
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при видаленні результату');
    }
  };

  const handleSearch = async (query: string) => {
    if (query.length < 2) {
      setSearchResults([]);
      return;
    }
    setIsSearching(true);
    try {
      // Search in existing players first (client-side filter for now or add API if needed)
      // For simplicity, we'll use searchUsers which searches both users and potentially players if implemented so
      const users = await searchUsers(query);
      const results: SearchResult[] = users.map(u => ({
        type: 'user',
        id: u.id,
        userId: u.id,
        firstName: u.firstName,
        lastName: u.lastName,
        email: u.email,
        group: u.group
      }));
      setSearchResults(results);
    } catch (err) {
      console.error(err);
    } finally {
      setIsSearching(false);
    }
  };

  const handleSelectSearchResult = (result: SearchResult) => {
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

  const handleAddParticipant = async () => {
    if (!newParticipant.firstName || !newParticipant.lastName || !newParticipant.group) {
      toast.error("Заповніть всі обов'язкові поля!");
      return;
    }

    try {
      let playerId = newParticipant.existingPlayerId;

      if (!playerId) {
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
      }

      await api.post(`/athletics-events/${details.id}/participants`, { 
        playerId, 
        groupName: newParticipant.group 
      });
      
      toast.success('Учасника успішно додано');
      setShowAddParticipant(false);
      setNewParticipant({ firstName: '', lastName: '', patronymic: '', group: '', userId: null, existingPlayerId: null, gender: 'None' });
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при додаванні учасника');
    }
  };

  const handleRemoveParticipant = async (playerId: number) => {
    if (!window.confirm('Видалити учасника зі змагання?')) return;
    try {
      await api.delete(`/athletics-events/${details.id}/participants/${playerId}`);
      toast.success('Учасника видалено');
      onRefresh();
    } catch (err) {
      console.error(err);
      toast.error('Помилка при видаленні учасника');
    }
  };

  // Group results by discipline
  const groupedResults = details.results.reduce((acc, result) => {
    if (!acc[result.disciplineName]) {
      acc[result.disciplineName] = [];
    }
    acc[result.disciplineName].push(result);
    return acc;
  }, {} as Record<string, AthleticsResultResponse[]>);

  // Sort results within each discipline by rank
  Object.keys(groupedResults).forEach(discipline => {
    groupedResults[discipline].sort((a, b) => a.rank - b.rank);
  });

  return (
    <div className={styles.athleticsContainer}>
      {isAdmin && (
        <div className={styles.adminControls}>
          <div style={{ display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
            <button 
              className={`${styles.addBtn} ${showAddResult ? styles.active : ''}`}
              onClick={() => {
                setShowAddResult(!showAddResult);
                setShowAddParticipant(false);
                setShowParticipants(false);
              }}
            >
              <i className={`fas ${showAddResult ? 'fa-times' : 'fa-plus-circle'}`}></i>
              {showAddResult ? 'Скасувати' : 'Додати результат'}
            </button>

            <button 
              className={styles.addBtn}
              onClick={() => {
                setShowParticipants(!showParticipants);
                setShowAddResult(false);
                setShowAddParticipant(false);
              }}
            >
              <i className="fas fa-users"></i>
              {showParticipants ? 'Сховати учасників' : 'Керувати учасниками'}
            </button>
          </div>

          {showAddResult && (
            <div className={styles.addResultForm}>
              <div className={styles.formGroup}>
                <label>Дисципліна</label>
                <input 
                  type="text" 
                  value={newResult.disciplineName}
                  onChange={e => setNewResult({...newResult, disciplineName: e.target.value})}
                  placeholder="напр. Підтягування"
                />
              </div>
              <div className={styles.formGroup}>
                <PlayerSearchInput 
                  label="Гравець (якщо індивідуально)"
                  players={allPlayers}
                  selectedId={newResult.playerId || 0}
                  onSelect={(id) => setNewResult({...newResult, playerId: id, teamName: ''})}
                  onQuickAdd={() => {}}
                  placeholder="Пошук за прізвищем..."
                />
              </div>
              <div className={styles.formGroup}>
                <label>Група / Команда (якщо командно)</label>
                <input 
                  type="text" 
                  value={newResult.teamName}
                  onChange={e => setNewResult({...newResult, teamName: e.target.value, playerId: undefined})}
                  placeholder="напр. 522 група"
                />
              </div>
              <div className={styles.formGroup}>
                <label>Результат</label>
                <input 
                  type="text" 
                  value={newResult.score}
                  onChange={e => setNewResult({...newResult, score: e.target.value})}
                  placeholder="напр. 34 рази або 30.39 сек"
                />
              </div>
              <div className={styles.formGroup}>
                <label>Місце</label>
                <input 
                  type="number" 
                  value={newResult.rank}
                  onChange={e => setNewResult({...newResult, rank: parseInt(e.target.value)})}
                />
              </div>
              <div style={{ display: 'flex', gap: '12px' }}>
                <button className={styles.submitBtn} onClick={handleAddResult}>Зберегти</button>
                <button className={styles.cancelBtn} onClick={() => setShowAddResult(false)}>Скасувати</button>
              </div>
            </div>
          )}

          {showParticipants && (
            <div className={styles.addResultForm} style={{ display: 'block', borderStyle: 'solid' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h3 style={{ margin: 0 }}>Список учасників ({details.participants.length})</h3>
                <button 
                  className={styles.addBtn} 
                  onClick={() => setShowAddParticipant(!showAddParticipant)}
                  style={{ padding: '6px 12px', fontSize: '13px' }}
                >
                  {showAddParticipant ? 'Скасувати' : '+ Додати учасника'}
                </button>
              </div>

              {showAddParticipant && (
                <div style={{ background: '#f8fafc', padding: '20px', borderRadius: '16px', marginBottom: '20px', border: '1px solid #e2e8f0' }}>
                  <div style={{ marginBottom: '15px', position: 'relative' }}>
                    <label style={{ display: 'block', fontSize: '13px', fontWeight: 700, marginBottom: '8px' }}>Швидкий пошук:</label>
                    <input 
                      type="text"
                      placeholder="Почніть вводити ім'я..."
                      value={searchQuery}
                      onChange={(e) => {
                        setSearchQuery(e.target.value);
                        handleSearch(e.target.value);
                      }}
                      style={{ width: '100%', padding: '10px', borderRadius: '10px', border: '1px solid #cbd5e1' }}
                    />
                    {isSearching && <div style={{ fontSize: '11px', marginTop: '4px' }}>Пошук...</div>}
                    {searchResults.length > 0 && (
                      <div style={{
                        position: 'absolute', top: '100%', left: 0, right: 0,
                        background: 'white', border: '1px solid #e2e8f0', borderRadius: '10px',
                        zIndex: 10, boxShadow: '0 4px 12px rgba(0,0,0,0.1)', maxHeight: '200px', overflowY: 'auto'
                      }}>
                        {searchResults.map(r => (
                          <div 
                            key={r.id} 
                            onClick={() => handleSelectSearchResult(r)}
                            style={{ padding: '10px', borderBottom: '1px solid #f1f5f9', cursor: 'pointer' }}
                          >
                            <strong>{r.firstName} {r.lastName}</strong> {r.group && <small>({r.group})</small>}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px', marginBottom: '12px' }}>
                    <div className={styles.formGroup}>
                      <label>Прізвище</label>
                      <input 
                        value={newParticipant.lastName}
                        onChange={e => setNewParticipant({...newParticipant, lastName: e.target.value})}
                      />
                    </div>
                    <div className={styles.formGroup}>
                      <label>Ім'я</label>
                      <input 
                        value={newParticipant.firstName}
                        onChange={e => setNewParticipant({...newParticipant, firstName: e.target.value})}
                      />
                    </div>
                  </div>
                  <div className={styles.formGroup} style={{ marginBottom: '12px' }}>
                    <label>Група</label>
                    <input 
                      value={newParticipant.group}
                      onChange={e => setNewParticipant({...newParticipant, group: e.target.value})}
                    />
                  </div>
                  <div style={{ display: 'flex', gap: '12px' }}>
                    <button className={styles.submitBtn} onClick={handleAddParticipant}>Додати</button>
                    <button className={styles.cancelBtn} onClick={() => setShowAddParticipant(false)}>Скасувати</button>
                  </div>
                </div>
              )}

              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', gap: '12px' }}>
                {details.participants.map(p => (
                  <div key={p.playerId} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: '#f1f5f9', padding: '8px 12px', borderRadius: '10px' }}>
                    <div style={{ fontSize: '13px' }}>
                      <strong>{p.playerName}</strong>
                      <div style={{ fontSize: '11px', color: '#64748b' }}>{p.groupName}</div>
                    </div>
                    <button 
                      onClick={() => handleRemoveParticipant(p.playerId)}
                      style={{ background: 'none', border: 'none', color: '#ef4444', cursor: 'pointer' }}
                    >
                      <i className="fas fa-trash"></i>
                    </button>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      <div className={styles.disciplinesGrid}>
        {Object.keys(groupedResults).length === 0 ? (
          <div className={styles.noResults}>Результати ще не додано</div>
        ) : (
          Object.keys(groupedResults).map(discipline => (
            <div key={discipline} className={styles.disciplineCard}>
              <h3 className={styles.disciplineTitle}>{discipline}</h3>
              <div className={styles.resultsList}>
                {groupedResults[discipline].map(result => (
                  <div key={result.id} className={styles.resultItem}>
                    <div className={styles.rankBadge}>
                      {result.rank === 1 ? '🥇' : result.rank === 2 ? '🥈' : result.rank === 3 ? '🥉' : result.rank}
                    </div>
                    <div className={styles.resultInfo}>
                      <span className={styles.participantName}>
                        {result.playerName || result.teamName}
                      </span>
                      <span className={styles.scoreValue}>{result.score}</span>
                    </div>
                    {isAdmin && (
                      <button 
                        className={styles.deleteResultBtn}
                        onClick={() => handleDeleteResult(result.id)}
                      >
                        ×
                      </button>
                    )}
                  </div>
                ))}
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default AthleticsTournamentView;
