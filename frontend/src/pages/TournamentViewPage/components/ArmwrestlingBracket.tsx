import React from 'react';
import { ArmwrestlingMatchResponse, ArmwrestlingTournamentType } from '@/shared/types/armwrestling.types';
import styles from '../TournamentViewPage.module.css';

interface ArmwrestlingBracketProps {
  matches: ArmwrestlingMatchResponse[];
  tournamentType?: ArmwrestlingTournamentType;
  isAdmin?: boolean;
  onSetWinner?: (matchId: number, winnerId: number) => void;
  onDeleteMatch?: (matchId: number) => void;
}

const STAGE_ORDER = [
  'Раунд 1', 'Раунд 2', 'Раунд 3', 'Раунд 4', 'Раунд 5',
  '1/16 Фіналу', '1/8 Фіналу', 'Чвертьфінал', 'Півфінал', 'Фінал',
  'Lower Раунд 1', 'Lower Раунд 2', 'Lower Раунд 3', 'Lower Півфінал', 'Lower Фінал', 'Grand Final'
];

const ArmwrestlingBracket: React.FC<ArmwrestlingBracketProps> = ({
  matches,
  tournamentType = 'SingleElimination',
  isAdmin,
  onSetWinner,
  onDeleteMatch
}) => {
  // Group matches by stage
  const stages = matches.reduce((acc, match) => {
    const stage = match.stage || 'Раунд 1';
    if (!acc[stage]) acc[stage] = [];
    acc[stage].push(match);
    return acc;
  }, {} as Record<string, ArmwrestlingMatchResponse[]>);

  // Split matches into Winners and Losers for Double Elimination
  const isDouble = tournamentType === 'DoubleElimination';
  
  const winnersStages = STAGE_ORDER.filter(s => !s.startsWith('Lower') && s !== 'Grand Final' && stages[s]);
  const losersStages = STAGE_ORDER.filter(s => s.startsWith('Lower') && stages[s]);
  const grandFinalStage = stages['Grand Final'] ? ['Grand Final'] : [];

  const renderStage = (stageName: string, columnIndex: number, totalStages: number) => (
    <div key={stageName} className={styles.bracketColumn}>
      <h3 className={styles.stageTitle}>
        {stageName.includes('Lower') ? <i className="fas fa-arrow-down-left" style={{ color: '#ef4444', marginRight: '8px' }}></i> : null}
        {stageName}
      </h3>
      <div className={styles.matchList} style={{ height: '100%', minHeight: '400px', justifyContent: 'space-around' }}>
        {stages[stageName].map((match, matchIndex) => {
          const isWinner1 = match.winnerId === match.player1Id;
          const isWinner2 = match.winnerId === match.player2Id;
          
          return (
            <div key={match.id} className={styles.matchBoxContainer} style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <div className={styles.matchBox} style={{ position: 'relative' }}>
                {isAdmin && onDeleteMatch && (
                  <button 
                    className={styles.deleteMatchBtn} 
                    onClick={() => onDeleteMatch(match.id)}
                    title="Видалити матч"
                    style={{
                      position: 'absolute',
                      top: '-10px',
                      right: '-10px',
                      width: '24px',
                      height: '24px',
                      borderRadius: '50%',
                      background: '#fee2e2',
                      color: '#ef4444',
                      border: '1px solid #fecaca',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: '10px',
                      cursor: 'pointer',
                      zIndex: 10
                    }}
                  >
                    <i className="fas fa-trash"></i>
                  </button>
                )}
                {/* Player 1 */}
                <div className={`${styles.playerRow} ${isWinner1 ? styles.winner : ''}`}>
                  <div className={styles.playerInfo}>
                    <span className={styles.playerName}>
                      {match.player1?.firstName} {match.player1?.lastName || '???'}
                    </span>
                    <span className={styles.playerGroup}>{match.player1?.group}</span>
                  </div>
                  {isAdmin && onSetWinner && (
                    <button
                      className={`${styles.setWinnerBtn} ${isWinner1 ? styles.winnerActive : ''}`}
                      onClick={() => onSetWinner(match.id, match.player1Id)}
                      title="Встановити переможцем"
                    >
                      <i className="fas fa-trophy"></i>
                    </button>
                  )}
                  {isWinner1 && !isAdmin && (
                    <i className="fas fa-crown" style={{ color: '#166534', fontSize: '12px' }}></i>
                  )}
                </div>

                <div className={styles.matchVs}>VS</div>

                {/* Player 2 */}
                <div className={`${styles.playerRow} ${isWinner2 ? styles.winner : ''}`}>
                  <div className={styles.playerInfo}>
                    <span className={styles.playerName}>
                      {match.player2?.firstName} {match.player2?.lastName || '???'}
                    </span>
                    <span className={styles.playerGroup}>{match.player2?.group}</span>
                  </div>
                  {isAdmin && onSetWinner && (
                    <button
                      className={`${styles.setWinnerBtn} ${isWinner2 ? styles.winnerActive : ''}`}
                      onClick={() => onSetWinner(match.id, match.player2Id)}
                      title="Встановити переможцем"
                    >
                      <i className="fas fa-trophy"></i>
                    </button>
                  )}
                  {isWinner2 && !isAdmin && (
                    <i className="fas fa-crown" style={{ color: '#166534', fontSize: '12px' }}></i>
                  )}
                </div>

                {match.tableNumber > 0 && (
                  <div className={styles.tableTag}>Стіл {match.tableNumber}</div>
                )}
              </div>

              {/* Enhanced Connector lines */}
              {columnIndex < totalStages - 1 && (
                <div className={styles.connectorWrapper} style={{ position: 'absolute', right: '-48px', height: '100%', width: '48px', pointerEvents: 'none' }}>
                   {/* Horizontal line from match to vertical rail */}
                   <div className={styles.connectorLine} style={{ width: '24px', height: '2px', top: '50%', right: '24px', background: '#cbd5e1', position: 'absolute' }}></div>
                   
                   {/* Vertical line connecting pairs */}
                   {matchIndex % 2 === 0 ? (
                     <div className={styles.connectorLine} style={{ width: '2px', height: '50.5%', top: '50%', right: '24px', background: '#cbd5e1', position: 'absolute' }}></div>
                   ) : (
                     <div className={styles.connectorLine} style={{ width: '2px', height: '50.5%', bottom: '50%', right: '24px', background: '#cbd5e1', position: 'absolute' }}></div>
                   )}

                   {/* Line to next stage (only for even matches to represent the pair) */}
                   {matchIndex % 2 === 0 && (
                     <div className={styles.connectorLine} style={{ width: '24px', height: '2px', top: '100%', right: '0', background: '#cbd5e1', position: 'absolute' }}></div>
                   )}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );

  return (
    <div className={styles.bracketContainer}>
      {isDouble ? (
        <div className={styles.doubleBracketWrapper}>
          <div className={styles.bracketSection}>
            <h2 className={styles.bracketSectionTitle}><i className="fas fa-trophy" style={{ color: '#f59e0b' }}></i> Winners Bracket</h2>
            <div className={styles.stagesRow}>
               {winnersStages.map((s, idx) => renderStage(s, idx, winnersStages.length))}
            </div>
          </div>
          
          <div className={styles.bracketSection} style={{ marginTop: '60px' }}>
            <h2 className={styles.bracketSectionTitle} style={{ color: '#ef4444' }}><i className="fas fa-heart-broken"></i> Losers Bracket</h2>
            <div className={styles.stagesRow}>
               {losersStages.map((s, idx) => renderStage(s, idx, losersStages.length))}
            </div>
          </div>

          {grandFinalStage.length > 0 && (
            <div className={styles.bracketSection} style={{ marginTop: '60px', alignItems: 'center', display: 'flex', flexDirection: 'column' }}>
               <h2 className={styles.bracketSectionTitle} style={{ color: '#7c3aed' }}><i className="fas fa-star"></i> Grand Final</h2>
               {renderStage('Grand Final', 0, 1)}
            </div>
          )}
        </div>
      ) : (
        <div className={styles.stagesRow}>
          {winnersStages.map((s, idx) => renderStage(s, idx, winnersStages.length))}
        </div>
      )}

      {matches.length === 0 && (
        <div className={styles.noData}>Сітка порожня. Додайте матчі для відображення.</div>
      )}
    </div>
  );
};

export default ArmwrestlingBracket;
