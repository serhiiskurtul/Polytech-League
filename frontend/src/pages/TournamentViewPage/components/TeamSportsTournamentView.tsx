import React, { useState, useEffect } from 'react';
import { PlayerDto } from '@/shared/types/player.types';
import { ChessEventResponse, ChessEventParticipantDto, ChessGameDto, ChessEventRequest, GameResult } from '@/shared/types/chess.types';
import { getAllPlayers } from '@/shared/api';
import { 
  updateVolleyballMatch, 
  updateBeachVolleyballMatch, 
  updateFutsalMatch, 
  updateBasketball3x3Match,
  updateBadmintonMatch,
  updateChessGame,
  updateCheckersGame,
  updateTableTennisMatch,
  createVolleyballMatch,
  createBeachVolleyballMatch,
  createFutsalMatch,
  createBasketball3x3Match,
  createBadmintonMatch,
  createChessGame,
  createCheckersGame,
  createTableTennisMatch,
  createVolleyballTeam,
  createBeachVolleyballTeam,
  createFutsalTeam,
  createBasketball3x3Team,
  createBadmintonTeam,
  createChessTeam,
  createCheckersTeam,
  createTableTennisTeam,
  createPlayer,
  updateVolleyballTeam,
  updateBeachVolleyballTeam,
  updateFutsalTeam,
  updateBasketball3x3Team,
  updateBadmintonTeam,
  updateChessTeam,
  updateCheckersTeam,
  updateTableTennisTeam,
  deleteCheckersGame,
  deleteTableTennisMatch,
  deleteVolleyballTeam,
  deleteBeachVolleyballTeam,
  deleteFutsalTeam,
  deleteBasketball3x3Team,
  deleteBadmintonTeam,
  deleteChessTeam,
  deleteCheckersTeam,
  deleteTableTennisTeam
} from '@/shared/api';
import TournamentPodium from './TournamentPodium';
import FutsalBracket from './FutsalBracket';
import styles from '../TournamentViewPage.module.css';
import { Link } from 'react-router-dom';

