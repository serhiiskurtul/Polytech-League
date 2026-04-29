import React from 'react';
import { FutsalMatchResponse } from '@/shared/types/futsal.types';
import styles from '../TournamentViewPage.module.css';

interface FutsalBracketProps {
  matches: FutsalMatchResponse[];
  isAdmin?: boolean;
  onDeleteMatch?: (matchId: number) => void;
  onOpenScorers?: (match: FutsalMatchResponse) => void;
}

const FutsalBracket: React.FC<FutsalBracketProps> = ({
  matches,
  isAdmin,
  onDeleteMatch,
  onOpenScorers
}) => {
  // Group matches by stage
  const stages = matches.reduce((acc, match) => {
    const stage = match.stage || 'Раунд 1';
    if (!acc[stage]) acc[stage] = [];
    acc[stage].push(match);
    return acc;
  }, {} as Record<string, FutsalMatchResponse[]>);

  // Dynamic sorting logic
  const getStageRank = (stage: string) => {
    const standardOrder: Record<string, number> = {
      '1/16 Фіналу': 100,
      '1/8 Фіналу': 110,
      'Чвертьфінал': 120,
      'Півфінал': 130,
      'Фінал': 140
    };
    if (standardOrder[stage]) return standardOrder[stage];

    const roundMatch = stage.match(/Раунд (\d+)/);
    if (roundMatch) return parseInt(roundMatch[1]);

    return 999;
  };

  const allStages = Object.keys(stages).sort((a, b) => getStageRank(a) - getStageRank(b));

  const renderStage = (stageName: string, columnIndex: number, totalStages: number) => (
    <div key={stageName} className={styles.bracketColumn}>
      <h3 className={styles.stageTitle}>{stageName}</h3>
      <div className={styles.matchList}>
        {stages[stageName].map((match, matchIndex) => {
          const isHomeWinner = (match.homeTeamScore || 0) > (match.awayTeamScore || 0);
          const isAwayWinner = (match.awayTeamScore || 0) > (match.homeTeamScore || 0);
          
          return (
            <div key={match.id} className={styles.matchBoxContainer}>
              <div className={styles.matchBox} style={{ minWidth: '220px' }}>
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

                {/* Team 1 (Home) */}
                <div className={`${styles.playerRow} ${isHomeWinner ? styles.winner : ''}`}>
                  <div className={styles.playerInfo}>
                    <div 
                        className={styles.teamAvatarMini} 
                        style={{ 
                            width: '24px', 
                            height: '24px', 
                            background: isHomeWinner ? '#f59e0b' : '#e2e8f0',
                            color: isHomeWinner ? 'white' : '#64748b',
                            borderRadius: '6px',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            fontSize: '10px',
                            fontWeight: 800,
                            marginRight: '8px'
                        }}
                    >
                        {match.homeTeam?.name?.[0] || 'T'}
                    </div>
                    <span className={styles.playerName} style={{ fontSize: '13px' }}>
                      {match.homeTeam?.name || 'Home'}
                    </span>
                  </div>
                  <div className={styles.matchScoreBadge} style={{ fontWeight: 800, fontSize: '14px', marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '6px' }}>
                    {match.homeTeamScore ?? 0}
                    {isHomeWinner && <i className="fas fa-crown" style={{ color: '#f59e0b', fontSize: '10px' }}></i>}
                  </div>
                </div>

                <div className={styles.matchVs} style={{ margin: '4px 0', fontSize: '10px', opacity: 0.5 }}>VS</div>

                {/* Team 2 (Away) */}
                <div className={`${styles.playerRow} ${isAwayWinner ? styles.winner : ''}`}>
                  <div className={styles.playerInfo}>
                    <div 
                        className={styles.teamAvatarMini} 
                        style={{ 
                            width: '24px', 
                            height: '24px', 
                            background: isAwayWinner ? '#f59e0b' : '#e2e8f0',
                            color: isAwayWinner ? 'white' : '#64748b',
                            borderRadius: '6px',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            fontSize: '10px',
                            fontWeight: 800,
                            marginRight: '8px'
                        }}
                    >
                        {match.awayTeam?.name?.[0] || 'T'}
                    </div>
                    <span className={styles.playerName} style={{ fontSize: '13px' }}>
                      {match.awayTeam?.name || 'Away'}
                    </span>
                  </div>
                  <div className={styles.matchScoreBadge} style={{ fontWeight: 800, fontSize: '14px', marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '6px' }}>
                    {match.awayTeamScore ?? 0}
                    {isAwayWinner && <i className="fas fa-crown" style={{ color: '#f59e0b', fontSize: '10px' }}></i>}
                  </div>
                </div>

                {isAdmin && onOpenScorers && (
                   <button 
                    className={styles.bracketScorersBtn}
                    onClick={() => onOpenScorers(match)}
                    style={{
                        marginTop: '10px',
                        width: '100%',
                        padding: '6px',
                        borderRadius: '8px',
                        border: '1px solid #e2e8f0',
                        background: '#f8fafc',
                        fontSize: '11px',
                        fontWeight: 600,
                        color: '#64748b',
                        cursor: 'pointer',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        gap: '6px'
                    }}
                   >
                     <i className="fas fa-futbol"></i> Результати
                   </button>
                )}
              </div>

              {/* Connector lines */}
              {columnIndex < totalStages - 1 && (
                <div className={styles.connectorWrapper} style={{ 
                  position: 'absolute', 
                  right: '-48px', 
                  top: 0, 
                  bottom: 0, 
                  width: '48px', 
                  pointerEvents: 'none' 
                }}>
                   <div style={{ 
                     width: '24px', 
                     height: '2px', 
                     top: '50%', 
                     left: 0, 
                     background: '#94a3b8', 
                     position: 'absolute' 
                   }}></div>
                   
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

                   {matchIndex % 2 === 0 && stages[allStages[columnIndex + 1]]?.length > (matchIndex / 2) && (
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
      <div className={styles.stagesRow} style={{ display: 'flex', gap: '48px', flex: 1, minHeight: '600px', overflowX: 'auto', padding: '20px 0' }}>
        {allStages.map((s, idx) => renderStage(s, idx, allStages.length))}
      </div>

      {matches.length === 0 && (
        <div className={styles.noData}>Сітка порожня. Додайте матчі для відображення.</div>
      )}
    </div>
  );
};

export default FutsalBracket;
