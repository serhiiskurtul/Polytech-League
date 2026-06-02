import React, { useState, useEffect, useMemo } from 'react';
import { PlayerDto, Gender } from '@/entities/player/model/player.types';
import { ChessEventResponse, ChessEventParticipantDto, ChessGameDto, ChessEventRequest, GameResult } from '@/entities/sport/model/chess.types';
import { searchPlayers } from '@/shared/api';
import { 
  updateVolleyballMatch, 
  updateBeachVolleyballMatch, 
  updateFootballMatch, 
  updateBasketball3x3Match,
  updateBadmintonMatch,
  updateChessGame,
  updateCheckersGame,
  updateTableTennisMatch,
  createVolleyballMatch,
  createBeachVolleyballMatch,
  createFootballMatch,
  createBasketball3x3Match,
  createBadmintonMatch,
  createChessGame,
  createCheckersGame,
  createTableTennisMatch,
  createVolleyballTeam,
  createBeachVolleyballTeam,
  createFootballTeam,
  createBasketball3x3Team,
  createBadmintonTeam,
  createChessTeam,
  createCheckersTeam,
  createTableTennisTeam,
  createPlayer,
  updateVolleyballTeam,
  updateBeachVolleyballTeam,
  updateFootballTeam,
  updateBasketball3x3Team,
  updateBadmintonTeam,
  updateChessTeam,
  updateCheckersTeam,
  updateTableTennisTeam,
  deleteChessGame,
  deleteCheckersGame,
  deleteTableTennisMatch,
  deleteVolleyballMatch,
  deleteBeachVolleyballMatch,
  deleteFootballMatch,
  deleteBasketball3x3Match,
  deleteBadmintonMatch,
  deleteVolleyballTeam,
  deleteBeachVolleyballTeam,
  deleteFootballTeam,
  deleteBasketball3x3Team,
  deleteBadmintonTeam,
  deleteChessTeam,
  deleteCheckersTeam,
  deleteTableTennisTeam,
  approveFootballParticipant,
  rejectFootballParticipant,
  removeFootballParticipant,
  approveVolleyballParticipant,
  rejectVolleyballParticipant,
  removeVolleyballParticipant,
  approveBasketball3x3Participant,
  rejectBasketball3x3Participant,
  removeBasketball3x3Participant,
  approveBeachVolleyballParticipant,
  rejectBeachVolleyballParticipant,
  removeBeachVolleyballParticipant,
  approveBadmintonParticipant,
  rejectBadmintonParticipant,
  removeBadmintonParticipant,
  approveTableTennisParticipant,
  rejectTableTennisParticipant,
  removeTableTennisParticipant
} from '@/shared/api';
import { RegistrationStatus } from '@/entities/tournament/model/tournament.types';
import FootballBracket from './FootballBracket';
import styles from '../TournamentViewPage.module.css';
import { Link } from 'react-router-dom';
import api from '@/shared/api/axiosInstance';
import toast from 'react-hot-toast';

type Sport = 'volleyball' | 'волейбол' | 'beach_volleyball' | 'beach-volleyball' | 'пляжний_волейбол' | 'football' | 'футбол' | 'basketball_3x3' | 'basketball3x3' | 'basketball-3x3' | 'баскетбол_3х3' | 'badminton' | 'бадмінтон' | 'chess' | 'шахи' | 'checkers' | 'шашки' | 'tabletennis' | 'table_tennis' | 'table-tennis' | 'настільний_теніс';

const getCanonicalSport = (s: Sport): Sport => {
  if (['volleyball', 'волейбол'].includes(s)) return 'volleyball';
  if (['beach_volleyball', 'beach-volleyball', 'пляжний_волейбол'].includes(s)) return 'beach_volleyball';
  if (['football', 'футбол'].includes(s)) return 'football';
  if (['basketball_3x3', 'basketball3x3', 'basketball-3x3', 'баскетбол_3х3'].includes(s)) return 'basketball_3x3';
  if (['badminton', 'бадмінтон'].includes(s)) return 'badminton';
  if (['chess', 'шахи'].includes(s)) return 'chess';
  if (['checkers', 'шашки'].includes(s)) return 'checkers';
  if (['tabletennis', 'table_tennis', 'table-tennis', 'настільний_теніс'].includes(s)) return 'tabletennis';
  return s;
};

interface TeamSportsTournamentViewProps {
  sport: Sport;
  details: any;
  isAdmin: boolean;
  onRefresh: () => void;
}

