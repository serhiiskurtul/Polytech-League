import React from 'react';
import { ArmwrestlingMatchResponse, ArmwrestlingTournamentType } from '@/entities/sport/model/armwrestling.types';
import styles from '../TournamentViewPage.module.css';

interface ArmwrestlingBracketProps {
  matches: ArmwrestlingMatchResponse[];
  tournamentType?: ArmwrestlingTournamentType;
  isAdmin?: boolean;
  onSetWinner?: (matchId: number, winnerId: number) => void;
  onDeleteMatch?: (matchId: number) => void;
}


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

  // Dynamic sorting logic
  const getStageRank = (stage: string) => {
    if (stage === 'Grand Final') return 1000;
    if (stage.startsWith('Lower')) {
      const match = stage.match(/Lower Раунд (\d+)/);
      if (match) return 500 + parseInt(match[1]);
      if (stage === 'Lower Півфінал') return 590;
      if (stage === 'Lower Фінал') return 595;
      return 600;
    }
    
    // Standard stages
    const standardOrder: Record<string, number> = {
      '1/16 Фіналу': 100,
      '1/8 Фіналу': 110,
      'Чвертьфінал': 120,
      'Півфінал': 130,
      'Фінал': 140
    };
    if (standardOrder[stage]) return standardOrder[stage];

    // Rounds
    const roundMatch = stage.match(/Раунд (\d+)/);
    if (roundMatch) return parseInt(roundMatch[1]);

    return 999; // Unknown
  };

  const allStages = Object.keys(stages).sort((a, b) => getStageRank(a) - getStageRank(b));
  
  const winnersStages = allStages.filter(s => !s.startsWith('Lower') && s !== 'Grand Final');
  const losersStages = allStages.filter(s => s.startsWith('Lower'));
  const grandFinalStage = stages['Grand Final'] ? ['Grand Final'] : [];

  const renderStage = (stageName: string, columnIndex: number, totalStages: number) => (
    <div key={stageName} className={styles.bracketColumn}>
      <h3 className={styles.stageTitle}>
        {stageName.includes('Lower') ? <i className="fas fa-arrow-down-left" style={{ color: '#ef4444', marginRight: '8px' }}></i> : null}
        {stageName}
      </h3>
      <div className={styles.matchList}>
        {stages[stageName].map((match, matchIndex) => {
          const isWinner1 = match.winnerId === match.player1Id;
          const isWinner2 = match.winnerId === match.player2Id;
          
          return (
            <div key={match.id} className={styles.matchBoxContainer}>
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

              {/* Enhanced Connector lines - CSS-based alignment */}
              {columnIndex < totalStages - 1 && (
                <div className={styles.connectorWrapper} style={{ 
                  position: 'absolute', 
                  right: '-48px', 
                  top: 0, 
                  bottom: 0, 
                  width: '48px', 
                  pointerEvents: 'none' 
                }}>
                   {/* Horizontal line from current match card center to rail */}
                   <div style={{ 
                     width: '24px', 
                     height: '2px', 
                     top: '50%', 
                     left: 0, 
                     background: '#94a3b8', 
                     position: 'absolute' 
                   }}></div>
                   
                   {/* Vertical rail connecting the pair */}
                   {matchIndex % 2 === 0 ? (
                     <div style={{ 
                       width: '2px', 
                       height: '50%', 
                       top: '50%', 
                       left: '24px', 
                       background: '#94a3b8', 
                       position: 'absolute'
                     }}></div>
                   ) : (
                     <div style={{ 
                       width: '2px', 
                       height: '50%', 
                       bottom: '50%', 
                       left: '24px', 
                       background: '#94a3b8', 
                       position: 'absolute'
                     }}></div>
                   )}

                   {/* Line to next stage - Center of the pair to the next card center */}
                   {matchIndex % 2 === 0 && stages[winnersStages[columnIndex + 1]]?.length > (matchIndex / 2) && (
                     <div style={{ 
                       width: '24px', 
                       height: '2px', 
                       top: '100%', 
                       left: '24px', 
                       background: '#94a3b8', 
                       position: 'absolute',
                       display: 'flex',
                       alignItems: 'center',
                       justifyContent: 'flex-end'
                     }}>
                       {/* Arrowhead */}
                       <div style={{ 
                         width: '6px', 
                         height: '6px', 
                         borderRight: '2px solid #94a3b8', 
                         borderTop: '2px solid #94a3b8', 
                         transform: 'rotate(45deg)',
                         marginRight: '-1px'
                       }}></div>
                     </div>
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
        <div className={styles.stagesRow} style={{ display: 'flex', gap: '48px', flex: 1, minHeight: '800px' }}>
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