type Sport = 'volleyball' | 'beach_volleyball' | 'beach-volleyball' | 'futsal' | 'basketball_3x3' | 'basketball3x3' | 'basketball-3x3' | 'badminton' | 'chess' | 'checkers' | 'tabletennis' | 'table_tennis' | 'table-tennis';

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
  const [activeTab, setActiveTab] = useState<'standings' | 'matches' | 'teams' | 'info' | 'bracket'>('matches');
  const [allPlayers, setAllPlayers] = useState<PlayerDto[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  
  // Create Match State
  const [isCreateMatchModalOpen, setIsCreateMatchModalOpen] = useState(false);
  const [newMatchForm, setNewMatchForm] = useState({
    homeTeamId: '',
    awayTeamId: '',
    stage: 'Груповий етап'
  });

  // Create Team State
  const [isCreateTeamModalOpen, setIsCreateTeamModalOpen] = useState(false);
  const [newTeamForm, setNewTeamForm] = useState({
    name: ''
  });

  // Team Roster Management State
  const [editingTeamId, setEditingTeamId] = useState<number | null>(null);
  const [isTeamRosterModalOpen, setIsTeamRosterModalOpen] = useState(false);

  // Match Scorers State
  const [isScorersModalOpen, setIsScorersModalOpen] = useState(false);
  const [selectedMatchForScorers, setSelectedMatchForScorers] = useState<any>(null);
  const [tempScorers, setTempScorers] = useState<{[playerId: number]: number}>({});
  const [matchGoalkeepers, setMatchGoalkeepers] = useState<{home: string, away: string}>({ home: '', away: '' });
  const [matchCaptains, setMatchCaptains] = useState<{home: string, away: string}>({ home: '', away: '' });

  // Quick Add Player State
  const [isQuickAddMode, setIsQuickAddMode] = useState(false);
  const [quickAddForm, setQuickAddForm] = useState({ firstName: '', lastName: '', group: '' });

  useEffect(() => {
    if (isAdmin) {
      loadAllPlayers();
    }
  }, [isAdmin]);

  const loadAllPlayers = async () => {
    try {
      const players = await getAllPlayers();
      setAllPlayers(players);
    } catch (err) {
      console.error('Failed to load players:', err);
    }
  };

  const getMatchUpdateApi = () => {
    switch (sport) {
      case 'volleyball': return updateVolleyballMatch;
      case 'beach_volleyball': return updateBeachVolleyballMatch;
      case 'futsal': return updateFutsalMatch;
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
          homeTeamScore: finalHomeScore, // For Futsal, Basketball, Table Tennis
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

  const handleOpenScorers = (match: any) => {
    setSelectedMatchForScorers(match);
    const initialScorers: {[playerId: number]: number} = {};
    match.goals?.forEach((g: any) => {
      initialScorers[g.playerId] = g.goalsCount;
    });
    setTempScorers(initialScorers);
    
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

      const payload = {
        id: selectedMatchForScorers.id,
        eventId: details.id,
        homeTeamId: selectedMatchForScorers.homeTeamId,
        awayTeamId: selectedMatchForScorers.awayTeamId,
        homeTeamScore: selectedMatchForScorers.homeTeamScore,
        awayTeamScore: selectedMatchForScorers.awayTeamScore,
        stage: selectedMatchForScorers.stage,
        // Update rosters to match team players if they were empty
        homeRosterIds: selectedMatchForScorers.homeRoster?.length > 0 
            ? selectedMatchForScorers.homeRoster.map((p: any) => p.id) 
            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players?.map((p: any) => p.id) || [],
        awayRosterIds: selectedMatchForScorers.awayRoster?.length > 0 
            ? selectedMatchForScorers.awayRoster.map((p: any) => p.id) 
            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players?.map((p: any) => p.id) || [],
        homeGoalkeeperId: matchGoalkeepers.home ? parseInt(matchGoalkeepers.home) : null,
        awayGoalkeeperId: matchGoalkeepers.away ? parseInt(matchGoalkeepers.away) : null,
        homeCaptainId: matchCaptains.home ? parseInt(matchCaptains.home) : null,
        awayCaptainId: matchCaptains.away ? parseInt(matchCaptains.away) : null,
        goals: goalsPayload
      };


      await updateFutsalMatch(selectedMatchForScorers.id, payload);
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
      switch (sport) {
        case 'volleyball': deleteApi = deleteVolleyballMatch; break;
        case 'beach_volleyball': deleteApi = deleteBeachVolleyballMatch; break;
        case 'futsal': deleteApi = deleteFutsalMatch; break;
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
    switch (sport) {
      case 'volleyball': return updateVolleyballTeam;
      case 'beach_volleyball': return updateBeachVolleyballTeam;
      case 'futsal': return updateFutsalTeam;
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

  const calculateStandings = () => {
    const teams = details.teams || [];
    const matches = details.matches || details.games || [];
    if (teams.length === 0) return [];

    const standings = teams.map((team: any) => {
      const stats = {
        id: team.id,
        name: team.name,
        gp: 0, // Games Played
        w: 0,  // Wins
        d: 0,  // Draws
        l: 0,  // Losses
        gf: 0, // Goals For
        ga: 0, // Goals Against
        manualPoints: team.points
      };

      matches.forEach((match: any) => {
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

      // Traditional points calculation if not overridden or added to override
      // Priority: Manual Points > Auto-calculated Points
      const autoPoints = stats.w * 3 + stats.d * 1;
      const finalPoints = (stats.manualPoints !== undefined && stats.manualPoints !== null) 
        ? stats.manualPoints 
        : autoPoints;
      
      return { ...stats, points: finalPoints };
    });

    const sortedStandings = standings.sort((a: any, b: any) => {
      // Priority 1: Points
      const aP = a.points;
      const bP = b.points;
      if (bP !== aP) return bP - aP;

      // Priority 2: Goal Difference
      const aDiff = a.gf - a.ga;
      const bDiff = b.gf - b.ga;
      if (bDiff !== aDiff) return bDiff - aDiff;

      // Priority 3: Goals Scored
      return b.gf - a.gf;
    });

    // Assign shared ranks (e.g. "1-2")
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

      const rankStr = tieCount > 1 
        ? `${i + 1}-${i + tieCount}` 
        : (i + 1).toString();

      for (let j = 0; j < tieCount; j++) {
        standingsWithRanks.push({ ...sortedStandings[i + j], displayRank: rankStr });
      }
      i += tieCount;
    }

    return standingsWithRanks;
  };

  const standingsData = calculateStandings();

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

      switch (sport) {
        case 'volleyball': createApi = createVolleyballMatch; break;
        case 'beach_volleyball': createApi = createBeachVolleyballMatch; break;
        case 'futsal': createApi = createFutsalMatch; break;
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
            round: 1, // Default to round 1
            board: 1, // Default to board 1
            result: 'NotPlayed'
          };
          break;
        case 'checkers': 
          createApi = createCheckersGame; 
          payload = {
            tournamentId: details.id,
            whiteTeamId: parseInt(newMatchForm.homeTeamId),
            blackTeamId: parseInt(newMatchForm.awayTeamId),
            round: 1,
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
      setNewMatchForm({ homeTeamId: '', awayTeamId: '', stage: 'Груповий етап' });
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
        eventId: details.id
      };

      let createApi;
      switch (sport) {
        case 'volleyball': createApi = createVolleyballTeam; break;
        case 'beach_volleyball': createApi = createBeachVolleyballTeam; break;
        case 'futsal': createApi = createFutsalTeam; break;
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
      setNewTeamForm({ name: '' });
      onRefresh();
    } catch (err) {
      console.error('Create Team Error:', err);
      alert(`Помилка при створенні команди: ${err instanceof Error ? err.message : 'Невідома помилка'}`);
    }
  };

  const handleTogglePlayerInTeam = async (teamId: number, playerId: number) => {
    try {
      const team = details.teams.find((t: any) => t.id === teamId);
      const currentParticipantIds = team.players?.map((p: any) => p.id) || [];
      
      let newPlayerIds: number[];
      if (currentParticipantIds.includes(playerId)) {
        newPlayerIds = currentParticipantIds.filter((id: number) => id !== playerId);
      } else {
        newPlayerIds = [...currentParticipantIds, playerId];
      }

      let updateApi;
      switch (sport) {
        case 'volleyball': updateApi = updateVolleyballTeam; break;
        case 'beach_volleyball': updateApi = updateBeachVolleyballTeam; break;
        case 'futsal': updateApi = updateFutsalTeam; break;
        case 'basketball_3x3': updateApi = updateBasketball3x3Team; break;
        case 'badminton': updateApi = updateBadmintonTeam; break;
        case 'chess': updateApi = updateChessTeam; break;
        case 'checkers': updateApi = updateCheckersTeam; break;
        case 'tabletennis':
        case 'table_tennis':
        case 'table-tennis': updateApi = updateTableTennisTeam; break;
        default: updateApi = updateVolleyballTeam;
      }


      const payload = {
        id: team.id,
        name: team.name,
        eventId: team.eventId,
        points: team.points,
        rank: team.rank,
        playerIds: newPlayerIds
      };

      await updateApi(teamId, payload);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні складу команди');
    }
  };

  const handleSetCaptain = async (teamId: number, playerId: number | null) => {
    try {
      const team = details.teams.find((t: any) => t.id === teamId);
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
      const team = details.teams.find((t: any) => t.id === teamId);
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

  const handleQuickAddPlayer = async () => {
    if (!quickAddForm.firstName || !quickAddForm.lastName || !quickAddForm.group) {
      alert('Будь ласка, заповніть усі поля');
      return;
    }

    try {
      const newPlayer = await createPlayer({
        firstName: quickAddForm.firstName,
        lastName: quickAddForm.lastName,
        group: quickAddForm.group,
        city: 'Чернівці'
      } as PlayerDto);

      // Add to team
      await handleTogglePlayerInTeam(editingTeamId!, newPlayer.id!);
      
      // Refresh players list
      await loadAllPlayers();
      
      // Reset form
      setQuickAddForm({ firstName: '', lastName: '', group: '' });
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
      switch (sport) {
        case 'volleyball': deleteApi = deleteVolleyballTeam; break;
        case 'beach_volleyball': deleteApi = deleteBeachVolleyballTeam; break;
        case 'futsal': deleteApi = deleteFutsalTeam; break;
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
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при видаленні команди');
    }
  };

  const getSportLabel = () => {
    switch (sport) {
      case 'volleyball':
      case 'волейбол': return 'Волейбол';
      case 'beach_volleyball':
      case 'beach-volleyball':
      case 'пляжний_волейбол': return 'Пляжний Волейбол';
      case 'futsal':
      case 'футзал': return 'Футзал';
      case 'basketball_3x3':
      case 'basketball3x3':
      case 'basketball-3x3':
      case 'баскетбол_3х3': return 'Баскетбол 3х3';
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

  const filteredPlayers = allPlayers.filter(p => 
    (p.firstName + ' ' + p.lastName).toLowerCase().includes(searchQuery.toLowerCase()) ||
    p.group?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className={styles.teamSportsView}>
      {/* Header Info */}
      <div className={styles.sportHeader}>
        <div className={styles.sportBadge}>
          <i className="fas fa-trophy"></i> {getSportLabel()}
        </div>
        <div className={styles.tabsNav}>
          <button className={activeTab === 'matches' ? styles.activeTab : ''} onClick={() => setActiveTab('matches')}>Матчі</button>
          <button className={activeTab === 'standings' ? styles.activeTab : ''} onClick={() => setActiveTab('standings')}>Таблиця</button>
          {(sport === 'futsal' || sport === 'футзал') && (
            <button className={activeTab === 'bracket' ? styles.activeTab : ''} onClick={() => setActiveTab('bracket')}>Сітка</button>
          )}
          <button className={activeTab === 'teams' ? styles.activeTab : ''} onClick={() => setActiveTab('teams')}>Команди</button>
          {isAdmin && activeTab === 'matches' && (
            <button className={styles.addMatchGlobalBtn} onClick={() => setIsCreateMatchModalOpen(true)}>
              <i className="fas fa-plus-circle"></i> Додати матч
            </button>
          )}
          {isAdmin && activeTab === 'teams' && (
            <button className={styles.addMatchGlobalBtn} onClick={() => setIsCreateTeamModalOpen(true)}>
              <i className="fas fa-users-cog"></i> Створити команду
            </button>
          )}
        </div>
      </div>

      {/* Podium */}
      {(details.firstPlaceTeam || details.secondPlaceTeam || details.thirdPlaceTeam) && (
        <div className={styles.podiumSection}>
           <TournamentPodium 
             firstPlace={details.firstPlaceTeam}
             secondPlace={details.secondPlaceTeam}
             thirdPlace={details.thirdPlaceTeam}
             isTeam={true}
           />
        </div>
      )}

      {/* Main Content Areas */}
      <div className={styles.tabContent}>
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
                <div key={match.id} className={styles.matchCard}>
                  <div className={styles.matchStage}>{match.stage || 'Груповий етап'}</div>
                  
                  <div className={styles.matchTeams}>
                    <div className={`${styles.teamRow} ${isHomeWinner ? styles.winnerRow : ''} ${isDraw ? styles.drawRow : ''}`}>
                      <div className={`${styles.teamLogo} ${isHomeWinner ? styles.winnerLogo : ''}`}>
                        {(match.whiteTeam?.name || match.homeTeam?.name || match.whitePlayer?.lastName)?.[0] || 'T'}
                      </div>
                      <div className={`${styles.teamName} ${isHomeWinner ? styles.winnerName : ''} ${isDraw ? styles.drawName : ''}`}>
                        {match.whiteTeam?.name || match.homeTeam?.name || match.whitePlayer?.name || match.whitePlayer?.lastName || 'Home'}
                        {isHomeWinner && <i className={`fas fa-crown ${styles.crownIcon}`}></i>}
                        {isDraw && <i className={`fas fa-handshake ${styles.drawIcon}`}></i>}
                      </div>
                      <div className={styles.scoreInput}>
                        <input 
                          type="number" 
                          min="0"
                          max={isChessLike ? 2 : undefined}
                          step={isChessLike ? "0.5" : "1"}
                          key={`${match.id}-home-${homeScore}`}
                          defaultValue={homeScore}
                          onBlur={(e) => handleUpdateScore(match.id, parseFloat(e.target.value) || 0, awayScore)}
                          disabled={!isAdmin}
                        />
                      </div>
                    </div>

                    <div className={styles.vsDivider}>VS</div>

                    <div className={`${styles.teamRow} ${isAwayWinner ? styles.winnerRow : ''} ${isDraw ? styles.drawRow : ''}`}>
                      <div className={`${styles.teamLogo} ${isAwayWinner ? styles.winnerLogo : ''}`}>
                        {(match.blackTeam?.name || match.awayTeam?.name || match.awayTeamName || match.blackPlayer?.lastName)?.[0] || 'T'}
                      </div>
                      <div className={`${styles.teamName} ${isAwayWinner ? styles.winnerName : ''} ${isDraw ? styles.drawName : ''}`}>
                        {match.blackTeam?.name || match.awayTeam?.name || match.awayTeamName || match.blackPlayer?.name || match.blackPlayer?.lastName || 'Away'}
                        {isAwayWinner && <i className={`fas fa-crown ${styles.crownIcon}`}></i>}
                        {isDraw && <i className={`fas fa-handshake ${styles.drawIcon}`}></i>}
                      </div>
                      <div className={styles.scoreInput}>
                        <input 
                          type="number" 
                          min="0"
                          step={isChessLike ? "0.5" : "1"}
                          key={`${match.id}-away-${awayScore}`}
                          defaultValue={awayScore}
                          onBlur={(e) => handleUpdateScore(match.id, homeScore, parseFloat(e.target.value) || 0)}
                          disabled={!isAdmin}
                        />
                      </div>
                    </div>
                  </div>

                  {isAdmin && (
                    <div className={styles.matchActions}>
                      {(sport === 'futsal' || sport === 'футзал') && (
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
                                {g.player?.futsalShirtNumber ? `№${g.player.futsalShirtNumber} ` : ''}{g.player?.lastName} ({g.goalsCount})
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
                                {g.player?.futsalShirtNumber ? `№${g.player.futsalShirtNumber} ` : ''}{g.player?.lastName} ({g.goalsCount})
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
          <div className={styles.standingsTable}>
            <table className={styles.leagueTable}>
              <thead>
                <tr>
                  <th title="Місце">#</th>
                  <th title="Назва команди">Команда (Група)</th>
                  <th title="Ігри">І</th>
                  <th title="Виграші">В</th>
                  <th title="Нічиї">Н</th>
                  <th title="Поразки">П</th>
                  <th title="Забиті - Пропущені">М'ячі (З-П)</th>
                  <th title="Очки (3 за перемогу, 1 за нічию)">Очки</th>
                </tr>
              </thead>
              <tbody>
                {standingsData.map((team: any, index: number) => (
                  <tr key={team.id}>
                    <td>{team.displayRank}</td>
                    <td className={styles.tableName}>{team.name}</td>
                    <td>{team.gp}</td>
                    <td>{team.w}</td>
                    <td>{team.d}</td>
                    <td>{team.l}</td>
                    <td>{team.gf} - {team.ga}</td>
                    <td className={styles.tablePoints}>
                      {isAdmin ? (
                        <input 
                          type="number" 
                          min="0"
                          className={styles.pointsInlineInput}
                          defaultValue={team.points}
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

        {activeTab === 'bracket' && (sport === 'futsal' || sport === 'футзал') && (
          <FutsalBracket 
            matches={details.matches || details.games || []}
            isAdmin={isAdmin}
            onDeleteMatch={handleDeleteMatch}
            onOpenScorers={handleOpenScorers}
          />
        )}

         {activeTab === 'teams' && (
          <div className={styles.teamsGrid}>
            {details.teams?.map((team: any) => (
              <div key={team.id} className={styles.teamCard}>
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
                      <button className={styles.deleteTeamBtn} onClick={() => handleDeleteTeam(team.id, team.name)} title="Видалити команду">
                        <i className="fas fa-trash"></i>
                      </button>
                    </div>
                  )}
                </div>
                <div className={styles.teamPlayersPreview}>
                  {team.players?.slice(0, 5).map((p: any) => (
                    <div key={p.id} className={`${styles.pMiniTag} ${team.captainId === p.id ? styles.captainTag : ''} ${team.goalkeeperId === p.id ? styles.goalkeeperTag : ''}`}>
                      {team.captainId === p.id && <i className="fas fa-copyright" style={{marginRight: '4px', fontSize: '10px'}} title="Капітан"></i>}
                      {team.goalkeeperId === p.id && <i className="fas fa-hands" style={{marginRight: '4px', fontSize: '10px'}} title="Воротар"></i>}
                      {p.lastName} {p.firstName[0]}.
                    </div>
                  ))}
                  {team.players?.length > 5 && <span className={styles.morePlayers}>+{team.players.length - 5}</span>}
                  {(!team.players || team.players.length === 0) && <p className={styles.noPlayersHint}>Гравців ще не додано</p>}
                </div>
              </div>
            ))}
            {(!details.teams || details.teams.length === 0) && <p className={styles.noData}>У цих змаганнях ще немає команд</p>}
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
                  <label>Команда 1</label>
                  <select 
                    value={newMatchForm.homeTeamId} 
                    onChange={e => setNewMatchForm({...newMatchForm, homeTeamId: e.target.value})}
                  >
                    <option value="">-- Оберіть команду --</option>
                    {details.teams?.map((t: any) => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <div className={styles.formGroup}>
                  <label>Команда 2</label>
                  <select 
                    value={newMatchForm.awayTeamId} 
                    onChange={e => setNewMatchForm({...newMatchForm, awayTeamId: e.target.value})}
                  >
                    <option value="">-- Оберіть команду --</option>
                    {details.teams?.map((t: any) => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <div className={styles.formGroup}>
                  <label>Етап змагання</label>
                  <input 
                    type="text" 
                    value={newMatchForm.stage} 
                    onChange={e => setNewMatchForm({...newMatchForm, stage: e.target.value})}
                    placeholder="напр. Груповий етап, Фінал..."
                  />
                </div>

                <button className={styles.saveBtn} onClick={handleCreateMatch}>
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
            
            {!isQuickAddMode ? (
              <>
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
                <div className={styles.modalBody} style={{maxHeight: '400px'}}>
                    <div className={styles.playersList}>
                        {filteredPlayers.map(player => {
                            const team = details.teams.find((t: any) => t.id === editingTeamId);
                            const isSelected = team.players?.some((p: any) => p.id === player.id);
                            
                            return (
                                <div key={player.id} className={`${styles.playerPickItem} ${isSelected ? styles.selected : ''}`}>
                                    <div className={styles.pickInfo} onClick={() => handleTogglePlayerInTeam(editingTeamId, player.id!)}>
                                        <span className={styles.pName}>{player.lastName} {player.firstName}</span>
                                        <span className={styles.pGroup}>{player.group}</span>
                                    </div>
                                    <div className={styles.pickStatus}>
                                        {(isSelected && (sport === 'futsal' || sport === 'футзал')) && (
                                          <div className={styles.roleToggles}>
                                            <button 
                                              className={`${styles.roleToggleBtn} ${team.captainId === player.id ? styles.isCaptain : ''}`}
                                              onClick={(e) => {
                                                e.stopPropagation();
                                                handleSetCaptain(editingTeamId, team.captainId === player.id ? null : player.id!);
                                              }}
                                              title={team.captainId === player.id ? "Прибрати капітана" : "Призначити капітаном"}
                                            >
                                              <i className="fas fa-copyright"></i>
                                            </button>
                                            <button 
                                              className={`${styles.roleToggleBtn} ${team.goalkeeperId === player.id ? styles.isGoalkeeper : ''}`}
                                              onClick={(e) => {
                                                e.stopPropagation();
                                                handleSetGoalkeeper(editingTeamId, team.goalkeeperId === player.id ? null : player.id!);
                                              }}
                                              title={team.goalkeeperId === player.id ? "Прибрати воротаря" : "Призначити воротарем"}
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
                        })}
                        {filteredPlayers.length === 0 && (
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
                    <label>Група / Статус</label>
                    <input 
                      type="text" 
                      value={quickAddForm.group} 
                      onChange={e => setQuickAddForm({...quickAddForm, group: e.target.value})}
                      placeholder="напр. 321 або Випускник"
                    />
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
          <div className={`${styles.rosterModal} ${styles.glassModal}`} style={{maxWidth: '600px'}} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Автори голів: {selectedMatchForScorers.homeTeam?.name} vs {selectedMatchForScorers.awayTeam?.name}</h3>
              <button className={styles.closeButton} onClick={() => setIsScorersModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
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
                            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.futsalShirtNumber ? `№${p.futsalShirtNumber} ` : ''}{p.lastName} {p.firstName}
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
                            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.futsalShirtNumber ? `№${p.futsalShirtNumber} ` : ''}{p.lastName} {p.firstName}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                  <div className={styles.scorersList}>
                    {(selectedMatchForScorers.homeRoster?.length > 0 
                        ? selectedMatchForScorers.homeRoster 
                        : details.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players || []
                    ).map((player: any) => (
                      <div key={player.id} className={styles.scorerInputItem}>
                        <span className={styles.playerNameMini}>
                          {player.futsalShirtNumber ? `№${player.futsalShirtNumber} ` : ''}
                          {player.lastName} {player.firstName[0]}.
                        </span>
                        <input 
                          type="number" 
                          min="0"
                          value={tempScorers[player.id] || 0}
                          onChange={(e) => setTempScorers({
                            ...tempScorers,
                            [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                          })}
                        />
                      </div>
                    ))}
                    {!(selectedMatchForScorers.homeRoster?.length > 0 || details.teams?.find((t: any) => t.id === selectedMatchForScorers.homeTeamId)?.players?.length > 0) && (
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
                            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.futsalShirtNumber ? `№${p.futsalShirtNumber} ` : ''}{p.lastName} {p.firstName}
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
                            : details.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players || []
                        ).map((p: any) => (
                          <option key={p.id} value={p.id}>
                            {p.futsalShirtNumber ? `№${p.futsalShirtNumber} ` : ''}{p.lastName} {p.firstName}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                  <div className={styles.scorersList}>
                    {(selectedMatchForScorers.awayRoster?.length > 0 
                        ? selectedMatchForScorers.awayRoster 
                        : details.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players || []
                    ).map((player: any) => (
                      <div key={player.id} className={styles.scorerInputItem}>
                        <span className={styles.playerNameMini}>
                          {player.futsalShirtNumber ? `№${player.futsalShirtNumber} ` : ''}
                          {player.lastName} {player.firstName[0]}.
                        </span>
                        <input 
                          type="number" 
                          min="0"
                          value={tempScorers[player.id] || 0}
                          onChange={(e) => setTempScorers({
                            ...tempScorers,
                            [player.id]: Math.max(0, parseInt(e.target.value) || 0)
                          })}
                        />
                      </div>
                    ))}
                    {!(selectedMatchForScorers.awayRoster?.length > 0 || details.teams?.find((t: any) => t.id === selectedMatchForScorers.awayTeamId)?.players?.length > 0) && (
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

    </div>
  );
};

export default TeamSportsTournamentView;
