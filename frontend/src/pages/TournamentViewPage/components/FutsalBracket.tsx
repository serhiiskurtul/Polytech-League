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

  const groupedMatches = matches.reduce((acc, match) => {
    const stage = match.stage || 'Груповий етап';
    if (!acc[stage]) acc[stage] = [];
    acc[stage].push(match);
    return acc;
  }, {} as Record<string, FutsalMatchResponse[]>);

  const sortedStages = Object.keys(groupedMatches).sort((a, b) => getStageRank(a) - getStageRank(b));

  return (
    <div className={styles.futsalBracketWrapper}>
      <div className={styles.stagesRow}>
        {sortedStages.map((stageName, stageIdx) => (
          <div key={stageName} className={styles.bracketColumn}>
            <div className={styles.stageHeader}>
              <span className={styles.stageNameBadge}>{stageName}</span>
            </div>
            
            <div className={styles.matchList}>
              {groupedMatches[stageName].map((match, matchIdx) => {
                const isHomeWinner = (match.homeTeamScore || 0) > (match.awayTeamScore || 0);
                const isAwayWinner = (match.awayTeamScore || 0) > (match.homeTeamScore || 0);
                const isDraw = (match.homeTeamScore !== null && match.awayTeamScore !== null) && 
                               (match.homeTeamScore === match.awayTeamScore);

                return (
                  <div key={match.id} className={styles.matchBoxContainer}>
                    <div className={styles.futsalMatchCard}>
                      {isAdmin && onDeleteMatch && (
                        <button 
                          className={styles.cardDeleteBtn}
                          onClick={() => onDeleteMatch(match.id)}
                        >
                          <i className="fas fa-trash"></i>
                        </button>
                      )}
                      
                      {/* Team 1 */}
                      <div className={`${styles.futsalTeamRow} ${isHomeWinner ? styles.futsalWinner : isDraw ? styles.futsalDraw : ''}`}>
                        <div className={styles.futsalTeamInfo}>
                           <span className={styles.futsalTeamName}>{match.homeTeam?.name || 'TBD'}</span>
                           <span className={styles.futsalTeamGroup}>Unknown</span>
                        </div>
                        <div className={styles.futsalTeamScore}>
                          {match.homeTeamScore ?? '-'}
                          {isHomeWinner && <i className="fas fa-trophy"></i>}
                        </div>
                      </div>

                      <div className={styles.futsalVsDivider}>vs</div>

                      {/* Team 2 */}
                      <div className={`${styles.futsalTeamRow} ${isAwayWinner ? styles.futsalWinner : isDraw ? styles.futsalDraw : ''}`}>
                        <div className={styles.futsalTeamInfo}>
                           <span className={styles.futsalTeamName}>{match.awayTeam?.name || 'TBD'}</span>
                           <span className={styles.futsalTeamGroup}>Unknown</span>
                        </div>
                        <div className={styles.futsalTeamScore}>
                          {match.awayTeamScore ?? '-'}
                          {isAwayWinner && <i className="fas fa-trophy"></i>}
                        </div>
                      </div>

                      {isAdmin && onOpenScorers && (
                        <button className={styles.futsalProtocolBtn} onClick={() => onOpenScorers(match)}>
                           <i className="fas fa-file-alt"></i> Протокол
                        </button>
                      )}
                    </div>

                    {/* Connectors with Arrow */}
                    {stageIdx < sortedStages.length - 1 && (
                      <div className={styles.futsalConnector}>
                        <div className={styles.futsalLineHorizontal}></div>
                        {matchIdx % 2 === 0 ? (
                           <div className={styles.futsalLineVerticalDown}></div>
                        ) : (
                           <div className={styles.futsalLineVerticalUp}></div>
                        )}
                        {matchIdx % 2 === 0 && (
                          <div className={styles.futsalArrowHead}></div>
                        )}
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          </div>
        ))}
      </div>
      {matches.length === 0 && (
        <div className={styles.emptyBracket}>
          <i className="fas fa-sitemap"></i>
          <p>Турнірна сітка порожня.</p>
        </div>
      )}
    </div>
  );
};

export default FutsalBracket;
