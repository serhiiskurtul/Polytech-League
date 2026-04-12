import React from 'react';
import { ArmwrestlingMatchResponse } from '@/shared/types/armwrestling.types';
import styles from '../TournamentViewPage.module.css';

interface ArmwrestlingBracketProps {
  matches: ArmwrestlingMatchResponse[];
  isAdmin?: boolean;
  onSetWinner?: (matchId: number, winnerId: number) => void;
}

const ArmwrestlingBracket: React.FC<ArmwrestlingBracketProps> = ({ 
  matches, 
  isAdmin, 
  onSetWinner 
}) => {
  // Group matches by stage
  const stages = matches.reduce((acc, match) => {
    const stage = match.stage || 'Раунд 1';
    if (!acc[stage]) acc[stage] = [];
    acc[stage].push(match);
    return acc;
  }, {} as Record<string, ArmwrestlingMatchResponse[]>);

  // Preferred order of stages
  const stageOrder = [
    'Раунд 1', 'Раунд 2', 'Раунд 3', 'Раунд 4', 'Раунд 5',
    '1/16 Фіналу', '1/8 Фіналу', 'Чвертьфінал', 'Півфінал', 'Фінал'
  ];

  const sortedStages = Object.keys(stages).sort((a, b) => {
    const idxA = stageOrder.indexOf(a);
    const idxB = stageOrder.indexOf(b);
    if (idxA === -1 && idxB === -1) return a.localeCompare(b);
    if (idxA === -1) return 1;
    if (idxB === -1) return -1;
    return idxA - idxB;
  });

  return (
    <div className={styles.bracketContainer}>
      {sortedStages.map((stageName) => (
        <div key={stageName} className={styles.bracketColumn}>
          <h3 className={styles.stageTitle}>{stageName}</h3>
          <div className={styles.matchList}>
            {stages[stageName].map((match) => (
              <div key={match.id} className={styles.matchBox}>
                {/* Player 1 */}
                <div className={`${styles.playerRow} ${match.winnerId === match.player1Id ? styles.winner : ''}`}>
                  <div className={styles.playerInfo}>
                    <span className={styles.playerName}>
                      {match.player1?.firstName} {match.player1?.lastName || '???'}
                    </span>
                    <span className={styles.playerGroup}>{match.player1?.group}</span>
                  </div>
                  {isAdmin && onSetWinner && (
                    <button 
                      className={`${styles.setWinnerBtn} ${match.winnerId === match.player1Id ? styles.winnerActive : ''}`}
                      onClick={() => onSetWinner(match.id, match.player1Id)}
                      title="Встановити переможцем"
                    >
                      <i className="fas fa-trophy"></i>
                    </button>
                  )}
                  {match.winnerId === match.player1Id && !isAdmin && (
                    <i className="fas fa-crown" style={{ color: '#166534', fontSize: '12px' }}></i>
                  )}
                </div>

                <div className={styles.matchVs}>VS</div>

                {/* Player 2 */}
                <div className={`${styles.playerRow} ${match.winnerId === match.player2Id ? styles.winner : ''}`}>
                  <div className={styles.playerInfo}>
                    <span className={styles.playerName}>
                      {match.player2?.firstName} {match.player2?.lastName || '???'}
                    </span>
                    <span className={styles.playerGroup}>{match.player2?.group}</span>
                  </div>
                  {isAdmin && onSetWinner && (
                    <button 
                      className={`${styles.setWinnerBtn} ${match.winnerId === match.player2Id ? styles.winnerActive : ''}`}
                      onClick={() => onSetWinner(match.id, match.player2Id)}
                      title="Встановити переможцем"
                    >
                      <i className="fas fa-trophy"></i>
                    </button>
                  )}
                  {match.winnerId === match.player2Id && !isAdmin && (
                    <i className="fas fa-crown" style={{ color: '#166534', fontSize: '12px' }}></i>
                  )}
                </div>

                {match.tableNumber > 0 && (
                  <div className={styles.tableTag}>Стіл {match.tableNumber}</div>
                )}
              </div>
            ))}
          </div>
        </div>
      ))}
      
      {matches.length === 0 && (
        <div className={styles.noData}>Сітка порожня. Додайте матчі для відображення.</div>
      )}
    </div>
  );
};

export default ArmwrestlingBracket;
