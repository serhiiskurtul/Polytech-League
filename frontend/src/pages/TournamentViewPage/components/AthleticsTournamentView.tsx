import React, { useState } from 'react';
import toast from 'react-hot-toast';
import { 
  AthleticsEventResponse, 
  AthleticsResultResponse 
} from '@/shared/types/athletics.types';
import api from '@/shared/api/axiosInstance';
import styles from '../TournamentViewPage.module.css';
import PlayerSearchInput from './PlayerSearchInput';

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
  const [newResult, setNewResult] = useState({
    disciplineName: '',
    playerId: undefined as number | undefined,
    teamName: '',
    score: '',
    rank: 1
  });

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
          <button 
            className={styles.addBtn}
            onClick={() => setShowAddResult(!showAddResult)}
          >
            {showAddResult ? 'Скасувати' : 'Додати результат'}
          </button>

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
              <button className={styles.submitBtn} onClick={handleAddResult}>Зберегти</button>
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
