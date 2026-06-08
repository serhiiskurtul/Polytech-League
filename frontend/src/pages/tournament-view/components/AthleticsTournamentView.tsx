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
  
  const [formData, setFormData] = useState<AthleticsResultDto>({
    disciplineName: '',
    rank: 1,
    score: '',
    teamName: '',
    playerId: undefined
  });

  const disciplines = Array.from(new Set(details.results?.map(r => r.disciplineName) || []));

  const handleSaveResult = async () => {
    if (!formData.disciplineName || !formData.rank) {
      toast.error('Дисципліна та місце обов\'язкові');
      return;
    }

    try {
      if (editingResultId) {
        await updateAthleticsResult(details.id, editingResultId, formData);
        toast.success('Результат оновлено');
      } else {
        await addAthleticsResult(details.id, formData);
        toast.success('Результат додано');
      }
      setIsAddingResult(false);
      setEditingResultId(null);
      setFormData({ disciplineName: '', rank: 1, score: '', teamName: '', playerId: undefined });
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
    // If teamName is present, give points to team. If player is present, try to extract group from participant list.
    let team = r.teamName;
    if (!team && r.playerId) {
      const participant = details.participants?.find(p => p.playerId === r.playerId);
      if (participant && participant.groupName) {
        team = participant.groupName;
      }
    }
    if (team) {
      teamPoints[team] = (teamPoints[team] || 0) + getPoints(r.rank);
    }
  });

  const sortedTeams = Object.entries(teamPoints)
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
        <button 
          className={activeTab === 'team_standings' ? styles.activeTab : styles.tab}
          onClick={() => setActiveTab('team_standings')}
        >
          <i className="fas fa-users"></i> Командний залік
        </button>
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
                  <label>Дисципліна (напр. "100 метрів (Юнаки)")</label>
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
                  <label>Команда / Група (для естафети)</label>
                  <input type="text" value={formData.teamName || ''} onChange={e => setFormData({...formData, teamName: e.target.value})} placeholder="Напр. 522 група" />
                </div>
                <div className={styles.formGroup}>
                  <label>Учасник</label>
                  <select value={formData.playerId || ''} onChange={e => setFormData({...formData, playerId: e.target.value ? parseInt(e.target.value) : undefined})}>
                    <option value="">-- Виберіть учасника (для особистих) --</option>
                    {details.participants?.map(p => (
                      <option key={p.playerId} value={p.playerId}>{p.playerName} ({p.groupName})</option>
                    ))}
                  </select>
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
                      <th>Спортсмен / Команда</th>
                      <th>Показник</th>
                      {isAdmin && <th>Дії</th>}
                    </tr>
                  </thead>
                  <tbody>
                    {(details.results || [])
                      .filter(r => r.disciplineName === discipline)
                      .sort((a, b) => a.rank - b.rank)
                      .map(r => (
                        <tr key={r.id}>
                          <td className={styles.rankCell}>
                            {r.rank === 1 ? '🥇' : r.rank === 2 ? '🥈' : r.rank === 3 ? '🥉' : `${r.rank}.`}
                          </td>
                          <td>
                            {r.playerId ? (
                              <strong>{r.playerName}</strong>
                            ) : (
                              <strong>{r.teamName}</strong>
                            )}
                            {r.playerId && r.teamName && <span className={styles.subtext}> ({r.teamName})</span>}
                            {r.playerId && !r.teamName && (
                              <span className={styles.subtext}>
                                {' '}
                                ({details.participants?.find(p => p.playerId === r.playerId)?.groupName})
                              </span>
                            )}
                          </td>
                          <td className={styles.scoreCell}>{r.score || '—'}</td>
                          {isAdmin && (
                            <td className={styles.actionsCell}>
                              <button onClick={() => handleEdit(r)} className={styles.iconBtn}><i className="fas fa-edit"></i></button>
                              <button onClick={() => handleDelete(r.id!)} className={styles.iconBtn}><i className="fas fa-trash"></i></button>
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

      {activeTab === 'team_standings' && (
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
