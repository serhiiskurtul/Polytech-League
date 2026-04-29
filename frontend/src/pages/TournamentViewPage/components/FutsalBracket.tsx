import React from 'react';
import { FutsalMatchResponse } from '@/shared/types/futsal.types';
import styles from '../TournamentViewPage.module.css';

interface FutsalBracketProps {
  matches: FutsalMatchResponse[];
  isAdmin?: boolean;
  onDeleteMatch?: (matchId: number) => void;
  onOpenScorers?: (match: FutsalMatchResponse) => void;
  onViewRoster?: (teamId: number) => void;
  onViewInfo?: (match: FutsalMatchResponse) => void;
  teams?: any[]; // For name lookup
}

const FutsalBracket: React.FC<FutsalBracketProps> = ({
  matches,
  isAdmin,
  onDeleteMatch,
  onOpenScorers,
  onViewRoster,
  onViewInfo,
  teams = []
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

  const getPlayerName = (teamId: number, playerId: number | null | undefined) => {
    if (!playerId) return null;
    const team = teams.find(t => t.id === teamId);
    if (!team) return null;
    const player = team.players?.find((p: any) => p.id === playerId);
    if (!player) return null;
    return `${player.lastName} ${player.firstName[0]}.`;
  };

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

                const homeCaptain = getPlayerName(match.homeTeamId, match.homeCaptainId);
                const homeGK = getPlayerName(match.homeTeamId, match.homeGoalkeeperId);
                const awayCaptain = getPlayerName(match.awayTeamId, match.awayCaptainId);
                const awayGK = getPlayerName(match.awayTeamId, match.awayGoalkeeperId);

                return (
                  <div key={match.id} className={styles.matchBoxContainer}>
                    <div 
                      className={styles.futsalMatchCard} 
                      onClick={() => onViewInfo && onViewInfo(match)}
                      style={{ cursor: onViewInfo ? 'pointer' : 'default' }}
                    >
                      {isAdmin && onDeleteMatch && (
                        <button 
                          className={styles.cardDeleteBtn}
                          onClick={() => onDeleteMatch(match.id)}
                          title="Видалити матч"
                        >
                          <i className="fas fa-trash"></i>
                        </button>
                      )}
                      
                      {/* Team 1 */}
                      <div className={`${styles.futsalTeamRow} ${isHomeWinner ? styles.futsalWinner : isDraw ? styles.futsalDraw : ''}`}>
                        <div className={styles.futsalTeamInfo}>
                           <div className={styles.futsalTeamHeader}>
                             <span className={styles.futsalTeamName}>{match.homeTeam?.name || 'TBD'}</span>
                             {onViewRoster && (
                               <button className={styles.rosterMiniBtn} onClick={() => onViewRoster(match.homeTeamId)} title="Переглянути склад">
                                 <i className="fas fa-users"></i>
                               </button>
                             )}
                           </div>
                           <div className={styles.futsalTeamRoles}>
                             {homeCaptain && <span className={styles.roleBadge} title="Капітан"><b>C:</b> {homeCaptain}</span>}
                             {homeGK && <span className={styles.roleBadge} title="Воротар"><b>GK:</b> {homeGK}</span>}
                           </div>
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
                           <div className={styles.futsalTeamHeader}>
                             <span className={styles.futsalTeamName}>{match.awayTeam?.name || 'TBD'}</span>
                             {onViewRoster && (
                               <button className={styles.rosterMiniBtn} onClick={() => onViewRoster(match.awayTeamId)} title="Переглянути склад">
                                 <i className="fas fa-users"></i>
                               </button>
                             )}
                           </div>
                           <div className={styles.futsalTeamRoles}>
                             {awayCaptain && <span className={styles.roleBadge} title="Капітан"><b>C:</b> {awayCaptain}</span>}
                             {awayGK && <span className={styles.roleBadge} title="Воротар"><b>GK:</b> {awayGK}</span>}
                           </div>
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

                    {/* Connectors */}
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
