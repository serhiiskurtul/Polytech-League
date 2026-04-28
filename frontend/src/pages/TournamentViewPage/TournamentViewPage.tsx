import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import toast from 'react-hot-toast';
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
  registerForChessEvent,
  approveChessParticipant,
  rejectChessParticipant,
  importDataFromFile,
  getProfile
} from '@/shared/api';
import { PlayerDto } from '@/shared/types/player.types';
import { RegistrationStatus } from '@/shared/types/tournament.types';
import {
  getCheckersEventById,
  updateCheckersEvent,
  addParticipantToCheckersEvent,
  removeParticipantFromCheckersEvent,
  updateParticipantInCheckersEvent,
  createCheckersGame,
  uploadCheckersPhoto,
  deleteCheckersPhoto,
  importCheckersDataFromFile,
  registerForCheckersEvent,
  approveCheckersParticipant,
  rejectCheckersParticipant
} from '@/shared/api/checkers.api';
import {
  getArmwrestlingEventById,
  updateArmwrestlingEvent,
  addParticipantToArmwrestlingEvent,
  removeParticipantFromArmwrestlingEvent,
  updateArmwrestlingParticipant,
  registerForArmwrestlingEvent,
  approveArmwrestlingParticipant,
  rejectArmwrestlingParticipant
} from '@/shared/api/armwrestling.api';
import { getVolleyballEventById, updateVolleyballEvent, registerForVolleyballEvent } from '@/shared/api/volleyball.api';
import { getBeachVolleyballEventById, updateBeachVolleyballEvent, registerForBeachVolleyballEvent } from '@/shared/api/beachVolleyball.api';
import { getFutsalEventById, updateFutsalEvent, registerForFutsalEvent } from '@/shared/api/futsal.api';
import { getBasketball3x3EventById, updateBasketball3x3Event, registerForBasketball3x3Event } from '@/shared/api/basketball3x3.api';
import {
  getBadmintonEventById,
  updateBadmintonEvent,
  addParticipantToBadmintonEvent,
  removeParticipantFromBadmintonEvent,
  updateBadmintonParticipant,
  registerForBadmintonEvent,
  approveBadmintonParticipant,
  rejectBadmintonParticipant
} from '@/shared/api/badminton.api';
import {
  getTableTennisEventById,
  updateTableTennisEvent,
  addParticipantToTableTennisEvent,
  removeParticipantFromTableTennisEvent,
  updateTableTennisParticipant,
  registerForTableTennisEvent,
  approveTableTennisParticipant,
  rejectTableTennisParticipant
} from '@/shared/api/tabletennis.api';
import ArmwrestlingTournamentView from './components/ArmwrestlingTournamentView';
import TeamSportsTournamentView from './components/TeamSportsTournamentView';
import TournamentPodium from './components/TournamentPodium';
import ChessGameViewerModal from './components/ChessGameViewerModal';
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
import api, { BASE_URL } from '@/shared/api/axiosInstance';
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
  const [isRegistering, setIsRegistering] = useState(false);
  const [currentUserPlayer, setCurrentUserPlayer] = useState<PlayerDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [selectedPhotoIndex, setSelectedPhotoIndex] = useState<number | null>(null);
  const [seasons, setSeasons] = useState<any[]>([]);

  // Admin access check (0 = User, 1 = Admin)
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';

  // Search Results type
  type SearchResult = {
    type: 'player' | 'user';
    id: number;
    userId?: number | null;
    firstName: string;
    lastName: string;
    patronymic?: string;
    email?: string;
    group?: string;
    studentId?: string;
    rating?: number;
    birthDate?: string;
    faculty?: string;
  };

  // Admin Form States
  const [newPlayer, setNewPlayer] = useState({
    firstName: '',
    patronymic: '',
    lastName: '',
    group: '',
    studentId: '',
    birthDate: '',
    city: 'Чернівці',
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
  const [newGame, setNewGame] = useState({ whitePlayerId: '', blackPlayerId: '', result: 'WhiteWin', round: 1, board: 1, pgn: '' });

  const [viewingPgnGame, setViewingPgnGame] = useState<ChessGameResponse | null>(null);

  const [editingGameId, setEditingGameId] = useState<number | null>(null);
  const [editingGameData, setEditingGameData] = useState<{ round: number; board: number; result: string; whitePlayerId: string; blackPlayerId: string, pgn: string }>({ round: 1, board: 1, result: 'WhiteWin', whitePlayerId: '', blackPlayerId: '', pgn: '' });

  const [editingParticipantId, setEditingParticipantId] = useState<number | null>(null);
  const [editingParticipantData, setEditingParticipantData] = useState<{ points: number; rank: number; weight: number; status: RegistrationStatus; firstName: string; lastName: string; groupName: string }>({ points: 0, rank: 0, weight: 0, status: RegistrationStatus.Approved, firstName: '', lastName: '', groupName: '' });
  const [gameFilters, setGameFilters] = useState({ round: '', board: '', participant: '', result: '' });
  const [isImporting, setIsImporting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

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
      const isTeamSport = !!(details as any).isTeam || ['futsal', 'volleyball', 'beach_volleyball', 'basketball_3x3', 'badminton'].includes(sportKey);

      // Build clean base payload with common fields to avoid "Extra fields" 400 error
      const payload: any = {
        name: editTournamentData.name,
        date: formatToISO(editTournamentData.date),
        location: editTournamentData.location,
        refereeIds: editTournamentData.refereeIds,
        level: editTournamentData.level,
        description: editTournamentData.description,
        rules: editTournamentData.rules,
        prizes: editTournamentData.prizes,
        headRefereeName: editTournamentData.headRefereeName,
        referees: editTournamentData.referees,
        schedule: editTournamentData.schedule,
        registrationStart: editTournamentData.level === 'CollegeGroup' ? formatToISO(editTournamentData.registrationStart) : null,
        registrationEnd: editTournamentData.level === 'CollegeGroup' ? formatToISO(editTournamentData.registrationEnd) : null,
        endDate: formatToISO(editTournamentData.endDate),
        isModerated: editTournamentData.level === 'CollegeGroup' ? editTournamentData.isModerated : false,
        isTeam: !!(details as any).isTeam,
        seasonId: editTournamentData.seasonId || null
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
        case 'tabletennis':
        case 'table_tennis': await updateTableTennisEvent(id, {
          ...payload,
          tournamentType: (details as any).tournamentType || 'RoundRobin',
          category: (details as any).category || 'Men'
        } as any); break;
        default: await updateChessEvent(id, payload as any);
      }

      toast.success("Призерів успішно встановлено!");
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
    isTeam: false,
    registrationStart: '',
    registrationEnd: '',
    endDate: '',
    isModerated: false,
    isPublic: true,
    seasonId: null,
    category: 'Mixed'
  });



  const handleImportData = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || !id) return;

    if (!window.confirm(`Ви впевнені? Це дію оновить список учасників з файлу "${file.name}". Існуючі результати учасників можуть бути змінені.`)) {
      if (fileInputRef.current) fileInputRef.current.value = '';
      return;
    }

    try {
      setIsImporting(true);
      if (tournament?.sport === 'chess') {
        await importDataFromFile(id, file);
      } else if (tournament?.sport === 'checkers') {
        await importCheckersDataFromFile(id, file);
      }
      toast.success('Дані успішно імпортовано з файлу!');
      fetchTournament(true);
    } catch (err: any) {
      console.error(err);
      alert('Помилка при імпорті даних з файлу: ' + (err.response?.data?.message || err.message));
    } finally {
      setIsImporting(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const [isUploading, setIsUploading] = useState(false);

  const handleDeleteTournament = async () => {
    if (!id || !tournament) return;
    if (window.confirm('Ви впевнені, що хочете видалити ці змагання? Цю дію неможливо скасувати.')) {
      try {
        const sportEndpointRecord: Record<string, string> = {
          checkers: '/checkers-events',
          chess: '/chess-events',
          futsal: '/futsal-events',
          volleyball: '/volleyball-events',
          beach_volleyball: '/beach-volleyball-events',
          beachvolleyball: '/beach-volleyball-events',
          basketball_3x3: '/basketball3x3-events',
          basketball3x3: '/basketball3x3-events',
          table_tennis: '/table-tennis-events',
          tabletennis: '/table-tennis-events',
          armwrestling: '/armwrestling-events',
          badminton: '/badminton-events'
        };
        const s = (tournament?.sport || '').toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
        const endpoint = sportEndpointRecord[s] || sportEndpointRecord[tournament?.sport || ''];

        if (!endpoint) {
          alert('Неможливо видалити змагання: невідомий вид спорту.');
          return;
        }

        await api.delete(`${endpoint}/${id}`);
        toast.success('Змагання успішно видалено.');
        navigate('/tournaments');
      } catch (error) {
        console.error('Помилка при видаленні змагання:', error);
        alert('Помилка при видаленні змагань. Деталі в консолі.');
      }
    }
  };
  
  const formatDateForInput = (dateStr: string | undefined | null) => {
    if (!dateStr) return '';
    try {
      const d = new Date(dateStr);
      if (isNaN(d.getTime())) return '';
      
      const pad = (num: number) => String(num).padStart(2, '0');
      const year = d.getFullYear();
      const month = pad(d.getMonth() + 1);
      const day = pad(d.getDate());
      const hours = pad(d.getHours());
      const minutes = pad(d.getMinutes());
      
      return `${year}-${month}-${day}T${hours}:${minutes}`;
    } catch (e) {
      return '';
    }
  };

  const formatToISO = (dateStr: string | undefined | null) => {
    if (!dateStr) return null;
    try {
      const d = new Date(dateStr);
      return isNaN(d.getTime()) ? null : d.toISOString();
    } catch (e) {
      return null;
    }
  };

  const handleEditTournament = () => {
    if (details) {
      setEditTournamentData({
        name: tournament?.title || details.name,
        date: formatDateForInput(details.date),
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
        isTeam: !!(details as any).isTeam,
        registrationStart: formatDateForInput(tournament?.registrationStart || details.registrationStart),
        registrationEnd: formatDateForInput(tournament?.registrationEnd || details.registrationEnd),
        endDate: formatDateForInput(tournament?.endDate || details.endDate),
        isModerated: !!details.isModerated,
        isPublic: details.isPublic !== undefined ? !!details.isPublic : true,
        seasonId: details.seasonId || null,
        category: (details as any).category || (tournament?.category as any) || 'Mixed'
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
        date: formatToISO(editTournamentData.date),
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
        registrationStart: editTournamentData.level === 'CollegeGroup' ? formatToISO(editTournamentData.registrationStart) : null,
        registrationEnd: editTournamentData.level === 'CollegeGroup' ? formatToISO(editTournamentData.registrationEnd) : null,
        endDate: formatToISO(editTournamentData.endDate),
        isModerated: editTournamentData.level === 'CollegeGroup' ? editTournamentData.isModerated : false,
        isPublic: editTournamentData.level === 'CollegeGroup' ? editTournamentData.isPublic : false,
        seasonId: editTournamentData.seasonId || null,
        category: editTournamentData.category || 'Mixed'
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
      } else if (sportKey === 'tabletennis' || sportKey === 'table_tennis') {
        await updateTableTennisEvent(id!, { 
          ...payload, 
          tournamentType: (details as any).tournamentType || 'RoundRobin'
        });
      } else {
        // Chess
        await updateChessEvent(id!, {
          ...payload,
          system: editTournamentData.system,
          timeControl: editTournamentData.timeControl,
          participantsCount: editTournamentData.participantsCount
        } as any);
      }
      toast.success("Дані змагань успішно оновлено!");
      setIsEditingTournament(false);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні змагань");
    }
  };

  const handleEditClick = (game: ChessGameResponse) => {
    setEditingGameId(game.id);
    setEditingGameData({
      round: game.round || 1,
      board: game.board || 1,
      result: game.result,
      whitePlayerId: game.whitePlayerId ? game.whitePlayerId.toString() : '',
      blackPlayerId: game.blackPlayerId ? game.blackPlayerId.toString() : '',
      pgn: game.pgn || ''
    });
  };

  const handleEditParticipantClick = (p: any) => {
    setEditingParticipantId(p.playerId);
    setEditingParticipantData({
      points: p.points || 0,
      rank: p.rank || 0,
      weight: p.weight || 0,
      status: p.status || RegistrationStatus.Approved,
      firstName: p.firstName || '',
      lastName: p.lastName || '',
      groupName: p.groupName || ''
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
          board: editingGameData.board || 1,
          pgn: editingGameData.pgn
        };
        await updateChessGame(gameId, payload);
      }
      toast.success("Матч успішно оновлено!");
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
          status: editingParticipantData.status,
          firstName: editingParticipantData.firstName,
          lastName: editingParticipantData.lastName,
          groupName: editingParticipantData.groupName
        };
        await updateParticipantInCheckersEvent(id!, playerId, payload);
      } else if (sportKey === 'armwrestling') {
        await updateArmwrestlingParticipant(id!, playerId, {
          playerId,
          weight: (editingParticipantData as any).weight || 0,
          rank: editingParticipantData.rank,
          status: editingParticipantData.status
        });
      } else if (sportKey === 'badminton') {
        await updateBadmintonParticipant(id!, playerId, {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
          status: editingParticipantData.status
        });
      } else if (sportKey === 'tabletennis' || sportKey === 'table_tennis') {
        await updateTableTennisParticipant(id!, playerId, {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
          status: editingParticipantData.status
        });
      } else {
        const payload: ChessEventParticipantDto = {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
          status: editingParticipantData.status,
          firstName: editingParticipantData.firstName,
          lastName: editingParticipantData.lastName,
          groupName: editingParticipantData.groupName
        };
        await updateParticipantInEvent(id!, playerId, payload);
      }

      toast.success("Дані учасника оновлено!");
      setEditingParticipantId(null);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      alert("Помилка при оновленні учасника");
    }
  };

  const handleRemoveParticipant = async (playerId: number, playerName: string) => {
    if (window.confirm(`Ви впевнені, що хочете видалити учасника "${playerName}" зі змагань?`)) {
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

        toast.success("Учасника видалено зі змагань!");
        fetchTournament(true);
      } catch (err) {
        console.error(err);
        alert("Помилка при видаленні учасника");
      }
    }
  };

  const handleRegister = async () => {
    if (!id || !user) {
      toast.error("Будь ласка, увійдіть в систему, щоб зареєструватися");
      return;
    }

    let activePlayer = currentUserPlayer;
    if (!activePlayer) {
      try {
        const profile = await getProfile();
        activePlayer = profile;
        setCurrentUserPlayer(profile);
        if (!profile) throw new Error();
      } catch (e) {
        toast.error("Не вдалося отримати ваш спортивний профіль. Спробуйте пізніше або зверніться до адміністратора.");
        return;
      }
    }

    if (!window.confirm("Ви впевнені, що хочете зареєструватися на ці змагання?")) return;

    try {
      setIsRegistering(true);
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      const playerId = activePlayer!.id!;

      switch (sportKey) {
        case 'checkers': await registerForCheckersEvent(id, playerId); break;
        case 'armwrestling': await registerForArmwrestlingEvent(id, playerId); break;
        case 'badminton': await registerForBadmintonEvent(id, playerId); break;
        case 'tabletennis':
        case 'table_tennis': await registerForTableTennisEvent(id, playerId); break;
        case 'futsal': await registerForFutsalEvent(id, playerId); break;
        case 'volleyball': await registerForVolleyballEvent(id, playerId); break;
        case 'basketball3x3':
        case 'basketball_3x3': await registerForBasketball3x3Event(id, playerId); break;
        case 'beachvolleyball':
        case 'beach_volleyball': await registerForBeachVolleyballEvent(id, playerId); break;
        default: await registerForChessEvent(id, playerId);
      }

      toast.success("Ви успішно зареєструвалися!");
      fetchTournament(true);
    } catch (err: any) {
      console.error(err);
      toast.error(err.response?.data?.message || "Помилка при реєстрації");
    } finally {
      setIsRegistering(false);
    }
  };

  const handleApproveParticipant = async (playerId: number, playerName: string) => {
    try {
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      switch (sportKey) {
        case 'checkers': await approveCheckersParticipant(id!, playerId); break;
        case 'armwrestling': await approveArmwrestlingParticipant(id!, playerId); break;
        case 'badminton': await approveBadmintonParticipant(id!, playerId); break;
        case 'tabletennis':
        case 'table_tennis': await approveTableTennisParticipant(id!, playerId); break;
        default: await approveChessParticipant(id!, playerId);
      }
      toast.success("Учасника схвалено!");
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      toast.error("Помилка при схваленні");
    }
  };

  const handleRejectParticipant = async (playerId: number, playerName: string) => {
    if (!window.confirm(`Відхилити участь ${playerName}?`)) return;
    try {
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      switch (sportKey) {
        case 'checkers': await rejectCheckersParticipant(id!, playerId); break;
        case 'armwrestling': await rejectArmwrestlingParticipant(id!, playerId); break;
        case 'badminton': await rejectBadmintonParticipant(id!, playerId); break;
        case 'tabletennis':
        case 'table_tennis': await rejectTableTennisParticipant(id!, playerId); break;
        default: await rejectChessParticipant(id!, playerId);
      }
      toast.success("Учасника відхилено");
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      toast.error("Помилка при відхиленні");
    }
  };

  const fetchTournament = async (quiet = false) => {
    if (!id) return;
    if (!quiet) setIsLoading(true);
    try {
      if (user && !currentUserPlayer) {
        getProfile().then(setCurrentUserPlayer).catch(() => {});
      }
      const data = await getTournamentById(id);
      setTournament(data);

      const sportKey = data.sport.toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
      setTournament({ ...data, sport: sportKey }); // Sync normalized sport back to state

      if (['chess', 'checkers', 'armwrestling', 'volleyball', 'beach_volleyball', 'futsal', 'basketball_3x3', 'basketball3x3', 'badminton', 'table_tennis', 'tabletennis'].includes(sportKey)) {
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
            case 'table_tennis':
            case 'tabletennis': detailsPromise = getTableTennisEventById(id); break;
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

              // Initialize editTournamentData to prevent 400 errors when saving winners
              setEditTournamentData({
                name: data.title || d.name || '',
                date: formatDateForInput(d.date),
                location: data.location || d.location || '',
                refereeIds: d.refereeIds || [],
                level: d.level || 'CollegeGroup',
                description: data.description || d.description || '',
                timeControl: d.timeControl || '',
                participantsCount: d.participantsCount || 0,
                system: d.system || 'Swiss',
                rules: d.rules || '',
                prizes: d.prizes || '',
                headRefereeName: d.headRefereeName || '',
                referees: d.referees || '',
                schedule: d.schedule || '',
                isTeam: !!d.isTeam,
                registrationStart: formatDateForInput(data.registrationStart || d.registrationStart),
                registrationEnd: formatDateForInput(data.registrationEnd || d.registrationEnd),
                endDate: formatDateForInput(data.endDate || d.endDate),
                isModerated: !!d.isModerated,
                isPublic: d.isPublic !== undefined ? !!d.isPublic : true,
                seasonId: d.seasonId || null
              });
            }
        } catch (eventErr) {
          console.error('Failed to fetch detailed event info:', eventErr);
        }
      }
    } catch (err) {
      console.error('Failed to fetch tournament:', err);
      setError('Не вдалося завантажити інформацію про змагання.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchTournament();
    fetchSeasons();
  }, [id, isAdmin]);

  const fetchSeasons = async () => {
    try {
      const response = await api.get('/SeasonRating/seasons');
      setSeasons(response.data);
    } catch (error) {
      console.error('Failed to fetch seasons:', error);
    }
  };

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
              patronymic: p.patronymic || '',
              lastName: p.lastName,
              group: p.group,
              studentId: p.studentId || '',
              birthDate: p.birthDate || '',
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
        patronymic: result.patronymic || '',
        lastName: result.lastName,
        group: result.group || '',
        studentId: result.studentId || '',
        birthDate: result.birthDate || '',
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
          alert("Цей користувач вже бере участь у змаганнях!");
          return;
        }

        const playerPayload: PlayerDto = {
          firstName: newPlayer.firstName,
          patronymic: newPlayer.patronymic || null,
          lastName: newPlayer.lastName,
          birthDate: newPlayer.birthDate || null,
          group: newPlayer.group,
          studentId: newPlayer.studentId || null,
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
        const isAlreadyParticipant = (details?.players || details?.participants || []).some((p: any) => p.playerId === playerId);
        if (isAlreadyParticipant) {
          alert("Цей гравець вже бере участь у змаганнях!");
          return;
        }
      }

      // Create participant binding
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';

      switch (sportKey) {
        case 'checkers':
          await addParticipantToCheckersEvent(id!, { playerId: playerId, points: 0, rank: 0, status: RegistrationStatus.Approved });
          break;
        case 'armwrestling':
          await addParticipantToArmwrestlingEvent(id!, { playerId: playerId, weight: newPlayer.weight || 0, rank: 0, status: RegistrationStatus.Approved });
          break;
        case 'badminton':
          await addParticipantToBadmintonEvent(id!, { playerId: playerId, points: 0, rank: 0, status: RegistrationStatus.Approved });
          break;
        case 'tabletennis':
        case 'table_tennis':
          await addParticipantToTableTennisEvent(id!, { playerId: playerId, points: 0, rank: 0, status: RegistrationStatus.Approved });
          break;
        default:
          await addParticipantToEvent(id!, { playerId: playerId, points: 0, rank: 0, status: RegistrationStatus.Approved });
      }
      toast.success("Учасник успішно доданий до змагань!");
      setNewPlayer({ firstName: '', patronymic: '', lastName: '', group: '', studentId: '', birthDate: '', city: 'Чернівці', ratingELO: 1500, fideId: '', title: '', userId: null, existingPlayerId: null, weight: 0 });
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
          board: Number(newGame.board) || 1,
          pgn: newGame.pgn
        };
        await createChessGame(payload);
      }
      toast.success("Матч успішно доданий!");
      setNewGame({ ...newGame, result: 'WhiteWin', board: newGame.board + 1, pgn: '' });
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
        <p>Завантаження інформації про змагання...</p>
      </div>
    );
  }

  if (error || !tournament) {
    return (
      <div className={styles.errorContainer}>
        <h2>Помилка</h2>
        <p>{error || 'Змагання не знайдено.'}</p>
        <button className="btn btn-primary" onClick={() => navigate('/tournaments')}>
          Повернутися до списку
        </button>
      </div>
    );
  }

  const sortedPlayers = (() => {
    const rawParticipants = details?.participants || details?.players || [];
    const filtered = isAdmin 
      ? rawParticipants 
      : rawParticipants.filter((p: any) => p.status === RegistrationStatus.Approved || !p.status);

    if (tournament?.sport === 'chess' || tournament?.sport === 'checkers') {
      return [...filtered].sort((a: any, b: any) => {
        if ((b.points || 0) !== (a.points || 0)) return (b.points || 0) - (a.points || 0);
        if (a.rank || b.rank) return (a.rank || 999) - (b.rank || 999);
        return 0;
      });
    }
    return filtered;
  })();

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
  const isTeamSport = !!(details as any)?.isTeam || ['futsal', 'volleyball', 'beach_volleyball', 'basketball3x3', 'basketball_3x3'].includes(tournament?.sport || '');

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

  const now = new Date();
  const regStart = tournament?.registrationStart ? new Date(tournament.registrationStart) : null;
  const regEnd = tournament?.registrationEnd ? new Date(tournament.registrationEnd) : null;
  const isRegistrationOpen = (!regStart || now >= regStart) && (!regEnd || now <= regEnd);
  const isFinished = tournament?.status === 'completed' || !!details?.firstPlace || !!details?.firstPlaceTeam || (tournament?.endDate && new Date(tournament.endDate) < now) || (regEnd && now > regEnd);
  
  const userRegistration = details?.participants?.find((p: any) => p.userId === user?.id);
  const isUserRegistered = !!userRegistration;

  return (
    <div className={styles.tournamentViewPage}>
      {viewingPgnGame && (
        <ChessGameViewerModal
          pgn={viewingPgnGame.pgn || ''}
          whitePlayer={viewingPgnGame.whitePlayerName || (tournament?.sport === 'chess' ? 'Білі' : 'Гравець 1')}
          blackPlayer={viewingPgnGame.blackPlayerName || (tournament?.sport === 'chess' ? 'Чорні' : 'Гравець 2')}
          onClose={() => setViewingPgnGame(null)}
        />
      )}
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
                <i className="fas fa-edit"></i> Редагувати змагання
              </button>
            )}
            {isAdmin && (
              <button onClick={handleDeleteTournament} style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '10px 20px', backgroundColor: '#ef4444', color: '#fff', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold' }}>
                <i className="fas fa-trash"></i> Видалити
              </button>
            )}
          </div>
        </div>

        <>
          {isAdmin && showWinnersAdmin && (
            <div className={styles.adminSection} style={{ background: '#f0fdf4', border: '1px solid #bbf7d0', padding: '24px', borderRadius: '16px', marginBottom: '24px' }}>
              <h3 style={{ marginTop: 0, marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '10px', color: '#166534' }}>
                <i className="fas fa-trophy"></i> Призначити призерів змагань
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
                    {!isTeamSport && sortedPlayers?.map((p: any) => (
                      <option key={`first-p-${p.playerId}`} value={p.playerId}>
                        {p.playerName} { (['chess', 'checkers', 'tabletennis', 'badminton'].includes(tournament?.sport)) && (p.points !== undefined) ? `(${p.points} очок)` : ''}
                      </option>
                    ))}
                    {isTeamSport && details?.teams?.map((t: any) => (
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
                    {!isTeamSport && sortedPlayers?.map((p: any) => (
                      <option key={`second-p-${p.playerId}`} value={p.playerId}>
                        {p.playerName} { (['chess', 'checkers', 'tabletennis', 'badminton'].includes(tournament?.sport)) && (p.points !== undefined) ? `(${p.points} очок)` : ''}
                      </option>
                    ))}
                    {isTeamSport && details?.teams?.map((t: any) => (
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
                    {!isTeamSport && sortedPlayers?.map((p: any) => (
                      <option key={`third-p-${p.playerId}`} value={p.playerId}>
                        {p.playerName} { (['chess', 'checkers', 'tabletennis', 'badminton'].includes(tournament?.sport)) && (p.points !== undefined) ? `(${p.points} очок)` : ''}
                      </option>
                    ))}
                    {isTeamSport && details?.teams?.map((t: any) => (
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
            <div className={styles.headerInfo}>
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
              <div className={styles.headerActions}>
                {user ? (
                  isUserRegistered ? (
                    <div className={`${styles.registrationStatus} ${styles[userRegistration.status?.toLowerCase()]}`}>
                      <i className={
                        userRegistration.status === RegistrationStatus.Approved ? "fas fa-check-circle" :
                        userRegistration.status === RegistrationStatus.Pending ? "fas fa-clock" :
                        "fas fa-times-circle"
                      }></i>
                      <span>
                        {userRegistration.status === RegistrationStatus.Approved ? "Ви зареєстровані" :
                         userRegistration.status === RegistrationStatus.Pending ? "Очікує підтвердження" :
                         "Заявку відхилено"}
                      </span>
                    </div>
                  ) : (
                    isRegistrationOpen && !isAdmin && details?.level === 'CollegeGroup' ? (
                      <button 
                        className={styles.registerButton} 
                        onClick={handleRegister}
                        disabled={isRegistering}
                      >
                        {isRegistering ? (
                          <><i className="fas fa-spinner fa-spin"></i> Реєстрація...</>
                        ) : (
                          <><i className="fas fa-user-plus"></i> Зареєструватися</>
                        )}
                      </button>
                    ) : null
                  )
                ) : (
                  details?.level === 'CollegeGroup' && !isFinished && (
                    <Link to="/login" className={styles.registerButton}>
                      <i className="fas fa-sign-in-alt"></i> Увійдіть, щоб зареєструватися
                    </Link>
                  )
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
                            tournament?.sport === 'badminton' ? "fas fa-shuttlecock" :
                              tournament?.sport === 'table_tennis' || tournament?.sport === 'table-tennis' || tournament?.sport === 'tabletennis' ? "fas fa-table-tennis" :
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
            <h2><i className="fas fa-info-circle"></i> Про змагання</h2>
            <div className={styles.infoGrid}>
              <div className={styles.infoSection}>
                <h3><i className="fas fa-align-left"></i> Опис</h3>
                <p>{tournament?.description}</p>
              </div>

              <div className={styles.infoSection}>
                <h3><i className="fas fa-clock"></i> {details?.level === 'CollegeGroup' ? 'Розклад та Реєстрація' : 'Розклад'}</h3>
                <div className={styles.refereeList}>
                  {details?.level === 'CollegeGroup' && (
                    <>
                      <div className={styles.refereeItem}>
                        <div className={styles.refereeAvatar}><i className="fas fa-calendar-plus"></i></div>
                        <div>
                          <strong>Початок реєстрації:</strong><br />
                          <small>{tournament?.registrationStart ? new Date(tournament.registrationStart).toLocaleString('uk-UA') : 'Не вказано'}</small>
                        </div>
                      </div>
                      <div className={styles.refereeItem}>
                        <div className={styles.refereeAvatar}><i className="fas fa-calendar-times"></i></div>
                        <div>
                          <strong>Кінець реєстрації:</strong><br />
                          <small>{tournament?.registrationEnd ? new Date(tournament.registrationEnd).toLocaleString('uk-UA') : 'Не вказано'}</small>
                        </div>
                      </div>
                    </>
                  )}
                  <div className={styles.refereeItem}>
                    <div className={styles.refereeAvatar}><i className="fas fa-play"></i></div>
                    <div>
                      <strong>Початок змагань:</strong><br />
                      <small>{details?.date ? new Date(details.date).toLocaleString('uk-UA', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit'
                      }) : (tournament?.date + ' ' + (tournament?.time || ''))}</small>
                    </div>
                  </div>
                  {details?.level === 'CollegeGroup' && (
                    <div className={styles.refereeItem}>
                      <div className={styles.refereeAvatar}>
                        <i className={details?.isPublic ? "fas fa-lock-open" : "fas fa-user-check"}></i>
                      </div>
                      <div>
                        <strong>Тип реєстрації:</strong><br />
                        <span style={{ 
                          fontSize: '13px', 
                          fontWeight: 'bold', 
                          color: details?.isPublic ? '#10b981' : '#f59e0b',
                          background: details?.isPublic ? '#ecfdf5' : '#fffbeb',
                          padding: '2px 8px',
                          borderRadius: '4px',
                          border: details?.isPublic ? '1px solid #bdf4e4' : '1px solid #fef3c7',
                          display: 'inline-block',
                          marginTop: '4px'
                        }}>
                          {details?.isPublic ? (
                            <><i className="fas fa-globe"></i> Відкрите (миттєве підтвердження)</>
                          ) : (
                            <><i className="fas fa-shield-alt"></i> З модерацією заявок</>
                          )}
                        </span>
                      </div>
                    </div>
                  )}
                  <div className={styles.refereeItem}>
                    <div className={styles.refereeAvatar}><i className="fas fa-flag-checkered"></i></div>
                    <div>
                      <strong>Кінець змагань:</strong><br />
                      <small>{tournament?.endDate ? new Date(tournament.endDate).toLocaleString('uk-UA') : 'Не вказано'}</small>
                    </div>
                  </div>
                </div>
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
                      {tournament?.sport === 'armwrestling' ? (
                        <>
                          • Заборонено відрив ліктя від підлокітника<br />
                          • Заборонено торкання головою чи плечем захвату<br />
                          • Перемога при торканні руки суперника валику<br />
                          • Рішення судді є остаточними
                        </>
                      ) : (
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
                        <small>Суддя змагань</small>
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

          <div className={styles.statsGrid}>
            {isTeamSport ? (
              <>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}><i className="fas fa-users-viewfinder"></i></div>
                  <div className={styles.statValue}>{actualParticipants}</div>
                  <div className={styles.statLabel}>Всього команд</div>
                </div>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}><i className="fas fa-user-group"></i></div>
                  <div className={styles.statValue}>{details?.teams?.reduce((acc: number, t: any) => acc + (t.players?.length || 0), 0) || 0}</div>
                  <div className={styles.statLabel}>Всього гравців</div>
                </div>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}><i className="fas fa-calendar-check"></i></div>
                  <div className={styles.statValue}>{allGamesOrMatches.length}</div>
                  <div className={styles.statLabel}>Всього матчів</div>
                </div>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}><i className="fas fa-chart-line"></i></div>
                  <div className={styles.statValue}>{playedGames}</div>
                  <div className={styles.statLabel}>Зіграно матчів</div>
                </div>
              </>
            ) : (
              <>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}>
                    <i className={
                      tournament?.sport === 'badminton' || 
                      tournament?.sport === 'table_tennis' || 
                      tournament?.sport === 'tabletennis' || 
                      tournament?.sport === 'armwrestling'
                        ? "fas fa-project-diagram" 
                        : "fas fa-chess-board"
                    }></i>
                  </div>
                  <div className={styles.statValue}>{totalRounds || '-'}</div>
                  <div className={styles.statLabel}>Всього турів</div>
                </div>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}><i className="fas fa-users"></i></div>
                  <div className={styles.statValue}>{actualParticipants}</div>
                  <div className={styles.statLabel}>{participantLabel}</div>
                </div>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}>
                    <i className={
                      tournament?.sport === 'badminton' ? "fas fa-shuttlecock" : 
                      tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? "fas fa-table-tennis" : 
                      tournament?.sport === 'armwrestling' ? "fas fa-hand-rock" : 
                      "fas fa-venus-mars"
                    }></i>
                  </div>
                  <div className={styles.statValue}>
                    {((details as any)?.category === 'Men' ? 'Чоловіки' : 
                      (details as any)?.category === 'Women' ? 'Жінки' : 
                      (details as any)?.category === 'Mixed' ? 'Змішане' : 
                      (tournament?.category === 'Чоловіки' ? 'Чоловіки' : 
                       tournament?.category === 'Жінки' ? 'Жінки' : 'Змішане'))}
                  </div>
                  <div className={styles.statLabel}>Категорія</div>
                </div>
                <div className={styles.statCard}>
                  <div className={styles.statIcon}><i className="fas fa-chart-line"></i></div>
                  <div className={styles.statValue}>{playedGames}</div>
                  <div className={styles.statLabel}>Зіграно {tournament?.sport === 'armwrestling' || tournament?.sport === 'badminton' || tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? 'матчів' : 'партій'}</div>
                </div>
              </>
            )}
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

                  <div className={styles.tabsContainer}>
                    <div className={styles.tabsHeader}>
                      <button
                        className={`${styles.tabButton} ${activeTab === 'standings' ? styles.active : ''}`}
                        onClick={() => setActiveTab('standings')}
                      >
                        <i className="fas fa-chart-simple"></i> Таблиця змагань
                      </button>
                      <button
                        className={`${styles.tabButton} ${activeTab === 'games' ? styles.active : ''}`}
                        onClick={() => setActiveTab('games')}
                      >
                        <i className={tournament?.sport === 'badminton' ? "fas fa-table-tennis-paddle-ball" : tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? "fas fa-table-tennis" : "fas fa-chess-board"}></i> {tournament?.sport === 'badminton' || tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? 'Всі матчі' : 'Всі партії'}
                      </button>
                      <button
                        className={`${styles.tabButton} ${activeTab === 'participants' ? styles.active : ''}`}
                        onClick={() => setActiveTab('participants')}
                      >
                        <i className="fas fa-users"></i> Учасники {actualParticipants > 0 && `(${actualParticipants})`}
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
                                <th>Група</th>
                                {tournament?.sport?.toLowerCase() === 'chess' ? (
                                  <>
                                    <th>Рейтинг</th>
                                    <th>Очки</th>
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
                                  {editingParticipantId === p.playerId ? (
                                    <>
                                      <td>
                                        <input type="number" value={editingParticipantData.rank} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, rank: parseInt(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                      </td>
                                      <td>
                                        {(tournament?.sport?.toLowerCase() === 'chess' || tournament?.sport?.toLowerCase() === 'checkers') ? (
                                          <div style={{ display: 'flex', gap: '4px' }}>
                                            <input 
                                              type="text" 
                                              placeholder="Прізвище"
                                              value={editingParticipantData.lastName} 
                                              onChange={(e) => setEditingParticipantData({ ...editingParticipantData, lastName: e.target.value })} 
                                              style={{ width: '100px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} 
                                            />
                                            <input 
                                              type="text" 
                                              placeholder="Ім'я"
                                              value={editingParticipantData.firstName} 
                                              onChange={(e) => setEditingParticipantData({ ...editingParticipantData, firstName: e.target.value })} 
                                              style={{ width: '80px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} 
                                            />
                                          </div>
                                        ) : (
                                          p.userId ? (
                                            <Link to={`/profile/${p.userId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                              {p.playerName}
                                            </Link>
                                          ) : (
                                            <span style={{ fontWeight: 'bold' }}>{p.playerName}</span>
                                          )
                                        )}
                                      </td>
                                      <td>
                                        {(tournament?.sport?.toLowerCase() === 'chess' || tournament?.sport?.toLowerCase() === 'checkers') ? (
                                          <input 
                                            type="text" 
                                            value={editingParticipantData.groupName} 
                                            onChange={(e) => setEditingParticipantData({ ...editingParticipantData, groupName: e.target.value })} 
                                            style={{ width: '80px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} 
                                          />
                                        ) : (
                                          p.groupName || '—'
                                        )}
                                      </td>
                                      {tournament?.sport?.toLowerCase() === 'chess' ? (
                                        <>
                                          <td>
                                            <input type="number" step="1" value={editingParticipantData.rating || 0} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, rating: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                          </td>
                                          <td>
                                            <input type="number" step="0.5" value={editingParticipantData.points} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, points: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                          </td>
                                        </>
                                      ) : (
                                        <>
                                          {tournament?.sport?.toLowerCase() === 'armwrestling' ? (
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
                                        {p.playerId ? (
                                          <Link to={`/players/${p.playerId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                            {p.playerName}
                                          </Link>
                                        ) : (
                                          <strong>{p.playerName}</strong>
                                        )}
                                        {p.status && p.status !== RegistrationStatus.Approved && (
                                          <span className={`${styles.statusBadge} ${styles[p.status.toLowerCase()]}`} style={{ marginLeft: '8px', fontSize: '10px', padding: '2px 6px', borderRadius: '4px', textTransform: 'uppercase', fontWeight: 'bold' }}>
                                            {p.status === RegistrationStatus.Pending ? 'Очікує' : 'Відхилено'}
                                          </span>
                                        )}
                                      </td>
                                      <td>{p.groupName || '—'}</td>
                                      {tournament?.sport?.toLowerCase() === 'chess' ? (
                                        <>
                                          <td>{p.rating}</td>
                                          <td><strong>{p.points}</strong></td>
                                        </>
                                      ) : (
                                        <td><strong>{tournament?.sport?.toLowerCase() === 'armwrestling' ? (p.weight + ' кг') : p.points}</strong></td>
                                      )}
                                      {isAdmin && <td>
                                        {p.status === RegistrationStatus.Pending && (
                                          <>
                                            <button type="button" onClick={() => handleApproveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }} title="Схвалити"><i className="fas fa-check"></i></button>
                                            <button type="button" onClick={() => handleRejectParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#f59e0b', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }} title="Відхилити"><i className="fas fa-user-slash"></i></button>
                                          </>
                                        )}
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
                            <h3 style={{ marginTop: 0, marginBottom: '16px', color: '#111' }}>
                              [Admin] Додати {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'матч' : 'партію'} (результат)
                            </h3>
                            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '16px', alignItems: 'center' }}>
                              <input type="number" min="1" placeholder="Тур" title="Номер туру" value={newGame.round} onChange={(e) => setNewGame({ ...newGame, round: parseInt(e.target.value) || 1 })} style={{ width: '60px', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                              <input type="number" min="1" placeholder="Дошка" title="Номер дошки" value={newGame.board} onChange={(e) => setNewGame({ ...newGame, board: parseInt(e.target.value) || 1 })} style={{ width: '60px', padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />

                              <select value={newGame.whitePlayerId} onChange={(e) => setNewGame({ ...newGame, whitePlayerId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                                <option value="">-- {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравець 1' : 'Білі'} --</option>
                                {sortedPlayers.map((p: any) => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                              </select>

                              <strong style={{ margin: '0 8px' }}> - </strong>

                              <select value={newGame.blackPlayerId} onChange={(e) => setNewGame({ ...newGame, blackPlayerId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                                <option value="">-- {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравець 2' : 'Чорні'} --</option>
                                {sortedPlayers.map((p: any) => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                              </select>

                              <select value={newGame.result} onChange={(e) => setNewGame({ ...newGame, result: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}>
                                <option value="WhiteWin">Перемога {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравця 1' : 'Білих'} (1-0)</option>
                                <option value="BlackWin">Перемога {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравця 2' : 'Чорних'} (0-1)</option>
                                <option value="Draw">Нічия (½-½)</option>
                              </select>

                              <button type="button" onClick={handleAddGame} style={{ padding: '8px 16px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}>Зберегти</button>
                            </div>

                            {tournament?.sport === 'chess' && (
                              <div style={{ marginTop: '16px' }}>
                                <label style={{ display: 'block', fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>PGN (опціонально)</label>
                                <textarea
                                  placeholder="Вставте PGN партії сюди..."
                                  value={newGame.pgn}
                                  onChange={(e) => setNewGame({ ...newGame, pgn: e.target.value })}
                                  style={{ width: '100%', padding: '10px', borderRadius: '6px', border: '1px solid #ccc', minHeight: '80px', fontFamily: 'monospace' }}
                                />
                              </div>
                            )}
                          </div>
                        )}

                        <div style={{ display: 'flex', gap: '16px', marginBottom: '16px', flexWrap: 'wrap', padding: '16px', background: '#f9fafb', borderRadius: '12px' }}>
                          <div style={{ display: 'flex', flexDirection: 'column' }}>
                            <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>Тур</label>
                            <input type="number" placeholder="Усі" value={gameFilters.round} onChange={(e) => setGameFilters({ ...gameFilters, round: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc', width: '80px' }} />
                          </div>
                           <div style={{ display: 'flex', flexDirection: 'column' }}>
                             <label style={{ fontSize: '12px', marginBottom: '4px', fontWeight: 'bold', color: '#666' }}>
                               {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? 'Стіл' : tournament?.sport === 'badminton' ? 'Корт' : 'Дошка'}
                             </label>
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
                              <option value="WhiteWin">Перемога {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравця 1' : 'Білих'} (1-0)</option>
                              <option value="BlackWin">Перемога {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравця 2' : 'Чорних'} (0-1)</option>
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
                                {tournament?.sport !== 'checkers' && <th>{tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? 'Стіл' : tournament?.sport === 'badminton' ? 'Корт' : 'Дошка'}</th>}
                                <th>{tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравець 1' : 'Білі'}</th>
                                <th>{tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравець 2' : 'Чорні'}</th>
                                <th>Результат</th>
                                {(isAdmin || tournament?.sport === 'chess') && <th>Дії</th>}
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
                                            <option value="">-- {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравець 1' : 'Білі'} --</option>
                                            {sortedPlayers.map((p: any) => <option key={`w${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                                          </select>
                                        </td>
                                        <td>
                                          <select value={editingGameData.blackPlayerId} onChange={(e) => setEditingGameData({ ...editingGameData, blackPlayerId: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                            <option value="">-- {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравець 2' : 'Чорні'} --</option>
                                            {sortedPlayers.map((p: any) => <option key={`b${p.playerId}`} value={p.playerId}>{p.playerName}</option>)}
                                          </select>
                                        </td>
                                        <td>
                                          <select value={editingGameData.result} onChange={(e) => setEditingGameData({ ...editingGameData, result: e.target.value })} style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                            <option value="WhiteWin">Перемога {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравця 1' : 'Білих'} (1-0)</option>
                                            <option value="BlackWin">Перемога {tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'Гравця 2' : 'Чорних'} (0-1)</option>
                                            <option value="Draw">Нічия (½-½)</option>
                                          </select>
                                        </td>
                                        {tournament?.sport === 'chess' && (
                                          <td>
                                            <textarea
                                              placeholder="PGN..."
                                              value={editingGameData.pgn}
                                              onChange={(e) => setEditingGameData({ ...editingGameData, pgn: e.target.value })}
                                              style={{ width: '100%', padding: '4px', borderRadius: '4px', border: '1px solid #ccc', minHeight: '60px', fontFamily: 'monospace' }}
                                            />
                                          </td>
                                        )}
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
                                              <i className={tournament?.sport === 'chess' || tournament?.sport === 'checkers' ? "fas fa-chess-king" : "fas fa-trophy"}></i> 1-0
                                            </span>
                                          ) : game.result === 'BlackWin' ? (
                                            <span className={`${styles.resultBadge} ${styles.resultBlackWin}`}>
                                              <i className={tournament?.sport === 'chess' || tournament?.sport === 'checkers' ? "fas fa-chess-queen" : "fas fa-trophy"}></i> 0-1
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
                                        <td>
                                          {isAdmin && (
                                            <button onClick={() => handleEditClick(game)} style={{ padding: '4px 8px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
                                              <i className="fas fa-edit"></i>
                                            </button>
                                          )}
                                          {game.pgn && (
                                            <button onClick={() => setViewingPgnGame(game)} style={{
                                              padding: isAdmin ? '4px 8px' : '6px 16px',
                                              background: '#10b981',
                                              color: '#fff',
                                              border: 'none',
                                              borderRadius: '6px',
                                              cursor: 'pointer',
                                              marginLeft: isAdmin ? '4px' : '0',
                                              fontSize: isAdmin ? '13px' : '14px',
                                              display: 'inline-flex',
                                              alignItems: 'center',
                                              gap: '6px'
                                            }} title={`Переглянути ${tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' || tournament?.sport === 'badminton' ? 'матч' : 'партію'}`}>

                                              <i className="fas fa-eye"></i> {!isAdmin && 'Дивитись'}
                                            </button>
                                          )}
                                        </td>
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
                          <>
                            {(tournament?.sport === 'chess' || tournament?.sport === 'checkers') && (
                              <div style={{ background: '#ebf5ff', padding: '24px', borderRadius: '12px', marginBottom: '16px', border: '1px solid #bfdbfe' }}>
                                <h3 style={{ marginTop: 0, marginBottom: '8px', color: '#1e40af', display: 'flex', alignItems: 'center', gap: '8px' }}>
                                  <i className="fas fa-file-excel"></i> Завантажити Excel з Chess-Results.com
                                </h3>
                                <p style={{ fontSize: '14px', color: '#1e3a8a', marginBottom: '16px' }}>
                                  Виберіть Excel-файл (.xlsx або .xls), завантажений з сайту chess-results.com (меню "Excel and Print"), щоб автоматично заповнити список учасників та їх результати.
                                </p>
                                <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                                  <input
                                    type="file"
                                    ref={fileInputRef}
                                    accept=".xlsx, .xls"
                                    onChange={handleImportData}
                                    style={{ display: 'none' }}
                                    id="excel-import-input"
                                  />
                                  <label
                                    htmlFor="excel-import-input"
                                    style={{
                                      padding: '10px 20px',
                                      background: isImporting ? '#94a3b8' : '#3b82f6',
                                      color: '#fff',
                                      border: 'none',
                                      borderRadius: '6px',
                                      cursor: isImporting ? 'not-allowed' : 'pointer',
                                      display: 'flex',
                                      alignItems: 'center',
                                      gap: '8px',
                                      fontWeight: 'bold'
                                    }}
                                  >
                                    {isImporting ? (
                                      <><i className="fas fa-spinner fa-spin"></i> Обробка...</>
                                    ) : (
                                      <><i className="fas fa-upload"></i> Вибрати та завантажити файл</>
                                    )}
                                  </label>
                                  {isImporting && <span style={{ color: '#1e40af', fontSize: '14px' }}>Це може зайняти кілька секунд...</span>}
                                </div>
                              </div>
                            )}

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
                                        <strong>{result.firstName} {result.patronymic} {result.lastName}</strong>
                                        {result.email && <span style={{ color: '#666', marginLeft: '8px' }}>({result.email})</span>}
                                        {result.group && <span style={{ color: '#666', marginLeft: '8px' }}>[{result.group}]</span>}
                                        {result.studentId && <span style={{ color: '#666', marginLeft: '8px' }}>[ID: {result.studentId}]</span>}
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
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>По батькові</label>
                                  <input type="text" placeholder="По батькові (опціонально)" value={newPlayer.patronymic} onChange={(e) => setNewPlayer({ ...newPlayer, patronymic: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Прізвище</label>
                                  <input type="text" placeholder="Прізвище" value={newPlayer.lastName} onChange={(e) => setNewPlayer({ ...newPlayer, lastName: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Дата народження</label>
                                  <input type="date" value={newPlayer.birthDate} onChange={(e) => setNewPlayer({ ...newPlayer, birthDate: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Група (напр. 211)</label>
                                  <input type="text" placeholder="Група (напр. 211)" value={newPlayer.group} onChange={(e) => setNewPlayer({ ...newPlayer, group: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
                                </div>
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Студ. квиток</label>
                                  <input type="text" placeholder="Студ. квиток (унікальний)" value={newPlayer.studentId} onChange={(e) => setNewPlayer({ ...newPlayer, studentId: e.target.value })} style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }} />
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
                          </>
                        )}

                        <div className={styles.tableWrapper}>
                          <table className={styles.dataTable}>
                            <thead>
                              <tr>
                                <th>№</th>
                                <th>Учасник</th>
                                <th>Група</th>
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
                                    {p.playerId ? (
                                      <Link to={`/players/${p.playerId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                        {p.playerName}
                                      </Link>
                                    ) : (
                                      p.playerName
                                    )}
                                  </td>
                                  <td style={{ textAlign: 'center' }}>
                                    {p.groupName ? (
                                      <span className={styles.systemBadge}>{p.groupName}</span>
                                    ) : (
                                      '—'
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
                                <p>Змагання проводяться згідно із розкладом, вказаним організаторами під час реєстрації або під час відкриття.</p>
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
                <h2>Галерея змагань</h2>
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
                    Редагувати змагання
                  </h2>
                  <button onClick={() => setIsEditingTournament(false)} className={styles.closeButton}>&times;</button>
                </div>
                <div className={styles.modalBody}>
                  <div className={styles.editFormGrid}>
                    <div className={styles.formGroup}>
                      <label>Назва змагань</label>
                      <input type="text" value={editTournamentData.name} onChange={(e) => setEditTournamentData({ ...editTournamentData, name: e.target.value })} />
                    </div>
                    <div className={styles.formGroup}>
                      <label>Дата проведення</label>
                      <input type="datetime-local" value={editTournamentData.date.includes('T') ? editTournamentData.date.substring(0, 16) : editTournamentData.date} onChange={(e) => setEditTournamentData({ ...editTournamentData, date: e.target.value })} />
                    </div>
                    {editTournamentData.level === 'CollegeGroup' && (
                      <>
                        <div className={styles.formGroup}>
                          <label>Початок реєстрації</label>
                          <input type="datetime-local" value={editTournamentData.registrationStart?.includes('T') ? editTournamentData.registrationStart.substring(0, 16) : editTournamentData.registrationStart} onChange={(e) => setEditTournamentData({ ...editTournamentData, registrationStart: e.target.value })} />
                        </div>
                        <div className={styles.formGroup}>
                          <label>Кінець реєстрації</label>
                          <input type="datetime-local" value={editTournamentData.registrationEnd?.includes('T') ? editTournamentData.registrationEnd.substring(0, 16) : editTournamentData.registrationEnd} onChange={(e) => setEditTournamentData({ ...editTournamentData, registrationEnd: e.target.value })} />
                        </div>
                      </>
                    )}
                    <div className={styles.formGroup}>
                      <label>Кінець змагань</label>
                      <input type="datetime-local" value={editTournamentData.endDate?.includes('T') ? editTournamentData.endDate.substring(0, 16) : editTournamentData.endDate} onChange={(e) => setEditTournamentData({ ...editTournamentData, endDate: e.target.value })} />
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
                    <div className={styles.formGroup}>
                      <label>Сезон рейтингу</label>
                      <select value={editTournamentData.seasonId || ''} onChange={(e) => setEditTournamentData({ ...editTournamentData, seasonId: Number(e.target.value) || null })}>
                        <option value="">-- Оберіть сезон --</option>
                        {seasons.map(s => (
                          <option key={s.id} value={s.id}>Сезон {s.name} {s.isCurrent ? '(Поточний)' : ''}</option>
                        ))}
                      </select>
                    </div>
                    <div className={styles.formGroup}>
                      <label>Категорія (стать)</label>
                      <select value={editTournamentData.category} onChange={(e) => setEditTournamentData({ ...editTournamentData, category: e.target.value as any })}>
                        <option value="Men">Чоловіки</option>
                        <option value="Women">Жінки</option>
                        <option value="Mixed">Змішане (Mixed)</option>
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
                    {editTournamentData.level === 'CollegeGroup' && (
                      <div className={styles.formGroup} style={{ gridColumn: '1 / -1', display: 'flex', gap: '20px' }}>
                        <label style={{ flex: 1, display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer', background: '#f8fafc', padding: '12px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                          <input
                            type="checkbox"
                            checked={editTournamentData.isTeam}
                            onChange={(e) => setEditTournamentData({ ...editTournamentData, isTeam: e.target.checked })}
                            style={{ width: '20px', height: '20px' }}
                          />
                          <div>
                            <strong style={{ display: 'block' }}>Командні змагання</strong>
                            <small style={{ color: '#64748b' }}>Матчі проводяться між командами (групами)</small>
                          </div>
                        </label>

                        <label style={{ flex: 1, display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer', background: '#f0f9ff', padding: '12px', borderRadius: '8px', border: '1px solid #bae6fd' }}>
                          <input
                            type="checkbox"
                            checked={editTournamentData.isPublic}
                            onChange={(e) => setEditTournamentData({ 
                              ...editTournamentData, 
                              isPublic: e.target.checked,
                              isModerated: e.target.checked ? false : editTournamentData.isModerated
                            })}
                            style={{ width: '20px', height: '20px' }}
                          />
                          <div>
                            <strong style={{ display: 'block' }}>Відкрите змагання</strong>
                            <small style={{ color: '#64748b' }}>Учасники додаються автоматично після реєстрації</small>
                          </div>
                        </label>

                        <label style={{ flex: 1, display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer', background: '#fff7ed', padding: '12px', borderRadius: '8px', border: '1px solid #fed7aa' }}>
                          <input
                            type="checkbox"
                            checked={editTournamentData.isModerated}
                            onChange={(e) => setEditTournamentData({ 
                              ...editTournamentData, 
                              isModerated: e.target.checked,
                              isPublic: e.target.checked ? false : editTournamentData.isPublic
                            })}
                            style={{ width: '20px', height: '20px' }}
                          />
                          <div>
                            <strong style={{ display: 'block' }}>Модерація заявок</strong>
                            <small style={{ color: '#64748b' }}>Адміністратор має схвалити або відхилити кожну заявку</small>
                          </div>
                        </label>
                      </div>
                    )}
                    {editTournamentData.level !== 'CollegeGroup' && (
                       <div className={styles.formGroup} style={{ gridColumn: '1 / -1', display: 'flex', gap: '20px' }}>
                        <label style={{ flex: 1, display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer', background: '#f8fafc', padding: '12px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                          <input
                            type="checkbox"
                            checked={editTournamentData.isTeam}
                            onChange={(e) => setEditTournamentData({ ...editTournamentData, isTeam: e.target.checked })}
                            style={{ width: '20px', height: '20px' }}
                          />
                          <div>
                            <strong style={{ display: 'block' }}>Командні змагання</strong>
                            <small style={{ color: '#64748b' }}>Матчі проводяться між командами (групами)</small>
                          </div>
                        </label>
                       </div>
                    )}
                    <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                      <label>Судді</label>
                      <textarea rows={3} placeholder="Введіть імена суддів..." value={editTournamentData.referees} onChange={(e) => setEditTournamentData({ ...editTournamentData, referees: e.target.value })} />
                    </div>
                    <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                      <label>Опис змагань</label>
                      <textarea rows={3} value={editTournamentData.description} onChange={(e) => setEditTournamentData({ ...editTournamentData, description: e.target.value })} />
                    </div>
                    <div className={styles.formGroup}>
                      <label>Правила</label>
                      <textarea rows={5} placeholder="Введіть правила змагань..." value={editTournamentData.rules} onChange={(e) => setEditTournamentData({ ...editTournamentData, rules: e.target.value })} />
                    </div>
                    <div className={styles.formGroup}>
                      <label>Призи</label>
                      <textarea rows={5} placeholder="Опишіть призовий фонд..." value={editTournamentData.prizes} onChange={(e) => setEditTournamentData({ ...editTournamentData, prizes: e.target.value })} />
                    </div>
                    <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                      <label>Розклад змагань</label>
                      <textarea rows={5} placeholder="Введіть детальний розклад (кожен пункт з нового рядка)..." value={editTournamentData.schedule} onChange={(e) => setEditTournamentData({ ...editTournamentData, schedule: e.target.value })} />
                    </div>

                    <div className={styles.photoManagementSection} style={{ gridColumn: '1 / -1' }}>
                      <label><i className="fas fa-images"></i> Фотографії змагань</label>

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
