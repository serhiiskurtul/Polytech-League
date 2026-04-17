import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '@/app/providers/AuthContext';
import {
  getTournamentById,
  getChessEventById,
  addParticipantToEvent,
  removeParticipantFromEvent,
  updateParticipantInEvent,
  createChessGame,
  updateChessGame,
  updateChessEvent,
  uploadTournamentPhoto,
  deleteTournamentPhoto,
  searchUsers,
  getAllPlayers,
  createPlayer,
} from '@/shared/api';
import { PlayerDto } from '@/shared/types/player.types';
import {
  getCheckersEventById,
  updateCheckersEvent,
  addParticipantToCheckersEvent,
  removeParticipantFromCheckersEvent,
  updateParticipantInCheckersEvent,
  createCheckersGame,
  uploadCheckersPhoto,
  deleteCheckersPhoto
} from '@/shared/api/checkers.api';
import {
  getArmwrestlingEventById,
  updateArmwrestlingEvent,
  addParticipantToArmwrestlingEvent,
  removeParticipantFromArmwrestlingEvent,
  updateArmwrestlingParticipant,
} from '@/shared/api/armwrestling.api';
import { getVolleyballEventById, updateVolleyballEvent } from '@/shared/api/volleyball.api';
import { getBeachVolleyballEventById, updateBeachVolleyballEvent } from '@/shared/api/beachVolleyball.api';
import { getFutsalEventById, updateFutsalEvent } from '@/shared/api/futsal.api';
import { getBasketball3x3EventById, updateBasketball3x3Event } from '@/shared/api/basketball3x3.api';
import {
  getBadmintonEventById,
  updateBadmintonEvent,
  addParticipantToBadmintonEvent,
  removeParticipantFromBadmintonEvent,
  updateBadmintonParticipant,
} from '@/shared/api/badminton.api';
import {
  getTableTennisEventById,
  addParticipantToTableTennisEvent,
  removeParticipantFromTableTennisEvent,
  updateTableTennisParticipant,
} from '@/shared/api/tabletennis.api';
import ArmwrestlingTournamentView from './components/ArmwrestlingTournamentView';
import TeamSportsTournamentView from './components/TeamSportsTournamentView';
import TournamentPodium from './components/TournamentPodium';
import { Tournament } from '@/shared/types/tournament.types';
import {
  ChessEventResponse,
  ChessGameDto,
  GameResult,
  ChessGameResponse,
  ChessEventParticipantResponse,
  ChessEventParticipantDto,
  ChessEventRequest,
  CompetitionLevel,
  ChessSystem
} from '@/shared/types/chess.types';
import {
  CheckersEventResponse,
  CheckersEventRequest,
  CheckersGameDto,
  CheckersEventParticipantDto
} from '@/shared/types/checkers.types';
import { BASE_URL } from '@/shared/api/axiosInstance';
import styles from './TournamentViewPage.module.css';

import {
  ArmwrestlingEventResponse
} from '@/shared/types/armwrestling.types';

const TournamentViewPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [tournament, setTournament] = useState<Tournament | null>(null);
  const [details, setDetails] = useState<any | null>(null);
  const [allPlayers, setAllPlayers] = useState<PlayerDto[]>([]);
  const [activeTab, setActiveTab] = useState<'standings' | 'games' | 'participants' | 'schedule'>('standings');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedPhotoIndex, setSelectedPhotoIndex] = useState<number | null>(null);

  // Admin access check (0 = User, 1 = Admin)
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';

  // Search Results type
  type SearchResult = {
    type: 'player' | 'user';
    id: number;
    userId?: number | null;
    firstName: string;
    lastName: string;
    email?: string;
    group?: string;
    rating?: number;
  };

  // Admin Form States
  const [newPlayer, setNewPlayer] = useState({
    firstName: '',
    lastName: '',
    group: '',
    city: 'Київ',
    ratingELO: 1500,
    fideId: '',
    title: '',
    userId: null as number | null,
    existingPlayerId: null as number | null,
    weight: 0
  });
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [newGame, setNewGame] = useState({ whitePlayerId: '', blackPlayerId: '', result: 'WhiteWin', round: 1, board: 1 });

  const [editingGameId, setEditingGameId] = useState<number | null>(null);
  const [editingGameData, setEditingGameData] = useState<{ round: number; board: number; result: string; whitePlayerId: string; blackPlayerId: string }>({ round: 1, board: 1, result: 'WhiteWin', whitePlayerId: '', blackPlayerId: '' });

  const [editingParticipantId, setEditingParticipantId] = useState<number | null>(null);
  const [editingParticipantData, setEditingParticipantData] = useState<{ points: number; rank: number; weight: number }>({ points: 0, rank: 0, weight: 0 });
  const [gameFilters, setGameFilters] = useState({ round: '', board: '', participant: '', result: '' });

  const [isEditingTournament, setIsEditingTournament] = useState(false);
  const [showWinnersAdmin, setShowWinnersAdmin] = useState(false);
  const [winners, setWinners] = useState({
    firstPlaceId: 0,
    secondPlaceId: 0,
    thirdPlaceId: 0
  });

  const handleSaveWinners = async () => {
    if (!details || !id) return;
    try {
      const sportRaw = tournament?.sport || '';
      const sportKey = sportRaw.toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
      const isTeamSport = ['futsal', 'volleyball', 'beach_volleyball', 'basketball_3x3', 'badminton'].includes(sportKey);

      // Build clean base payload with common fields to avoid "Extra fields" 400 error
      const payload: any = {
        name: editTournamentData.name,
        date: editTournamentData.date,
        location: editTournamentData.location,
        refereeIds: editTournamentData.refereeIds,
        level: editTournamentData.level,
        description: editTournamentData.description,
        rules: editTournamentData.rules,
        prizes: editTournamentData.prizes,
        headRefereeName: editTournamentData.headRefereeName,
        referees: editTournamentData.referees,
        schedule: editTournamentData.schedule,
      };

      if (isTeamSport) {
        payload.firstPlaceTeamId = winners.firstPlaceId || null;
        payload.secondPlaceTeamId = winners.secondPlaceId || null;
        payload.thirdPlaceTeamId = winners.thirdPlaceId || null;

        if (['futsal', 'badminton', 'basketball_3x3', 'basketball3x3'].includes(sportKey)) {
          payload.tournamentType = (details as any).tournamentType || 'RoundRobin';
        }
        if (['volleyball', 'beach_volleyball'].includes(sportKey)) {
          payload.setsToWin = (details as any).setsToWin || 2;
        }
      } else {
        payload.firstPlaceId = winners.firstPlaceId || null;
        payload.secondPlaceId = winners.secondPlaceId || null;
        payload.thirdPlaceId = winners.thirdPlaceId || null;

        if (sportKey === 'chess') {
          payload.system = editTournamentData.system;
          payload.timeControl = editTournamentData.timeControl;
          payload.participantsCount = editTournamentData.participantsCount;
        } else if (sportKey === 'checkers') {
          payload.variant = (details as any).variant;
        } else if (sportKey === 'armwrestling') {
          payload.hand = (details as any).hand;
          payload.weightClass = (details as any).weightClass;
          payload.category = (details as any).category;
          payload.timeControl = editTournamentData.timeControl;
          payload.participantsCount = editTournamentData.participantsCount;
        }
      }

      switch (sportKey) {
        case 'checkers': await updateCheckersEvent(id, payload as any); break;
        case 'chess': await updateChessEvent(id, payload as any); break;
        case 'armwrestling': await updateArmwrestlingEvent(id, payload as any); break;
        case 'futsal': await updateFutsalEvent(id, payload as any); break;
        case 'volleyball': await updateVolleyballEvent(id, payload as any); break;
        case 'beach_volleyball': await updateBeachVolleyballEvent(id, payload as any); break;
        case 'basketball_3x3':
        case 'basketball3x3': await updateBasketball3x3Event(id, payload as any); break;
        case 'badminton': await updateBadmintonEvent(id, payload as any); break;
        default: await updateChessEvent(id, payload as any);
      }

      alert("Призерів успішно встановлено!");
      setShowWinnersAdmin(false);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при збереженні призерів");
    }
  };

  const [editTournamentData, setEditTournamentData] = useState<ChessEventRequest>({
    name: '',
    date: '',
    location: '',
    refereeIds: [],
    level: 'CollegeGroup',
    description: '',
    timeControl: '',
    participantsCount: 0,
    system: 'Swiss',
    rules: '',
    prizes: '',
    headRefereeName: '',
    referees: '',
    schedule: '',
    isTeam: false
  });

  const [isUploading, setIsUploading] = useState(false);

  const handleEditTournament = () => {
    if (details) {
      // Use the actual backend date which is likely ISO string
      let dateValue = '';
      if (details.date) {
        try {
          dateValue = new Date(details.date).toISOString().split('T')[0];
        } catch (e) {
          dateValue = details.date.split(' ')[0];
        }
      }

      setEditTournamentData({
        name: tournament?.title || details.name,
        date: dateValue,
        location: tournament?.location || details.location,
        refereeIds: details.refereeIds || [],
        level: details.level || 'CollegeGroup',
        description: tournament?.description || details.description,
        timeControl: details.timeControl || '',
        participantsCount: details.participantsCount || 0,
        system: (details as any).system || 'Swiss',
        rules: details.rules || '',
        prizes: details.prizes || '',
        headRefereeName: details.headRefereeName || '',
        referees: (details as any).referees || '',
        schedule: details.schedule || '',
        isTeam: !!(details as any).isTeam
      });
      setIsEditingTournament(true);
    }
  };

  const handlePhotoUpload = async (e: React.ChangeEvent<HTMLInputElement> | React.DragEvent) => {
    let file: File | undefined;

    if ('files' in e.target && e.target.files) {
      file = e.target.files[0];
    } else if ('dataTransfer' in e && e.dataTransfer.files) {
      e.preventDefault();
      file = e.dataTransfer.files[0];
    }

    if (!file) return;

    try {
      setIsUploading(true);
      if (tournament?.sport === 'checkers') {
        await uploadCheckersPhoto(id!, file);
      } else {
        await uploadTournamentPhoto(id!, file);
      }
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при завантаженні фото");
    } finally {
      setIsUploading(false);
    }
  };

  const handleDeletePhoto = async (photoId: number) => {
    if (!window.confirm("Ви дійсно хочете видалити це фото?")) return;
    try {
      if (tournament?.sport === 'checkers') {
        await deleteCheckersPhoto(id!, photoId);
      } else {
        await deleteTournamentPhoto(id!, photoId);
      }
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при видаленні фото");
    }
  };

  const handleSaveTournament = async () => {
    try {
      if (!editTournamentData.name || !editTournamentData.date) {
        alert("Назва та дата обов'язкові!");
        return;
      }
      const sport = tournament?.sport;
      if (!sport) {
        alert("Помилка: Вид спорту не визначений");
        return;
      }

      const sportKey = sport.toLowerCase().replace(/-/g, '_').replace(/\s/g, '');

      // Build clean base payload
      const payload: any = {
        name: editTournamentData.name,
        date: editTournamentData.date,
        location: editTournamentData.location,
        refereeIds: editTournamentData.refereeIds,
        level: editTournamentData.level,
        description: editTournamentData.description,
        rules: editTournamentData.rules,
        prizes: editTournamentData.prizes,
        headRefereeName: editTournamentData.headRefereeName,
        referees: editTournamentData.referees,
        schedule: editTournamentData.schedule,
        isTeam: editTournamentData.isTeam,
      };

      if (sportKey === 'checkers') {
        await updateCheckersEvent(id!, { ...payload, variant: (details as any).variant || 'Russian' } as any);
      } else if (sportKey === 'armwrestling') {
        await updateArmwrestlingEvent(id!, {
          ...payload,
          hand: (details as any).hand,
          weightClass: (details as any).weightClass,
          category: (details as any).category,
          timeControl: editTournamentData.timeControl,
          participantsCount: editTournamentData.participantsCount
        });
      } else if (sportKey === 'volleyball' || sportKey === 'beach_volleyball') {
        const updateFn = sportKey === 'volleyball' ? updateVolleyballEvent : updateBeachVolleyballEvent;
        await updateFn(id!, { ...payload, setsToWin: (details as any).setsToWin || 2 });
      } else if (sportKey === 'futsal' || sportKey === 'badminton') {
        const updateFn = sportKey === 'futsal' ? updateFutsalEvent : updateBadmintonEvent;
        await updateFn(id!, { ...payload, tournamentType: (details as any).tournamentType || 'RoundRobin' });
      } else if (sportKey === 'basketball_3x3' || sportKey === 'basketball3x3') {
        await updateBasketball3x3Event(id!, { ...payload, tournamentType: (details as any).tournamentType || 'RoundRobin' });
      } else {
        // Chess
        await updateChessEvent(id!, {
          ...payload,
          system: editTournamentData.system,
          timeControl: editTournamentData.timeControl,
          participantsCount: editTournamentData.participantsCount
        } as any);
      }
      alert("Дані турніру успішно оновлено!");
      setIsEditingTournament(false);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні турніру");
    }
  };

  const handleEditClick = (game: ChessGameResponse) => {
    setEditingGameId(game.id);
    setEditingGameData({
      round: game.round || 1,
      board: game.board || 1,
      result: game.result,
      whitePlayerId: game.whitePlayerId ? game.whitePlayerId.toString() : '',
      blackPlayerId: game.blackPlayerId ? game.blackPlayerId.toString() : ''
    });
  };

  const handleEditParticipantClick = (p: any) => {
    setEditingParticipantId(p.playerId);
    setEditingParticipantData({ 
      points: p.points || 0, 
      rank: p.rank || 0,
      weight: p.weight || 0 
    });
  };

  const handleSaveEdit = async (gameId: number) => {
    if (!editingGameData.whitePlayerId || !editingGameData.blackPlayerId) {
      alert("Оберіть обох супротивників!");
      return;
    }
    if (editingGameData.whitePlayerId === editingGameData.blackPlayerId) {
      alert("Гравець не може грати сам проти себе!");
      return;
    }

    try {
      if (tournament?.sport === 'checkers') {
        const payload: CheckersGameDto = {
          tournamentId: Number(id),
          whitePlayerId: editingGameData.whitePlayerId ? Number(editingGameData.whitePlayerId) : 0,
          blackPlayerId: editingGameData.blackPlayerId ? Number(editingGameData.blackPlayerId) : 0,
          result: editingGameData.result as GameResult,
          round: editingGameData.round
        };
        // Reuse games controller or similar. We need to be careful with ID mapping.
        // For simplicity let's assume we handle update.
        await updateChessGame(gameId, payload as any); // If common logic can handle it
      } else {
        const payload: ChessGameDto = {
          tournamentId: Number(id),
          whitePlayerId: editingGameData.whitePlayerId ? Number(editingGameData.whitePlayerId) : null,
          blackPlayerId: editingGameData.blackPlayerId ? Number(editingGameData.blackPlayerId) : null,
          result: editingGameData.result as GameResult,
          round: editingGameData.round,
          board: 1 // default UI mock
        };
        await updateChessGame(gameId, payload);
      }
      alert("Матч успішно оновлено!");
      setEditingGameId(null);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні матчу");
    }
  };

  const handleCancelEdit = () => {
    setEditingGameId(null);
  };

  const handleCancelParticipantEdit = () => {
    setEditingParticipantId(null);
  };

  const handleSaveParticipantEdit = async (playerId: number) => {
    try {
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      
      if (sportKey === 'checkers') {
        const payload: CheckersEventParticipantDto = {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
        };
        await updateParticipantInCheckersEvent(id!, playerId, payload);
      } else if (sportKey === 'armwrestling') {
        await updateArmwrestlingParticipant(id!, playerId, {
          playerId,
          weight: (editingParticipantData as any).weight || 0,
          rank: editingParticipantData.rank,
        });
      } else if (sportKey === 'badminton') {
        await updateBadmintonParticipant(id!, playerId, {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
        });
      } else if (sportKey === 'tabletennis' || sportKey === 'table_tennis') {
        await updateTableTennisParticipant(id!, playerId, {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
        });
      } else {
        const payload: ChessEventParticipantDto = {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
        };
        await updateParticipantInEvent(id!, playerId, payload);
      }
      
      alert("Дані учасника оновлено!");
      setEditingParticipantId(null);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні учасника");
    }
  };

  const handleRemoveParticipant = async (playerId: number, playerName: string) => {
    if (window.confirm(`Ви впевнені, що хочете видалити учасника "${playerName}" з турніру?`)) {
      try {
        const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
        
        switch (sportKey) {
          case 'checkers':
            await removeParticipantFromCheckersEvent(id!, playerId);
            break;
          case 'armwrestling':
            await removeParticipantFromArmwrestlingEvent(id!, playerId);
            break;
          case 'badminton':
            await removeParticipantFromBadmintonEvent(id!, playerId);
            break;
          case 'tabletennis':
          case 'table_tennis':
            await removeParticipantFromTableTennisEvent(id!, playerId);
            break;
          default:
            await removeParticipantFromEvent(id!, playerId);
        }
        
        alert("Учасника видалено з турніру!");
        fetchTournament(true);
      } catch (err) {
        console.error(err);
        alert("Помилка при видаленні учасника");
      }
    }
  };

  const fetchTournament = async (quiet = false) => {
    if (!id) return;
    if (!quiet) setIsLoading(true);
    try {
      const data = await getTournamentById(id);
      setTournament(data);

      const sportKey = data.sport.toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
      setTournament({ ...data, sport: sportKey }); // Sync normalized sport back to state

      if (['chess', 'checkers', 'armwrestling', 'volleyball', 'beach_volleyball', 'futsal', 'basketball_3x3', 'basketball3x3', 'badminton'].includes(sportKey)) {
        try {
          const playersGetter = getAllPlayers;
          let detailsPromise;

          switch (sportKey) {
            case 'checkers': detailsPromise = getCheckersEventById(id); break;
            case 'armwrestling': detailsPromise = getArmwrestlingEventById(id); break;
            case 'volleyball': detailsPromise = getVolleyballEventById(id); break;
            case 'beach_volleyball': detailsPromise = getBeachVolleyballEventById(id); break;
            case 'futsal': detailsPromise = getFutsalEventById(id); break;
            case 'basketball_3x3':
            case 'basketball3x3': detailsPromise = getBasketball3x3EventById(id); break;
            case 'badminton': detailsPromise = getBadmintonEventById(id); break;
            default: detailsPromise = getChessEventById(id);
          }

          const [detailsData, playersData] = await Promise.all([
            detailsPromise,
            playersGetter()
          ]);
          setDetails(detailsData);
          setAllPlayers(playersData);

          // Initialize winners state
          if (detailsData) {
            const d = detailsData as any;
            setWinners({
              firstPlaceId: d.firstPlace?.id || d.firstPlaceTeam?.id || 0,
              secondPlaceId: d.secondPlace?.id || d.secondPlaceTeam?.id || 0,
              thirdPlaceId: d.thirdPlace?.id || d.thirdPlaceTeam?.id || 0
            });

            // Sync editTournamentData for eventual updates
            setEditTournamentData({
              name: detailsData.name,
              date: detailsData.date.split('T')[0],
              location: detailsData.location,
              refereeIds: detailsData.refereeIds,
              level: detailsData.level,
              description: detailsData.description,
              timeControl: (detailsData as any).timeControl || '',
              participantsCount: (detailsData as any).participantsCount || 0,
              system: (detailsData as any).system || 'Swiss',
              rules: detailsData.rules || '',
              prizes: detailsData.prizes || '',
              headRefereeName: detailsData.headRefereeName || '',
              referees: (detailsData as any).referees || '',
              schedule: detailsData.schedule || '',
              isTeam: !!(detailsData as any).isTeam
            });
          }
        } catch (eventErr) {
          console.error('Failed to fetch detailed event info:', eventErr);
        }
      }
    } catch (err) {
      console.error('Failed to fetch tournament:', err);
      setError('Не вдалося завантажити інформацію про турнір.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchTournament();
  }, [id]);

  useEffect(() => {
    const delayDebounceFn = setTimeout(async () => {
      if (searchQuery.length >= 2) {
        setIsSearching(true);
        try {
          // 1. Search in existing Players (local search for now, as we have allPlayers)
          const playerResults: SearchResult[] = allPlayers
            .filter(p =>
              (p.firstName + ' ' + p.lastName).toLowerCase().includes(searchQuery.toLowerCase()) ||
              p.group?.toLowerCase().includes(searchQuery.toLowerCase())
            )
            .map(p => ({
              type: 'player',
              id: p.id!,
              userId: p.userId,
              firstName: p.firstName,
              lastName: p.lastName,
              group: p.group,
              rating: tournament?.sport === 'checkers' ? (p.checkersRating || 0) : (p.chessRating || 0)
            }));

          // 2. Search in Users (remote search)
          const users = await searchUsers(searchQuery);

          // Filter out users who are already in playerResults (to avoid duplicates)
          const userResults: SearchResult[] = users
            .filter(u => !playerResults.some(pr => pr.userId === u.id))
            .map(u => ({
              type: 'user',
              id: u.id,
              userId: u.id,
              firstName: u.firstName,
              lastName: u.lastName,
              email: u.email
            }));

          setSearchResults([...playerResults, ...userResults]);
        } catch (err) {
          console.error('Search failed:', err);
        } finally {
          setIsSearching(false);
        }
      } else {
        setSearchResults([]);
      }
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [searchQuery, allPlayers, tournament?.sport]);

  const handleSelectResult = (result: SearchResult) => {
    if (result.type === 'player') {
      setNewPlayer({
        ...newPlayer,
        firstName: result.firstName,
        lastName: result.lastName,
        group: result.group || '',
        ratingELO: result.rating || 1500,
        userId: result.userId || null,
        existingPlayerId: result.id
      });
    } else {
      setNewPlayer({
        ...newPlayer,
        firstName: result.firstName,
        lastName: result.lastName,
        userId: result.userId || null,
        existingPlayerId: null
      });
    }
    setSearchResults([]);
    setSearchQuery('');
  };

  const handleAddPlayer = async () => {
    if (!newPlayer.firstName || !newPlayer.lastName || !newPlayer.group) {
      alert("Заповніть ім'я, прізвище та групу користувача!");
      return;
    }
    try {
      let playerId = newPlayer.existingPlayerId;

      if (!playerId) {
        // Check if player is already in the tournament (by userId)
        if (newPlayer.userId && details?.players.some((p: any) => p.userId === newPlayer.userId)) {
          alert("Цей користувач вже бере участь у турнірі!");
          return;
        }

        const playerPayload: PlayerDto = {
          firstName: newPlayer.firstName,
          lastName: newPlayer.lastName,
          group: newPlayer.group,
          city: newPlayer.city,
          chessRating: tournament?.sport === 'chess' ? newPlayer.ratingELO : undefined,
          checkersRating: tournament?.sport === 'checkers' ? newPlayer.ratingELO : undefined,
          userId: newPlayer.userId,
          chessFideId: newPlayer.fideId.trim() || null,
          chessTitle: newPlayer.title.trim() || null
        };
        const created = await createPlayer(playerPayload);
        playerId = created.id!;
      } else {
        // Check if player is already in the tournament
        const isAlreadyParticipant = details?.players.some((p: any) => p.playerId === playerId);
        if (isAlreadyParticipant) {
          alert("Цей гравець вже бере участь у турнірі!");
          return;
        }
      }

      // Create participant binding
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      
      switch (sportKey) {
        case 'checkers':
          await addParticipantToCheckersEvent(id!, { playerId: playerId, points: 0, rank: 0 });
          break;
        case 'armwrestling':
          await addParticipantToArmwrestlingEvent(id!, { playerId: playerId, weight: newPlayer.weight || 0, rank: 0 });
          break;
        case 'badminton':
          await addParticipantToBadmintonEvent(id!, { playerId: playerId, points: 0, rank: 0 });
          break;
        case 'tabletennis':
        case 'table_tennis':
          await addParticipantToTableTennisEvent(id!, { playerId: playerId, points: 0, rank: 0 });
          break;
        default:
          await addParticipantToEvent(id!, { playerId: playerId, points: 0, rank: 0 });
      }
      alert("Учасник успішно доданий до турніру!");
      setNewPlayer({ firstName: '', lastName: '', group: '', city: 'Київ', ratingELO: 1500, fideId: '', title: '', userId: null, existingPlayerId: null });
      fetchTournament(true); // Refresh
    } catch (err) {
      console.error(err);
      alert("Помилка при додаванні учасника");
    }
  };

  const handleAddGame = async () => {
    if (!newGame.whitePlayerId || !newGame.blackPlayerId) {
      alert("Оберіть обох супротивників!");
      return;
    }
    if (newGame.whitePlayerId === newGame.blackPlayerId) {
      alert("Гравець не може грати сам проти себе!");
      return;
    }
    try {
      if (tournament?.sport === 'checkers') {
        const payload: CheckersGameDto = {
          tournamentId: Number(id),
          whitePlayerId: newGame.whitePlayerId ? Number(newGame.whitePlayerId) : 0,
          blackPlayerId: newGame.blackPlayerId ? Number(newGame.blackPlayerId) : 0,
          result: newGame.result as GameResult,
          round: Number(newGame.round) || 1
        };
        await createCheckersGame(payload);
      } else {
        const payload: ChessGameDto = {
          tournamentId: Number(id),
          whitePlayerId: newGame.whitePlayerId ? Number(newGame.whitePlayerId) : null,
          blackPlayerId: newGame.blackPlayerId ? Number(newGame.blackPlayerId) : null,
          result: newGame.result as GameResult,
          round: Number(newGame.round) || 1,
          board: Number(newGame.board) || 1
        };
        await createChessGame(payload);
      }
      alert("Матч успішно доданий!");
      setNewGame({ ...newGame, result: 'WhiteWin', board: newGame.board + 1 });
      fetchTournament(true); // Refresh
    } catch (err) {
      console.error(err);
      alert("Помилка при додаванні матчу");
    }
  };

  if (isLoading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
        <p>Завантаження інформації про турнір...</p>
      </div>
    );
  }

  if (error || !tournament) {
    return (
      <div className={styles.errorContainer}>
        <h2>Помилка</h2>
        <p>{error || 'Турнір не знайдено.'}</p>
        <button className="btn btn-primary" onClick={() => navigate('/tournaments')}>
          Повернутися до списку
        </button>
      </div>
    );
  }

  const sortedPlayers = (tournament?.sport === 'chess' || tournament?.sport === 'checkers') && details?.players
    ? [...details.players].sort((a, b) => {
      if (b.points !== a.points) return b.points - a.points;
      if (a.rank || b.rank) return (a.rank || 999) - (b.rank || 999);
      return 0;
    })
    : (details as any)?.players || [];

  const getLevelBadgeParams = (level?: string) => {
    switch (level) {
      case 'CollegeGroup': return { label: 'Міжгрупові (коледж)', className: styles.levelCollege };
      case 'Regional': return { label: 'Обласні', className: styles.levelRegional };
      case 'National': return { label: 'Всеукраїнські', className: styles.levelNational };
      default: return { label: level || 'Невідомо', className: styles.levelCollege };
    }
  };

  const getSystemLabel = (system?: string) => {
    switch (system) {
      case 'Swiss': return 'Швейцарська система';
      case 'RoundRobin': return 'Кругова система';
      case 'Knockout': return 'Олімпійська система';
      default: return system || 'Невідома система';
    }
  };

  const getVariantLabel = (variant?: string) => {
    if (variant === 'Russian') return 'Класичні (64)';
    return variant || 'Класичні';
  }

  const getImageUrl = (url: string) => {
    if (!url) return '';
    if (url.startsWith('http')) return url;
    return `${BASE_URL}${url}`;
  };

  const badgeObj = getLevelBadgeParams(details?.level);
  const allGamesOrMatches = (details as any)?.games || (details as any)?.matches || [];
  const playedGames = allGamesOrMatches.filter((g: any) =>
    (g.result && g.result !== 'NotPlayed') || (g.winnerId != null)
  ).length;

  const totalRounds = allGamesOrMatches.reduce((acc: number, game: any) =>
    Math.max(acc, game.round || game.roundNumber || 0), 0
  );
  const isTeamSport = !!(details as any)?.isTeam;

  const actualParticipants = isTeamSport
    ? (details?.teams?.length || 0)
    : ((details as any)?.players?.length || (details as any)?.participants?.length || 0);

  const participantLabel = isTeamSport ? 'команд' : 'учасників';

  const filteredGames = (tournament?.sport === 'chess' || tournament?.sport === 'checkers') && (details as any)?.games
    ? [...(details as any).games].filter((g: any) => {
      const round = g.round || g.roundNumber;
      if (gameFilters.round && round.toString() !== gameFilters.round) return false;
      if (gameFilters.board && g.board?.toString() !== gameFilters.board) return false;
      if (gameFilters.participant) {
        const q = gameFilters.participant.toLowerCase();
        if (!g.whitePlayerName?.toLowerCase().includes(q) && !g.blackPlayerName?.toLowerCase().includes(q)) {
          return false;
        }
      }
      if (gameFilters.result && g.result !== gameFilters.result) return false;
      return true;
    }).sort((a: any, b: any) => {
      const aR = a.round || a.roundNumber;
      const bR = b.round || b.roundNumber;
      if (aR !== bR) return aR - bR;
      return (a.board || 0) - (b.board || 0);
    }) : [];

  return (
    <div className={styles.tournamentViewPage}>
      <div className={styles.container}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
          <button className={styles.backButton} onClick={() => navigate('/tournaments')}>
            <i className="fas fa-arrow-left"></i> Назад
          </button>

          <div style={{ display: 'flex', gap: '8px' }}>
            {isAdmin && (
              <button
                onClick={() => setShowWinnersAdmin(!showWinnersAdmin)}
                style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 20px', backgroundColor: '#10b981', color: '#fff', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' }}
              >
                <i className="fas fa-medal"></i> Призначити переможців
              </button>
            )}
            {isAdmin && (
              <button onClick={handleEditTournament} style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 20px', backgroundColor: '#3b82f6', color: '#fff', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' }}>
                <i className="fas fa-edit"></i> Редагувати турнір
              </button>
            )}
          </div>
        </div>

        <>
        {isAdmin && showWinnersAdmin && (
          <div className={styles.adminSection} style={{ background: '#f0fdf4', border: '1px solid #bbf7d0', padding: '24px', borderRadius: '16px', marginBottom: '24px' }}>
            <h3 style={{ marginTop: 0, marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '10px', color: '#166534' }}>
              <i className="fas fa-trophy"></i> Призначити призерів турніру
            </h3>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '20px' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', fontSize: '14px', color: '#374151' }}>
                  <i className="fas fa-crown" style={{ color: '#fbbf24' }}></i> 1-ше місце (Золото)
                </label>
                <select
                  value={winners.firstPlaceId}
                  onChange={(e) => setWinners({ ...winners, firstPlaceId: Number(e.target.value) })}
                  style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                >
                  <option key="default-first" value="0">-- Оберіть переможця --</option>
                  {details?.players?.map((p: any) => (
                    <option key={`first-p-${p.playerId}`} value={p.playerId}>{p.playerName} ({p.points} очок)</option>
                  ))}
                  {details?.teams?.map((t: any) => (
                    <option key={`first-t-${t.id}`} value={t.id}>{t.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', fontSize: '14px', color: '#374151' }}>
                  <i className="fas fa-medal" style={{ color: '#9ca3af' }}></i> 2-ге місце (Срібло)
                </label>
                <select
                  value={winners.secondPlaceId}
                  onChange={(e) => setWinners({ ...winners, secondPlaceId: Number(e.target.value) })}
                  style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                >
                  <option key="default-second" value="0">-- Оберіть переможця --</option>
                  {details?.players?.map((p: any) => (
                    <option key={`second-p-${p.playerId}`} value={p.playerId}>{p.playerName} ({p.points} очок)</option>
                  ))}
                  {details?.teams?.map((t: any) => (
                    <option key={`second-t-${t.id}`} value={t.id}>{t.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', fontSize: '14px', color: '#374151' }}>
                  <i className="fas fa-medal" style={{ color: '#b45309' }}></i> 3-тє місце (Бронза)
                </label>
                <select
                  value={winners.thirdPlaceId}
                  onChange={(e) => setWinners({ ...winners, thirdPlaceId: Number(e.target.value) })}
                  style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                >
                  <option key="default-third" value="0">-- Оберіть переможця --</option>
                  {details?.players?.map((p: any) => (
                    <option key={`third-p-${p.playerId}`} value={p.playerId}>{p.playerName} ({p.points} очок)</option>
                  ))}
                  {details?.teams?.map((t: any) => (
                    <option key={`third-t-${t.id}`} value={t.id}>{t.name}</option>
                  ))}
                </select>
              </div>
            </div>
            <div style={{ marginTop: '24px', display: 'flex', gap: '12px' }}>
              <button
                onClick={handleSaveWinners}
                style={{ padding: '12px 24px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' }}
              >
                Зберегти результати
              </button>
              <button
                onClick={() => setShowWinnersAdmin(false)}
                style={{ padding: '12px 24px', background: '#6b7280', color: '#fff', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' }}
              >
                Скасувати
              </button>
            </div>
          </div>
        )}

        <div className={styles.headerContent}>
          <div>
            <div className={styles.tournamentBadges}>
              {details?.level && (
                <span className={`${styles.levelBadge} ${badgeObj.className}`}>
                  <i className="fas fa-graduation-cap"></i> {badgeObj.label}
                </span>
              )}
              {(tournament?.sport === 'chess' || tournament?.sport === 'checkers') && (details as any)?.system && (
                <span className={styles.systemBadge}>
                  <i className="fas fa-chess-board"></i> {getSystemLabel((details as any).system)}
                </span>
              )}
              {tournament?.sport === 'checkers' && (
                <span className={styles.systemBadge} style={{ background: '#10b981' }}>
                  <i className="fas fa-dice"></i> {getVariantLabel((details as any)?.variant)}
                </span>
              )}
            </div>
            <h1 className={styles.tournamentName}>{tournament?.title}</h1>
            <div className={styles.tournamentMeta}>
              <div className={styles.metaItem}>
                <i className="fas fa-calendar-alt"></i>
                <span>{tournament?.date}</span>
              </div>
              <div className={styles.metaItem}>
                <i className="fas fa-map-marker-alt"></i>
                <span>{tournament?.location}</span>
              </div>
              <div className={styles.metaItem}>
                <i className="fas fa-users"></i>
                <span>{actualParticipants} {participantLabel}</span>
              </div>
              {details?.timeControl && (
                <div className={styles.metaItem}>
                  <i className="fas fa-clock"></i>
                  <span>Контроль часу: {details.timeControl}</span>
                </div>
              )}
            </div>
          </div>
          <div className={styles.tournamentImage}>
            {details?.photos && details.photos.length > 0 ? (
              <img src={getImageUrl(details.photos[0].url)} alt={tournament?.title} />
            ) : (
              <i className={
                tournament?.sport === 'checkers' ? "fas fa-braille" :
                  tournament?.sport === 'armwrestling' ? "fas fa-hand-rock" :
                    tournament?.sport === 'futsal' ? "fas fa-futbol" :
                      tournament?.sport === 'volleyball' || tournament?.sport === 'beach_volleyball' ? "fas fa-volleyball-ball" :
                        tournament?.sport === 'basketball_3x3' ? "fas fa-basketball-ball" :
                          tournament?.sport === 'badminton' ? "fas fa-user-circle" :
                            "fas fa-chess-queen"
              }></i>
            )}
          </div>
        </div>

        {(tournament?.sport === 'chess' || 
          tournament?.sport === 'checkers' || 
          tournament?.sport === 'badminton' || 
          tournament?.sport === 'table_tennis' || 
          tournament?.sport === 'table-tennis') && details && (
          <TournamentPodium
            firstPlace={details.firstPlace}
            secondPlace={details.secondPlace}
            thirdPlace={details.thirdPlace}
          />
        )}

        <div className={styles.infoCard}>
          <h2><i className="fas fa-info-circle"></i> Про турнір</h2>
          <div className={styles.infoGrid}>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-align-left"></i> Опис</h3>
              <p>{tournament?.description}</p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-trophy"></i> Призи</h3>
              <p style={{ whiteSpace: 'pre-line' }}>
                {details?.prizes || 'Призи будуть оголошені пізніше.'}
              </p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-gavel"></i> Правила</h3>
              <p style={{ whiteSpace: 'pre-line' }}>
                {details?.rules ? details.rules : (
                  <>
                    {(tournament?.sport === 'chess' || tournament?.sport === 'checkers') && (details as any)?.system && (
                      <>• {getSystemLabel((details as any).system)}<br /></>
                    )}
                    {tournament?.sport !== 'armwrestling' && (
                      <>• Контроль часу: {details?.timeControl || 'Не вказано'}<br /></>
                    )}
                    • Заборонені підказки та мобільні телефони<br />
                    • Рішення судді остаточні
                  </>
                )}
              </p>
            </div>
            <div className={styles.infoSection}>
              <h3><i className="fas fa-users"></i> Судді</h3>
              <div className={styles.refereeList}>
                {!(details as any)?.referees && !details?.headRefereeName && (
                  <div className={styles.refereeItem}>
                    <div className={styles.refereeAvatar}>
                      <i className="fas fa-user-tie"></i>
                    </div>
                    <div>
                      <strong>Призначається організатором</strong>
                    </div>
                  </div>
                )}
                {details?.headRefereeName && !(details as any)?.referees && (
                  <div className={styles.refereeItem}>
                    <div className={styles.refereeAvatar}>
                      <i className="fas fa-user-tie"></i>
                    </div>
                    <div>
                      <strong>{details.headRefereeName}</strong><br />
                      <small>Суддя турніру</small>
                    </div>
                  </div>
                )}
                {(details as any)?.referees && (
                  <div style={{ color: '#4b5563', fontSize: '0.95rem', whiteSpace: 'pre-line', padding: '8px 12px', background: '#fff', borderRadius: '10px', border: '1px solid #e5e7eb' }}>
                    {(details as any).referees}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>

        {(() => {
          const s = tournament?.sport.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
          const isTeamFromDetails = !!(details as any)?.isTeam;
          const mandatoryTeamSports = ['volleyball', 'beach_volleyball', 'futsal', 'basketball_3x3', 'basketball3x3'];
          
          if (isTeamFromDetails === true || (isTeamFromDetails === false && mandatoryTeamSports.includes(s) && (details as any).isTeam === undefined)) {
            return (
              <TeamSportsTournamentView
                sport={s as any}
                details={details}
                isAdmin={isAdmin}
                onRefresh={() => fetchTournament(true)}
              />
            );
          }

          if (s === 'armwrestling' && details) {
            return (
              <ArmwrestlingTournamentView
                details={details as any}
                isAdmin={isAdmin}
                onRefresh={() => fetchTournament(true)}
              />
            );
          }

          if (s === 'chess' || s === 'checkers' || s === 'badminton' || s === 'tabletennis' || s === 'table_tennis') {
            return (
              <>
                <div className={styles.statsGrid}>
                  <div className={styles.statCard}>
                    <div className={styles.statIcon}><i className="fas fa-chess-board"></i></div>
                    <div className={styles.statValue}>{totalRounds || '-'}</div>
                    <div className={styles.statLabel}>Всього турів</div>
                  </div>
                  <div className={styles.statCard}>
                    <div className={styles.statIcon}><i className="fas fa-users"></i></div>
                    <div className={styles.statValue}>{actualParticipants}</div>
                    <div className={styles.statLabel}>Учасників</div>
                  </div>
                  <div className={styles.statCard}>
                    <div className={styles.statIcon}>
                      <i className={tournament?.sport === 'badminton' ? "fas fa-shuttlecock" : "fas fa-clock"}></i>
                    </div>
                    <div className={styles.statValue}>{details?.timeControl || (tournament?.sport === 'badminton' ? 'За регламентом' : '-')}</div>
                    <div className={styles.statLabel}>{tournament?.sport === 'badminton' ? 'Формат' : 'Контроль часу'}</div>
                  </div>
                  <div className={styles.statCard}>
                    <div className={styles.statIcon}><i className="fas fa-chart-line"></i></div>
                    <div className={styles.statValue}>{playedGames}</div>
                    <div className={styles.statLabel}>Зіграно партій</div>
                  </div>
                </div>

                <div className={styles.tabsContainer}>
                  <div className={styles.tabsHeader}>
                    <button
                      className={`${styles.tabButton} ${activeTab === 'standings' ? styles.active : ''}`}
                      onClick={() => setActiveTab('standings')}
                    >
                      <i className="fas fa-chart-simple"></i> Турнірна таблиця
                    </button>
                    <button
                      className={`${styles.tabButton} ${activeTab === 'games' ? styles.active : ''}`}
                      onClick={() => setActiveTab('games')}
                    >
                      <i className={tournament?.sport === 'badminton' ? "fas fa-table-tennis-paddle-ball" : "fas fa-chess-board"}></i> {tournament?.sport === 'badminton' ? 'Всі матчі' : 'Всі партії'}
                    </button>
                    <button
                      className={`${styles.tabButton} ${activeTab === 'participants' ? styles.active : ''}`}
                      onClick={() => setActiveTab('participants')}
                    >
                      <i className="fas fa-users"></i> Учасники
                    </button>
                    <button
                      className={`${styles.tabButton} ${activeTab === 'schedule' ? styles.active : ''}`}
                      onClick={() => setActiveTab('schedule')}
                    >
                      <i className="fas fa-calendar-week"></i> Розклад турів
                    </button>
                  </div>

                  {activeTab === 'standings' && (
                    <div className={styles.tabContent}>
                      <div className={styles.tableWrapper}>
                        <table className={styles.dataTable}>
                          <thead>
                            <tr>
                              <th>Місце</th>
                              <th>Учасник</th>
                              <th>Рейтинг</th>
                              <th>Очки</th>
                              {isAdmin && <th>Дії</th>}
                            </tr>
                          </thead>
                          <tbody>
                            {sortedPlayers.length > 0 ? sortedPlayers.map((p: any, idx: number) => (
                              <tr key={p.id}>
                                {editingParticipantId === p.playerId ? (
                                  <>
                                    <td>
                                      <input type="number" value={editingParticipantData.rank} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, rank: parseInt(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                    </td>
                                    <td>
                                      {p.userId ? (
                                        <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                          {p.playerName}
                                        </Link>
                                      ) : (
                                        <span style={{ fontWeight: 'bold' }}>{p.playerName}</span>
                                      )}
                                    </td>
                                    {tournament?.sport === 'chess' ? (
                                      <>
                                        <td>{p.rating}</td>
                                        <td>
                                          <input type="number" step="0.5" value={editingParticipantData.points} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, points: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                        </td>
                                      </>
                                    ) : (
                                      <>
                                        {tournament?.sport === 'armwrestling' ? (
                                          <td>
                                            <input type="number" step="0.1" value={editingParticipantData.weight} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, weight: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                          </td>
                                        ) : (
                                          <td>
                                            <input type="number" step="0.5" value={editingParticipantData.points} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, points: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                          </td>
                                        )}
                                      </>
                                    )}
                                    <td>
                                      <button type="button" onClick={() => handleSaveParticipantEdit(p.playerId)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }}><i className="fas fa-check"></i></button>
                                      <button type="button" onClick={handleCancelParticipantEdit} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}><i className="fas fa-times"></i></button>
                                    </td>
                                  </>
                                ) : (
                                  <>
                                    <td>{p.rank || idx + 1}</td>
                                    <td>
                                      {p.userId ? (
                                        <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                          {p.playerName}
                                        </Link>
                                      ) : (
                                        <strong>{p.playerName}</strong>
                                      )}
                                    </td>
                                    {tournament?.sport === 'chess' ? (
                                      <>
                                        <td>{p.rating}</td>
                                        <td><strong>{p.points}</strong></td>
                                      </>
                                    ) : (
                                      <td><strong>{tournament?.sport === 'armwrestling' ? (p.weight + ' кг') : p.points}</strong></td>
                                    )}
                                    {isAdmin && <td>
                                      <button type="button" onClick={() => handleEditParticipantClick(p)} style={{ padding: '4px 8px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }} title="Редагувати"><i className="fas fa-edit"></i></button>
                                      <button type="button" onClick={() => handleRemoveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Видалити"><i className="fas fa-trash"></i></button>
                                    </td>}
                                  </>
                                )}
                              </tr>
                            )) : (
                              <tr><td colSpan={6}>Ще немає даних</td></tr>
                            )}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  )}

                  {activeTab === 'games' && (
                    <div className={styles.tabContent}>
                      {isAdmin && (
                        <div style={{ background: '#f5f5f5', padding: '16px', borderRadius: '12px', marginBottom: '24px' }}>
                          <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>[Admin] Додати партію (результат)</h3>
                          <div style={{ display: 'flex', flexWrap: 'wrap', gap: '16px', alignItems: 'center' }}>
                            <input type="number" min="1" placeholder="Тур" title="Номер туру" value={newGame.round} onChange={(e) => setNewGame({ ...newGame, round: parseInt(e.target.value) || 1 })} style={{ width: '60px', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                            <input type="number" min="1" placeholder="Дошка" title="Номер дошки" value={newGame.board} onChange={(e) => setNewGame({ ...newGame, board: parseInt(e.target.value) || 1 })} style={{ width: '60px', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />

                            <select value={newGame.whitePlayerId} onChange={(e) => setNewGame({ ...newGame, whitePlayerId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                              <option value="">-- Білі --</option>
                              {sortedPlayers.map((p: any) => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                            </select>

                            <strong style={{ margin: '0 8px' }}> - </strong>

                            <select value={newGame.blackPlayerId} onChange={(e) => setNewGame({ ...newGame, blackPlayerId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                              <option value="">-- Чорні --</option>
                              {sortedPlayers.map((p: any) => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                            </select>

                            <select value={newGame.result} onChange={(e) => setNewGame({ ...newGame, result: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                              <option value="WhiteWin">Перемога Білих (1-0)</option>
                              <option value="BlackWin">Перемога Чорних (0-1)</option>
                              <option value="Draw">Нічия (½-½)</option>
                            </select>

                            <button type="button" onClick={handleAddGame} style={{ padding: '8px 16px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}>Зберегти</button>
                          </div>
                        </div>
                      )}

                      <div style={{ display: 'flex', gap: '16px', marginBottom: '16px', flexWrap: 'wrap', padding: '16px', background: '#f9fafb', borderRadius: '12px' }}>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Тур</label>
                          <input type="number" placeholder="Усі" value={gameFilters.round} onChange={(e) => setGameFilters({ ...gameFilters, round: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', width: '80px' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Дошка</label>
                          <input type="number" placeholder="Усі" value={gameFilters.board} onChange={(e) => setGameFilters({ ...gameFilters, board: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', width: '80px' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Учасник (ім'я)</label>
                          <input type="text" placeholder="Пошук..." value={gameFilters.participant} onChange={(e) => setGameFilters({ ...gameFilters, participant: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', minWidth: '200px' }} />
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Результат</label>
                          <select value={gameFilters.result} onChange={(e) => setGameFilters({ ...gameFilters, result: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                            <option value="">Усі результати</option>
                            <option value="WhiteWin">Перемога Білих (1-0)</option>
                            <option value="BlackWin">Перемога Чорних (0-1)</option>
                            <option value="Draw">Нічия (½-½)</option>
                          </select>
                        </div>
                        <div style={{ display: 'flex', alignItems: 'flex-end' }}>
                          <button
                            type="button"
                            onClick={() => setGameFilters({ round: '', board: '', participant: '', result: '' })}
                            style={{ padding: '8px 16px', background: '#6b7280', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
                          >
                            Скинути
                          </button>
                        </div>
                      </div>

                      <div className={styles.tableWrapper}>
                        <table className={styles.dataTable}>
                          <thead>
                            <tr>
                              <th>Тур</th>
                              {tournament?.sport !== 'checkers' && <th>Дошка</th>}
                              <th>Білі</th>
                              <th>Чорні</th>
                              <th>Результат</th>
                              {isAdmin && <th>Дії</th>}
                            </tr>
                          </thead>
                          <tbody>
                            {filteredGames.length > 0 ? (
                              filteredGames.map((game, idx) => (
                                <tr key={game.id}>
                                  {editingGameId === game.id ? (
                                    <>
                                      <td>
                                        <input
                                          type="number"
                                          min="1"
                                          value={editingGameData.round}
                                          onChange={(e) => setEditingGameData({ ...editingGameData, round: parseInt(e.target.value) || 1 })}
                                          style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}
                                        />
                                      </td>
                                      {tournament?.sport !== 'checkers' && (
                                        <td>
                                          <input
                                            type="number"
                                            min="1"
                                            value={editingGameData.board}
                                            onChange={(e) => setEditingGameData({ ...editingGameData, board: parseInt(e.target.value) || 1 })}
                                            style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}
                                          />
                                        </td>
                                      )}
                                      <td>
                                        <select value={editingGameData.whitePlayerId} onChange={(e) => setEditingGameData({ ...editingGameData, whitePlayerId: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                          <option value="">-- Білі --</option>
                                          {sortedPlayers.map((p: any) => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                                        </select>
                                      </td>
                                      <td>
                                        <select value={editingGameData.blackPlayerId} onChange={(e) => setEditingGameData({ ...editingGameData, blackPlayerId: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                          <option value="">-- Чорні --</option>
                                          {sortedPlayers.map((p: any) => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                                        </select>
                                      </td>
                                      <td>
                                        <select value={editingGameData.result} onChange={(e) => setEditingGameData({ ...editingGameData, result: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                          <option value="WhiteWin">Перемога Білих (1-0)</option>
                                          <option value="BlackWin">Перемога Чорних (0-1)</option>
                                          <option value="Draw">Нічия (½-½)</option>
                                        </select>
                                      </td>
                                      <td>
                                        <button type="button" onClick={() => handleSaveEdit(game.id)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }}><i className="fas fa-check"></i></button>
                                        <button type="button" onClick={handleCancelEdit} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}><i className="fas fa-times"></i></button>
                                      </td>
                                    </>
                                  ) : (
                                    <>
                                      <td>{(game as any).round || (game as any).roundNumber}</td>
                                      {tournament?.sport !== 'checkers' && <td>{(game as any).board || idx + 1}</td>}
                                      <td>
                                        {game.whiteUserId ? (
                                          <Link to={`/profile/${game.whiteUserId}`} style={{ color: '#3b82f6', textDecoration: 'none' }}>
                                            {game.whitePlayerName}
                                          </Link>
                                        ) : (
                                          game.whitePlayerName || '—'
                                        )}
                                      </td>
                                      <td>
                                        {game.blackUserId ? (
                                          <Link to={`/profile/${game.blackUserId}`} style={{ color: '#3b82f6', textDecoration: 'none' }}>
                                            {game.blackPlayerName}
                                          </Link>
                                        ) : (
                                          game.blackPlayerName || '—'
                                        )}
                                      </td>
                                      <td>
                                        {game.result === 'WhiteWin' ? (
                                          <span className={`${styles.resultBadge} ${styles.resultWhiteWin}`}>
                                            <i className="fas fa-chess-king"></i> 1-0
                                          </span>
                                        ) : game.result === 'BlackWin' ? (
                                          <span className={`${styles.resultBadge} ${styles.resultBlackWin}`}>
                                            <i className="fas fa-chess-queen"></i> 0-1
                                          </span>
                                        ) : game.result === 'Draw' ? (
                                          <span className={`${styles.resultBadge} ${styles.resultDraw}`}>
                                            <i className="fas fa-handshake"></i> ½-½
                                          </span>
                                        ) : (
                                          <span className={`${styles.resultBadge} ${styles.resultNotPlayed}`}>
                                            <i className="fas fa-clock"></i> Очікується
                                          </span>
                                        )}
                                      </td>
                                      {isAdmin && (
                                        <td>
                                          <button onClick={() => handleEditClick(game)} style={{ padding: '4px 8px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
                                            <i className="fas fa-edit"></i>
                                          </button>
                                        </td>
                                      )}
                                    </>
                                  )}
                                </tr>
                              ))
                            ) : (
                              <tr><td colSpan={isAdmin ? 6 : 5} style={{ textAlign: 'center', padding: '24px', color: '#666' }}>Партії не знайдені за вказаними фільтрами</td></tr>
                            )}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  )}

                  {activeTab === 'participants' && (
                    <div className={styles.tabContent}>
                      {isAdmin && (
                        <div style={{ background: '#f5f5f5', padding: '24px', borderRadius: '12px', marginBottom: '24px' }}>
                          <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>[Admin] Додати учасника</h3>

                          <div style={{ marginBottom: '20px', position: 'relative' }}>
                            <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold' }}>Пошук гравця або користувача:</label>
                            <input
                              type="text"
                              placeholder="Введіть ім'я, прізвище або email для пошуку в базі..."
                              value={searchQuery}
                              onChange={(e) => setSearchQuery(e.target.value)}
                              style={{ width: '100%', padding: '10px', borderRadius: '6px', border: '1px solid #ccc' }}
                            />
                            {isSearching && <div style={{ marginTop: '8px' }}>Пошук...</div>}
                            {searchResults.length > 0 && (
                              <div style={{
                                position: 'absolute',
                                top: '100%',
                                left: 0,
                                right: 0,
                                background: '#fff',
                                border: '1px solid #ccc',
                                borderRadius: '6px',
                                zIndex: 10,
                                maxHeight: '200px',
                                overflowY: 'auto',
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
                              }}>
                                {searchResults.map((result, idx) => (
                                  <div
                                    key={`${result.type}-${result.id}-${idx}`}
                                    onClick={() => handleSelectResult(result)}
                                    style={{ padding: '10px', cursor: 'pointer', borderBottom: '1px solid #eee' }}
                                    onMouseOver={(e) => (e.currentTarget.style.background = '#f0f0f0')}
                                    onMouseOut={(e) => (e.currentTarget.style.background = '#fff')}
                                  >
                                    <span style={{
                                      fontSize: '10px',
                                      background: result.type === 'player' ? '#10b981' : '#3b82f6',
                                      color: '#fff',
                                      padding: '2px 6px',
                                      borderRadius: '4px',
                                      marginRight: '8px',
                                      textTransform: 'uppercase'
                                    }}>
                                      {result.type === 'player' ? 'Гравець' : 'Користувач'}
                                    </span>
                                    <strong>{result.firstName} {result.lastName}</strong>
                                    {result.email && <span style={{ color: '#666', marginLeft: '8px' }}>({result.email})</span>}
                                    {result.group && <span style={{ color: '#666', marginLeft: '8px' }}>[{result.group}]</span>}
                                  </div>
                                ))}
                              </div>
                            )}
                          </div>

                          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginBottom: '16px' }}>
                            <div style={{ display: 'flex', flexDirection: 'column' }}>
                              <label style={{ marginBottom: '4px', fontSize: '12px' }}>Ім'я</label>
                              <input type="text" placeholder="Ім'я" value={newPlayer.firstName} onChange={(e) => setNewPlayer({ ...newPlayer, firstName: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                            </div>
                            <div style={{ display: 'flex', flexDirection: 'column' }}>
                              <label style={{ marginBottom: '4px', fontSize: '12px' }}>Прізвище</label>
                              <input type="text" placeholder="Прізвище" value={newPlayer.lastName} onChange={(e) => setNewPlayer({ ...newPlayer, lastName: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                            </div>
                            <div style={{ display: 'flex', flexDirection: 'column' }}>
                              <label style={{ marginBottom: '4px', fontSize: '12px' }}>Група (напр. 211)</label>
                              <input type="text" placeholder="Група (напр. 211)" value={newPlayer.group} onChange={(e) => setNewPlayer({ ...newPlayer, group: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                            </div>
                            {tournament?.sport === 'chess' && (
                              <>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Рейтинг ELO</label>
                                  <input type="number" placeholder="Рейтинг ELO" value={newPlayer.ratingELO} onChange={(e) => setNewPlayer({ ...newPlayer, ratingELO: parseInt(e.target.value) || 1500 })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>FIDE ID (опціонально)</label>
                                  <input type="text" placeholder="FIDE ID (опціонально)" value={newPlayer.fideId} onChange={(e) => setNewPlayer({ ...newPlayer, fideId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Звання (напр. CM, FM, IM, GM) (опціонально)</label>
                                  <input type="text" placeholder="Звання (напр. CM, FM, IM, GM) (опціонально)" value={newPlayer.title} onChange={(e) => setNewPlayer({ ...newPlayer, title: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                              </>
                            )}
                            {tournament?.sport === 'armwrestling' && (
                              <div style={{ display: 'flex', flexDirection: 'column' }}>
                                <label style={{ marginBottom: '4px', fontSize: '12px' }}>Вага (кг)</label>
                                <input type="number" step="0.1" placeholder="Вага" value={newPlayer.weight} onChange={(e) => setNewPlayer({ ...newPlayer, weight: parseFloat(e.target.value) || 0 })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                              </div>
                            )}
                          </div>
                          <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                            <button type="button" onClick={handleAddPlayer} style={{ padding: '10px 24px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}>
                              {newPlayer.existingPlayerId ? 'Додати існуючого гравця' : newPlayer.userId ? 'Створити профіль гравця та додати' : 'Створити гостя та додати'}
                            </button>
                            {(newPlayer.userId || newPlayer.existingPlayerId) && (
                              <button
                                onClick={() => setNewPlayer({ ...newPlayer, userId: null, existingPlayerId: null, firstName: '', lastName: '', group: '' })}
                                style={{ padding: '10px 24px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
                              >
                                Скинути вибір
                              </button>
                            )}
                          </div>
                        </div>
                      )}

                      <div className={styles.tableWrapper}>
                        <table className={styles.dataTable}>
                          <thead>
                            <tr>
                              <th>№</th>
                              <th>Учасник</th>
                              {tournament?.sport === 'chess' ? (
                                <>
                                  <th>Рейтинг ELO</th>
                                  <th>Титул</th>
                                  <th>FIDE ID</th>
                                </>
                              ) : (
                                <th>Очки</th>
                              )}
                              {isAdmin && <th>Дії</th>}
                            </tr>
                          </thead>
                          <tbody>
                            {sortedPlayers.length > 0 ? sortedPlayers.map((p: any, idx: number) => (
                              <tr key={p.id}>
                                <td>{idx + 1}</td>
                                <td>
                                  {p.userId ? (
                                    <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                      {p.playerName}
                                    </Link>
                                  ) : (
                                    p.playerName
                                  )}
                                </td>
                                {tournament?.sport === 'chess' ? (
                                  <>
                                    <td>{p.rating}</td>
                                    <td>
                                      {p.title ? (
                                        <span className={styles.systemBadge} style={{ fontWeight: 'bold' }}>{p.title}</span>
                                      ) : '—'}
                                    </td>
                                    <td>{p.fideId || '—'}</td>
                                  </>
                                ) : (
                                  <td>{p.points}</td>
                                )}
                                {isAdmin && (
                                  <td>
                                    <button onClick={() => handleRemoveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Видалити">
                                      <i className="fas fa-trash"></i>
                                    </button>
                                  </td>
                                )}
                              </tr>
                            )) : (
                              <tr><td colSpan={isAdmin ? 6 : 5}>Учасники не знайдені</td></tr>
                            )}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  )}

                  {activeTab === 'schedule' && (
                    <div className={styles.tabContent}>
                      <div className={styles.infoGrid}>
                        <div className={styles.infoSection}>
                          <h3><i className="fas fa-calendar-day"></i> Розклад</h3>
                          {details?.schedule ? (
                            <p style={{ whiteSpace: 'pre-line' }}>{details.schedule}</p>
                          ) : (
                            <>
                              <p>Турнір проводиться згідно із розкладом, вказаним організаторами під час реєстрації або під час відкриття.</p>
                              <br />
                              <p>
                                <strong>Примірний розклад:</strong><br />
                                <strong>10:00</strong> - Реєстрація учасників<br />
                                <strong>11:00</strong> - Жеребкування та відкриття<br />
                                <strong>Далі:</strong> за календарем турів
                              </p>
                            </>
                          )}
                        </div>
                      </div>
                    </div>
                  )}
                </div>
              </>
            );
          }
          return null;
        })()}

        {/* Photo Gallery Section */}
        {details?.photos && details.photos.length > 0 && (
          <div className={styles.gallerySection}>
            <div className={styles.galleryHeader}>
              <i className="fas fa-images"></i>
              <h2>Галерея турніру</h2>
            </div>
            <div className={styles.photoGrid}>
              {details.photos.map((photo: any, index: number) => (
                <div 
                  key={photo.id} 
                  className={styles.galleryItem}
                  onClick={() => setSelectedPhotoIndex(index)}
                >
                  <img src={getImageUrl(photo.url)} alt={`Фото ${index + 1}`} loading="lazy" />
                  <div className={styles.photoOverlay}>
                    <i className="fas fa-search-plus"></i>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Lightbox / Fullscreen Preview */}
        {selectedPhotoIndex !== null && details?.photos && (
          <div className={styles.lightboxOverlay} onClick={() => setSelectedPhotoIndex(null)}>
            <div className={styles.lightboxContent} onClick={(e) => e.stopPropagation()}>
              <button className={styles.lightboxClose} onClick={() => setSelectedPhotoIndex(null)}>
                <i className="fas fa-times"></i>
              </button>
              
              <img 
                src={getImageUrl(details.photos[selectedPhotoIndex].url)} 
                alt="Tournament Fullscreen" 
                className={styles.lightboxImage} 
              />
              
              <div className={styles.lightboxNav}>
                <button 
                  className={styles.lightboxBtn}
                  onClick={(e) => {
                    e.stopPropagation();
                    setSelectedPhotoIndex((selectedPhotoIndex - 1 + details.photos.length) % details.photos.length);
                  }}
                >
                  <i className="fas fa-chevron-left"></i> Назад
                </button>
                <button 
                  className={styles.lightboxBtn}
                  onClick={(e) => {
                    e.stopPropagation();
                    setSelectedPhotoIndex((selectedPhotoIndex + 1) % details.photos.length);
                  }}
                >
                  Вперед <i className="fas fa-chevron-right"></i>
                </button>
              </div>
            </div>
          </div>
        )}
        {isEditingTournament && (
          <div className={styles.modalOverlay}>
            <div className={styles.modalContent}>
              <div className={styles.modalHeader}>
                <h2 style={{ margin: 0, fontSize: '1.5rem', display: 'flex', alignItems: 'center', gap: '12px' }}>
                  <i className="fas fa-edit" style={{ color: '#3b82f6' }}></i>
                  Редагувати турнір
                </h2>
                <button onClick={() => setIsEditingTournament(false)} className={styles.closeButton}>&times;</button>
              </div>
              <div className={styles.modalBody}>
                <div className={styles.editFormGrid}>
                  <div className={styles.formGroup}>
                    <label>Назва турніру</label>
                    <input type="text" value={editTournamentData.name} onChange={(e) => setEditTournamentData({ ...editTournamentData, name: e.target.value })} />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Дата проведення</label>
                    <input type="date" value={editTournamentData.date} onChange={(e) => setEditTournamentData({ ...editTournamentData, date: e.target.value })} />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Локація</label>
                    <input type="text" value={editTournamentData.location} onChange={(e) => setEditTournamentData({ ...editTournamentData, location: e.target.value })} />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Рівень змагань</label>
                    <select value={editTournamentData.level} onChange={(e) => setEditTournamentData({ ...editTournamentData, level: e.target.value as CompetitionLevel })}>
                      <option value="CollegeGroup">Міжгрупові (коледж)</option>
                      <option value="Regional">Обласні</option>
                      <option value="National">Всеукраїнські</option>
                    </select>
                  </div>
                  {(tournament?.sport === 'chess' || tournament?.sport === 'checkers') && (
                    <>
                      <div className={styles.formGroup}>
                        <label>Система проведення</label>
                        <select value={editTournamentData.system} onChange={(e) => setEditTournamentData({ ...editTournamentData, system: e.target.value as ChessSystem })}>
                          <option value="Swiss">Швейцарська система</option>
                          <option value="RoundRobin">Кругова система</option>
                          <option value="Knockout">Олімпійська система</option>
                        </select>
                      </div>
                      <div className={styles.formGroup}>
                        <label>Контроль часу</label>
                        <input type="text" placeholder="напр. 10хв + 5с" value={editTournamentData.timeControl} onChange={(e) => setEditTournamentData({ ...editTournamentData, timeControl: e.target.value })} />
                      </div>
                    </>
                  )}
                  <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                    <label style={{ display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer', background: '#f8fafc', padding: '12px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                      <input 
                        type="checkbox" 
                        checked={editTournamentData.isTeam} 
                        onChange={(e) => setEditTournamentData({ ...editTournamentData, isTeam: e.target.checked })} 
                        style={{ width: '20px', height: '20px' }}
                      />
                      <div>
                        <strong style={{ display: 'block' }}>Командний турнір</strong>
                        <small style={{ color: '#64748b' }}>Активуйте, якщо матчі проводяться між командами (групами)</small>
                      </div>
                    </label>
                  </div>
                  <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                    <label>Судді</label>
                    <textarea rows={3} placeholder="Введіть імена суддів..." value={editTournamentData.referees} onChange={(e) => setEditTournamentData({ ...editTournamentData, referees: e.target.value })} />
                  </div>
                  <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                    <label>Опис турніру</label>
                    <textarea rows={3} value={editTournamentData.description} onChange={(e) => setEditTournamentData({ ...editTournamentData, description: e.target.value })} />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Правила</label>
                    <textarea rows={5} placeholder="Введіть правила турніру..." value={editTournamentData.rules} onChange={(e) => setEditTournamentData({ ...editTournamentData, rules: e.target.value })} />
                  </div>
                  <div className={styles.formGroup}>
                    <label>Призи</label>
                    <textarea rows={5} placeholder="Опишіть призовий фонд..." value={editTournamentData.prizes} onChange={(e) => setEditTournamentData({ ...editTournamentData, prizes: e.target.value })} />
                  </div>
                  <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                    <label>Розклад турніру</label>
                    <textarea rows={5} placeholder="Введіть детальний розклад (кожен пункт з нового рядка)..." value={editTournamentData.schedule} onChange={(e) => setEditTournamentData({ ...editTournamentData, schedule: e.target.value })} />
                  </div>

                  <div className={styles.photoManagementSection} style={{ gridColumn: '1 / -1' }}>
                    <label><i className="fas fa-images"></i> Фотографії турніру</label>

                    <div className={styles.photoUploadArea}
                      onDragOver={(e) => e.preventDefault()}
                      onDrop={handlePhotoUpload}>
                      {isUploading ? (
                        <div className={styles.uploadSpinner}><i className="fas fa-circle-notch fa-spin"></i> Завантаження...</div>
                      ) : (
                        <div className={styles.uploadPlaceholder}>
                          <i className="fas fa-cloud-upload-alt"></i>
                          <p>Перетягніть фото сюди або <span>виберіть файл</span></p>
                          <input type="file" accept="image/*" onChange={handlePhotoUpload} className={styles.fileInput} />
                        </div>
                      )}
                    </div>

                    {details?.photos && details.photos.length > 0 && (
                      <div className={styles.currentPhotosGrid}>
                        {details.photos.map((photo: any) => (
                          <div key={photo.id} className={styles.photoPreviewCard}>
                            <img src={getImageUrl(photo.url)} alt="Tournament" />
                            <button onClick={() => handleDeletePhoto(photo.id)} className={styles.deletePhotoBtn}>
                              <i className="fas fa-times"></i>
                            </button>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
              </div>
            </div>
            <div className={styles.modalFooter}>
              <button onClick={() => setIsEditingTournament(false)} className={styles.cancelBtn}>Скасувати</button>
              <button onClick={handleSaveTournament} className={styles.saveBtn}>Зберегти зміни</button>
            </div>
          </div>
        </div>
      )}
        </>
      </div>
    </div>
  );
};

export default TournamentViewPage;
