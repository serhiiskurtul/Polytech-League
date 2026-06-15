import React, { useState } from 'react';
import { AthleticsEventResponse, AthleticsResultDto } from '@/entities/sport/model/athletics.types';
import { addAthleticsResult, updateAthleticsResult, deleteAthleticsResult } from '@/shared/api';
import toast from 'react-hot-toast';
import styles from './AthleticsTournamentView.module.css';

interface AthleticsTournamentViewProps {
  details: AthleticsEventResponse;
  isAdmin: boolean;
  isFizorg: boolean;
  userGroup?: string;
  onRefresh: () => void;
}

const AthleticsTournamentView: React.FC<AthleticsTournamentViewProps> = ({ details, isAdmin, onRefresh }) => {
  const [activeTab, setActiveTab] = useState<'results' | 'team_standings'>('results');
  const [isAddingResult, setIsAddingResult] = useState(false);
  const [editingResultId, setEditingResultId] = useState<number | null>(null);
  const [playerSearch, setPlayerSearch] = useState('');
  
  const [formData, setFormData] = useState<AthleticsResultDto>({
    disciplineName: '',
    rank: 1,
    score: '',
    teamName: '',
    playerId: undefined
  });

  const [selectedPlayers, setSelectedPlayers] = useState<Array<{id: number, name: string}>>([]);

  const disciplines = Array.from(new Set(details.results?.map(r => r.disciplineName) || []));

  const handleSaveResult = async () => {
    if (!formData.disciplineName || !formData.rank) {
      toast.error('Дисципліна та місце обов\'язкові');
      return;
    }

    try {
      if (editingResultId) {
        const pId = selectedPlayers.length > 0 ? selectedPlayers[0].id : undefined;
        await updateAthleticsResult(details.id, editingResultId, { ...formData, playerId: pId });
        toast.success('Результат оновлено');
      } else {
        if (selectedPlayers.length > 0) {
          for (const p of selectedPlayers) {
            await addAthleticsResult(details.id, { ...formData, playerId: p.id });
          }
        } else {
          await addAthleticsResult(details.id, { ...formData, playerId: undefined });
        }
        toast.success('Результат додано');
      }
      setIsAddingResult(false);
      setEditingResultId(null);
      setFormData({ disciplineName: '', rank: 1, score: '', teamName: '', playerId: undefined });
      setSelectedPlayers([]);
      setPlayerSearch('');
      onRefresh();
    } catch (error) {
      toast.error('Помилка збереження результату');
      console.error(error);
    }
  };

  const handleEdit = (r: AthleticsResultDto) => {
    setFormData(r);
    setEditingResultId(r.id!);
    setIsAddingResult(true);
    
    if (r.playerId) {
      const p = details.participants?.find(x => x.playerId === r.playerId);
      if (p) {
        setSelectedPlayers([{ id: p.playerId, name: `${p.playerName} (${p.groupName})` }]);
        setPlayerSearch('');
      }
    } else {
      setSelectedPlayers([]);
      setPlayerSearch('');
    }
  };

  const handleDelete = async (resultId: number) => {
    if (!window.confirm('Видалити цей результат?')) return;
    try {
      await deleteAthleticsResult(details.id, resultId);
      toast.success('Видалено');
      onRefresh();
    } catch (e) {
      toast.error('Помилка видалення');
    }
  };

  // Group results visually
  const groupedResults = React.useMemo(() => {
    if (!details.results) return [];
    const groups: any[] = [];
    details.results.forEach(r => {
      const group = groups.find(g => 
        g.disciplineName === r.disciplineName && 
        g.rank === r.rank && 
        g.score === r.score && 
        g.teamName === r.teamName
      );
      if (group) {
        group.records.push(r);
      } else {
        groups.push({
          disciplineName: r.disciplineName,
          rank: r.rank,
          score: r.score,
          teamName: r.teamName,
          records: [r]
        });
      }
    });
    return groups;
  }, [details.results]);

  // Calculate team standings (1st=10, 2nd=8, 3rd=6, 4th=5, 5th=4, 6th=3, 7th=2, 8th=1)
  const getPoints = (rank: number) => {
    if (rank === 1) return 10;
    if (rank === 2) return 8;
    if (rank === 3) return 6;
    if (rank === 4) return 5;
    if (rank === 5) return 4;
    if (rank === 6) return 3;
    if (rank === 7) return 2;
    if (rank === 8) return 1;
    return 0;
  };

  const teamPoints: Record<string, number> = {};
  details.results?.forEach(r => {
    let team = r.teamName;
    if (!team && r.playerId) {
      const participant = details.participants?.find(p => p.playerId === r.playerId);
      if (participant && participant.groupName) {
        team = participant.groupName;
      }
    }
    if (team) {
      // In a relay, multiple players might have the same teamName and rank.
      // We don't want to award 40 points for 1st place in a relay. We should only add points per unique team/rank/discipline!
      // But this calculation is simple and runs for CollegeGroup. If they have relays in CollegeGroup, they might get extra points.
      // To fix this, let's unique it.
    }
  });

  // Re-calculate team points properly based on grouped results to avoid duplicate points for relays
  const correctTeamPoints: Record<string, number> = {};
  groupedResults.forEach(g => {
    // For each group, determine the team name
    // If teamName exists, use it. Otherwise, collect all group names from players
    let teamsForThisGroup = new Set<string>();
    if (g.teamName) {
      teamsForThisGroup.add(g.teamName);
    } else {
      g.records.forEach((r: any) => {
        if (r.playerId) {
          const participant = details.participants?.find(p => p.playerId === r.playerId);
          if (participant && participant.groupName) teamsForThisGroup.add(participant.groupName);
        }
      });
    }
    
    teamsForThisGroup.forEach(team => {
      correctTeamPoints[team] = (correctTeamPoints[team] || 0) + getPoints(g.rank);
    });
  });

  const sortedTeams = Object.entries(correctTeamPoints)
    .map(([name, points]) => ({ name, points }))
    .sort((a, b) => b.points - a.points);

  return (
    <div className={styles.container}>
      <div className={styles.tabs}>
        <button 
          className={activeTab === 'results' ? styles.activeTab : styles.tab}
          onClick={() => setActiveTab('results')}
        >
          <i className="fas fa-running"></i> Результати за дисциплінами
        </button>
        {details.level === 'CollegeGroup' && (
          <button 
            className={activeTab === 'team_standings' ? styles.activeTab : styles.tab}
            onClick={() => setActiveTab('team_standings')}
          >
            <i className="fas fa-users"></i> Командний залік
          </button>
        )}
      </div>

      {activeTab === 'results' && (
        <div className={styles.tabContent}>
          {isAdmin && (
            <div className={styles.adminControls}>
              <button onClick={() => setIsAddingResult(!isAddingResult)} className={styles.addButton}>
                <i className={isAddingResult ? "fas fa-times" : "fas fa-plus"}></i> 
                {isAddingResult ? 'Скасувати' : 'Додати результат'}
              </button>
            </div>
          )}

          {isAddingResult && (
            <div className={styles.addForm}>
              <h4>{editingResultId ? 'Редагувати результат' : 'Додати результат'}</h4>
              <div className={styles.formGrid}>
                <div className={styles.formGroup}>
                  <label>Дисципліна (напр. "Естафета 4х100")</label>
                  <input type="text" value={formData.disciplineName} onChange={e => setFormData({...formData, disciplineName: e.target.value})} placeholder="Назва дисципліни" />
                </div>
                <div className={styles.formGroup}>
                  <label>Місце</label>
                  <input type="number" min={1} value={formData.rank} onChange={e => setFormData({...formData, rank: parseInt(e.target.value) || 1})} />
                </div>
                <div className={styles.formGroup}>
                  <label>Показник (Час, Метри)</label>
                  <input type="text" value={formData.score || ''} onChange={e => setFormData({...formData, score: e.target.value})} placeholder="Напр. 11.5с" />
                </div>
                <div className={styles.formGroup}>
                  <label>Команда / Група (необов'язково)</label>
                  <input type="text" value={formData.teamName || ''} onChange={e => setFormData({...formData, teamName: e.target.value})} placeholder="Напр. Збірна області" />
                </div>
                <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                  <label>Учасники (можна додати декілька для естафети)</label>
                  <div style={{ display: 'flex', gap: '8px', marginBottom: '8px', flexWrap: 'wrap' }}>
                    {selectedPlayers.map(p => (
                      <span key={p.id} style={{ background: '#e0e7ff', color: '#4338ca', padding: '4px 8px', borderRadius: '4px', fontSize: '0.9rem', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        {p.name}
                        <i className="fas fa-times" style={{ cursor: 'pointer' }} onClick={() => setSelectedPlayers(selectedPlayers.filter(x => x.id !== p.id))}></i>
                      </span>
                    ))}
                  </div>
                  {!editingResultId && (
                    <input 
                      list="athletics-participants" 
                      value={playerSearch}
                      onChange={e => {
                         const val = e.target.value;
                         setPlayerSearch(val);
                         const match = details.participants?.find(p => `${p.playerName} (${p.groupName})` === val);
                         if (match && !selectedPlayers.find(x => x.id === match.playerId)) {
                            setSelectedPlayers([...selectedPlayers, { id: match.playerId, name: val }]);
                            setPlayerSearch('');
                         }
                      }}
                      placeholder="Пошук та додавання учасників..." 
                      autoComplete="off"
                    />
                  )}
                  <datalist id="athletics-participants">
                    {details.participants?.map(p => (
                      <option key={p.playerId} value={`${p.playerName} (${p.groupName})`} />
                    ))}
                  </datalist>
                </div>
              </div>
              <button onClick={handleSaveResult} className={styles.saveButton}>Зберегти</button>
            </div>
          )}

          <div className={styles.disciplinesList}>
            {disciplines.length === 0 && !isAddingResult && <p className={styles.noData}>Немає доданих результатів</p>}
            
            {disciplines.map(discipline => (
              <div key={discipline} className={styles.disciplineCard}>
                <h3 className={styles.disciplineTitle}>{discipline}</h3>
                <table className={styles.resultsTable}>
                  <thead>
                    <tr>
                      <th style={{ width: '60px' }}>Місце</th>
                      <th>Спортсмен(и) / Команда</th>
                      <th>Показник</th>
                      {isAdmin && <th>Дії</th>}
                    </tr>
                  </thead>
                  <tbody>
                    {groupedResults
                      .filter(g => g.disciplineName === discipline)
                      .sort((a, b) => a.rank - b.rank)
                      .map((g, idx) => (
                        <tr key={idx}>
                          <td className={styles.rankCell}>
                            {g.rank === 1 ? '🥇' : g.rank === 2 ? '🥈' : g.rank === 3 ? '🥉' : `${g.rank}.`}
                          </td>
                          <td>
                            {g.teamName && <strong>{g.teamName}</strong>}
                            {g.teamName && g.records.some((r: any) => r.playerId) && <br/>}
                            {g.records.map((r: any, i: number) => (
                              <span key={r.id}>
                                {r.playerId ? <strong>{r.playerName}</strong> : ''}
                                {r.playerId && !g.teamName && (
                                  <span className={styles.subtext}>
                                    {' '}
                                    ({details.participants?.find(p => p.playerId === r.playerId)?.groupName})
                                  </span>
                                )}
                                {i < g.records.length - 1 && r.playerId ? ', ' : ''}
                              </span>
                            ))}
                          </td>
                          <td className={styles.scoreCell}>{g.score || '—'}</td>
                          {isAdmin && (
                            <td className={styles.actionsCell}>
                              <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                                {g.records.map((r: any) => (
                                  <div key={r.id} style={{ display: 'flex', border: '1px solid #e5e7eb', borderRadius: '4px', overflow: 'hidden' }}>
                                    <button onClick={() => handleEdit(r)} className={styles.iconBtn} title={`Редагувати ${r.playerName || 'результат'}`} style={{ padding: '4px', borderRight: '1px solid #e5e7eb' }}><i className="fas fa-edit" style={{fontSize: '12px'}}></i></button>
                                    <button onClick={() => handleDelete(r.id!)} className={styles.iconBtn} title={`Видалити ${r.playerName || 'результат'}`} style={{ padding: '4px' }}><i className="fas fa-trash" style={{fontSize: '12px'}}></i></button>
                                  </div>
                                ))}
                              </div>
                            </td>
                          )}
                        </tr>
                      ))}
                  </tbody>
                </table>
              </div>
            ))}
          </div>
        </div>
      )}

      {activeTab === 'team_standings' && details.level === 'CollegeGroup' && (
        <div className={styles.tabContent}>
          <div className={styles.standingsCard}>
            <h3>Командний рейтинг</h3>
            <p className={styles.infoText}>Бали нараховуються автоматично: 1 місце - 10б, 2 - 8б, 3 - 6б, 4 - 5б, 5 - 4б, 6 - 3б, 7 - 2б, 8 - 1б.</p>
            {sortedTeams.length === 0 ? (
              <p className={styles.noData}>Немає даних для рейтингу</p>
            ) : (
              <table className={styles.resultsTable}>
                <thead>
                  <tr>
                    <th>Місце</th>
                    <th>Навчальна група / Команда</th>
                    <th>Сума балів</th>
                  </tr>
                </thead>
                <tbody>
                  {sortedTeams.map((t, index) => (
                    <tr key={t.name}>
                      <td className={styles.rankCell}>
                        {index === 0 ? '🏆 1' : index === 1 ? '🥈 2' : index === 2 ? '🥉 3' : `${index + 1}.`}
                      </td>
                      <td><strong>{t.name}</strong></td>
                      <td>{t.points}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default AthleticsTournamentView;