const TeamSportsTournamentView: React.FC<TeamSportsTournamentViewProps> = ({ 
  sport, 
  details, 
  isAdmin,
  onRefresh
}) => {
  const hasBracket = (sport === 'football' || sport === 'футбол') && details?.tournamentType !== 'RoundRobin';
  const [activeTab, setActiveTab] = useState<'standings' | 'matches' | 'teams' | 'info' | 'bracket' | 'participants' | 'stats' | 'voting'>(hasBracket ? 'bracket' : 'matches');
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredPlayers, setFilteredPlayers] = useState<PlayerDto[]>([]);
  const [isSearchingPlayers, setIsSearchingPlayers] = useState(false);
  const [voteStats, setVoteStats] = useState<any>(null);
  const [isVoting, setIsVoting] = useState(false);
  const isRegionalOrNational = details?.level === 'Regional' || details?.level === 'National';
  
  // Create Match State
  const [isCreateMatchModalOpen, setIsCreateMatchModalOpen] = useState(false);
  const [newMatchForm, setNewMatchForm] = useState({
    homeTeamId: '',
    awayTeamId: '',
    stage: 'Груповий етап',
    board: 1
  });

  // Create Team State
  const [isCreateTeamModalOpen, setIsCreateTeamModalOpen] = useState(false);
  const [newTeamForm, setNewTeamForm] = useState({
    name: '',
    tournamentGroupName: '',
    isGeneralTableIncluded: false
  });

  // Team Roster Management State
  const [editingTeamId, setEditingTeamId] = useState<number | null>(null);
  const [isTeamRosterModalOpen, setIsTeamRosterModalOpen] = useState(false);

  // Match Scorers State
  const [isScorersModalOpen, setIsScorersModalOpen] = useState(false);
  const [selectedMatchForScorers, setSelectedMatchForScorers] = useState<any>(null);
  const [tempScorers, setTempScorers] = useState<{[playerId: number]: number}>({});
  const [tempYellowCards, setTempYellowCards] = useState<{[playerId: number]: number}>({});
  const [tempRedCards, setTempRedCards] = useState<{[playerId: number]: number}>({});
  const [matchGoalkeepers, setMatchGoalkeepers] = useState<{home: string, away: string}>({ home: '', away: '' });
  const [matchCaptains, setMatchCaptains] = useState<{home: string, away: string}>({ home: '', away: '' });
  const [matchScores, setMatchScores] = useState<{home: number, away: number}>({ home: 0, away: 0 });
  const [matchPenaltyScores, setMatchPenaltyScores] = useState<{home: number | null, away: number | null}>({ home: null, away: null });

  // Quick Add Player State
  const [isQuickAddMode, setIsQuickAddMode] = useState(false);
  const [quickAddForm, setQuickAddForm] = useState({ firstName: '', lastName: '', patronymic: '', group: '', gender: 'None' as Gender });

  // Olympic Filtering State
  const [roundNumber, setRoundNumber] = useState(1);
  const [stageType, setStageType] = useState('Груповий етап');
  const [useStrictFiltering, setUseStrictFiltering] = useState(true);
  const [selectedMatchForInfo, setSelectedMatchForInfo] = useState<any>(null);

  const footballStats = useMemo(() => {
    if (sport !== 'football' && sport !== 'футбол') return null;

    const matches = details.matches || details.games || [];
    const playerGoals: Record<number, { player: any; goals: number; teamName?: string }> = {};
    const teamGoals: Record<number, { name: string; goals: number }> = {};
    const teamYellowCards: Record<number, { name: string; cards: number }> = {};

    details.teams?.forEach((team: any) => {
      teamGoals[team.id] = { name: team.name, goals: 0 };
      teamYellowCards[team.id] = { name: team.name, cards: 0 };
    });

    matches.forEach((match: any) => {
      const homeTeamId = match.homeTeamId;
      const awayTeamId = match.awayTeamId;

      if (homeTeamId && teamGoals[homeTeamId]) {
        teamGoals[homeTeamId].goals += (match.homeTeamScore ?? 0);
      }
      if (awayTeamId && teamGoals[awayTeamId]) {
        teamGoals[awayTeamId].goals += (match.awayTeamScore ?? 0);
      }

      match.goals?.forEach((goal: any) => {
        const playerId = goal.playerId;
        if (playerId) {
          if (!playerGoals[playerId]) {
            let teamName = '';
            if (match.homeRoster?.some((p: any) => p.id === playerId)) {
              teamName = details.teams?.find((t: any) => t.id === match.homeTeamId)?.name || '';
            } else if (match.awayRoster?.some((p: any) => p.id === playerId)) {
              teamName = details.teams?.find((t: any) => t.id === match.awayTeamId)?.name || '';
            } else {
              const team = details.teams?.find((t: any) => t.players?.some((p: any) => p.id === playerId));
              if (team) teamName = team.name;
            }
            playerGoals[playerId] = { player: goal.player, goals: 0, teamName };
          }
          playerGoals[playerId].goals += (goal.goalsCount ?? 0);
        }
      });

      match.yellowCards?.forEach((card: any) => {
        const playerId = card.playerId;
        let teamId = null;
        if (match.homeRoster?.some((p: any) => p.id === playerId)) {
          teamId = homeTeamId;
        } else if (match.awayRoster?.some((p: any) => p.id === playerId)) {
          teamId = awayTeamId;
        } else {
          const team = details.teams?.find((t: any) => t.players?.some((p: any) => p.id === playerId));
          if (team) teamId = team.id;
        }

        if (teamId && teamYellowCards[teamId]) {
          teamYellowCards[teamId].cards += 1;
        }
      });
    });

    const topPlayers = Object.values(playerGoals)
      .sort((a, b) => b.goals - a.goals)
      .slice(0, 3);

    const topTeamsByGoals = Object.values(teamGoals)
      .sort((a, b) => b.goals - a.goals)
      .slice(0, 3);

    const topTeamsByCards = Object.values(teamYellowCards)
      .sort((a, b) => b.cards - a.cards);
    
    const teamMostCards = topTeamsByCards[0] || { name: '—', cards: 0 };

    return {
      topPlayers,
      topTeamsByGoals,
      teamMostCards
    };
  }, [details, sport]);
  const [isMatchInfoModalOpen, setIsMatchInfoModalOpen] = useState(false);

  useEffect(() => {
    // Kept empty, just for logic preservation
  }, [isAdmin]);

  useEffect(() => {
    if (!isAdmin) return;
    
    const delayDebounceFn = setTimeout(async () => {
      if (searchQuery.length >= 2) {
        setIsSearchingPlayers(true);
        try {
          const players = await searchPlayers(searchQuery);
          setFilteredPlayers(players);
        } catch (e) {
          console.error(e);
        } finally {
          setIsSearchingPlayers(false);
        }
      } else {
        setFilteredPlayers([]);
      }
    }, 400);

    return () => clearTimeout(delayDebounceFn);
  }, [searchQuery, isAdmin]);

  useEffect(() => {
    if (details?.id) {
      fetchVoteStats();
    }
  }, [details?.id]);

  const fetchVoteStats = async () => {
    try {
      const response = await api.get(`/tournaments/${details.id}/votes`);
      setVoteStats(response.data);
    } catch (err) {
      console.error('Failed to fetch vote stats:', err);
    }
  };

  const handleVote = async (teamId?: number, playerId?: number) => {
    setIsVoting(true);
    try {
      let url = `/tournaments/${details.id}/vote`;
      const params = new URLSearchParams();
      if (teamId) params.append('teamId', teamId.toString());
      if (playerId) params.append('playerId', playerId.toString());
      if (params.toString()) url += `?${params.toString()}`;

      await api.post(url);
      toast.success('Ваш голос враховано!');
      fetchVoteStats();
    } catch (err: any) {
      console.error('Failed to vote:', err);
      if (err.response?.status === 401) {
        toast.error('Будь ласка, авторизуйтесь, щоб проголосувати.');
      } else {
        toast.error(err.response?.data || 'Помилка при голосуванні');
      }
    } finally {
      setIsVoting(false);
    }
  };

  // Removed loadAllPlayers

  const getMatchUpdateApi = () => {
    switch (getCanonicalSport(sport)) {
      case 'volleyball': return updateVolleyballMatch;
      case 'beach_volleyball': return updateBeachVolleyballMatch;
      case 'football': return updateFootballMatch;
      case 'basketball_3x3': return updateBasketball3x3Match;
      case 'badminton': return updateBadmintonMatch;
      case 'chess': return updateChessGame;
      case 'checkers': return updateCheckersGame;
      case 'tabletennis':
      case 'table_tennis':
      case 'table-tennis': return updateTableTennisMatch;
      default: return updateVolleyballMatch;
    }

  };

  const handleUpdateScore = async (matchId: number, homeScore: number, awayScore: number) => {
    try {
      const updateApi = getMatchUpdateApi();
      const allMatches = details.matches || details.games || [];
      const match = allMatches.find((m: any) => m.id === matchId);
      if (!match) return;
      
      const isChessLike = sport === 'chess' || sport === 'checkers';
      const maxScore = isChessLike ? 2 : 1000; // Arbitrary high default for other sports
      
      const finalHomeScore = isChessLike ? Math.min(maxScore, Math.max(0, homeScore)) : Math.round(homeScore);
      const finalAwayScore = isChessLike ? Math.min(maxScore, Math.max(0, awayScore)) : Math.round(awayScore);
      
      let payload: any;

      if (sport === 'volleyball' || sport === 'beach_volleyball' || sport === 'beach-volleyball') {
        payload = {
          id: matchId,
          eventId: details.id,
          homeTeamId: match.homeTeamId,
          awayTeamId: match.awayTeamId,
          stage: match.stage,
          homeSetsWon: finalHomeScore,
          awaySetsWon: finalAwayScore,
          setsToWin: match.setsToWin || details.setsToWin || 2,
          homeRosterIds: match.homeRoster?.map((p: any) => p.id) || match.homeRosterIds || [],
          awayRosterIds: match.awayRoster?.map((p: any) => p.id) || match.awayRosterIds || []
        };
      } else if (sport === 'chess' || sport === 'checkers') {
        // Auto-calculate result for board games
        let calculatedResult: GameResult = 'NotPlayed';
        if (finalHomeScore > finalAwayScore) calculatedResult = 'WhiteWin';
        else if (finalAwayScore > finalHomeScore) calculatedResult = 'BlackWin';
        else if (finalHomeScore > 0 || finalAwayScore > 0) calculatedResult = 'Draw';

        payload = {
          id: matchId,
          tournamentId: details.id,
          whitePlayerId: match.whitePlayerId,
          blackPlayerId: match.blackPlayerId,
          whiteTeamId: match.whiteTeamId,
          blackTeamId: match.blackTeamId,
          whiteTeamScore: finalHomeScore,
          blackTeamScore: finalAwayScore,
          result: calculatedResult,
          round: match.round || 1,
          board: match.board || 1
        };
      } else {
        payload = {
          id: matchId,
          eventId: details.id,
          homeTeamId: match.homeTeamId,
          awayTeamId: match.awayTeamId,
          homeTeamScore: finalHomeScore, // For football, Basketball, Table Tennis
          awayTeamScore: finalAwayScore,
          homeScore: finalHomeScore,     // For Badminton
          awayScore: finalAwayScore,
          stage: match.stage,
          setsToWin: match.setsToWin || details.setsToWin || 2,
          homeRosterIds: match.homeRoster?.map((p: any) => p.id) || match.homeRosterIds || [],
          awayRosterIds: match.awayRoster?.map((p: any) => p.id) || match.awayRosterIds || [],
          homeGoalkeeperId: match.homeGoalkeeperId,
          awayGoalkeeperId: match.awayGoalkeeperId,
          homeCaptainId: match.homeCaptainId,
          awayCaptainId: match.awayCaptainId
        };
      }


      await updateApi(matchId, payload);
      onRefresh();
    } catch (err) {
      console.error('Update Score Error:', err);
      alert('Помилка при оновленні рахунку');
    }
  };

  const handleUpdatePenalty = async (matchId: number, homePenalty: number | null, awayPenalty: number | null) => {
    try {
      const match = (details.matches || details.games || []).find((m: any) => m.id === matchId);
      if (!match) return;

      const payload = {
        ...match,
        homePenaltyScore: homePenalty,
        awayPenaltyScore: awayPenalty,
        homeRosterIds: match.homeRoster?.map((p: any) => p.id) || match.homeRosterIds || [],
        awayRosterIds: match.awayRoster?.map((p: any) => p.id) || match.awayRosterIds || []
      };

      await updateFootballMatch(matchId, payload);
      onRefresh();
    } catch (err) {
      console.error('Update Penalty Error:', err);
      alert('Помилка при оновленні рахунку пенальті');
    }
  };

  const handleOpenScorers = (match: any) => {
    setSelectedMatchForScorers(match);
    const initialScorers: {[playerId: number]: number} = {};
    const initialYellowCards: {[playerId: number]: number} = {};
    const initialRedCards: {[playerId: number]: number} = {};
    
    match.goals?.forEach((g: any) => {
      initialScorers[g.playerId] = g.goalsCount;
    });
    match.yellowCards?.forEach((c: any) => {
      initialYellowCards[c.playerId] = (initialYellowCards[c.playerId] || 0) + 1;
    });
    match.redCards?.forEach((c: any) => {
      initialRedCards[c.playerId] = (initialRedCards[c.playerId] || 0) + 1;
    });

    setTempScorers(initialScorers);
    setTempYellowCards(initialYellowCards);
    setTempRedCards(initialRedCards);
    
    // Default match goalkeepers to team defaults if not set for the match
    const homeTeam = details.teams?.find((t: any) => t.id === match.homeTeamId);
    const awayTeam = details.teams?.find((t: any) => t.id === match.awayTeamId);

    setMatchGoalkeepers({
      home: match.homeGoalkeeperId?.toString() || homeTeam?.goalkeeperId?.toString() || '',
      away: match.awayGoalkeeperId?.toString() || awayTeam?.goalkeeperId?.toString() || ''
    });
    setMatchCaptains({
      home: match.homeCaptainId?.toString() || homeTeam?.captainId?.toString() || '',
      away: match.awayCaptainId?.toString() || awayTeam?.captainId?.toString() || ''
    });
    setMatchScores({
      home: match.homeTeamScore ?? 0,
      away: match.awayTeamScore ?? 0
    });
    setMatchPenaltyScores({
      home: match.homePenaltyScore ?? null,
      away: match.awayPenaltyScore ?? null
    });
    setIsScorersModalOpen(true);
  };

  const handleSaveScorers = async () => {
    if (!selectedMatchForScorers) return;

    try {
      const goalsPayload = Object.entries(tempScorers)
        .filter(([_, count]) => count > 0)
        .map(([playerId, count]) => ({
          playerId: parseInt(playerId),
          goalsCount: count
        }));

      const yellowCardsPayload = Object.entries(tempYellowCards)
        .filter(([_, count]) => count > 0)
        .map(([playerId, count]) => {
          const cards = [];
          for(let i=0; i<count; i++) cards.push({ playerId: parseInt(playerId) });
          return cards;
        }).flat();

      const redCardsPayload = Object.entries(tempRedCards)
        .filter(([_, count]) => count > 0)
        .map(([playerId, count]) => {
          const cards = [];
          for(let i=0; i<count; i++) cards.push({ playerId: parseInt(playerId) });
          return cards;
        }).flat();

      const payload = {
        id: selectedMatchForScorers.id,
        eventId: details.id,
        homeTeamId: selectedMatchForScorers.homeTeamId,
        awayTeamId: selectedMatchForScorers.awayTeamId,
        stage: selectedMatchForScorers.stage,
        // Update rosters to match team players if they were empty
        homeRosterIds: selectedMatchForScorers.homeRoster?.length > 0 
            ? selectedMatchForScorers.homeRoster.map((p: any) => p.id) 
            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players?.map((p: any) => p.id) || [],
        awayRosterIds: selectedMatchForScorers.awayRoster?.length > 0 
            ? selectedMatchForScorers.awayRoster.map((p: any) => p.id) 
            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players?.map((p: any) => p.id) || [],
        homeGoalkeeperId: parseInt(matchGoalkeepers.home) || null,
        awayGoalkeeperId: parseInt(matchGoalkeepers.away) || null,
        homeCaptainId: parseInt(matchCaptains.home) || null,
        awayCaptainId: parseInt(matchCaptains.away) || null,
        homeTeamScore: matchScores.home,
        awayTeamScore: matchScores.away,
        homeScore: matchScores.home,
        awayScore: matchScores.away,
        homePenaltyScore: matchScores.home === matchScores.away ? matchPenaltyScores.home : null,
        awayPenaltyScore: matchScores.home === matchScores.away ? matchPenaltyScores.away : null,
        goals: goalsPayload,
        yellowCards: yellowCardsPayload,
        redCards: redCardsPayload
      };


      await updateFootballMatch(selectedMatchForScorers.id, payload);
      setIsScorersModalOpen(false);
      onRefresh();
    } catch (err) {
      console.error('Save Scorers Error:', err);
      alert('Помилка при збереженні авторів голів');
    }
  };

  const handleDeleteMatch = async (matchId: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей матч? Цю дію неможливо скасувати.')) {
      return;
    }

    try {
      let deleteApi;
      switch (getCanonicalSport(sport)) {
        case 'volleyball': deleteApi = deleteVolleyballMatch; break;
        case 'beach_volleyball': deleteApi = deleteBeachVolleyballMatch; break;
        case 'football': deleteApi = deleteFootballMatch; break;
        case 'basketball_3x3': deleteApi = (id: number) => deleteBasketball3x3Match(id); break;
        case 'badminton': deleteApi = deleteBadmintonMatch; break;
        case 'chess': deleteApi = (id: number) => deleteChessGame(id, details.id); break;
        case 'checkers': deleteApi = (id: number) => deleteCheckersGame(id, details.id); break;
        case 'tabletennis':
        case 'table_tennis':
        case 'table-tennis': deleteApi = deleteTableTennisMatch; break;
        default: deleteApi = deleteVolleyballMatch;
      }


      await deleteApi(matchId);
      onRefresh();
    } catch (err) {
      console.error('Delete Match Error:', err);
      alert('Помилка при видаленні матчу');
    }
  };

  const getTeamUpdateApi = () => {
    switch (getCanonicalSport(sport)) {
      case 'volleyball': return updateVolleyballTeam;
      case 'beach_volleyball': return updateBeachVolleyballTeam;
      case 'football': return updateFootballTeam;
      case 'basketball_3x3': return updateBasketball3x3Team;
      case 'badminton': return updateBadmintonTeam;
      case 'chess': return updateChessTeam;
      case 'checkers': return updateCheckersTeam;
      case 'tabletennis':
      case 'table_tennis':
      case 'table-tennis': return updateTableTennisTeam;
      default: return updateVolleyballTeam;
    }

  };

  const handleUpdateTeamPoints = async (teamId: number, points: number) => {
    try {
      const updateApi = getTeamUpdateApi();
      const team = details.teams.find((t: any) => t.id === teamId);
      await updateApi(teamId, { ...team, points: Math.max(0, points) });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні очок');
    }
  };

  const calculateStandings = (groupFilter?: string) => {
    let teams = details.teams || [];
    const matches = details.matches || details.games || [];
    
    if (groupFilter === 'GENERAL_TABLE') {
      // General Table: teams with flag OR teams with no group at all
      teams = teams.filter((t: any) => t.isGeneralTableIncluded || !t.tournamentGroupName);
    } else if (groupFilter) {
      teams = teams.filter((t: any) => t.tournamentGroupName === groupFilter);
    }
    
    if (teams.length === 0) return [];

    const standings = teams.map((team: any) => {
      const stats = {
        id: team.id,
        name: team.name,
        tournamentGroupName: team.tournamentGroupName,
        gp: 0, // Games Played
        w: 0,  // Wins
        d: 0,  // Draws
        l: 0,  // Losses
        gf: 0, // Goals For
        ga: 0, // Goals Against
        manualPoints: team.points
      };

      matches.forEach((match: any) => {
        // For specific group tables, only include group stage matches
        if (groupFilter && groupFilter !== 'GENERAL_TABLE' && match.stage !== 'Груповий етап') return;
        
        // For general table, we can include all matches if needed, or specific ones. 
        // Usually general table is for final ranking, so we include matches that affect it.
        // For simplicity, we filter matches by teams involved.

        const isChessLike = sport === 'chess' || sport === 'checkers';
        
        let isHome = false;
        let isAway = false;
        let hScore = 0;
        let aScore = 0;
        let isPlayed = false;

        if (isChessLike) {
          isHome = match.whiteTeamId === team.id;
          isAway = match.blackTeamId === team.id;
          hScore = match.whiteTeamScore || 0;
          aScore = match.blackTeamScore || 0;
          isPlayed = match.whiteTeamScore !== null || match.blackTeamScore !== null;
        } else {
          isHome = match.homeTeamId === team.id;
          isAway = match.awayTeamId === team.id;
          const isVolleyball = sport.includes('volleyball') || sport === 'badminton';
          hScore = isVolleyball ? (match.homeSetsWon ?? match.homeScore ?? 0) : (match.homeTeamScore ?? 0);
          aScore = isVolleyball ? (match.awaySetsWon ?? match.awayScore ?? 0) : (match.awayTeamScore ?? 0);
          isPlayed = isVolleyball 
            ? (match.homeSetsWon != null || match.awaySetsWon != null || match.homeScore != null || match.awayScore != null) 
            : (match.homeTeamScore != null || match.awayTeamScore != null);
        }
        if (isHome || isAway) {
          if (isPlayed) {
            stats.gp++;
            const tScore = isHome ? hScore : aScore;
            const oScore = isHome ? aScore : hScore;

            stats.gf += tScore;
            stats.ga += oScore;

            if (tScore > oScore) stats.w++;
            else if (tScore < oScore) stats.l++;
            else stats.d++;
          }
        }
      });

      // Traditional points calculation
      const autoPoints = stats.w * 3 + stats.d * 1;
      
      const finalPoints = (stats.manualPoints > 0 && stats.gp === 0) 
        ? stats.manualPoints 
        : autoPoints;
      
      return { ...stats, points: finalPoints };
    });

    const sortedStandings = standings.sort((a: any, b: any) => {
      const aP = a.points;
      const bP = b.points;
      if (bP !== aP) return bP - aP;
      const aDiff = a.gf - a.ga;
      const bDiff = b.gf - b.ga;
      if (bDiff !== aDiff) return bDiff - aDiff;
      return b.gf - a.gf;
    });

    const standingsWithRanks = [];
    let i = 0;
    while (i < sortedStandings.length) {
      let tieCount = 1;
      while (i + tieCount < sortedStandings.length && 
             sortedStandings[i + tieCount].points === sortedStandings[i].points &&
             (sortedStandings[i + tieCount].gf - sortedStandings[i + tieCount].ga) === (sortedStandings[i].gf - sortedStandings[i].ga) &&
             sortedStandings[i + tieCount].gf === sortedStandings[i].gf) {
        tieCount++;
      }
      const rankStr = tieCount > 1 ? `${i + 1}-${i + tieCount}` : (i + 1).toString();
      for (let j = 0; j < tieCount; j++) {
        standingsWithRanks.push({ ...sortedStandings[i + j], displayRank: rankStr });
      }
      i += tieCount;
    }
    return standingsWithRanks;
  };

  const isGroupsPlayoff = details.tournamentType === 'GroupsPlayoff';
  const groupNames = Array.from(new Set((details.teams || []).map((t: any) => t.tournamentGroupName).filter(Boolean))) as string[];
  const sortedGroups = groupNames.sort();
  
  const generalTableTeams = (details.teams || []).filter((t: any) => t.isGeneralTableIncluded || !t.tournamentGroupName);
  const hasGeneralTable = generalTableTeams.length > 0;
  
  // For GroupsPlayoff, we show multiple tables. For others, just one.
  const standingsData = !isGroupsPlayoff ? calculateStandings() : [];

  const handleCreateMatch = async () => {
    try {
      if (!newMatchForm.homeTeamId || !newMatchForm.awayTeamId) {
        alert('Будь ласка, оберіть обидві команди');
        return;
      }
      if (newMatchForm.homeTeamId === newMatchForm.awayTeamId) {
        alert('Команди мають бути різними');
        return;
      }

      const homeTeam = details.teams.find((t: any) => t.id === parseInt(newMatchForm.homeTeamId));
      const awayTeam = details.teams.find((t: any) => t.id === parseInt(newMatchForm.awayTeamId));

      let createApi;
      let payload: any = {
        eventId: details.id,
        homeTeamId: parseInt(newMatchForm.homeTeamId),
        awayTeamId: parseInt(newMatchForm.awayTeamId),
        stage: newMatchForm.stage,
        setsToWin: details.setsToWin || 2,
        homeRosterIds: homeTeam?.players?.map((p: any) => p.id) || [],
        awayRosterIds: awayTeam?.players?.map((p: any) => p.id) || [],
        homeGoalkeeperId: homeTeam?.goalkeeperId,
        awayGoalkeeperId: awayTeam?.goalkeeperId,
        homeCaptainId: homeTeam?.captainId,
        awayCaptainId: awayTeam?.captainId
      };

      switch (getCanonicalSport(sport)) {
        case 'volleyball': createApi = createVolleyballMatch; break;
        case 'beach_volleyball': createApi = createBeachVolleyballMatch; break;
        case 'football': createApi = createFootballMatch; break;
        case 'basketball_3x3': createApi = createBasketball3x3Match; break;
        case 'badminton': 
          createApi = createBadmintonMatch; 
          payload.eventId = details.id;
          payload.homeTeamId = parseInt(newMatchForm.homeTeamId);
          payload.awayTeamId = parseInt(newMatchForm.awayTeamId);
          payload.homeScore = 0;
          payload.awayScore = 0;
          break;
        case 'chess': 
          createApi = createChessGame; 
          payload = {
            tournamentId: details.id,
            whiteTeamId: parseInt(newMatchForm.homeTeamId),
            blackTeamId: parseInt(newMatchForm.awayTeamId),
            round: newMatchForm.stage === 'Фінал' ? 100 : (newMatchForm.stage.startsWith('Раунд') ? parseInt(newMatchForm.stage.replace('Раунд ', '')) : 1),
            result: 'NotPlayed'
          };
          break;
        case 'checkers': 
          createApi = createCheckersGame; 
          payload = {
            tournamentId: details.id,
            whiteTeamId: parseInt(newMatchForm.homeTeamId),
            blackTeamId: parseInt(newMatchForm.awayTeamId),
            round: newMatchForm.stage === 'Фінал' ? 100 : (newMatchForm.stage.startsWith('Раунд') ? parseInt(newMatchForm.stage.replace('Раунд ', '')) : 1),
            result: 'NotPlayed'
          };
          break;
        case 'tabletennis':
        case 'table_tennis':
        case 'table-tennis':
          createApi = createTableTennisMatch;
          payload = {
            tournamentId: details.id,
            homeTeamId: parseInt(newMatchForm.homeTeamId),
            awayTeamId: parseInt(newMatchForm.awayTeamId),
            stage: newMatchForm.stage,
            setsToWin: details.setsToWin || 3
          };
          break;
        default: createApi = createVolleyballMatch;
      }


      await createApi(payload);
      setIsCreateMatchModalOpen(false);
      setNewMatchForm(prev => ({ ...prev, homeTeamId: '', awayTeamId: '' }));
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при створенні матчу');
    }
  };

  const handleCreateTeam = async () => {
    try {
      if (!newTeamForm.name) {
        alert('Будь ласка, введіть назву команди');
        return;
      }
      const payload = {
        name: newTeamForm.name,
        eventId: details.id,
        tournamentGroupName: newTeamForm.tournamentGroupName || null,
        isGeneralTableIncluded: newTeamForm.isGeneralTableIncluded
      };

      let createApi;
      switch (getCanonicalSport(sport)) {
        case 'volleyball': createApi = createVolleyballTeam; break;
        case 'beach_volleyball': createApi = createBeachVolleyballTeam; break;
        case 'football': createApi = createFootballTeam; break;
        case 'basketball_3x3': createApi = createBasketball3x3Team; break;
        case 'badminton': createApi = createBadmintonTeam; break;
        case 'chess': createApi = createChessTeam; break;
        case 'checkers': createApi = createCheckersTeam; break;
        case 'tabletennis':
        case 'table_tennis':
        case 'table-tennis': createApi = createTableTennisTeam; break;
        default: createApi = createVolleyballTeam;
      }


      await createApi(payload);

      setIsCreateTeamModalOpen(false);
      setNewTeamForm({ name: '', tournamentGroupName: '', isGeneralTableIncluded: false });
      onRefresh();
    } catch (err) {
      console.error('Create Team Error:', err);
      alert(`Помилка при створенні команди: ${err instanceof Error ? err.message : 'Невідома помилка'}`);
    }
  };

  const handleTogglePlayerInTeam = async (teamId: number, playerId: number) => {
    try {
      const updateApi = getTeamUpdateApi();
      const team = details.teams?.find((t: any) => t.id === teamId);
      if (!team) return;

      const playerIds = team.players?.map((p: any) => p.id) || [];
      const isAlreadyIn = playerIds.includes(playerId);

      const newPlayerIds = isAlreadyIn 
        ? playerIds.filter((id: number) => id !== playerId)
        : [...playerIds, playerId];

      await updateApi(teamId, { ...team, playerIds: newPlayerIds });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при зміні складу команди');
    }
  };

  const handleApproveParticipant = async (playerId: number) => {
    try {
      let approveApi;
      switch (getCanonicalSport(sport)) {
        case 'football': approveApi = approveFootballParticipant; break;
        case 'volleyball': approveApi = approveVolleyballParticipant; break;
        case 'beach_volleyball': approveApi = approveBeachVolleyballParticipant; break;
        case 'basketball_3x3': approveApi = approveBasketball3x3Participant; break;
        case 'badminton': approveApi = approveBadmintonParticipant; break;
        case 'table_tennis':
        case 'tabletennis': approveApi = approveTableTennisParticipant; break;
        default: approveApi = approveFootballParticipant;
      }
      await approveApi(details.id, playerId);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при підтвердженні заявки');
    }
  };

  const handleRejectParticipant = async (playerId: number) => {
    try {
      let rejectApi;
      switch (getCanonicalSport(sport)) {
        case 'football': rejectApi = rejectFootballParticipant; break;
        case 'volleyball': rejectApi = rejectVolleyballParticipant; break;
        case 'beach_volleyball': rejectApi = rejectBeachVolleyballParticipant; break;
        case 'basketball_3x3': rejectApi = rejectBasketball3x3Participant; break;
        case 'badminton': rejectApi = rejectBadmintonParticipant; break;
        case 'table_tennis':
        case 'tabletennis': rejectApi = rejectTableTennisParticipant; break;
        default: rejectApi = rejectFootballParticipant;
      }
      await rejectApi(details.id, playerId);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при відхиленні заявки');
    }
  };

  const handleRemoveParticipant = async (playerId: number) => {
    if (!window.confirm('Ви впевнені, що хочете повністю видалити цей запис про реєстрацію?')) return;
    try {
      let removeApi;
      switch (getCanonicalSport(sport)) {
        case 'football': removeApi = removeFootballParticipant; break;
        case 'volleyball': removeApi = removeVolleyballParticipant; break;
        case 'beach_volleyball': removeApi = removeBeachVolleyballParticipant; break;
        case 'basketball_3x3': removeApi = removeBasketball3x3Participant; break;
        case 'badminton': removeApi = removeBadmintonParticipant; break;
        case 'table_tennis':
        case 'tabletennis': removeApi = removeTableTennisParticipant; break;
        default: removeApi = removeFootballParticipant;
      }
      await removeApi(details.id, playerId);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при видаленні учасника');
    }
  };
  const handleSetCaptain = async (teamId: number, playerId: number | null) => {
    try {
      const team = details.teams?.find((t: any) => t.id === teamId);
      if (!team) return;

      const updateApi = getTeamUpdateApi();
      
      const payload = {
        id: team.id,
        name: team.name,
        eventId: team.eventId,
        points: team.points,
        rank: team.rank,
        captainId: playerId,
        goalkeeperId: team.goalkeeperId,
        playerIds: team.players?.map((p: any) => p.id) || []
      };

      await updateApi(teamId, payload);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при призначенні капітана');
    }
  };

  const handleSetGoalkeeper = async (teamId: number, playerId: number | null) => {
    try {
      const team = details.teams?.find((t: any) => t.id === teamId);
      if (!team) return;

      const updateApi = getTeamUpdateApi();
      
      const payload = {
        id: team.id,
        name: team.name,
        eventId: team.eventId,
        points: team.points,
        rank: team.rank,
        captainId: team.captainId,
        goalkeeperId: playerId,
        playerIds: team.players?.map((p: any) => p.id) || []
      };

      await updateApi(teamId, payload);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при призначенні воротаря');
    }
  };
  
  const handleToggleGeneralTable = async (teamId: number) => {
    try {
      const team = details.teams?.find((t: any) => t.id === teamId);
      if (!team) return;

      const updateApi = getTeamUpdateApi();
      await updateApi(teamId, { ...team, isGeneralTableIncluded: !team.isGeneralTableIncluded });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при зміні статусу загальної таблиці');
    }
  };

  const handleQuickAddPlayer = async () => {
    if (!quickAddForm.firstName || !quickAddForm.lastName || !quickAddForm.group) {
      alert('Будь ласка, заповніть усі поля');
      return;
    }

    try {
      const newPlayer = await createPlayer({
        firstName: quickAddForm.firstName,
        lastName: quickAddForm.lastName,
        patronymic: quickAddForm.patronymic || null,
        group: quickAddForm.group,
        city: 'Чернівці',
        gender: quickAddForm.gender
      } as PlayerDto);

      // Add to team
      await handleTogglePlayerInTeam(editingTeamId!, newPlayer.id!);
      
      // Refresh players list
      // Reset form
      setQuickAddForm({ firstName: '', lastName: '', patronymic: '', group: '', gender: 'None' });
      setIsQuickAddMode(false);
      setSearchQuery('');
    } catch (err) {
      console.error('Quick Add Error:', err);
      alert('Помилка при швидкому додаванні гравця');
    }
  };

  const handleDeleteTeam = async (teamId: number, teamName: string) => {
    if (!window.confirm(`Ви впевнені, що хочете видалити команду "${teamName}"? Цю дію неможливо скасувати.`)) {
      return;
    }

    try {
      let deleteApi;
      switch (getCanonicalSport(sport)) {
        case 'volleyball': deleteApi = deleteVolleyballTeam; break;
        case 'beach_volleyball': deleteApi = deleteBeachVolleyballTeam; break;
        case 'football': deleteApi = deleteFootballTeam; break;
        case 'basketball_3x3': deleteApi = deleteBasketball3x3Team; break;
        case 'badminton': deleteApi = deleteBadmintonTeam; break;
        case 'chess': deleteApi = deleteChessTeam; break;
        case 'checkers': deleteApi = deleteCheckersTeam; break;
        case 'tabletennis':
        case 'table_tennis':
        case 'table-tennis': deleteApi = deleteTableTennisTeam; break;
        default: deleteApi = deleteVolleyballTeam;
      }
      await deleteApi(teamId);
      if (editingTeamId === teamId) {
        setEditingTeamId(null);
        setIsTeamRosterModalOpen(false);
      }
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при видаленні команди');
    }
  };

  const STAGE_ORDER = [
    '1/16 Фіналу', '1/8 Фіналу', 'Чвертьфінал', 'Півфінал', 'Матч за 3-те місце', 'Фінал'
  ];

  const getEligibleTeams = (stage: string) => {
    const allTeams = details.teams || [];
    if (!useStrictFiltering || sport !== 'football') return allTeams;
    
    const currentStageIdx = STAGE_ORDER.indexOf(stage);
    if (currentStageIdx <= 0) return allTeams;

    const prevStage = STAGE_ORDER[currentStageIdx - 1];
    const matches = details.matches || details.games || [];
    
    // For 3rd place match, we need losers of the previous stage (Semifinal)
    const isThirdPlaceMatch = stage === 'Матч за 3-те місце';
    
    const winnersOfPrevStage = matches
      .filter((m: any) => m.stage === prevStage)
      .map((m: any) => {
        const hScore = m.homeTeamScore ?? 0;
        const aScore = m.awayTeamScore ?? 0;
        if (isThirdPlaceMatch) {
            // Get Losers
            if (hScore < aScore) return m.homeTeamId;
            if (aScore < hScore) return m.awayTeamId;
        } else {
            // Get Winners
            if (hScore > aScore) return m.homeTeamId;
            if (aScore > hScore) return m.awayTeamId;
        }
        return null;
      })
      .filter((id: any) => id !== null);

    return allTeams.filter((t: any) => winnersOfPrevStage.includes(t.id));
  };

  const eligibleTeams = getEligibleTeams(newMatchForm.stage);

  const getSportLabel = () => {
    switch (getCanonicalSport(sport)) {
      case 'volleyball':
      case 'волейбол': return 'Волейбол';
      case 'beach_volleyball':
      case 'beach-volleyball':
      case 'пляжний_волейбол': return 'Пляжний Волейбол';
      case 'football':
      case 'футбол': return 'Футбол';
      case 'basketball_3x3':
      case 'basketball3x3':
      case 'basketball-3x3':
      case 'баскетбол_3х3': return 'Баскетбол';
      case 'badminton':
      case 'бадмінтон': return 'Бадмінтон';
      case 'chess':
      case 'шахи': return 'Шахи';
      case 'checkers':
      case 'шашки': return 'Шашки';
      case 'tabletennis':
      case 'table_tennis':
      case 'table-tennis':
      case 'настільний_теніс': return 'Настільний Теніс';
      default: return 'Спорт';
    }
  };



  return (
    <div className={styles.teamSportsView}>
      {/* Header Info */}
      <div className={styles.sportHeader}>
        <div className={styles.sportBadge}>
          <i className="fas fa-trophy"></i> {getSportLabel()}
        </div>
        <div className={styles.tabsNav}>
          {!hasBracket && (
            <button className={activeTab === 'matches' ? styles.activeTab : ''} onClick={() => setActiveTab('matches')}>Матчі</button>
          )}
          <button className={activeTab === 'standings' ? styles.activeTab : ''} onClick={() => setActiveTab('standings')}>Таблиця</button>
          {hasBracket && (
            <button className={activeTab === 'bracket' ? styles.activeTab : ''} onClick={() => setActiveTab('bracket')}>Сітка</button>
          )}
          <button className={activeTab === 'teams' ? styles.activeTab : ''} onClick={() => setActiveTab('teams')}>Команди</button>
          <button className={activeTab === 'voting' ? styles.activeTab : ''} onClick={() => setActiveTab('voting')}>Голосування</button>
          {(sport === 'football' || sport === 'футбол') && (
            <button className={activeTab === 'stats' ? styles.activeTab : ''} onClick={() => setActiveTab('stats')}>Статистика</button>
          )}
          {isAdmin && !isRegionalOrNational && (
            <button className={activeTab === 'participants' ? styles.activeTab : ''} onClick={() => setActiveTab('participants')}>
              Учасники {details.participants?.filter((p: any) => p.status === RegistrationStatus.Pending)?.length > 0 && (
                <span style={{ 
                  marginLeft: '6px', 
                  background: '#ef4444', 
                  color: '#fff', 
                  padding: '1px 6px', 
                  borderRadius: '10px', 
                  fontSize: '11px',
                  fontWeight: 'bold',
                  boxShadow: '0 2px 4px rgba(239,68,68,0.3)'
                }}>
                  {details.participants.filter((p: any) => p.status === RegistrationStatus.Pending).length}
                </span>
              )}
            </button>
          )}
          {isAdmin && activeTab === 'matches' && (
            <button className={styles.addMatchGlobalBtn} onClick={() => setIsCreateMatchModalOpen(true)}>
              <i className="fas fa-plus-circle"></i> <span>Додати матч</span>
            </button>
          )}
          {isAdmin && activeTab === 'teams' && (
            <button className={styles.addMatchGlobalBtn} onClick={() => setIsCreateTeamModalOpen(true)}>
              <i className="fas fa-users-cog"></i> <span>Створити команду</span>
            </button>
          )}
        </div>
      </div>




      {/* Main Content Areas */}
      <div className={styles.tabContent}>
        {activeTab === 'voting' && (
          <div className={styles.votingSection} style={{ marginBottom: '20px', background: '#fff', padding: '15px', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.05)' }}>
            <h3 style={{ fontSize: '16px', fontWeight: 'bold', marginBottom: '10px', color: '#1e293b', display: 'flex', alignItems: 'center' }}>
              <i className="fas fa-poll" style={{ marginRight: '8px', color: '#3b82f6' }}></i>
              Голосування за переможця (Всього голосів: {voteStats?.totalVotes || 0})
              <i className="fas fa-info-circle" title="Голосування має суто розважальний характер і не впливає на офіційні результати змагань" style={{ marginLeft: '6px', color: '#64748b', cursor: 'help', fontSize: '14px' }}></i>
            </h3>
            
            {voteStats?.hasVoted && (
              <p style={{ color: '#10b981', fontSize: '14px', marginBottom: '10px' }}>
                <i className="fas fa-check-circle" style={{ marginRight: '4px' }}></i>
                Ви вже проголосували!
              </p>
            )}

            <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
              {(details.teams || []).map((team: any) => {
                const stats = voteStats?.optionStats?.find((s: any) => s.id === team.id);
                const votes = stats?.votes || 0;
                const percentage = stats?.percentage || 0;
                return (
                  <div key={team.id} style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                    <div style={{ width: '150px', fontSize: '14px', fontWeight: 500, color: '#334155' }}>
                      {team.name}
                    </div>
                    <div style={{ flex: 1, height: '8px', background: '#e2e8f0', borderRadius: '4px', overflow: 'hidden' }}>
                      <div style={{ width: `${percentage}%`, height: '100%', background: '#3b82f6' }}></div>
                    </div>
                    <div style={{ width: '100px', fontSize: '12px', color: '#64748b', textAlign: 'right' }}>
                      {votes} ({percentage}%)
                    </div>
                    {!voteStats?.hasVoted && !isVoting && new Date(details.rawDate || details.date) > new Date() && (
                      <button 
                        onClick={() => handleVote(details.isTeam ? team.id : undefined, !details.isTeam ? team.id : undefined)}
                        style={{ padding: '2px 8px', fontSize: '12px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
                      >
                        Голосувати
                      </button>
                    )}
                  </div>
                );
              })}
            </div>
          </div>
        )}
        {/* football Admin Panel (Horizontal) */}
        {isAdmin && activeTab === 'bracket' && hasBracket && (
            <div className={styles.footballAdminPanel}>
                <div className={styles.panelHeader}>
                    <i className="fas fa-plus-circle"></i> Додати новий поєдинок
                </div>
                <div className={styles.panelForm}>
                    <div className={styles.panelGroup}>
                        <label>Команда 1</label>
                        <select 
                            value={newMatchForm.homeTeamId} 
                            onChange={e => setNewMatchForm({...newMatchForm, homeTeamId: e.target.value})}
                        >
                            <option value="">Оберіть...</option>
                            {eligibleTeams.map((t: any) => (
                                <option key={t.id} value={t.id}>{t.name}</option>
                            ))}
                        </select>
                    </div>
                    
                    <div className={styles.panelVs}>vs</div>

                    <div className={styles.panelGroup}>
                        <label>Команда 2</label>
                        <select 
                            value={newMatchForm.awayTeamId} 
                            onChange={e => setNewMatchForm({...newMatchForm, awayTeamId: e.target.value})}
                        >
                            <option value="">Оберіть...</option>
                            {eligibleTeams.map((t: any) => (
                                <option key={t.id} value={t.id}>{t.name}</option>
                            ))}
                        </select>
                    </div>

                    <div className={styles.panelGroup} style={{ width: '160px' }}>
                        <label>Етап</label>
                        <select 
                            value={stageType} 
                            onChange={e => {
                                const val = e.target.value;
                                setStageType(val);
                                if (val === 'Раунд') {
                                    setNewMatchForm({...newMatchForm, stage: `Раунд ${roundNumber}`});
                                } else {
                                    setNewMatchForm({...newMatchForm, stage: val});
                                }
                            }}
                        >
                            <option value="Груповий етап">Груповий етап</option>
                            <option value="Раунд">Раунд...</option>
                            <option value="1/16 Фіналу">1/16 Фіналу</option>
                            <option value="1/8 Фіналу">1/8 Фіналу</option>
                            <option value="Чвертьфінал">Чвертьфінал</option>
                            <option value="Півфінал">Півфінал</option>
                            <option value="Матч за 3-те місце">Матч за 3-те місце</option>
                            <option value="Фінал">Фінал</option>
                        </select>
                    </div>

                    {stageType === 'Раунд' && (
                        <div className={styles.panelGroup} style={{ width: '80px' }}>
                            <label>№</label>
                            <input 
                                type="number" 
                                min="1"
                                value={roundNumber}
                                onFocus={(e) => e.target.select()}
                                onChange={e => {
                                    const num = parseInt(e.target.value) || 1;
                                    setRoundNumber(num);
                                    setNewMatchForm({...newMatchForm, stage: `Раунд ${num}`});
                                }}
                            />
                        </div>
                    )}

                    <div className={styles.panelGroup} style={{ width: 'auto' }}>
                        <label style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                            Фільтрація <i className="fas fa-question-circle" style={{ fontSize: '10px', color: '#94a3b8', cursor: 'help' }} title="Якщо увімкнено, при створенні матчу можна вибрати лише ті команди, які виграли на попередньому етапі (1/16 -> 1/8 -> Чвертьфінал і т.д.)"></i>
                        </label>
                        <div className={styles.panelFilterToggle}>
                            <label className={styles.switchMini}>
                                <input 
                                    type="checkbox" 
                                    checked={useStrictFiltering} 
                                    onChange={(e) => setUseStrictFiltering(e.target.checked)}
                                />
                                <span className={styles.sliderMini}></span>
                            </label>
                            <span style={{ fontSize: '11px', fontWeight: 600, color: useStrictFiltering ? '#3b82f6' : '#94a3b8' }}>
                                {useStrictFiltering ? 'Увімк' : 'Вимк'}
                            </span>
                        </div>
                    </div>

                    <button className={styles.panelCreateBtn} onClick={handleCreateMatch}>
                        Створити
                    </button>
                </div>
            </div>
        )}

        {activeTab === 'matches' && (
          <div className={styles.matchesGrid}>
            {(details.matches || details.games)?.map((match: any) => {
              const isVolleyball = sport.includes('volleyball') || sport === 'badminton';
              const isChessLike = sport === 'chess' || sport === 'checkers';
              
              // Get scores based on sport type
              const homeScore = isVolleyball ? (match.homeSetsWon ?? match.homeScore ?? 0) : 
                               isChessLike ? (match.whiteTeamScore ?? match.whitePlayerPoints ?? 0) :
                               (match.homeTeamScore ?? 0);
              const awayScore = isVolleyball ? (match.awaySetsWon ?? match.awayScore ?? 0) : 
                               isChessLike ? (match.blackTeamScore ?? match.blackPlayerPoints ?? 0) :
                               (match.awayTeamScore ?? 0);
              
              const isHomeWinner = Number(homeScore) > Number(awayScore);
              const isAwayWinner = Number(awayScore) > Number(homeScore);
              const isDraw = Number(homeScore) === Number(awayScore) && Number(homeScore) > 0;

              return (
                <div key={match.id} className={styles.matchCard} onClick={() => { setSelectedMatchForInfo(match); setIsMatchInfoModalOpen(true); }} style={{ cursor: 'pointer' }}>
                  <div className={styles.matchStage}>
                    {match.stage || (match.round === 100 ? 'Фінал' : `Раунд ${match.round}`)}
                  </div>
                  
                  <div className={styles.matchTeams}>
                    <div className={`${styles.teamRow} ${isHomeWinner ? styles.winnerRow : ''} ${isDraw ? styles.drawRow : ''}`}>
                      <div className={`${styles.teamLogo} ${isHomeWinner ? styles.winnerLogo : ''}`}>
                        {(match.whiteTeam?.name || match.homeTeam?.name || match.whitePlayer?.lastName)?.[0] || 'T'}
                      </div>
                      <div 
                        className={`${styles.teamName} ${isHomeWinner ? styles.winnerName : ''} ${isDraw ? styles.drawName : ''}`}
                        onClick={(e) => {
                          e.stopPropagation();
                          const tId = match.whiteTeamId || match.homeTeamId;
                          if (tId) { setEditingTeamId(tId); setIsTeamRosterModalOpen(true); }
                        }}
                        style={{ cursor: 'pointer' }}
                      >
                        {match.whiteTeam?.name || match.homeTeam?.name || match.whitePlayer?.name || match.whitePlayer?.lastName || 'Home'}
                        {isHomeWinner && <i className={`fas fa-crown ${styles.crownIcon}`}></i>}
                        {isDraw && <i className={`fas fa-handshake ${styles.drawIcon}`}></i>}
                      </div>
                      <div className={styles.scoreInput} onClick={(e) => e.stopPropagation()}>
                        <input 
                          type="number" 
                          min="0"
                          max={isChessLike ? 2 : undefined}
                          step={isChessLike ? "0.5" : "1"}
                          key={`${match.id}-home-${homeScore}`}
                          defaultValue={homeScore}
                          onFocus={(e) => e.target.select()}
                          onBlur={(e) => handleUpdateScore(match.id, parseFloat(e.target.value) || 0, awayScore)}
                          disabled={!isAdmin}
                        />
                      </div>
                      {homeScore === awayScore && (sport === 'football' || sport === 'футбол') && (
                        <div style={{ marginLeft: '8px' }} onClick={(e) => e.stopPropagation()}>
                          <input 
                            type="number" 
                            min="0"
                            placeholder="П"
                            style={{ width: '40px', padding: '4px', borderRadius: '4px', border: '1px solid #f59e0b', textAlign: 'center' }}
                            defaultValue={match.homePenaltyScore ?? ''}
                            onBlur={(e) => handleUpdatePenalty(match.id, e.target.value ? parseInt(e.target.value) : null, match.awayPenaltyScore)}
                            disabled={!isAdmin}
                            title="Пенальті"
                          />
                        </div>
                      )}
                    </div>

                    <div className={styles.vsDivider}>
                      VS
                      {match.homePenaltyScore !== null && match.awayPenaltyScore !== null && match.homePenaltyScore !== undefined && match.awayPenaltyScore !== undefined && (
                        <div style={{ fontSize: '11px', color: '#b45309', background: '#fef3c7', padding: '2px 6px', borderRadius: '4px', marginTop: '4px', fontWeight: 'bold' }}>
                          пен. {match.homePenaltyScore}:{match.awayPenaltyScore}
                        </div>
                      )}
                    </div>

                    <div className={`${styles.teamRow} ${isAwayWinner ? styles.winnerRow : ''} ${isDraw ? styles.drawRow : ''}`}>
                      <div className={`${styles.teamLogo} ${isAwayWinner ? styles.winnerLogo : ''}`}>
                        {(match.blackTeam?.name || match.awayTeam?.name || match.awayTeamName || match.blackPlayer?.lastName)?.[0] || 'T'}
                      </div>
                      <div 
                        className={`${styles.teamName} ${isAwayWinner ? styles.winnerName : ''} ${isDraw ? styles.drawName : ''}`}
                        onClick={(e) => {
                          e.stopPropagation();
                          const tId = match.blackTeamId || match.awayTeamId;
                          if (tId) { setEditingTeamId(tId); setIsTeamRosterModalOpen(true); }
                        }}
                        style={{ cursor: 'pointer' }}
                      >
                        {match.blackTeam?.name || match.awayTeam?.name || match.awayTeamName || match.blackPlayer?.name || match.blackPlayer?.lastName || 'Away'}
                        {isAwayWinner && <i className={`fas fa-crown ${styles.crownIcon}`}></i>}
                        {isDraw && <i className={`fas fa-handshake ${styles.drawIcon}`}></i>}
                      </div>
                      <div className={styles.scoreInput} onClick={(e) => e.stopPropagation()}>
                        <input 
                          type="number" 
                          min="0"
                          step={isChessLike ? "0.5" : "1"}
                          key={`${match.id}-away-${awayScore}`}
                          defaultValue={awayScore}
                          onFocus={(e) => e.target.select()}
                          onBlur={(e) => handleUpdateScore(match.id, homeScore, parseFloat(e.target.value) || 0)}
                          disabled={!isAdmin}
                        />
                      </div>
                      {homeScore === awayScore && (sport === 'football' || sport === 'футбол') && (
                        <div style={{ marginLeft: '8px' }} onClick={(e) => e.stopPropagation()}>
                          <input 
                            type="number" 
                            min="0"
                            placeholder="П"
                            style={{ width: '40px', padding: '4px', borderRadius: '4px', border: '1px solid #f59e0b', textAlign: 'center' }}
                            defaultValue={match.awayPenaltyScore ?? ''}
                            onBlur={(e) => handleUpdatePenalty(match.id, match.homePenaltyScore, e.target.value ? parseInt(e.target.value) : null)}
                            disabled={!isAdmin}
                            title="Пенальті"
                          />
                        </div>
                      )}
                    </div>
                  </div>

                  {isAdmin && (
                    <div className={styles.matchActions} onClick={(e) => e.stopPropagation()}>
                      {(sport === 'football' || sport === 'футбол') && (
                        <button 
                          className={styles.scorersBtn}
                          onClick={() => handleOpenScorers(match)}
                          title="Автори голів"
                        >
                          <i className="fas fa-futbol"></i>
                        </button>
                      )}
                      <button 
                        className={styles.deleteMatchBtn} 
                        onClick={() => handleDeleteMatch(match.id)}
                        title="Видалити матч"
                      >
                        <i className="fas fa-trash-alt"></i>
                      </button>
                    </div>
                  )}
                  
                   {/* Goals summary for non-admins or small preview */}
                   {(match.goals?.length > 0 || match.homeGoalkeeper || match.awayGoalkeeper || match.homeCaptain || match.awayCaptain) && (
                     <div className={styles.scorersPreview}>
                        <div className={styles.scorersColumn}>
                           {match.homeCaptain && (
                             <span className={styles.captainBadge} title="Капітан">
                               <i className="fas fa-copyright"></i> {match.homeCaptain.lastName}
                             </span>
                           )}
                           {match.homeGoalkeeper && (
                             <span className={styles.goalkeeperBadge} title="Воротар">
                               <i className="fas fa-hands"></i> {match.homeGoalkeeper.lastName}
                             </span>
                           )}
                           {match.goals?.filter((g: any) => match.homeRoster?.some((p: any) => p.id === g.playerId)).map((g: any) => (
                              <span key={g.id} className={styles.scorerItem}>
                                {g.player?.footballShirtNumber ? `№${g.player.footballShirtNumber} ` : ''}{g.player?.lastName} ({g.goalsCount})
                              </span>
                           ))}
                        </div>
                        <div className={styles.scorersColumn}>
                           {match.awayCaptain && (
                             <span className={styles.captainBadge} title="Капітан">
                               <i className="fas fa-copyright"></i> {match.awayCaptain.lastName}
                             </span>
                           )}
                           {match.awayGoalkeeper && (
                             <span className={styles.goalkeeperBadge} title="Воротар">
                               <i className="fas fa-hands"></i> {match.awayGoalkeeper.lastName}
                             </span>
                           )}
                           {match.goals?.filter((g: any) => match.awayRoster?.some((p: any) => p.id === g.playerId)).map((g: any) => (
                              <span key={g.id} className={styles.scorerItem}>
                                {g.player?.footballShirtNumber ? `№${g.player.footballShirtNumber} ` : ''}{g.player?.lastName} ({g.goalsCount})
                              </span>
                           ))}
                        </div>
                     </div>
                   )}

                </div>
              );
            })}
            {(!(details.matches || details.games) || (details.matches || details.games).length === 0) && <p className={styles.noData}>Матчів ще не заплановано</p>}
          </div>
        )}

        {activeTab === 'standings' && (
          <div className={styles.standingsSection}>
            {isGroupsPlayoff ? (
              <>
                {hasGeneralTable && (
                   <div className={styles.groupStandings} style={{ marginBottom: '40px' }}>
                      <h3 className={styles.groupTitle} style={{ color: '#3b82f6' }}>
                        <i className="fas fa-star" style={{ marginRight: '8px' }}></i>
                        Загальна таблиця (Фінальний етап)
                      </h3>
                      <div className={styles.standingsTable}>
                        <table className={styles.leagueTable}>
                          <thead>
                            <tr>
                              <th title="Місце">#</th>
                              <th title="Назва команди">Команда</th>
                              <th title="Ігри">І</th>
                              <th title="Виграші">В</th>
                              <th title="Поразки">П</th>
                              <th title="Забиті - Пропущені">Сети (З-П)</th>
                              <th title="Очки">Очки</th>
                            </tr>
                          </thead>
                          <tbody>
                            {calculateStandings('GENERAL_TABLE').map((team: any) => (
                              <tr key={team.id} style={team.isGeneralTableIncluded ? { backgroundColor: 'rgba(59, 130, 246, 0.05)' } : {}}>
                                <td>{team.displayRank}</td>
                                <td className={styles.tableName} onClick={() => { setEditingTeamId(team.id); setIsTeamRosterModalOpen(true); }} style={{ cursor: 'pointer' }}>
                                  {team.name}
                                  {team.isGeneralTableIncluded && <i className="fas fa-check-circle" style={{ marginLeft: '6px', color: '#3b82f6', fontSize: '12px' }} title="Додано вручну"></i>}
                                </td>
                                <td>{team.gp}</td>
                                <td>{team.w}</td>
                                <td>{team.l}</td>
                                <td>{team.gf} - {team.ga}</td>
                                <td className={team.points > 0 ? styles.tablePointsHighlight : styles.tablePoints}>
                                  {isAdmin ? (
                                    <input 
                                      type="number" 
                                      min="0"
                                      className={styles.pointsInlineInput}
                                      defaultValue={team.points}
                                      onFocus={(e) => e.target.select()}
                                      onBlur={(e) => handleUpdateTeamPoints(team.id, parseInt(e.target.value) || 0)}
                                    />
                                  ) : (
                                    team.points
                                  )}
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                   </div>
                )}

                {sortedGroups.map(groupName => (
                  <div key={groupName} className={styles.groupStandings} style={{ borderTop: '1px solid #e2e8f0', paddingTop: '20px', marginTop: '20px' }}>
                    <h3 className={styles.groupTitle}>Група {groupName}</h3>
                    <div className={styles.standingsTable}>
                      <table className={styles.leagueTable}>
                        <thead>
                          <tr>
                            <th title="Місце">#</th>
                            <th title="Назва команди">Команда</th>
                            <th title="Ігри">І</th>
                            <th title="Виграші">В</th>
                            <th title="Поразки">П</th>
                            <th title="Забиті - Пропущені">Сети (З-П)</th>
                            <th title="Очки">Очки</th>
                          </tr>
                        </thead>
                        <tbody>
                          {calculateStandings(groupName).map((team: any) => (
                            <tr key={team.id}>
                              <td>{team.displayRank}</td>
                              <td className={styles.tableName} onClick={() => { setEditingTeamId(team.id); setIsTeamRosterModalOpen(true); }} style={{ cursor: 'pointer' }}>{team.name}</td>
                              <td>{team.gp}</td>
                              <td>{team.w}</td>
                              <td>{team.l}</td>
                              <td>{team.gf} - {team.ga}</td>
                              <td className={styles.tablePoints}>
                                {isAdmin ? (
                                  <input 
                                    type="number" 
                                    min="0"
                                    className={styles.pointsInlineInput}
                                    defaultValue={team.points}
                                    onFocus={(e) => e.target.select()}
                                    onBlur={(e) => handleUpdateTeamPoints(team.id, parseInt(e.target.value) || 0)}
                                  />
                                ) : (
                                  team.points
                                )}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                ))}
              </>
            ) : (
              <div className={styles.standingsTable}>
                <table className={styles.leagueTable}>
                  <thead>
                    <tr>
                      <th title="Місце">#</th>
                      <th title="Назва команди">Команда</th>
                      <th title="Ігри">І</th>
                      <th title="Виграші">В</th>
                      <th title="Поразки">П</th>
                      {(sport === 'chess' || sport === 'checkers') ? (
                        <th title="Набрані очки">Очки (З-П)</th>
                      ) : (
                        <th title="Забиті - Пропущені">Сети (З-П)</th>
                      )}
                      <th title="Очки">Очки</th>
                    </tr>
                  </thead>
                  <tbody>
                    {(standingsData || []).map((team: any) => (
                      <tr key={team.id}>
                        <td>{team.displayRank}</td>
                        <td className={styles.tableName} onClick={() => { setEditingTeamId(team.id); setIsTeamRosterModalOpen(true); }} style={{ cursor: 'pointer' }}>
                          {team.name}
                        </td>
                        <td>{team.gp}</td>
                        <td>{team.w}</td>
                        <td>{team.l}</td>
                        <td>{team.gf} - {team.ga}</td>
                        <td className={team.points > 0 ? styles.tablePointsHighlight : styles.tablePoints}>
                          {isAdmin ? (
                            <input 
                              type="number" 
                              min="0"
                              className={styles.pointsInlineInput}
                              defaultValue={team.points}
                              onFocus={(e) => e.target.select()}
                              onBlur={(e) => handleUpdateTeamPoints(team.id, parseInt(e.target.value) || 0)}
                            />
                          ) : (
                            team.points
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

          </div>
        )}

        {activeTab === 'bracket' && hasBracket && (
          <FootballBracket 
            matches={details.matches || details.games || []}
            isAdmin={isAdmin}
            onDeleteMatch={handleDeleteMatch}
            onOpenScorers={handleOpenScorers}
            onViewRoster={(teamId) => { setEditingTeamId(teamId); setIsTeamRosterModalOpen(true); }}
            onViewInfo={(match) => { setSelectedMatchForInfo(match); setIsMatchInfoModalOpen(true); }}
            teams={details.teams || []}
          />
        )}

         {activeTab === 'teams' && (
          <div className={styles.teamsGrid}>
            {details.teams?.map((team: any) => (
              <div key={team.id} className={styles.teamCard} onClick={() => { setEditingTeamId(team.id); setIsTeamRosterModalOpen(true); }} style={{ cursor: 'pointer' }}>
                <div className={styles.teamCardHeader}>
                  <div className={styles.teamAvatar}>{team.name?.[0] || 'T'}</div>
                  <div className={styles.teamInfo}>
                    <h4>Команда {team.name}</h4>
                    <p>{team.players?.length || 0} гравців</p>
                  </div>
                  {isAdmin && (
                    <div className={styles.teamActions}>
                      <button className={styles.editTeamBtn} onClick={() => { setEditingTeamId(team.id); setIsTeamRosterModalOpen(true); }}>
                        <i className="fas fa-user-edit"></i> Склад
                      </button>
                      <button 
                        className={styles.deleteTeamBtn} 
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDeleteTeam(team.id, team.name);
                        }} 
                        title="Видалити команду"
                      >
                        <i className="fas fa-trash"></i>
                      </button>
                    </div>
                  )}
                </div>
                <div className={styles.teamPlayersPreview}>
                  {team?.players?.slice(0, 5).map((p: any) => (
                    <div key={p.id} className={`${styles.pMiniTag} ${team?.captainId === p.id ? styles.captainTag : ''} ${team?.goalkeeperId === p.id ? styles.goalkeeperTag : ''}`}>
                      {team?.captainId === p.id && <i className="fas fa-copyright" style={{marginRight: '4px', fontSize: '10px'}} title="Капітан"></i>}
                      {team?.goalkeeperId === p.id && <i className="fas fa-hands" style={{marginRight: '4px', fontSize: '10px'}} title="Воротар"></i>}
                      {p.lastName} {p.firstName?.[0] || ''}.
                    </div>
                  ))}
                  {team?.players?.length > 5 && <span className={styles.morePlayers}>+{team.players.length - 5}</span>}
                  {(!team?.players || team.players.length === 0) && <p className={styles.noPlayersHint}>Гравців ще не додано</p>}
                </div>
              </div>
            ))}
            {(!details.teams || details.teams.length === 0) && <p className={styles.noData}>У цих змаганнях ще немає команд</p>}
          </div>
        )}

        {activeTab === 'participants' && !isRegionalOrNational && (
          <div className={styles.participantsList}>
            <div className={styles.tableWrapper}>
              <table className={styles.dataTable}>
                <thead>
                  <tr>
                    <th>№</th>
                    <th>Учасник</th>
                    <th>Група</th>
                    <th style={{ textAlign: 'center' }}>Статус</th>
                    {isAdmin && <th style={{ textAlign: 'center' }}>Дії</th>}
                  </tr>
                </thead>
                <tbody>
                  {(details.participants || []).length > 0 ? (details.participants || []).map((p: any, idx: number) => (
                    <tr key={p.id || p.playerId}>
                      <td>{idx + 1}</td>
                      <td>
                        <strong>{p.playerName}</strong>
                      </td>
                      <td>{p.groupName || '—'}</td>
                      <td style={{ textAlign: 'center' }}>
                        <span style={{
                          fontSize: '11px',
                          fontWeight: 'bold',
                          padding: '4px 10px',
                          borderRadius: '6px',
                          textTransform: 'uppercase',
                          letterSpacing: '0.5px',
                          background: p.status === RegistrationStatus.Approved ? '#ecfdf5' : p.status === RegistrationStatus.Pending ? '#fffbeb' : '#fef2f2',
                          color: p.status === RegistrationStatus.Approved ? '#10b981' : p.status === RegistrationStatus.Pending ? '#f59e0b' : '#ef4444',
                          border: `1px solid ${p.status === RegistrationStatus.Approved ? '#bdf4e4' : p.status === RegistrationStatus.Pending ? '#fef3c7' : '#fee2e2'}`,
                          display: 'inline-block',
                          boxShadow: '0 1px 2px rgba(0,0,0,0.05)'
                        }}>
                          {p.status === RegistrationStatus.Approved ? 'Підтверджено' : 
                           p.status === RegistrationStatus.Pending ? 'Очікує' : 'Відхилено'}
                        </span>
                      </td>
                      {isAdmin && (
                        <td style={{ textAlign: 'center' }}>
                          <div style={{ display: 'flex', gap: '8px', justifyContent: 'center' }}>
                            {p.status === RegistrationStatus.Pending && (
                              <>
                                <button 
                                  onClick={() => handleApproveParticipant(p.playerId)} 
                                  style={{ padding: '6px 10px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(16,185,129,0.2)' }} 
                                  title="Підтвердити"
                                  onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                                  onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                                >
                                  <i className="fas fa-check"></i>
                                </button>
                                <button 
                                  onClick={() => handleRejectParticipant(p.playerId)} 
                                  style={{ padding: '6px 10px', background: '#f59e0b', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(245,158,11,0.2)' }} 
                                  title="Відхилити"
                                  onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                                  onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                                >
                                  <i className="fas fa-times"></i>
                                </button>
                              </>
                            )}
                            <button 
                              onClick={() => handleRemoveParticipant(p.playerId)} 
                              style={{ padding: '6px 10px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(239,68,68,0.2)' }} 
                              title="Видалити зовсім"
                              onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                              onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                            >
                              <i className="fas fa-trash"></i>
                            </button>
                          </div>
                        </td>
                      )}
                    </tr>
                  )) : (
                    <tr><td colSpan={isAdmin ? 5 : 4} className={styles.noData}>Учасників ще немає</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {activeTab === 'stats' && (sport === 'football' || sport === 'футбол') && (
          <div className={styles.statsSection}>
            <div className={styles.statsGrid}>
              
              {/* Top Scorers */}
              <div className={styles.statsCard}>
                <div className={styles.statsCardHeader}>
                  <i className="fas fa-futbol"></i>
                  <h4>Топ 3 бомбардири</h4>
                </div>
                <div className={styles.statsCardBody}>
                  {(footballStats?.topPlayers?.length ?? 0) > 0 ? (
                    footballStats?.topPlayers.map((item: any, idx: number) => (
                      <div key={idx} className={styles.statsItem}>
                        <span className={styles.statsRank}>{idx + 1}</span>
                        <span className={styles.statsName}>
                          {item.player?.footballShirtNumber ? `№${item.player.footballShirtNumber} ` : ''}
                          {item.player?.lastName} {item.player?.firstName}
                          {item.teamName ? ` (${item.teamName})` : ''}
                        </span>
                        <span className={styles.statsValue}>{item.goals} голів</span>
                      </div>
                    ))
                  ) : (
                    <p className={styles.noData}>Немає даних</p>
                  )}
                </div>
              </div>

              {/* Top Teams by Goals */}
              <div className={styles.statsCard}>
                <div className={styles.statsCardHeader}>
                  <i className="fas fa-bullseye"></i>
                  <h4>Топ 3 команди по голам</h4>
                </div>
                <div className={styles.statsCardBody}>
                  {(footballStats?.topTeamsByGoals?.length ?? 0) > 0 ? (
                    footballStats?.topTeamsByGoals.map((item: any, idx: number) => (
                      <div key={idx} className={styles.statsItem}>
                        <span className={styles.statsRank}>{idx + 1}</span>
                        <span className={styles.statsName}>{item.name}</span>
                        <span className={styles.statsValue}>{item.goals} голів</span>
                      </div>
                    ))
                  ) : (
                    <p className={styles.noData}>Немає даних</p>
                  )}
                </div>
              </div>

              {/* Most Yellow Cards */}
              <div className={styles.statsCard}>
                <div className={styles.statsCardHeader}>
                  <i className="fas fa-square" style={{ color: '#f59e0b' }}></i>
                  <h4>Найбільше жовтих карток</h4>
                </div>
                <div className={styles.statsCardBody}>
                  {(footballStats?.teamMostCards?.cards ?? 0) > 0 ? (
                    <div className={styles.statsItem}>
                      <span className={styles.statsRank}><i className="fas fa-exclamation-triangle" style={{ color: '#f59e0b' }}></i></span>
                      <span className={styles.statsName}>{footballStats?.teamMostCards.name}</span>
                      <span className={styles.statsValue}>{footballStats?.teamMostCards.cards} карток</span>
                    </div>
                  ) : (
                    <p className={styles.noData}>Немає даних (або 0 карток)</p>
                  )}
                </div>
              </div>

            </div>
          </div>
        )}

      </div>

      {/* Create Match Modal */}
      {isCreateMatchModalOpen && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsCreateMatchModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.createMatchModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Спланувати новий матч</h3>
              <button className={styles.closeButton} onClick={() => setIsCreateMatchModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
              <div className={styles.createMatchForm}>
                <div className={styles.formGroup}>
                  <label>Етап змагання</label>
                  <div style={{ display: 'flex', gap: '8px' }}>
                    <select 
                        style={{ flex: 2, padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0' }}
                        value={stageType} 
                        onChange={e => {
                            const val = e.target.value;
                            setStageType(val);
                            if (val === 'Раунд') {
                                setNewMatchForm({...newMatchForm, stage: `Раунд ${roundNumber}`});
                            } else {
                                setNewMatchForm({...newMatchForm, stage: val});
                            }
                        }}
                    >
                        <option value="Груповий етап">Груповий етап</option>
                        <option value="Раунд">Раунд...</option>
                        {STAGE_ORDER.map(s => (
                          <option key={s} value={s}>{s}</option>
                        ))}
                    </select>
                    {stageType === 'Раунд' && (
                        <input 
                            type="number" 
                            min="1" 
                            style={{ flex: 1, width: '60px', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0' }}
                            value={roundNumber}
                            onFocus={(e) => e.target.select()}
                            onChange={e => {
                                const num = parseInt(e.target.value) || 1;
                                setRoundNumber(num);
                                setNewMatchForm({...newMatchForm, stage: `Раунд ${num}`});
                            }}
                        />
                    )}
                  </div>
                </div>

                {(sport === 'football' || sport === 'футбол') && (
                  <div className={styles.formGroup} style={{ marginBottom: '20px' }}>
                    <label style={{ display: 'flex', alignItems: 'center', gap: '8px', fontSize: '14px', color: '#64748b' }}>
                      <i className="fas fa-filter"></i> Розумна фільтрація
                    </label>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginTop: '8px' }}>
                      <label className={styles.switch}>
                        <input 
                          type="checkbox" 
                          checked={useStrictFiltering} 
                          onChange={(e) => setUseStrictFiltering(e.target.checked)}
                        />
                        <span className={styles.slider}></span>
                      </label>
                      <span style={{ fontSize: '12px', color: '#94a3b8' }}>
                        {useStrictFiltering ? 'Показувати лише переможців попереднього етапу' : 'Показувати всі команди'}
                      </span>
                    </div>
                  </div>
                )}

                <div className={styles.formGroup}>
                  <label>Команда 1 (Господарі)</label>
                  <select 
                    value={newMatchForm.homeTeamId} 
                    onChange={e => setNewMatchForm({...newMatchForm, homeTeamId: e.target.value})}
                  >
                    <option value="">-- Оберіть команду --</option>
                    {eligibleTeams.map((t: any) => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <div className={styles.formGroup}>
                  <label>Команда 2 (Гості)</label>
                  <select 
                    value={newMatchForm.awayTeamId} 
                    onChange={e => setNewMatchForm({...newMatchForm, awayTeamId: e.target.value})}
                  >
                    <option value="">-- Оберіть команду --</option>
                    {eligibleTeams.map((t: any) => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <button className={styles.saveBtn} onClick={handleCreateMatch} style={{ marginTop: '10px' }}>
                  Створити матч
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
      {/* Create Team Modal */}
      {isCreateTeamModalOpen && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsCreateTeamModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.glassModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Створити нову команду (групу)</h3>
              <button className={styles.closeButton} onClick={() => setIsCreateTeamModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
              <div className={styles.formGroup}>
                <label>Назва команди / Номер групи</label>
                <input 
                  type="text" 
                  value={newTeamForm.name} 
                  onChange={e => setNewTeamForm({...newTeamForm, name: e.target.value})}
                  placeholder="напр. 221"
                />
              </div>

              {isGroupsPlayoff && (
                <div className={styles.formGroup} style={{ marginTop: '16px' }}>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '8px', cursor: 'pointer' }}>
                    <input 
                      type="checkbox" 
                      checked={newTeamForm.isGeneralTableIncluded} 
                      onChange={e => setNewTeamForm({...newTeamForm, isGeneralTableIncluded: e.target.checked})}
                      style={{ width: '18px', height: '18px' }}
                    />
                    Включити в загальну таблицю
                  </label>
                  <p style={{ fontSize: '12px', color: '#94a3b8', marginLeft: '26px', marginTop: '4px' }}>
                    Команда з'явиться у фінальному рейтингу турніру
                  </p>
                </div>
              )}

              <div className={styles.modalFooter} style={{padding: '24px 0 0 0', background: 'transparent', borderTop: 'none'}}>
                <button className={styles.saveBtn} onClick={handleCreateTeam}>Створити команду</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Team Roster Management Modal */}
      {isTeamRosterModalOpen && editingTeamId && (
        <div className={styles.rosterModalOverlay} onClick={() => { setIsTeamRosterModalOpen(false); setEditingTeamId(null); setIsQuickAddMode(false); }}>
          <div className={`${styles.rosterModal} ${styles.glassModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Управління складом: {details.teams.find((t: any) => t.id === editingTeamId)?.name}</h3>
              <button className={styles.closeButton} onClick={() => { setIsTeamRosterModalOpen(false); setEditingTeamId(null); setIsQuickAddMode(false); }}>&times;</button>
            </div>
            
             {isAdmin && !isQuickAddMode && (
                <div className={styles.searchBox} style={{padding: '20px 40px 0 40px', display: 'flex', gap: '10px'}}>
                    <input 
                        type="text" 
                        placeholder="Пошук гравця..." 
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        style={{flex: 1, padding: '12px 18px', borderRadius: '12px', border: '1px solid rgba(0,0,0,0.1)'}}
                    />
                    <button 
                      className={styles.quickAddToggleBtn}
                      onClick={() => {
                        setIsQuickAddMode(true);
                        // Try to parse search query if it looks like Name Surname
                        const parts = searchQuery.trim().split(/\s+/);
                        if (parts.length >= 2) {
                          setQuickAddForm({ ...quickAddForm, lastName: parts[0], firstName: parts[1] });
                        } else if (parts.length === 1) {
                          setQuickAddForm({ ...quickAddForm, lastName: parts[0] });
                        }
                      }}
                      title="Додати нового гравця вручну"
                    >
                      <i className="fas fa-user-plus"></i>
                    </button>
                </div>
             )}
             
             {!isQuickAddMode ? (
               <>
                <div className={styles.modalBody} style={{maxHeight: '400px'}}>
                    <div className={styles.playersList}>
                        {isAdmin ? filteredPlayers.map(player => {
                            const team = details.teams?.find((t: any) => t.id === editingTeamId);
                            const isSelected = team?.players?.some((p: any) => p.id === player.id);
                            
                            return (
                                <div key={player.id} className={`${styles.playerPickItem} ${isSelected ? styles.selected : ''}`}>
                                    <div className={styles.pickInfo} onClick={() => handleTogglePlayerInTeam(editingTeamId, player.id!)}>
                                        <span className={styles.pName}>{player.lastName} {player.firstName}</span>
                                        <span className={styles.pGroup}>{player.group}</span>
                                    </div>
                                    <div className={styles.pickStatus}>
                                        {(isSelected && (sport === 'football' || sport === 'футбол')) && (
                                          <div className={styles.roleToggles}>
                                            <button 
                                              className={`${styles.roleToggleBtn} ${team?.captainId === player.id ? styles.isCaptain : ''}`}
                                              onClick={(e) => {
                                                e.stopPropagation();
                                                handleSetCaptain(editingTeamId, team?.captainId === player.id ? null : player.id!);
                                              }}
                                              title={team?.captainId === player.id ? "Прибрати капітана" : "Призначити капітаном"}
                                            >
                                              <i className="fas fa-copyright"></i>
                                            </button>
                                            <button 
                                              className={`${styles.roleToggleBtn} ${team?.goalkeeperId === player.id ? styles.isGoalkeeper : ''}`}
                                              onClick={(e) => {
                                                e.stopPropagation();
                                                handleSetGoalkeeper(editingTeamId, team?.goalkeeperId === player.id ? null : player.id!);
                                              }}
                                              title={team?.goalkeeperId === player.id ? "Прибрати воротаря" : "Призначити воротарем"}
                                            >
                                              <i className="fas fa-hands"></i>
                                            </button>
                                          </div>
                                        )}
                                        <div style={{display: 'inline-block', marginLeft: '10px'}} onClick={() => handleTogglePlayerInTeam(editingTeamId, player.id!)}>
                                          {isSelected ? <i className="fas fa-check-circle"></i> : <i className="far fa-circle"></i>}
                                        </div>
                                    </div>
                                </div>
                            );
                        }) : (
                          // Read-only view for regular users
                          details?.teams?.find((t: any) => t.id === editingTeamId)?.players?.map((player: any) => (
                            <div key={player.id} className={`${styles.playerPickItem} ${styles.selected}`}>
                                <div className={styles.pickInfo}>
                                    <span className={styles.pName}>{player.lastName} {player.firstName}</span>
                                    <span className={styles.pGroup}>{player.group}</span>
                                </div>
                                <div className={styles.pickStatus}>
                                   {((sport === 'football' || sport === 'футбол')) && (
                                      <div className={styles.roleToggles}>
                                        {details?.teams?.find((t: any) => t.id === editingTeamId)?.captainId === player.id && (
                                          <span className={styles.isCaptain} title="Капітан"><i className="fas fa-copyright"></i></span>
                                        )}
                                        {details?.teams?.find((t: any) => t.id === editingTeamId)?.goalkeeperId === player.id && (
                                          <span className={styles.isGoalkeeper} title="Воротар"><i className="fas fa-hands"></i></span>
                                        )}
                                      </div>
                                   )}
                                </div>
                            </div>
                          ))
                        )}
                        {isAdmin && filteredPlayers.length === 0 && (
                          <div className={styles.noPlayersHint} style={{textAlign: 'center', padding: '20px'}}>
                            Гравців не знайдено. 
                            <button 
                              className={styles.linkButton} 
                              onClick={() => {
                                setIsQuickAddMode(true);
                                const parts = searchQuery.trim().split(/\s+/);
                                if (parts.length >= 2) {
                                  setQuickAddForm({ ...quickAddForm, lastName: parts[0], firstName: parts[1] });
                                } else if (parts.length === 1) {
                                  setQuickAddForm({ ...quickAddForm, lastName: parts[0] });
                                }
                              }}
                            >
                              Додати {searchQuery} як нового гравця?
                            </button>
                          </div>
                        )}
                    </div>
                </div>
              </>
            ) : (
              <div className={styles.modalBody}>
                <div className={styles.quickAddForm}>
                  <div className={styles.formRow}>
                    <div className={styles.formGroup}>
                      <label>Прізвище</label>
                      <input 
                        type="text" 
                        value={quickAddForm.lastName} 
                        onChange={e => setQuickAddForm({...quickAddForm, lastName: e.target.value})}
                        placeholder="напр. Іванов"
                      />
                    </div>
                    <div className={styles.formGroup}>
                      <label>Ім'я</label>
                      <input 
                        type="text" 
                        value={quickAddForm.firstName} 
                        onChange={e => setQuickAddForm({...quickAddForm, firstName: e.target.value})}
                        placeholder="напр. Іван"
                      />
                    </div>
                  </div>
                  <div className={styles.formGroup}>
                    <label>По батькові (необов'язково)</label>
                    <input 
                      type="text" 
                      value={quickAddForm.patronymic} 
                      onChange={e => setQuickAddForm({...quickAddForm, patronymic: e.target.value})}
                      placeholder="напр. Миколайович"
                    />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Група / Статус</label>
                    <input 
                      type="text" 
                      value={quickAddForm.group} 
                      onChange={e => setQuickAddForm({...quickAddForm, group: e.target.value})}
                      placeholder="напр. 321 або Випускник"
                    />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Стать</label>
                    <select
                      value={quickAddForm.gender}
                      onChange={e => setQuickAddForm({...quickAddForm, gender: e.target.value as Gender})}
                      className={styles.modalSelect}
                      style={{width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #e2e8f0'}}
                    >
                      <option value="None">Оберіть стать...</option>
                      <option value="Male">Чоловік</option>
                      <option value="Female">Жінка</option>
                    </select>
                  </div>
                  <div className={styles.modalFooter} style={{padding: '20px 0 0 0', display: 'flex', gap: '12px', background: 'transparent', borderTop: 'none'}}>
                    <button className={styles.cancelBtn} onClick={() => setIsQuickAddMode(false)}>Назад до пошуку</button>
                    <button className={styles.saveBtn} onClick={handleQuickAddPlayer} style={{flex: 1}}>Створити та додати</button>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
      
      {/* Match Scorers Modal */}
      {isScorersModalOpen && selectedMatchForScorers && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsScorersModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.glassModal}`} style={{ width: '90%', maxWidth: '1200px' }} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Автори голів: {selectedMatchForScorers.homeTeam?.name} vs {selectedMatchForScorers.awayTeam?.name}</h3>
              <button className={styles.closeButton} onClick={() => setIsScorersModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
              {/* Score Input Section */}
              <div className={styles.scoreEditSection} style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '32px', marginBottom: '32px', padding: '24px', background: '#f8fafc', borderRadius: '16px', border: '1px solid #e2e8f0' }}>
                 <div style={{ textAlign: 'center' }}>
                    <div style={{ fontSize: '11px', fontWeight: 700, color: '#64748b', marginBottom: '8px', textTransform: 'uppercase' }}>{selectedMatchForScorers.homeTeam?.name}</div>
                    <input 
                      type="number" 
                      min="0"
                      style={{ width: '80px', height: '60px', fontSize: '32px', fontWeight: 800, textAlign: 'center', borderRadius: '12px', border: '2px solid #3b82f6', color: '#1e293b' }}
                      value={matchScores.home}
                      onFocus={(e) => e.target.select()}
                      onChange={e => setMatchScores({...matchScores, home: parseInt(e.target.value) || 0})}
                    />
                 </div>
                 <div style={{ fontSize: '24px', fontWeight: 800, color: '#cbd5e1', paddingTop: '20px' }}>:</div>
                 <div style={{ textAlign: 'center' }}>
                    <div style={{ fontSize: '11px', fontWeight: 700, color: '#64748b', marginBottom: '8px', textTransform: 'uppercase' }}>{selectedMatchForScorers.awayTeam?.name}</div>
                    <input 
                      type="number" 
                      min="0"
                      style={{ width: '80px', height: '60px', fontSize: '32px', fontWeight: 800, textAlign: 'center', borderRadius: '12px', border: '2px solid #3b82f6', color: '#1e293b' }}
                      value={matchScores.away}
                      onFocus={(e) => e.target.select()}
                      onChange={e => setMatchScores({...matchScores, away: parseInt(e.target.value) || 0})}
                    />
                 </div>
              </div>

              {matchScores.home === matchScores.away && (
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '32px', marginBottom: '32px', padding: '16px', background: '#fffbeb', borderRadius: '16px', border: '1px solid #fef3c7' }}>
                   <div style={{ textAlign: 'center' }}>
                      <div style={{ fontSize: '11px', fontWeight: 700, color: '#b45309', marginBottom: '8px', textTransform: 'uppercase' }}>Пенальті</div>
                      <input 
                        type="number" 
                        min="0"
                        style={{ width: '60px', height: '45px', fontSize: '24px', fontWeight: 800, textAlign: 'center', borderRadius: '10px', border: '2px solid #f59e0b', color: '#78350f' }}
                        value={matchPenaltyScores.home ?? ''}
                        onFocus={(e) => e.target.select()}
                        onChange={e => setMatchPenaltyScores({...matchPenaltyScores, home: e.target.value ? parseInt(e.target.value) : null})}
                      />
                   </div>
                   <div style={{ fontSize: '20px', fontWeight: 800, color: '#fcd34d', paddingTop: '20px' }}>:</div>
                   <div style={{ textAlign: 'center' }}>
                      <div style={{ fontSize: '11px', fontWeight: 700, color: '#b45309', marginBottom: '8px', textTransform: 'uppercase' }}>Пенальті</div>
                      <input 
                        type="number" 
                        min="0"
                        style={{ width: '60px', height: '45px', fontSize: '24px', fontWeight: 800, textAlign: 'center', borderRadius: '10px', border: '2px solid #f59e0b', color: '#78350f' }}
                        value={matchPenaltyScores.away ?? ''}
                        onFocus={(e) => e.target.select()}
                        onChange={e => setMatchPenaltyScores({...matchPenaltyScores, away: e.target.value ? parseInt(e.target.value) : null})}
                      />
                   </div>
                </div>
              )}

              <div className={styles.scorersGrid}>
                {/* Home Team Scorers */}
                <div className={styles.teamScorersSection}>
                  <h5>{selectedMatchForScorers.homeTeam?.name}</h5>
                  <div className={styles.matchRoleSelects}>
                    <div className={styles.goalkeeperSelect}>
                      <label>Воротар</label>
                      <select 
                        value={matchGoalkeepers.home} 
                        onChange={e => setMatchGoalkeepers({...matchGoalkeepers, home: e.target.value})}
                      >
                        <option value="">-- Оберіть воротаря --</option>
                        {(selectedMatchForScorers.homeRoster?.length > 0 
                            ? selectedMatchForScorers.homeRoster 
                            : details?.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.footballShirtNumber ? `№${p.footballShirtNumber} ` : ''}{p.lastName} {p.firstName}
                          </option>
                        ))}
                      </select>
                    </div>
                    <div className={styles.goalkeeperSelect}>
                      <label>Капітан</label>
                      <select 
                        value={matchCaptains.home} 
                        onChange={e => setMatchCaptains({...matchCaptains, home: e.target.value})}
                      >
                        <option value="">-- Оберіть капітана --</option>
                        {(selectedMatchForScorers.homeRoster?.length > 0 
                            ? selectedMatchForScorers.homeRoster 
                            : details?.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.footballShirtNumber ? `№${p.footballShirtNumber} ` : ''}{p.lastName} {p.firstName}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                  <div className={styles.scorersList}>
                    {(selectedMatchForScorers.homeRoster?.length > 0 
                        ? selectedMatchForScorers.homeRoster 
                        : details?.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players || []
                    ).map((player: any) => (
                      <div key={player.id} className={styles.scorerInputItem} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px 16px', background: '#fff', borderRadius: '10px', marginBottom: '8px', border: '1px solid #e2e8f0' }}>
                        <div style={{ flex: 1, minWidth: '150px', fontWeight: 600, color: '#1e293b' }}>
                          {player.footballShirtNumber ? `№${player.footballShirtNumber} ` : ''}
                          {player.lastName} {player.firstName[0]}.
                        </div>
                        <div style={{ display: 'flex', gap: '16px', alignItems: 'center' }}>
                          <div title="Голи" style={{ position: 'relative' }}>
                            <i className="fas fa-futbol" style={{ fontSize: '12px', position: 'absolute', left: '-14px', top: '50%', transform: 'translateY(-50%)', color: '#64748b' }}></i>
                            <input 
                              type="number" 
                              min="0"
                              value={tempScorers[player.id] || 0}
                              onFocus={(e) => e.target.select()}
                              onChange={(e) => setTempScorers({
                                ...tempScorers,
                                [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                              })}
                              style={{ width: '50px', padding: '6px', borderRadius: '6px', border: '1px solid #cbd5e1', textAlign: 'center', fontWeight: 'bold' }}
                            />
                          </div>
                          
                          <div title="Жовті картки" style={{ position: 'relative' }}>
                            <div style={{ width: '10px', height: '14px', background: '#fbbf24', borderRadius: '2px', position: 'absolute', left: '-14px', top: '50%', transform: 'translateY(-50%)' }}></div>
                            <input 
                              type="number" 
                              min="0"
                              max="2"
                              value={tempYellowCards[player.id] || 0}
                              onFocus={(e) => e.target.select()}
                              onChange={(e) => setTempYellowCards({
                                ...tempYellowCards,
                                [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                              })}
                              style={{ width: '50px', padding: '6px', borderRadius: '6px', border: '1px solid #cbd5e1', textAlign: 'center', fontWeight: 'bold' }}
                            />
                          </div>

                          <div title="Червоні картки" style={{ position: 'relative' }}>
                            <div style={{ width: '10px', height: '14px', background: '#ef4444', borderRadius: '2px', position: 'absolute', left: '-14px', top: '50%', transform: 'translateY(-50%)' }}></div>
                            <input 
                              type="number" 
                              min="0"
                              max="1"
                              value={tempRedCards[player.id] || 0}
                              onFocus={(e) => e.target.select()}
                              onChange={(e) => setTempRedCards({
                                ...tempRedCards,
                                [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                              })}
                              style={{ width: '50px', padding: '6px', borderRadius: '6px', border: '1px solid #cbd5e1', textAlign: 'center', fontWeight: 'bold' }}
                            />
                          </div>
                        </div>

                      </div>
                    ))}
                    {!(selectedMatchForScorers.homeRoster?.length > 0 || details?.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players?.length > 0) && (
                        <p className={styles.noPlayersHint}>У цій команді ще немає гравців</p>
                    )}
                  </div>
                </div>

                <div className={styles.scorersDivider}></div>

                {/* Away Team Scorers */}
                <div className={styles.teamScorersSection}>
                  <h5>{selectedMatchForScorers.awayTeam?.name}</h5>
                  <div className={styles.matchRoleSelects}>
                    <div className={styles.goalkeeperSelect}>
                      <label>Воротар</label>
                      <select 
                        value={matchGoalkeepers.away} 
                        onChange={e => setMatchGoalkeepers({...matchGoalkeepers, away: e.target.value})}
                      >
                        <option value="">-- Оберіть воротаря --</option>
                        {(selectedMatchForScorers.awayRoster?.length > 0 
                            ? selectedMatchForScorers.awayRoster 
                            : details?.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.footballShirtNumber ? `№${p.footballShirtNumber} ` : ''}{p.lastName} {p.firstName}
                          </option>
                        ))}
                      </select>
                    </div>
                    <div className={styles.goalkeeperSelect}>
                      <label>Капітан</label>
                      <select 
                        value={matchCaptains.away} 
                        onChange={e => setMatchCaptains({...matchCaptains, away: e.target.value})}
                      >
                        <option value="">-- Оберіть капітана --</option>
                        {(selectedMatchForScorers.awayRoster?.length > 0 
                            ? selectedMatchForScorers.awayRoster 
                            : details?.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.footballShirtNumber ? `№${p.footballShirtNumber} ` : ''}{p.lastName} {p.firstName}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                  <div className={styles.scorersList}>
                    {(selectedMatchForScorers.awayRoster?.length > 0 
                        ? selectedMatchForScorers.awayRoster 
                        : details?.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players || []
                    ).map((player: any) => (
                      <div key={player.id} className={styles.scorerInputItem} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px 16px', background: '#fff', borderRadius: '10px', marginBottom: '8px', border: '1px solid #e2e8f0' }}>
                        <div style={{ flex: 1, minWidth: '150px', fontWeight: 600, color: '#1e293b' }}>
                          {player.footballShirtNumber ? `№${player.footballShirtNumber} ` : ''}
                          {player.lastName} {player.firstName[0]}.
                        </div>
                        <div style={{ display: 'flex', gap: '16px', alignItems: 'center' }}>
                          <div title="Голи" style={{ position: 'relative' }}>
                            <i className="fas fa-futbol" style={{ fontSize: '12px', position: 'absolute', left: '-14px', top: '50%', transform: 'translateY(-50%)', color: '#64748b' }}></i>
                            <input 
                              type="number" 
                              min="0"
                              value={tempScorers[player.id] || 0}
                              onFocus={(e) => e.target.select()}
                              onChange={(e) => setTempScorers({
                                ...tempScorers,
                                [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                              })}
                              style={{ width: '50px', padding: '6px', borderRadius: '6px', border: '1px solid #cbd5e1', textAlign: 'center', fontWeight: 'bold' }}
                            />
                          </div>
                          
                          <div title="Жовті картки" style={{ position: 'relative' }}>
                            <div style={{ width: '10px', height: '14px', background: '#fbbf24', borderRadius: '2px', position: 'absolute', left: '-14px', top: '50%', transform: 'translateY(-50%)' }}></div>
                            <input 
                              type="number" 
                              min="0"
                              max="2"
                              value={tempYellowCards[player.id] || 0}
                              onFocus={(e) => e.target.select()}
                              onChange={(e) => setTempYellowCards({
                                ...tempYellowCards,
                                [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                              })}
                              style={{ width: '50px', padding: '6px', borderRadius: '6px', border: '1px solid #cbd5e1', textAlign: 'center', fontWeight: 'bold' }}
                            />
                          </div>

                          <div title="Червоні картки" style={{ position: 'relative' }}>
                            <div style={{ width: '10px', height: '14px', background: '#ef4444', borderRadius: '2px', position: 'absolute', left: '-14px', top: '50%', transform: 'translateY(-50%)' }}></div>
                            <input 
                              type="number" 
                              min="0"
                              max="1"
                              value={tempRedCards[player.id] || 0}
                              onFocus={(e) => e.target.select()}
                              onChange={(e) => setTempRedCards({
                                ...tempRedCards,
                                [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                              })}
                              style={{ width: '50px', padding: '6px', borderRadius: '6px', border: '1px solid #cbd5e1', textAlign: 'center', fontWeight: 'bold' }}
                            />
                          </div>
                        </div>

                      </div>
                    ))}
                    {!(selectedMatchForScorers.awayRoster?.length > 0 || details?.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players?.length > 0) && (
                        <p className={styles.noPlayersHint}>У цій команді ще немає гравців</p>
                    )}
                  </div>
                </div>
              </div>

              <div className={styles.modalFooter} style={{padding: '24px 0 0 0', background: 'transparent', borderTop: 'none'}}>
                <button className={styles.saveBtn} onClick={handleSaveScorers}>Зберегти зміни</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Match Info Modal (View Only) */}
      {isMatchInfoModalOpen && selectedMatchForInfo && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsMatchInfoModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.glassModal}`} style={{maxWidth: '1000px'}} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Інформація про матч</h3>
              <button className={styles.closeButton} onClick={() => setIsMatchInfoModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
              <div style={{ textAlign: 'center', marginBottom: '32px' }}>
                <div style={{ fontSize: '14px', fontWeight: 700, color: '#64748b', textTransform: 'uppercase', letterSpacing: '1px' }}>{selectedMatchForInfo.stage}</div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '40px', marginTop: '20px' }}>
                  <div style={{ textAlign: 'center', flex: 1 }}>
                    <div style={{ fontSize: '24px', fontWeight: 800, color: '#1e293b' }}>{selectedMatchForInfo.homeTeam?.name}</div>
                  </div>
                  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
                    <div style={{ fontSize: '48px', fontWeight: 900, color: '#3b82f6', display: 'flex', gap: '20px', justifyContent: 'center' }}>
                      <span>{selectedMatchForInfo.homeTeamScore ?? 0}</span>
                      <span style={{ color: '#cbd5e1' }}>:</span>
                      <span>{selectedMatchForInfo.awayTeamScore ?? 0}</span>
                    </div>
                    {selectedMatchForInfo.homePenaltyScore !== null && selectedMatchForInfo.awayPenaltyScore !== null && selectedMatchForInfo.homePenaltyScore !== undefined && selectedMatchForInfo.awayPenaltyScore !== undefined && (
                      <div style={{ fontSize: '18px', fontWeight: 700, color: '#f59e0b', marginTop: '4px', textAlign: 'center' }}>
                        (пен. {selectedMatchForInfo.homePenaltyScore} : {selectedMatchForInfo.awayPenaltyScore})
                      </div>
                    )}
                  </div>
                  <div style={{ textAlign: 'center', flex: 1 }}>
                    <div style={{ fontSize: '24px', fontWeight: 800, color: '#1e293b' }}>{selectedMatchForInfo.awayTeam?.name}</div>
                  </div>
                </div>
              </div>

              <div className={styles.scorersGrid}>
                {/* Home Info */}
                <div className={styles.teamScorersSection}>
                  <div style={{ marginBottom: '16px' }}>
                    <h5 style={{ color: '#3b82f6' }}>Автори голів</h5>
                    {selectedMatchForInfo.goals?.filter((g: any) => selectedMatchForInfo.homeRoster?.some((p: any) => p.id === g.playerId) || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players?.some((p: any) => p.id === g.playerId)).map((g: any) => (
                      <div key={g.id} style={{ fontSize: '14px', margin: '4px 0', fontWeight: 600 }}>
                        ⚽ {g.player?.lastName || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players?.find((p: any) => p.id === g.playerId)?.lastName} - {g.goalsCount}
                      </div>
                    ))}
                  </div>
                  {(selectedMatchForInfo.yellowCards?.length > 0 || selectedMatchForInfo.redCards?.length > 0) && (
                    <div style={{ marginBottom: '16px' }}>
                      <h5 style={{ color: '#eab308' }}>Дисциплінарні заходи</h5>
                      {selectedMatchForInfo.yellowCards?.filter((c: any) => selectedMatchForInfo.homeRoster?.some((p: any) => p.id === c.playerId) || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players?.some((p: any) => p.id === c.playerId)).map((c: any, idx: number) => (
                        <div key={`y-${idx}`} style={{ fontSize: '14px', margin: '4px 0', fontWeight: 600 }}>
                          🟨 {c.player?.lastName || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players?.find((p: any) => p.id === c.playerId)?.lastName}
                        </div>
                      ))}
                      {selectedMatchForInfo.redCards?.filter((c: any) => selectedMatchForInfo.homeRoster?.some((p: any) => p.id === c.playerId) || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players?.some((p: any) => p.id === c.playerId)).map((c: any, idx: number) => (
                        <div key={`r-${idx}`} style={{ fontSize: '14px', margin: '4px 0', fontWeight: 600 }}>
                          🟥 {c.player?.lastName || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players?.find((p: any) => p.id === c.playerId)?.lastName}
                        </div>
                      ))}
                    </div>
                  )}
                  <h5 style={{ marginTop: '20px' }}>Склад команди</h5>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                    {(selectedMatchForInfo.homeRoster?.length > 0 
                        ? selectedMatchForInfo.homeRoster 
                        : details?.teams?.find((t: any) => t.id === selectedMatchForInfo.homeTeamId)?.players || []
                    ).map((p: any) => (
                      <span key={p.id} style={{ background: '#f1f5f9', padding: '4px 10px', borderRadius: '8px', fontSize: '12px', fontWeight: 600 }}>
                        {p.lastName} {p.id === selectedMatchForInfo.homeCaptainId && <b style={{color: '#f59e0b'}}>(C)</b>} {p.id === selectedMatchForInfo.homeGoalkeeperId && <b style={{color: '#3b82f6'}}>(GK)</b>}
                      </span>
                    ))}
                  </div>
                </div>

                <div className={styles.scorersDivider}></div>

                {/* Away Info */}
                <div className={styles.teamScorersSection}>
                  <div style={{ marginBottom: '16px' }}>
                    <h5 style={{ color: '#3b82f6' }}>Автори голів</h5>
                    {selectedMatchForInfo.goals?.filter((g: any) => selectedMatchForInfo.awayRoster?.some((p: any) => p.id === g.playerId) || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players?.some((p: any) => p.id === g.playerId)).map((g: any) => (
                      <div key={g.id} style={{ fontSize: '14px', margin: '4px 0', fontWeight: 600 }}>
                        ⚽ {g.player?.lastName || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players?.find((p: any) => p.id === g.playerId)?.lastName} - {g.goalsCount}
                      </div>
                    ))}
                  </div>
                  {(selectedMatchForInfo.yellowCards?.length > 0 || selectedMatchForInfo.redCards?.length > 0) && (
                    <div style={{ marginBottom: '16px' }}>
                      <h5 style={{ color: '#eab308' }}>Дисциплінарні заходи</h5>
                      {selectedMatchForInfo.yellowCards?.filter((c: any) => selectedMatchForInfo.awayRoster?.some((p: any) => p.id === c.playerId) || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players?.some((p: any) => p.id === c.playerId)).map((c: any, idx: number) => (
                        <div key={`y-${idx}`} style={{ fontSize: '14px', margin: '4px 0', fontWeight: 600 }}>
                          🟨 {c.player?.lastName || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players?.find((p: any) => p.id === c.playerId)?.lastName}
                        </div>
                      ))}
                      {selectedMatchForInfo.redCards?.filter((c: any) => selectedMatchForInfo.awayRoster?.some((p: any) => p.id === c.playerId) || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players?.some((p: any) => p.id === c.playerId)).map((c: any, idx: number) => (
                        <div key={`r-${idx}`} style={{ fontSize: '14px', margin: '4px 0', fontWeight: 600 }}>
                          🟥 {c.player?.lastName || details?.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players?.find((p: any) => p.id === c.playerId)?.lastName}
                        </div>
                      ))}
                    </div>
                  )}
                  <h5 style={{ marginTop: '20px' }}>Склад команди</h5>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                    {(selectedMatchForInfo.awayRoster?.length > 0 
                        ? selectedMatchForInfo.awayRoster 
                        : details.teams?.find((t: any) => t.id === selectedMatchForInfo.awayTeamId)?.players || []
                    ).map((p: any) => (
                      <span key={p.id} style={{ background: '#f1f5f9', padding: '4px 10px', borderRadius: '8px', fontSize: '12px', fontWeight: 600 }}>
                        {p.lastName} {p.id === selectedMatchForInfo.awayCaptainId && <b style={{color: '#f59e0b'}}>(C)</b>} {p.id === selectedMatchForInfo.awayGoalkeeperId && <b style={{color: '#3b82f6'}}>(GK)</b>}
                      </span>
                    ))}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

    </div>
  );
};

export default TeamSportsTournamentView;

