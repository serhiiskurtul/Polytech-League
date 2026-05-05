import React from 'react';
import { FutsalMatchResponse } from '@/shared/types/futsal.types';
import UniversalBracket, { BracketRound, BracketMatch } from '@/components/Tournament/UniversalBracket/UniversalBracket';
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

  const otherStages = Object.keys(groupedMatches)
    .filter(s => s !== 'Матч за 3-тє місце')
    .sort((a, b) => getStageRank(a) - getStageRank(b));

  const mappedRounds: BracketRound[] = otherStages.map(stageName => {
    const stageMatches = groupedMatches[stageName];
    return {
      title: stageName,
      matches: stageMatches.map(m => ({
        id: m.id,
        teams: [
          { 
            name: m.homeTeam?.name || 'TBD', 
            score: m.homeTeamScore ?? '-', 
            isWinner: m.winnerTeamId === m.homeTeamId && m.winnerTeamId !== null 
          },
          { 
            name: m.awayTeam?.name || 'TBD', 
            score: m.awayTeamScore ?? '-', 
            isWinner: m.winnerTeamId === m.awayTeamId && m.winnerTeamId !== null 
          }
        ],
        // Attach original match for actions
        originalMatch: m
      }))
    };
  });

  const getPlayerName = (teamId: number, playerId: number | null | undefined) => {
    if (!playerId) return null;
    const team = teams.find(t => t.id === teamId);
    if (!team) return null;
    const player = team?.players?.find((p: any) => p.id === playerId);
    if (!player) return null;
    return `${player.lastName} ${player.firstName?.[0] || ''}.`;
  };

  const renderFutsalMatchCard = (bracketMatch: any) => {
    const match: FutsalMatchResponse = bracketMatch.originalMatch;
    const isHomeWinner = (match.homeTeamScore || 0) > (match.awayTeamScore || 0);
    const isAwayWinner = (match.awayTeamScore || 0) > (match.homeTeamScore || 0);
    const isDraw = (match.homeTeamScore !== null && match.awayTeamScore !== null) && 
                   (match.homeTeamScore === match.awayTeamScore);

    const homeCaptain = getPlayerName(match.homeTeamId, match.homeCaptainId);
    const homeGK = getPlayerName(match.homeTeamId, match.homeGoalkeeperId);
    const awayCaptain = getPlayerName(match.awayTeamId, match.awayCaptainId);
    const awayGK = getPlayerName(match.awayTeamId, match.awayGoalkeeperId);

    return (
      <div 
        className={styles.futsalMatchCard} 
        onClick={() => onViewInfo && onViewInfo(match)}
        style={{ cursor: onViewInfo ? 'pointer' : 'default', border: 'none', boxShadow: 'none', background: 'transparent' }}
      >
        {isAdmin && onDeleteMatch && (
          <button 
            className={styles.cardDeleteBtn}
            onClick={(e) => { e.stopPropagation(); onDeleteMatch(match.id); }}
            title="Видалити матч"
            style={{ top: '-4px', right: '-4px' }}
          >
            <i className="fas fa-trash" style={{ fontSize: '10px' }}></i>
          </button>
        )}
        
        <div className={`${styles.futsalTeamRow} ${isHomeWinner ? styles.futsalWinner : isDraw ? styles.futsalDraw : ''}`}>
          <div className={styles.futsalTeamInfo}>
             <div className={styles.futsalTeamHeader}>
               <span 
                 className={styles.futsalTeamName}
                 onClick={(e) => { e.stopPropagation(); onViewRoster && onViewRoster(match.homeTeamId); }}
                 style={{ cursor: onViewRoster ? 'pointer' : 'default' }}
               >
                 {match.homeTeam?.name || 'TBD'}
               </span>
               {onViewRoster && (
                 <button className={styles.rosterMiniBtn} onClick={(e) => { e.stopPropagation(); onViewRoster(match.homeTeamId); }}>
                   <i className="fas fa-users"></i>
                 </button>
               )}
             </div>
             <div className={styles.futsalTeamRoles}>
               {homeCaptain && <span className={styles.roleBadge}><b>C:</b> {homeCaptain}</span>}
               {homeGK && <span className={styles.roleBadge}><b>GK:</b> {homeGK}</span>}
             </div>
          </div>
          <div className={styles.futsalTeamScore}>
            {match.homeTeamScore ?? '-'}
            {isHomeWinner && <i className="fas fa-trophy"></i>}
          </div>
        </div>

        <div className={styles.futsalVsDivider}>vs</div>

        <div className={`${styles.futsalTeamRow} ${isAwayWinner ? styles.futsalWinner : isDraw ? styles.futsalDraw : ''}`}>
          <div className={styles.futsalTeamInfo}>
             <div className={styles.futsalTeamHeader}>
               <span 
                 className={styles.futsalTeamName}
                 onClick={(e) => { e.stopPropagation(); onViewRoster && onViewRoster(match.awayTeamId); }}
                 style={{ cursor: onViewRoster ? 'pointer' : 'default' }}
               >
                 {match.awayTeam?.name || 'TBD'}
               </span>
               {onViewRoster && (
                 <button className={styles.rosterMiniBtn} onClick={(e) => { e.stopPropagation(); onViewRoster(match.awayTeamId); }}>
                   <i className="fas fa-users"></i>
                 </button>
               )}
             </div>
             <div className={styles.futsalTeamRoles}>
               {awayCaptain && <span className={styles.roleBadge}><b>C:</b> {awayCaptain}</span>}
               {awayGK && <span className={styles.roleBadge}><b>GK:</b> {awayGK}</span>}
             </div>
          </div>
          <div className={styles.futsalTeamScore}>
            {match.awayTeamScore ?? '-'}
            {isAwayWinner && <i className="fas fa-trophy"></i>}
          </div>
        </div>

        {isAdmin && onOpenScorers && (
          <button className={styles.futsalProtocolBtn} onClick={(e) => { e.stopPropagation(); onOpenScorers(match); }}>
             <i className="fas fa-file-alt"></i> Протокол
          </button>
        )}
      </div>
    );
  };

  return (
    <div className={styles.futsalBracketWrapper}>
      {matches.length > 0 ? (
        <UniversalBracket 
          rounds={mappedRounds} 
          isAdmin={isAdmin}
          onDeleteMatch={(id) => onDeleteMatch && onDeleteMatch(Number(id))}
          onOpenScorers={(match: any) => onOpenScorers && onOpenScorers(match.originalMatch)}
          renderMatchCard={renderFutsalMatchCard}
        />
      ) : (

        <div className={styles.emptyBracket}>
          <i className="fas fa-sitemap"></i>
          <p>Турнірна сітка порожня.</p>
        </div>
      )}
    </div>
  );
};

export default FutsalBracket;
