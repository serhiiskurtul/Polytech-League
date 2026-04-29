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
  // Sort stages based on standard olympic order
  const STAGE_ORDER = [
    'Груповий етап',
    '1/16 Фіналу',
    '1/8 Фіналу',
    'Чвертьфінал',
    'Півфінал',
    'Фінал'
  ];

  const getStageRank = (stage: string) => {
    const idx = STAGE_ORDER.indexOf(stage);
    if (idx !== -1) return idx;
    
    const roundMatch = stage.match(/Раунд (\d+)/);
    if (roundMatch) return -10 + parseInt(roundMatch[1]);
    
    return 999;
  };

  // Group and sort matches
  const groupedMatches = matches.reduce((acc, match) => {
    const stage = match.stage || 'Груповий етап';
    if (!acc[stage]) acc[stage] = [];
    acc[stage].push(match);
    return acc;
  }, {} as Record<string, FutsalMatchResponse[]>);

  const sortedStages = Object.keys(groupedMatches).sort((a, b) => getStageRank(a) - getStageRank(b));

  const renderStage = (stageName: string, stageIdx: number, totalStages: number) => (
    <div key={stageName} className={styles.bracketColumn}>
      <div className={styles.stageHeader}>
        <span className={styles.stageNameBadge}>{stageName}</span>
        <span className={styles.stageMatchCount}>{groupedMatches[stageName].length} матчів</span>
      </div>
      
      <div className={styles.matchList}>
        {groupedMatches[stageName].map((match, matchIdx) => {
          const isHomeWinner = (match.homeTeamScore || 0) > (match.awayTeamScore || 0);
          const isAwayWinner = (match.awayTeamScore || 0) > (match.homeTeamScore || 0);
          const isPlayed = match.homeTeamScore !== null && match.awayTeamScore !== null;

          return (
            <div key={match.id} className={styles.matchBoxContainer}>
              <div className={`${styles.matchBox} ${isAdmin ? styles.adminNode : ''}`}>
                {isAdmin && onDeleteMatch && (
                  <button 
                    className={styles.deleteMatchBtn}
                    onClick={() => onDeleteMatch(match.id)}
                    title="Видалити"
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
                        fontSize: '12px',
                        cursor: 'pointer',
                        zIndex: 10
                    }}
                  >
                    <i className="fas fa-trash"></i>
                  </button>
                )}
                
                {/* Home Team */}
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
                         {match.homeTeam?.name?.[0] || 'H'}
                     </div>
                     <span className={styles.playerName}>{match.homeTeam?.name || 'TBD'}</span>
                  </div>
                  <div className={styles.matchScoreBadge} style={{ fontWeight: 800, fontSize: '14px', marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '6px' }}>
                    {match.homeTeamScore ?? 0}
                    {isHomeWinner && <i className="fas fa-crown" style={{ color: '#f59e0b', fontSize: '10px' }}></i>}
                  </div>
                </div>

                <div className={styles.matchVs}>VS</div>

                {/* Away Team */}
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
                         {match.awayTeam?.name?.[0] || 'A'}
                     </div>
                     <span className={styles.playerName}>{match.awayTeam?.name || 'TBD'}</span>
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
                    <i className="fas fa-futbol"></i> Протокол
                  </button>
                )}
              </div>

              {/* Connector lines (same as Armwrestling) */}
              {stageIdx < totalStages - 1 && (
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
                   
                   {matchIdx % 2 === 0 ? (
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

                   {matchIdx % 2 === 0 && groupedMatches[sortedStages[stageIdx + 1]]?.length > (matchIdx / 2) && (
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
        {sortedStages.map((s, idx) => renderStage(s, idx, sortedStages.length))}
      </div>

      {matches.length === 0 && (
        <div className={styles.noData}>Сітка порожня. Додайте матчі для відображення.</div>
      )}
    </div>
  );
};

export default FutsalBracket;
