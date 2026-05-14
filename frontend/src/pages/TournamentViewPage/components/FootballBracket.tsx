import React from 'react';
import { footballMatchResponse } from '@/shared/types/football.types';
import UniversalBracket, { BracketRound, BracketMatch } from '@/components/Tournament/UniversalBracket/UniversalBracket';
import styles from '../TournamentViewPage.module.css';
import bracketStyles from '@/components/Tournament/UniversalBracket/UniversalBracket.module.css';

interface FootballBracketProps {
  matches: footballMatchResponse[];
  isAdmin?: boolean;
  onDeleteMatch?: (matchId: number) => void;
  onOpenScorers?: (match: footballMatchResponse) => void;
  onViewRoster?: (teamId: number) => void;
  onViewInfo?: (match: footballMatchResponse) => void;
  teams?: any[]; // For name lookup
}

const FootballBracket: React.FC<FootballBracketProps> = ({
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
    'Матч за 3-тє місце',
    'За 3 місце',
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
  }, {} as Record<string, footballMatchResponse[]>);

  const otherStages = Object.keys(groupedMatches)
    .filter(s => s !== 'Матч за 3-тє місце' && s !== 'За 3 місце')
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

  const renderFootballMatchCard = (bracketMatch: any) => {
    const match: footballMatchResponse = bracketMatch.originalMatch;
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
        className={styles.footballMatchCard} 
        onClick={() => onViewInfo && onViewInfo(match)}
        style={{ cursor: onViewInfo ? 'pointer' : 'default', border: 'none', boxShadow: 'none', background: 'transparent', padding: 0, width: '100%' }}
      >
        {isAdmin && onDeleteMatch && (
          <button 
            className={bracketStyles.cardDeleteBtn}
            onClick={(e) => { e.stopPropagation(); onDeleteMatch(match.id); }}
            title="Видалити матч"
          >
            <i className="fas fa-times"></i>
          </button>
        )}
        
        <div className={`${bracketStyles.teamRow} ${isHomeWinner ? bracketStyles.isWinner : ''}`}>
          {isHomeWinner && <div className={bracketStyles.winnerIndicator} />}
          <div className={bracketStyles.teamInfo}>
             <span 
               className={bracketStyles.teamName}
               onClick={(e) => { e.stopPropagation(); onViewRoster && onViewRoster(match.homeTeamId); }}
               style={{ cursor: onViewRoster ? 'pointer' : 'default' }}
             >
               {match.homeTeam?.name || 'TBD'}
             </span>
             {isHomeWinner && <i className={`fas fa-trophy ${bracketStyles.trophyIcon}`}></i>}
          </div>
          <div className={bracketStyles.score}>
            {match.homeTeamScore ?? '-'}
          </div>
        </div>

        <div className={bracketStyles.vsDivider}>
          <div className={bracketStyles.vsLine} />
          <span className={bracketStyles.vsText}>vs</span>
          <div className={bracketStyles.vsLine} />
        </div>

        <div className={`${bracketStyles.teamRow} ${isAwayWinner ? bracketStyles.isWinner : ''}`}>
          {isAwayWinner && <div className={bracketStyles.winnerIndicator} />}
          <div className={bracketStyles.teamInfo}>
             <span 
               className={bracketStyles.teamName}
               onClick={(e) => { e.stopPropagation(); onViewRoster && onViewRoster(match.awayTeamId); }}
               style={{ cursor: onViewRoster ? 'pointer' : 'default' }}
             >
               {match.awayTeam?.name || 'TBD'}
             </span>
             {isAwayWinner && <i className={`fas fa-trophy ${bracketStyles.trophyIcon}`}></i>}
          </div>
          <div className={bracketStyles.score}>
            {match.awayTeamScore ?? '-'}
          </div>
        </div>

        {isAdmin && onOpenScorers && (
          <button className={bracketStyles.protocolBtn} onClick={(e) => { e.stopPropagation(); onOpenScorers(match); }}>
             <i className="fas fa-file-alt"></i> Протокол
          </button>
        )}
      </div>
    );
  };

  return (
    <div className={styles.footballBracketWrapper}>
      {matches.length > 0 ? (
        <UniversalBracket 
          rounds={mappedRounds} 
          isAdmin={isAdmin}
          onDeleteMatch={(id) => onDeleteMatch && onDeleteMatch(Number(id))}
          onOpenScorers={(match: any) => onOpenScorers && onOpenScorers(match.originalMatch)}
          renderMatchCard={renderFootballMatchCard}
          cardHeight={160}
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

export default FootballBracket;
