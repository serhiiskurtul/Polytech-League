import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { useAuth } from '@/entities/session';
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
  uploadEventMedia,
  deleteEventMedia,
  setMainPhoto,
  submitUserEventMedia,
  getApprovedUserEventMedia,
  getPendingUserEventMedia,
  updateUserEventMediaStatus,
  deleteUserEventMedia,
  searchUsers,
  searchPlayers,
  createPlayer,
  registerForChessEvent,
  approveChessParticipant,
  rejectChessParticipant,
  importDataFromFile,
  getProfile,
  // Checkers
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
  rejectCheckersParticipant,
  // Armwrestling
  getArmwrestlingEventById,
  updateArmwrestlingEvent,
  addParticipantToArmwrestlingEvent,
  removeParticipantFromArmwrestlingEvent,
  updateArmwrestlingParticipant,
  registerForArmwrestlingEvent,
  approveArmwrestlingParticipant,
  rejectArmwrestlingParticipant,
  // Volleyball
  getVolleyballEventById,
  updateVolleyballEvent,
  registerForVolleyballEvent,
  approveVolleyballParticipant,
  rejectVolleyballParticipant,
  removeVolleyballParticipant,
  // Beach Volleyball
  getBeachVolleyballEventById,
  updateBeachVolleyballEvent,
  registerForBeachVolleyballEvent,
  approveBeachVolleyballParticipant,
  rejectBeachVolleyballParticipant,
  removeBeachVolleyballParticipant,
  // football
  getFootballEventById,
  updateFootballEvent,
  registerForFootballEvent,
  approveFootballParticipant,
  rejectFootballParticipant,
  removeFootballParticipant,
  // Basketball 3x3
  getBasketball3x3EventById,
  updateBasketball3x3Event,
  registerForBasketball3x3Event,
  approveBasketball3x3Participant,
  rejectBasketball3x3Participant,
  removeBasketball3x3Participant,
  // Badminton
  getBadmintonEventById,
  updateBadmintonEvent,
  addParticipantToBadmintonEvent,
  removeParticipantFromBadmintonEvent,
  updateBadmintonParticipant,
  registerForBadmintonEvent,
  approveBadmintonParticipant,
  rejectBadmintonParticipant,
  removeBadmintonParticipant,
  // Athletics
  getAthleticsEventById,
  updateAthleticsEvent,
  addParticipantToAthleticsEvent,
  removeParticipantFromAthleticsEvent,
  addAthleticsResult,
  updateAthleticsResult,
  deleteAthleticsResult
} from '@/shared/api';
import { PlayerDto, Gender } from '@/entities/player/model/player.types';
import { RegistrationStatus, ModerationStatus } from '@/entities/tournament/model/tournament.types';
import {
  getTableTennisEventById,
  updateTableTennisEvent,
  addParticipantToTableTennisEvent,
  removeTableTennisParticipant,
  updateTableTennisParticipant,
  registerForTableTennisEvent,
  approveTableTennisParticipant,
  rejectTableTennisParticipant
} from '@/entities/sport/api/tabletennis.api';
import ArmwrestlingTournamentView from './components/ArmwrestlingTournamentView';
import TeamSportsTournamentView from './components/TeamSportsTournamentView';

import AthleticsTournamentView from './components/AthleticsTournamentView';
import TournamentPodium from './components/TournamentPodium';
import ChessGameViewerModal from './components/ChessGameViewerModal';
import { Tournament } from '@/entities/tournament/model/tournament.types';
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
} from '@/entities/sport/model/chess.types';
import {
  CheckersEventResponse,
  CheckersEventRequest,
  CheckersGameDto,
  CheckersEventParticipantDto
} from '@/entities/sport/model/checkers.types';
import api, { BASE_URL } from '@/shared/api/axiosInstance';
import styles from './TournamentViewPage.module.css';

import {
  ArmwrestlingEventResponse
} from '@/entities/sport/model/armwrestling.types';

const TournamentViewPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [tournament, setTournament] = useState<Tournament | null>(null);
  const [details, setDetails] = useState<any | null>(null);
  const [viewingRosterTeamId, setViewingRosterTeamId] = useState<number | null>(null);
  const [activeTab, setActiveTab] = useState<'standings' | 'games' | 'participants' | 'schedule' | 'voting'>('standings');
  const [isLoading, setIsLoading] = useState(true);
  const [isRegistering, setIsRegistering] = useState(false);
  const [currentUserPlayer, setCurrentUserPlayer] = useState<PlayerDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [selectedPhotoIndex, setSelectedPhotoIndex] = useState<number | null>(null);
  const [isMainPhotoLightboxOpen, setIsMainPhotoLightboxOpen] = useState(false);
  const [seasons, setSeasons] = useState<any[]>([]);

  const [userMedia, setUserMedia] = useState<any[]>([]);
  const [isUploadingUserMedia, setIsUploadingUserMedia] = useState(false);
  const [userMediaComment, setUserMediaComment] = useState('');
  const [userMediaFile, setUserMediaFile] = useState<File | null>(null);
  const [pendingUserMedia, setPendingUserMedia] = useState<any[]>([]);
  const [selectedUserPhotoIndex, setSelectedUserPhotoIndex] = useState<number | null>(null);

  const fetchUserMedia = async () => {
    if (!id) return;
    try {
      const data = await getApprovedUserEventMedia(id);
      setUserMedia(data);
    } catch (err) {
      console.error('Failed to fetch user media:', err);
    }
  };

  const fetchPendingUserMedia = async () => {
    const isUserAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';
    if (!isUserAdmin) return;
    try {
      const data = await getPendingUserEventMedia();
      const filtered = data.filter((m: any) => m.eventId === Number(id));
      setPendingUserMedia(filtered);
    } catch (err) {
      console.error('Failed to fetch pending media:', err);
    }
  };

  const [voteStats, setVoteStats] = useState<any>(null);
  const [isVoting, setIsVoting] = useState(false);

  const fetchVoteStats = async () => {
    try {
      const response = await api.get(`/tournaments/${id}/votes`);
      setVoteStats(response.data);
    } catch (err) {
      console.error('Failed to fetch vote stats:', err);
    }
  };

  const handleVote = async (teamId?: number, playerId?: number) => {
    setIsVoting(true);
    try {
      let url = `/tournaments/${id}/vote`;
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

  useEffect(() => {
    if (id) {
      fetchVoteStats();
      fetchUserMedia();
      fetchPendingUserMedia();
    }
  }, [id]);

  // Admin access check (0 = User, 1 = Admin)
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';
  
  // Fizorg access check (2 = Fizorg)
  const isFizorg = user?.role === 2 || user?.role === '2' || user?.role === 'Fizorg';

  // Search Results type
  type SearchResult = {
    type: 'player' | 'user';
    id: number;
    userId?: number | null;
    firstName: string;
    lastName: string;
    patronymic?: string | null;
    email?: string;
    group?: string | null;
    studentId?: string | null;
    studyYears?: string | null;
    city?: string;
    rating?: number;
  };

  // Centralized sport checks
  const getNormalizedSportKey = () => {
    const rawSport = (tournament?.sport || '').toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
    const sportMap: Record<string, string> = {
      'футбол': 'football',
      'волейбол': 'volleyball',
      'пляжний_волейбол': 'beach_volleyball',
      'пляжнийволейбол': 'beach_volleyball',
      'баскетбол_3х3': 'basketball_3x3',
      'баскетбол3х3': 'basketball_3x3',
      'бадмінтон': 'badminton',
      'настільний_теніс': 'table_tennis',
      'настільнийтеніс': 'table_tennis',
      'шахи': 'chess',
      'шашки': 'checkers',
      'армрестлінг': 'armwrestling',
      'легка_атлетика': 'athletics',
      'легкаатлетика': 'athletics'
    };
    return sportMap[rawSport] || rawSport;
  };

  const sportKey = getNormalizedSportKey();
  const isChess = sportKey === 'chess';
  const isCheckers = sportKey === 'checkers';
  const isBadminton = sportKey === 'badminton';
  const isTableTennis = sportKey === 'tabletennis' || sportKey === 'table_tennis';
  const isfootball = sportKey === 'football';
  const isVolleyball = sportKey === 'volleyball' || sportKey === 'beach_volleyball';
  const isBasketball3x3 = sportKey === 'basketball_3x3' || sportKey === 'basketball3x3';
  const isAthletics = sportKey === 'athletics';

  // Admin Form States
  const [newPlayer, setNewPlayer] = useState({
    firstName: '',
    patronymic: '',
    lastName: '',
    group: '',
    studentId: '',
    studyYears: '',
    city: 'Чернівці',
    ratingELO: 1500,
    fideId: '',
    title: '',
    userId: null as number | null,
    existingPlayerId: null as number | null,
    weight: 0,
    gender: 'None' as Gender
  });
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [newGame, setNewGame] = useState({ whitePlayerId: '', blackPlayerId: '', result: 'WhiteWin', round: 1, board: 1, pgn: '' });

  const [viewingPgnGame, setViewingPgnGame] = useState<ChessGameResponse | null>(null);

  const [editingGameId, setEditingGameId] = useState<number | null>(null);
  const [editingGameData, setEditingGameData] = useState<{ round: number; board: number; result: string; whitePlayerId: string; blackPlayerId: string, pgn: string }>({ round: 1, board: 1, result: 'WhiteWin', whitePlayerId: '', blackPlayerId: '', pgn: '' });

  const [editingParticipantId, setEditingParticipantId] = useState<number | null>(null);
  const [editingParticipantData, setEditingParticipantData] = useState<{ points: number; rank: number; boardRank?: number; weight: number; rating?: number; status: RegistrationStatus; firstName: string; lastName: string; groupName: string }>({ points: 0, rank: 0, weight: 0, rating: 0, status: RegistrationStatus.Approved, firstName: '', lastName: '', groupName: '' });
  const [gameFilters, setGameFilters] = useState({ round: '', board: '', participant: '', result: '' });
  const [isImporting, setIsImporting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [isEditingTournament, setIsEditingTournament] = useState(false);
  const isMandatoryTeamSport = ['football', 'volleyball', 'beach-volleyball', 'beach_volleyball', 'basketball-3x3', 'basketball_3x3', 'basketball3x3'].includes(sportKey);
  const [showWinnersAdmin, setShowWinnersAdmin] = useState(false);
  const [winners, setWinners] = useState({
    firstPlaceId: 0,
    secondPlaceId: 0,
    thirdPlaceId: 0,
    mvpId: 0,
    board1_1st: 0,
    board1_2nd: 0,
    board1_3rd: 0,
    board2_1st: 0,
    board2_2nd: 0,
    board2_3rd: 0
  });



  const handleSaveWinners = async () => {
    if (!details || !id) return;
    try {
      const isTeamSport = !!(details as any).isTeam || ['football', 'volleyball', 'beach_volleyball', 'basketball_3x3'].includes(sportKey);

      // Build clean base payload with common fields to avoid "Extra fields" 400 error
      // Ensuring ALL required fields from the backend DTO are present
      const payload: any = {
        name: editTournamentData.name || (details as any).name || tournament?.title || '',
        date: formatToISO(editTournamentData.date) || (details as any).date,
        location: editTournamentData.location || (details as any).location || tournament?.location || '',
        refereeIds: editTournamentData.refereeIds || (details as any).refereeIds || [],
        level: editTournamentData.level || (details as any).level || 'CollegeGroup',
        description: editTournamentData.description || (details as any).description || tournament?.description || '',
        rules: editTournamentData.rules || (details as any).rules || '',
        prizes: editTournamentData.prizes || (details as any).prizes || '',
        headRefereeName: editTournamentData.headRefereeName || (details as any).headRefereeName || '',
        referees: editTournamentData.referees || (details as any).referees || '',
        schedule: editTournamentData.schedule || (details as any).schedule || '',
        registrationStart: formatToISO(editTournamentData.registrationStart) || (details as any).registrationStart || null,
        registrationEnd: formatToISO(editTournamentData.registrationEnd) || (details as any).registrationEnd || null,
        endDate: formatToISO(editTournamentData.endDate) || (details as any).endDate || null,
        isModerated: editTournamentData.level === 'CollegeGroup' ? !!editTournamentData.isModerated : !!(details as any).isModerated,
        isPublic: editTournamentData.isPublic !== undefined ? !!editTournamentData.isPublic : ((details as any).isPublic !== undefined ? !!(details as any).isPublic : true),
        disableNotifications: (editTournamentData as any).disableNotifications !== undefined ? !!(editTournamentData as any).disableNotifications : !!(details as any).disableNotifications,
        isTeam: isTeamSport,
        seasonId: editTournamentData.seasonId || (details as any).seasonId || null,
        category: editTournamentData.category || (details as any).category || (isVolleyball ? 'Men' : 'Mixed')
      };

      if (['chess', 'checkers'].includes(sportKey)) {
        payload.timeControl = (details as any).timeControl || "Rapid";
        payload.system = (details as any).system !== undefined ? (details as any).system : 0;
        payload.participantsCount = (details as any).participantsCount || 0;
      }

      if (isTeamSport) {
        payload.firstPlaceTeamId = winners.firstPlaceId || null;
        payload.secondPlaceTeamId = winners.secondPlaceId || null;
        payload.thirdPlaceTeamId = winners.thirdPlaceId || null;
        payload.mvpId = winners.mvpId || null;


        if (['football', 'badminton', 'basketball_3x3', 'basketball3x3'].includes(sportKey)) {
          payload.tournamentType = (details as any).tournamentType || 'RoundRobin';
        }

        // Update Board Ranks for Chess/Checkers
        if (['chess', 'checkers'].includes(sportKey) && (details as any).level !== 'CollegeGroup') {
          // 1. Clear boardRank for all participants in this tournament
          for (const p of (details as any).participants || []) {
            if (p.boardRank) {
              const pPayload = { ...p, boardRank: null };
              if (sportKey === 'checkers') {
                await updateParticipantInCheckersEvent(id!, p.playerId, pPayload);
              } else {
                await updateParticipantInEvent(id!, p.playerId, pPayload);
              }
            }
          }

          // 2. Set new board ranks
          const boardUpdates = [
            { playerId: winners.board1_1st, rank: 1 },
            { playerId: winners.board1_2nd, rank: 2 },
            { playerId: winners.board1_3rd, rank: 3 },
            { playerId: winners.board2_1st, rank: 1 },
            { playerId: winners.board2_2nd, rank: 2 },
            { playerId: winners.board2_3rd, rank: 3 },
          ];


          const allTeamPlayers = (details as any).teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamId: t.id }))) || [];

          for (const update of boardUpdates) {
            if (update.playerId && update.playerId !== 0) {
              const participant = (details as any).participants?.find((p: any) => Number(p.playerId || p.id) === Number(update.playerId));
              const player = allTeamPlayers.find((p: any) => Number(p.id) === Number(update.playerId));


              if (participant) {
                const pPayload = { ...participant, boardRank: update.rank };
                if (sportKey === 'checkers') {
                  await updateParticipantInCheckersEvent(id!, update.playerId, pPayload);
                } else {
                  await updateParticipantInEvent(id!, update.playerId, pPayload);
                }
              } else if (player) {
                const pPayload = {
                  playerId: update.playerId,
                  boardRank: update.rank,
                  status: 'Approved',
                  chessEventId: Number(id)
                };
                if (sportKey === 'checkers') {
                  await addParticipantToCheckersEvent(id!, pPayload as any);
                } else {
                  await addParticipantToEvent(id!, pPayload as any);
                }
              }
            }
          }
        }
        if (['volleyball', 'beach_volleyball'].includes(sportKey)) {
          payload.setsToWin = (details as any).setsToWin || 2;
        }
      } else {
        payload.firstPlaceId = winners.firstPlaceId || null;
        payload.secondPlaceId = winners.secondPlaceId || null;
        payload.thirdPlaceId = winners.thirdPlaceId || null;
        payload.mvpId = winners.mvpId || null;


        if (sportKey === 'chess') {
          payload.system = editTournamentData.system;
          payload.timeControl = editTournamentData.timeControl;
          payload.participantsCount = editTournamentData.participantsCount;
        } else if (sportKey === 'checkers') {
          payload.variant = (details as any).variant || 'Russian';
        } else if (sportKey === 'armwrestling') {
          payload.hand = (details as any).hand || 'Both';
          payload.weightClass = (details as any).weightClass || 'UpTo70';
          payload.category = editTournamentData.category || (details as any).category || 'Mixed';
          payload.timeControl = editTournamentData.timeControl || (details as any).timeControl || '';
          payload.participantsCount = editTournamentData.participantsCount || (details as any).participantsCount || 0;
        }
      }



      switch (sportKey) {
        case 'checkers': await updateCheckersEvent(id, payload as any); break;
        case 'chess': await updateChessEvent(id, payload as any); break;
        case 'armwrestling': await updateArmwrestlingEvent(id, payload as any); break;
        case 'football': await updateFootballEvent(id, payload as any); break;
        case 'volleyball': await updateVolleyballEvent(id, payload as any); break;
        case 'beach_volleyball': await updateBeachVolleyballEvent(id, payload as any); break;
        case 'basketball_3x3':
        case 'basketball3x3': await updateBasketball3x3Event(id, payload as any); break;
        case 'badminton': await updateBadmintonEvent(id, {
          ...payload,
          tournamentType: (details as any).tournamentType || 'RoundRobin'
        } as any); break;
        case 'athletics': await api.put(`/athletics-events/${id}`, payload); break;
        case 'table_tennis': await updateTableTennisEvent(id, {
          ...payload,
          tournamentType: (details as any).tournamentType || 'RoundRobin'
        } as any); break;
        default: await updateChessEvent(id, payload as any);
      }

      toast.success("Призерів успішно встановлено!");
      setShowWinnersAdmin(false);
      fetchTournament(true);
    } catch (err: any) {
      console.error("Error saving winners:", err);
      if (err.response?.data) {
        console.error("Server response:", err.response.data);
      }
      toast.error("Помилка при збереженні призерів");
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
    disableNotifications: false,
    seasonId: null,
    category: 'Mixed',
    firstPlaceId: null,
    secondPlaceId: null,
    thirdPlaceId: null,
    firstPlaceTeamId: null,
    secondPlaceTeamId: null,
    thirdPlaceTeamId: null
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
      toast.error('Помилка при імпорті даних з файлу: ' + (err.response?.data?.message || err.message));
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
          football: '/football-events',
          volleyball: '/volleyball-events',
          beach_volleyball: '/beach-volleyball-events',
          beachvolleyball: '/beach-volleyball-events',
          basketball_3x3: '/basketball3x3-events',
          basketball3x3: '/basketball3x3-events',
          table_tennis: '/table-tennis-events',
          tabletennis: '/table-tennis-events',
          armwrestling: '/armwrestling-events',
          badminton: '/badminton-events',
          athletics: '/athletics-events'
        };
        const s = (tournament?.sport || '').toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
        const endpoint = sportEndpointRecord[s] || sportEndpointRecord[tournament?.sport || ''];

        if (!endpoint) {
          toast.error('Неможливо видалити змагання: невідомий вид спорту.');
          return;
        }

        await api.delete(`${endpoint}/${id}`);
        toast.success('Змагання успішно видалено.');
        navigate('/tournaments');
      } catch (error) {
        console.error('Помилка при видаленні змагання:', error);
        toast.error('Помилка при видаленні змагань. Деталі в консолі.');
      }
    }
  };

  const handleDeleteUserMedia = async (mediaId: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити це медіа? Цю дію неможливо скасувати.')) return;
    try {
      await deleteUserEventMedia(mediaId);
      toast.success('Медіа видалено');
      fetchUserMedia();
      fetchPendingUserMedia();
      if (selectedUserPhotoIndex !== null) {
        setSelectedUserPhotoIndex(null);
      }
    } catch (err: any) {
      toast.error('Помилка при видаленні медіа');
      console.error(err);
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
        date: formatDateForInput(details.rawDate || details.date),
        location: tournament?.location || details.location,
        refereeIds: details.refereeIds || [],
        level: details.level || 'CollegeGroup',
        description: tournament?.description || details.description,
        timeControl: details.timeControl || '',
        participantsCount: details.participantsCount || 0,
        system: (details as any).system || 'Swiss',
        tournamentType: (details as any).tournamentType || 'RoundRobin',
        rules: details.rules || '',
        prizes: details.prizes || '',
        headRefereeName: details.headRefereeName || '',
        referees: (details as any).referees || '',
        schedule: details.schedule || '',
        isTeam: (['volleyball', 'beach_volleyball', 'football', 'basketball_3x3', 'basketball3x3'].includes((tournament?.sport || '').toLowerCase().replace(/-/g, '_'))) ? true : !!(details as any).isTeam,
        registrationStart: formatDateForInput(tournament?.registrationStart || details.registrationStart),
        registrationEnd: formatDateForInput(tournament?.registrationEnd || details.registrationEnd),
        endDate: formatDateForInput(tournament?.endDate || details.endDate),
        isModerated: !!details.isModerated,
        isPublic: details.isPublic !== undefined ? !!details.isPublic : true,
        disableNotifications: (details as any).disableNotifications || false,
        seasonId: details.seasonId || null,
        category: (details as any).category ||
          (tournament?.category === 'Чоловіки' ? 'Men' :
            tournament?.category === 'Жінки' ? 'Women' :
              tournament?.category === 'Змішаний' ? 'Mixed' :
                (tournament?.category as any)) || (isVolleyball ? 'Men' : 'Mixed'),
        firstPlaceId: details.firstPlace?.id || null,
        secondPlaceId: details.secondPlace?.id || null,
        thirdPlaceId: details.thirdPlace?.id || null,
        firstPlaceTeamId: details.firstPlaceTeam?.id || null,
        secondPlaceTeamId: details.secondPlaceTeam?.id || null,
        thirdPlaceTeamId: details.thirdPlaceTeam?.id || null,
        mvpId: details.mvpId || null
      } as any);

      const findBoardForPlayer = (playerId: number) => {
        const team = (details as any).teams?.find((t: any) => t.players?.some((p: any) => p.id === playerId));
        if (!team) return 1;
        const playerIndex = team.players.findIndex((p: any) => p.id === playerId);
        return playerIndex + 1;
      };

      const b1_1st = (details as any).participants?.find((p: any) => p.boardRank === 1 && findBoardForPlayer(p.playerId) === 1);
      const b2_1st = (details as any).participants?.find((p: any) => p.boardRank === 1 && findBoardForPlayer(p.playerId) === 2);
      const b1_2nd = (details as any).participants?.find((p: any) => p.boardRank === 2 && findBoardForPlayer(p.playerId) === 1);
      const b2_2nd = (details as any).participants?.find((p: any) => p.boardRank === 2 && findBoardForPlayer(p.playerId) === 2);
      const b1_3rd = (details as any).participants?.find((p: any) => p.boardRank === 3 && findBoardForPlayer(p.playerId) === 1);
      const b2_3rd = (details as any).participants?.find((p: any) => p.boardRank === 3 && findBoardForPlayer(p.playerId) === 2);

      setWinners({
        firstPlaceId: details.isTeam ? (details.firstPlaceTeam?.id || 0) : (details.firstPlace?.id || 0),
        secondPlaceId: details.isTeam ? (details.secondPlaceTeam?.id || 0) : (details.secondPlace?.id || 0),
        thirdPlaceId: details.isTeam ? (details.thirdPlaceTeam?.id || 0) : (details.thirdPlace?.id || 0),
        mvpId: details.mvpId || 0,
        board1_1st: b1_1st?.playerId || 0,
        board2_1st: b2_1st?.playerId || 0,
        board1_2nd: b1_2nd?.playerId || 0,
        board2_2nd: b2_2nd?.playerId || 0,
        board1_3rd: b1_3rd?.playerId || 0,
        board2_3rd: b2_3rd?.playerId || 0
      });

      setIsEditingTournament(true);

    }
  };

  const handlePhotoUpload = async (e: React.ChangeEvent<HTMLInputElement> | React.DragEvent) => {
    let files: File[] = [];

    if ('files' in e.target && e.target.files) {
      files = Array.from(e.target.files);
    } else if ('dataTransfer' in e && e.dataTransfer.files) {
      e.preventDefault();
      files = Array.from(e.dataTransfer.files);
    }

    if (files.length === 0) return;

    // Перевірка розміру файлів (напр. 500 МБ на файл)
    const MAX_FILE_SIZE = 500 * 1024 * 1024;
    const largeFiles = files.filter(f => f.size > MAX_FILE_SIZE);
    if (largeFiles.length > 0) {
      toast.error(`Файл(и) занадто великі: ${largeFiles.map(f => f.name).join(', ')}. Максимум 500 МБ.`);
      return;
    }

    try {
      setIsUploading(true);
      const response = await uploadEventMedia(id!, files);
      toast.success(files.length > 1 ? "Медіафайли завантажено" : "Медіафайл завантажено");
      setDetails((prev: any) => ({
        ...prev,
        photos: [...(prev?.photos || []), ...(response.photos || [])]
      }));
    } catch (err: any) {
      console.error(err);
      const errorMessage = err.response?.data?.error || err.response?.data?.message || "Помилка при завантаженні медіа";
      toast.error(errorMessage);
    } finally {
      setIsUploading(false);
    }
  };

  const handleDeletePhoto = async (photoId: number) => {
    if (!window.confirm("Ви дійсно хочете видалити цей медіафайл?")) return;
    try {
      await deleteEventMedia(photoId);
      toast.success("Видалено");
      setDetails((prev: any) => ({
        ...prev,
        photos: prev?.photos?.filter((p: any) => p.id !== photoId) || []
      }));
    } catch (err) {
      console.error(err);
      toast.error("Помилка при видаленні");
    }
  };

  const handleSaveTournament = async () => {
    try {
      if (!editTournamentData.name || !editTournamentData.date) {
        toast.error("Назва та дата обов'язкові!");
        return;
      }
      const sport = tournament?.sport;
      if (!sport) {
        toast.error("Помилка: Вид спорту не визначений");
        return;
      }

      const sportKey = sport.toLowerCase().replace(/-/g, '_').replace(/\s/g, '');

      // Build clean base payload
      const payload: any = {
        name: editTournamentData.name || (details as any).name || tournament?.title || '',
        date: formatToISO(editTournamentData.date) || (details as any).date,
        location: editTournamentData.location || (details as any).location || tournament?.location || '',
        refereeIds: editTournamentData.refereeIds || (details as any).refereeIds || [],
        level: editTournamentData.level || (details as any).level || 'CollegeGroup',
        description: editTournamentData.description || (details as any).description || tournament?.description || '',
        rules: editTournamentData.rules || (details as any).rules || '',
        prizes: editTournamentData.prizes || (details as any).prizes || '',
        headRefereeName: editTournamentData.headRefereeName || (details as any).headRefereeName || '',
        referees: editTournamentData.referees || (details as any).referees || '',
        schedule: editTournamentData.schedule || (details as any).schedule || '',
        isTeam: editTournamentData.isTeam !== undefined ? !!editTournamentData.isTeam : !!(details as any).isTeam,
        registrationStart: formatToISO(editTournamentData.registrationStart) || (details as any).registrationStart || null,
        registrationEnd: formatToISO(editTournamentData.registrationEnd) || (details as any).registrationEnd || null,
        endDate: formatToISO(editTournamentData.endDate) || (details as any).endDate || null,
        isModerated: editTournamentData.level === 'CollegeGroup' ? !!editTournamentData.isModerated : !!(details as any).isModerated,
        isPublic: editTournamentData.isPublic !== undefined ? !!editTournamentData.isPublic : ((details as any).isPublic !== undefined ? !!(details as any).isPublic : true),
        disableNotifications: (editTournamentData as any).disableNotifications || false,
        seasonId: editTournamentData.seasonId || (details as any).seasonId || null,
        category: editTournamentData.category || (details as any).category || (isVolleyball ? 'Men' : 'Mixed'),
        firstPlaceId: (editTournamentData as any).firstPlaceId || (details as any).firstPlace?.id || null,
        secondPlaceId: (editTournamentData as any).secondPlaceId || (details as any).secondPlace?.id || null,
        thirdPlaceId: (editTournamentData as any).thirdPlaceId || (details as any).thirdPlace?.id || null,
        firstPlaceTeamId: (editTournamentData as any).firstPlaceTeamId || (details as any).firstPlaceTeam?.id || null,
        secondPlaceTeamId: (editTournamentData as any).secondPlaceTeamId || (details as any).secondPlaceTeam?.id || null,
        thirdPlaceTeamId: (editTournamentData as any).thirdPlaceTeamId || (details as any).thirdPlaceTeam?.id || null
      };

      console.log("Updating tournament with payload:", payload);

      if (sportKey === 'checkers') {
        await updateCheckersEvent(id!, { ...payload, variant: (details as any).variant || 'Russian' } as any);
      } else if (sportKey === 'armwrestling') {
        await updateArmwrestlingEvent(id!, {
          ...payload,
          hand: (details as any).hand || 'Both',
          weightClass: (details as any).weightClass || 'UpTo70',
          category: editTournamentData.category || (details as any).category || 'Mixed',
          timeControl: editTournamentData.timeControl || (details as any).timeControl || '',
          participantsCount: editTournamentData.participantsCount || (details as any).participantsCount || 0
        });
      } else if (sportKey === 'volleyball' || sportKey === 'beach_volleyball') {
        const updateFn = sportKey === 'volleyball' ? updateVolleyballEvent : updateBeachVolleyballEvent;
        await updateFn(id!, { ...payload, setsToWin: (details as any).setsToWin || 2 });
      } else if (sportKey === 'football' || sportKey === 'badminton') {
        const updateFn = sportKey === 'football' ? updateFootballEvent : updateBadmintonEvent;
        await updateFn(id!, { ...payload, tournamentType: (editTournamentData as any).tournamentType || (details as any).tournamentType || 'RoundRobin' });
      } else if (sportKey === 'basketball_3x3' || sportKey === 'basketball3x3') {
        await updateBasketball3x3Event(id!, { ...payload, tournamentType: (editTournamentData as any).tournamentType || (details as any).tournamentType || 'RoundRobin' });
      } else if (sportKey === 'tabletennis' || sportKey === 'table_tennis') {
        await updateTableTennisEvent(id!, {
          ...payload,
          tournamentType: (editTournamentData as any).tournamentType || (details as any).tournamentType || 'RoundRobin'
        });
      } else if (sportKey === 'athletics') {
        await api.put(`/athletics-events/${id!}`, payload);
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
      toast.error("Помилка при оновленні змагань");
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
      boardRank: p.boardRank,
      weight: p.weight || 0,
      status: p.status || RegistrationStatus.Approved,
      firstName: p.firstName || '',
      lastName: p.lastName || '',
      groupName: p.groupName || '',
      rating: p.rating || 0
    });
  };

  const handleSaveEdit = async (gameId: number) => {
    if (!editingGameData.whitePlayerId || !editingGameData.blackPlayerId) {
      toast.error("Оберіть обох супротивників!");
      return;
    }
    if (editingGameData.whitePlayerId === editingGameData.blackPlayerId) {
      toast.error("Гравець не може грати сам проти себе!");
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
      toast.error("Помилка при оновленні матчу");
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
          boardRank: editingParticipantData.boardRank,
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
          status: editingParticipantData.status,
          firstName: editingParticipantData.firstName,
          lastName: editingParticipantData.lastName,
          groupName: editingParticipantData.groupName
        });
      } else if (sportKey === 'badminton') {
        await updateBadmintonParticipant(id!, playerId, {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
          status: editingParticipantData.status,
          firstName: editingParticipantData.firstName,
          lastName: editingParticipantData.lastName,
          groupName: editingParticipantData.groupName
        });
      } else if (sportKey === 'tabletennis' || sportKey === 'table_tennis') {
        await updateTableTennisParticipant(id!, playerId, {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
          status: editingParticipantData.status,
          firstName: editingParticipantData.firstName,
          lastName: editingParticipantData.lastName,
          groupName: editingParticipantData.groupName
        });
      } else if (sportKey === 'athletics') {
         toast.error("Редагування учасника недоступне для легкої атлетики");
         setEditingParticipantId(null);
         return;
      } else {
        const payload: ChessEventParticipantDto = {
          playerId,
          points: editingParticipantData.points,
          rank: editingParticipantData.rank,
          boardRank: editingParticipantData.boardRank,
          status: editingParticipantData.status,
          firstName: editingParticipantData.firstName,
          lastName: editingParticipantData.lastName,
          groupName: editingParticipantData.groupName,
          rating: editingParticipantData.rating
        };
        await updateParticipantInEvent(id!, playerId, payload);
      }

      toast.success("Дані учасника оновлено!");
      setEditingParticipantId(null);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      toast.error("Помилка при оновленні учасника");
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
            await removeTableTennisParticipant(id!, playerId);
            break;
          case 'volleyball':
            await removeVolleyballParticipant(id!, playerId);
            break;
          case 'beach_volleyball':
            await removeBeachVolleyballParticipant(id!, playerId);
            break;
          case 'football':
            await removeFootballParticipant(id!, playerId);
            break;
          case 'basketball3x3':
            await removeBasketball3x3Participant(id!, playerId);
            break;
          case 'athletics':
            await removeParticipantFromAthleticsEvent(id!, playerId);
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

  const handleApproveParticipant = async (playerId: number, playerName?: string) => {
    try {
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      switch (sportKey) {
        case 'checkers': await approveCheckersParticipant(id!, playerId); break;
        case 'armwrestling': await approveArmwrestlingParticipant(id!, playerId); break;
        case 'badminton': await approveBadmintonParticipant(id!, playerId); break;
        case 'tabletennis':
        case 'table_tennis': await approveTableTennisParticipant(id!, playerId); break;
        case 'volleyball': await approveVolleyballParticipant(id!, playerId); break;
        case 'beach_volleyball': await approveBeachVolleyballParticipant(id!, playerId); break;
        case 'football': await approveFootballParticipant(id!, playerId); break;
        case 'basketball3x3': await approveBasketball3x3Participant(id!, playerId); break;
        case 'athletics': 
            toast.success('Легка атлетика не потребує підтвердження учасників');
            return;
        default: await approveChessParticipant(id!, playerId);
      }
      toast.success(`Учасника ${playerName ? `"${playerName}" ` : ''}схвалено!`);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      toast.error("Помилка при схваленні");
    }
  };

  const handleRejectParticipant = async (playerId: number, playerName?: string) => {
    if (!window.confirm(`Відхилити участь учасника${playerName ? ` "${playerName}"` : ''}?`)) return;
    try {
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';
      switch (sportKey) {
        case 'checkers': await rejectCheckersParticipant(id!, playerId); break;
        case 'armwrestling': await rejectArmwrestlingParticipant(id!, playerId); break;
        case 'badminton': await rejectBadmintonParticipant(id!, playerId); break;
        case 'tabletennis':
        case 'table_tennis': await rejectTableTennisParticipant(id!, playerId); break;
        case 'volleyball': await rejectVolleyballParticipant(id!, playerId); break;
        case 'beach_volleyball': await rejectBeachVolleyballParticipant(id!, playerId); break;
        case 'football': await rejectFootballParticipant(id!, playerId); break;
        case 'basketball3x3': await rejectBasketball3x3Participant(id!, playerId); break;
        case 'athletics':
            await removeParticipantFromAthleticsEvent(id!, playerId);
            toast.success(`Учасника ${playerName ? `"${playerName}" ` : ''}видалено`);
            fetchTournament(true);
            return;
        default: await rejectChessParticipant(id!, playerId);
      }
      toast.success(`Учасника ${playerName ? `"${playerName}" ` : ''}відхилено`);
      fetchTournament(true);
    } catch (err) {
      console.error(err);
      toast.error("Помилка при відхиленні");
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

    if (activePlayer?.group === 'Гість') {
      toast.error("Користувачі з групою 'Гість' не можуть реєструватися на змагання. Будь ласка, вкажіть вашу групу в профілі.");
      return;
    }

    if (!activePlayer?.studentId) {
      toast.error("Для реєстрації на змагання необхідно вказати серію та номер студентського квитка в профілі (наприклад: PH12345678).");
      return;
    }

    if (!activePlayer?.gender || activePlayer.gender === 'None') {
      toast.error("Для реєстрації на змагання необхідно вказати стать у профілі.");
      return;
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
        case 'football': await registerForFootballEvent(id, playerId); break;
        case 'volleyball': await registerForVolleyballEvent(id, playerId); break;
        case 'basketball3x3':
        case 'basketball_3x3': await registerForBasketball3x3Event(id, playerId); break;
        case 'beachvolleyball':
        case 'beach_volleyball': await registerForBeachVolleyballEvent(id, playerId); break;
        case 'athletics': await api.post(`/athletics-events/${id}/participants`, { playerId }); break;
        default: await registerForChessEvent(id, playerId);
      }

      const isModerated = tournament?.isModerated || (details && details.isPublic === false) || details?.isModerated;

      if (isModerated) {
        toast.success("Ваша заявка надіслана на модерацію. Очікуйте на підтвердження адміністратором.");
      } else {
        toast.success("Ви успішно зареєструвалися!");
      }
      fetchTournament(true);
    } catch (err: any) {
      console.error(err);
      toast.error(err.response?.data?.detail || err.response?.data?.message || "Помилка при реєстрації");
    } finally {
      setIsRegistering(false);
    }
  };

  const fetchTournament = async (quiet = false) => {
    if (!id) return;
    if (!quiet) setIsLoading(true);
    try {
      let profilePromise = Promise.resolve<any>(null);
      if (user && !currentUserPlayer) {
        profilePromise = getProfile().catch(() => null);
      }
      const data = await getTournamentById(id);

      const profileData = await profilePromise;
      if (profileData) {
        setCurrentUserPlayer(profileData);
      }
      setTournament(data);

      const sportKey = data.sport.toLowerCase().replace(/-/g, '_').replace(/\s/g, '');
      setTournament({ ...data, sport: sportKey }); // Sync normalized sport back to state

      if (['chess', 'checkers', 'armwrestling', 'volleyball', 'beach_volleyball', 'football', 'basketball_3x3', 'basketball_3x3', 'basketball3x3', 'badminton', 'table_tennis', 'tabletennis', 'athletics'].includes(sportKey)) {
        try {
          let detailsPromise;

          switch (sportKey) {
            case 'checkers': detailsPromise = getCheckersEventById(id); break;
            case 'armwrestling': detailsPromise = getArmwrestlingEventById(id); break;
            case 'volleyball': detailsPromise = getVolleyballEventById(id); break;
            case 'beach_volleyball': detailsPromise = getBeachVolleyballEventById(id); break;
            case 'football': detailsPromise = getFootballEventById(id); break;
            case 'basketball_3x3':
            case 'basketball3x3': detailsPromise = getBasketball3x3EventById(id); break;
            case 'badminton': detailsPromise = getBadmintonEventById(id); break;
            case 'table_tennis':
            case 'tabletennis': detailsPromise = getTableTennisEventById(id); break;
            case 'athletics': detailsPromise = getAthleticsEventById(id); break;
            default: detailsPromise = getChessEventById(id);
          }

          const detailsData = await detailsPromise;
          setDetails(detailsData);

          // Initialize winners state
          if (detailsData) {
            const d = detailsData as any;
            const findBoardForPlayer = (playerId: number) => {
              const team = d.teams?.find((t: any) => t.players?.some((p: any) => p.id === playerId));
              if (!team) return 1;
              const playerIndex = team.players.findIndex((p: any) => p.id === playerId);
              return playerIndex + 1;
            };

            const board1_1st = d.participants?.find((p: any) => p.boardRank === 1 && findBoardForPlayer(p.playerId) === 1);
            const board2_1st = d.participants?.find((p: any) => p.boardRank === 1 && findBoardForPlayer(p.playerId) === 2);

            const board1_2nd = d.participants?.find((p: any) => p.boardRank === 2 && findBoardForPlayer(p.playerId) === 1);
            const board2_2nd = d.participants?.find((p: any) => p.boardRank === 2 && findBoardForPlayer(p.playerId) === 2);

            const board1_3rd = d.participants?.find((p: any) => p.boardRank === 3 && findBoardForPlayer(p.playerId) === 1);
            const board2_3rd = d.participants?.find((p: any) => p.boardRank === 3 && findBoardForPlayer(p.playerId) === 2);

            setWinners({
              firstPlaceId: d.firstPlace?.id || d.firstPlaceTeam?.id || 0,
              secondPlaceId: d.secondPlace?.id || d.secondPlaceTeam?.id || 0,
              thirdPlaceId: d.thirdPlace?.id || d.thirdPlaceTeam?.id || 0,
              mvpId: d.mvpId || 0,
              board1_1st: board1_1st?.playerId || 0,
              board2_1st: board2_1st?.playerId || 0,
              board1_2nd: board1_2nd?.playerId || 0,
              board2_2nd: board2_2nd?.playerId || 0,
              board1_3rd: board1_3rd?.playerId || 0,
              board2_3rd: board2_3rd?.playerId || 0
            });


            // Initialize editTournamentData to prevent 400 errors when saving winners
            setEditTournamentData({
              name: data.title || d.name || '',
              date: formatDateForInput(data.rawDate || d.date),
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
              disableNotifications: !!d.disableNotifications,
              seasonId: d.seasonId || null,
              category: d.category || (data.category === 'Чоловіки' ? 'Men' : data.category === 'Жінки' ? 'Women' : data.category === 'Змішаний' ? 'Mixed' : data.category) || (sportKey === 'volleyball' || sportKey === 'beach_volleyball' ? 'Men' : 'Mixed'),
              firstPlaceId: d.firstPlace?.id || null,
              secondPlaceId: d.secondPlace?.id || null,
              thirdPlaceId: d.thirdPlace?.id || null,
              firstPlaceTeamId: d.firstPlaceTeam?.id || null,
              secondPlaceTeamId: d.secondPlaceTeam?.id || null,
              thirdPlaceTeamId: d.thirdPlaceTeam?.id || null
            } as any);
          }
        } catch (eventErr) {
          console.error('Failed to fetch detailed event info:', eventErr);
        }
      }
    } catch (err: any) {
      console.error('Failed to fetch tournament:', err);
      setError(`Не вдалося завантажити інформацію про змагання. (ID: ${id}) Деталі: ${err.message || 'Невідома помилка'}`);
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
          // 1. Search in existing Players via API
          const players = await searchPlayers(searchQuery);
          const playerResults: SearchResult[] = players
            .map(p => ({
              type: 'player',
              id: p.id!,
              userId: p.userId,
              firstName: p.firstName,
              patronymic: p.patronymic || '',
              lastName: p.lastName,
              group: p.group || null,
              studentId: p.studentId || '',
              studyYears: p.studyYears || '',
              rating: tournament?.sport === 'checkers' ? (p.checkersRating || 0) : (p.chessRating || 0)
            } as SearchResult));

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
  }, [searchQuery, tournament?.sport]);

  const handleSelectResult = (result: SearchResult) => {
    if (result.type === 'player') {
      setNewPlayer({
        ...newPlayer,
        firstName: result.firstName,
        patronymic: result.patronymic || '',
        lastName: result.lastName,
        group: result.group || '',
        studentId: result.studentId || '',
        studyYears: result.studyYears || '',
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
    if (!newPlayer.firstName || !newPlayer.lastName) {
      toast.error("Заповніть ім'я та прізвище користувача!");
      return;
    }
    if (tournament?.sport !== 'chess' && !newPlayer.group) {
      toast.error("Група є обов'язковою для цього виду спорту!");
      return;
    }
    try {
      let playerId = newPlayer.existingPlayerId;

      if (!playerId) {
        // Check if player is already in the tournament (by userId)
        if (newPlayer.userId && (details?.players || details?.participants || []).some((p: any) => p.userId === newPlayer.userId)) {
          toast.error("Цей користувач вже бере участь у змаганнях!");
          return;
        }

        const playerPayload: PlayerDto = {
          firstName: newPlayer.firstName,
          patronymic: newPlayer.patronymic || null,
          lastName: newPlayer.lastName,
          group: newPlayer.group,
          studentId: newPlayer.studentId || null,
          studyYears: newPlayer.studyYears || null,
          city: newPlayer.city,
          chessRating: tournament?.sport === 'chess' ? newPlayer.ratingELO : undefined,
          checkersRating: tournament?.sport === 'checkers' ? newPlayer.ratingELO : undefined,
          userId: newPlayer.userId,
          chessFideId: newPlayer.fideId.trim() || null,
          chessTitle: newPlayer.title.trim() || null,
          isGuest: !newPlayer.group,
          competitionsCount: 0,
          gender: newPlayer.gender
        };
        const created = await createPlayer(playerPayload);
        playerId = created.id!;
      } else {
        // Check if player is already in the tournament
        const isAlreadyParticipant = (details?.players || details?.participants || []).some((p: any) => p.playerId === playerId);
        if (isAlreadyParticipant) {
          toast.error("Цей гравець вже бере участь у змаганнях!");
          return;
        }
      }

      // Create participant binding
      const sportKey = tournament?.sport?.toLowerCase().replace(/-/g, '_').replace(/\s/g, '') || '';

      switch (sportKey) {
        case 'checkers':
          await addParticipantToCheckersEvent(id!, {
            playerId: playerId,
            points: 0,
            rank: 0,
            status: RegistrationStatus.Approved,
            firstName: newPlayer.firstName,
            lastName: newPlayer.lastName,
            groupName: newPlayer.group
          });
          break;
        case 'armwrestling':
          await addParticipantToArmwrestlingEvent(id!, {
            playerId: playerId,
            weight: newPlayer.weight || 0,
            rank: 0,
            status: RegistrationStatus.Approved,
            firstName: newPlayer.firstName,
            lastName: newPlayer.lastName,
            groupName: newPlayer.group
          });
          break;
        case 'badminton':
          await addParticipantToBadmintonEvent(id!, {
            playerId: playerId,
            points: 0,
            rank: 0,
            status: RegistrationStatus.Approved,
            firstName: newPlayer.firstName,
            lastName: newPlayer.lastName,
            groupName: newPlayer.group
          });
          break;
        case 'tabletennis':
        case 'table_tennis':
          await addParticipantToTableTennisEvent(id!, {
            playerId: playerId,
            points: 0,
            rank: 0,
            status: RegistrationStatus.Approved,
            firstName: newPlayer.firstName,
            lastName: newPlayer.lastName,
            groupName: newPlayer.group
          });
          break;
        case 'athletics':
          await addParticipantToAthleticsEvent(id!, {
            playerId: playerId,
            groupName: newPlayer.group
          });
          break;
        default:
          await addParticipantToEvent(id!, {
            playerId: playerId,
            points: 0,
            rank: 0,
            status: RegistrationStatus.Approved,
            firstName: newPlayer.firstName,
            lastName: newPlayer.lastName,
            rating: newPlayer.ratingELO,
            groupName: newPlayer.group
          });
      }
      toast.success("Учасник успішно доданий до змагань!");
      setNewPlayer({ firstName: '', patronymic: '', lastName: '', group: '', studentId: '', studyYears: '', city: 'Чернівці', ratingELO: 1500, fideId: '', title: '', userId: null, existingPlayerId: null, weight: 0, gender: 'None' });
      fetchTournament(true); // Refresh
    } catch (err) {
      console.error(err);
      toast.error("Помилка при додаванні учасника");
    }
  };

  const handleAddGame = async () => {
    if (!newGame.whitePlayerId || !newGame.blackPlayerId) {
      toast.error("Оберіть обох супротивників!");
      return;
    }
    if (newGame.whitePlayerId === newGame.blackPlayerId) {
      toast.error("Гравець не може грати сам проти себе!");
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
      toast.error("Помилка при додаванні матчу");
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

  const getTournamentSystemLabel = () => {
    const sport = tournament?.sport?.toLowerCase() || '';
    if (sport === 'chess' || sport === 'checkers') {
      const system = (details as any)?.system;
      switch (system) {
        case 'Swiss': return 'Швейцарська система';
        case 'RoundRobin': return 'Кругова система';
        case 'Knockout': return 'Олімпійська система';
        default: return system ? getSystemLabel(system) : '';
      }
    } else {
      const type = (details as any)?.tournamentType;
      switch (type) {
        case 'RoundRobin': return 'Кругова система';
        case 'SingleElimination': return 'Олімпійська система (Single Elimination)';
        case 'DoubleElimination': return 'Double Elimination (з вибуванням після двох поразок)';
        case 'GroupsPlayoff': return 'Груповий етап + Плей-офф';
        default: return type || '';
      }
    }
  };

  const getImageUrl = (url: string) => {
    if (!url) return '';
    if (url.startsWith('http')) return url;
    return `${BASE_URL}${url}`;
  };

  const isVideo = (url: string) => {
    const videoExtensions = ['.mp4', '.webm', '.ogg', '.mov'];
    return videoExtensions.some(ext => url.toLowerCase().endsWith(ext));
  };

  const badgeObj = getLevelBadgeParams(details?.level);
  const allGamesOrMatches = (details as any)?.games || (details as any)?.matches || [];
  const playedGames = allGamesOrMatches.filter((g: any) =>
    (g.result && g.result !== 'NotPlayed') || (g.winnerId != null)
  ).length;

  const totalRounds = allGamesOrMatches.reduce((acc: number, game: any) =>
    Math.max(acc, game.round || game.roundNumber || 0), 0
  );
  const isTeamSport = !!(details as any)?.isTeam || ['football', 'volleyball', 'beach_volleyball', 'basketball3x3', 'basketball_3x3'].includes(tournament?.sport || '');

  const actualParticipants = isTeamSport
    ? (details?.teams?.length || 0)
    : ((details as any)?.players?.length || (details as any)?.participants?.length || 0);

  const participantLabel = isTeamSport ? 'команд' : 'учасників';

  const pendingParticipantsCount = (details?.participants?.filter((p: any) => p.status === RegistrationStatus.Pending)?.length || 0) +
    (details?.teams?.filter((t: any) => t.status === RegistrationStatus.Pending)?.length || 0);

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

  const userRegistration = details?.participants?.find((p: any) =>
    (p.userId && p.userId === user?.id) ||
    (p.playerId && currentUserPlayer && p.playerId === currentUserPlayer.id)
  );
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
        <div className={styles.headerActions}>
          <button className={styles.backButton} onClick={() => navigate('/tournaments')}>
            <i className="fas fa-arrow-left"></i> Назад
          </button>

          <div className={styles.adminActions}>
            {isAdmin && sportKey !== 'athletics' && (
              <button
                onClick={() => setShowWinnersAdmin(!showWinnersAdmin)}
                className={`${styles.adminBtn} ${styles.winnersBtn}`}
              >
                <i className="fas fa-medal"></i> <span>Призначити призерів</span>
              </button>
            )}
            {isAdmin && (
              <button
                onClick={handleEditTournament}
                className={`${styles.adminBtn} ${styles.editBtn}`}
              >
                <i className="fas fa-edit"></i> <span>Редагувати змагання</span>
              </button>
            )}
            {isAdmin && (
              <button
                onClick={handleDeleteTournament}
                className={`${styles.adminBtn} ${styles.deleteBtn}`}
              >
                <i className="fas fa-trash"></i> <span>Видалити</span>
              </button>
            )}
          </div>
        </div>

        <>
          {isAdmin && showWinnersAdmin && sportKey !== 'athletics' && (
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
                        {p.playerName} {(['chess', 'checkers', 'tabletennis', 'badminton'].includes(tournament?.sport)) && (p.points !== undefined) ? `(${p.points} очок)` : ''}
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
                        {p.playerName} {(['chess', 'checkers', 'tabletennis', 'badminton'].includes(tournament?.sport)) && (p.points !== undefined) ? `(${p.points} очок)` : ''}
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
                        {p.playerName} {(['chess', 'checkers', 'tabletennis', 'badminton'].includes(tournament?.sport)) && (p.points !== undefined) ? `(${p.points} очок)` : ''}
                      </option>
                    ))}
                    {isTeamSport && details?.teams?.map((t: any) => (
                      <option key={`third-t-${t.id}`} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>
              </div> {/* CLOSE GRID HERE */}

              {!['chess', 'checkers', 'athletics', 'table_tennis', 'tabletennis', 'badminton', 'beach_volleyball', 'beachvolleyball', 'armwrestling'].includes(tournament?.sport?.toLowerCase() || '') && (
                <div style={{ marginTop: '16px' }}>
                  <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', fontSize: '14px', color: '#374151' }}>
                    <i className="fas fa-star" style={{ color: '#f59e0b' }}></i> Найкращий гравець (MVP)
                  </label>
                  <select
                    value={winners.mvpId}
                    onChange={(e) => setWinners({ ...winners, mvpId: Number(e.target.value) })}
                    style={{ width: '100%', padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                  >
                    <option key="default-mvp" value="0">-- Оберіть MVP --</option>
                    {isTeamSport ? (
                      details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                        <option key={`mvp-p-${p.id}`} value={p.id}>
                          {p.firstName} {p.lastName} ({p.teamName})
                        </option>
                      ))
                    ) : (
                      sortedPlayers?.map((p: any) => (
                        <option key={`mvp-p-${p.playerId}`} value={p.playerId}>{p.playerName}</option>
                      ))
                    )}
                  </select>
                </div>
              )}

              {['chess', 'checkers'].includes(tournament?.sport?.toLowerCase() || '') && isTeamSport && details?.level !== 'CollegeGroup' && (
                <>
                  <div style={{ marginTop: '24px' }}>
                    <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', fontSize: '14px', color: '#374151' }}>
                      <i className="fas fa-medal" style={{ color: '#f59e0b' }}></i> Кращі на 1 дошці
                    </label>
                    <div style={{ display: 'flex', gap: '8px', marginBottom: '8px' }}>
                      <select
                        value={winners.board1_1st || 0}
                        onChange={(e) => setWinners({ ...winners, board1_1st: Number(e.target.value) })}
                        style={{ flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                      >
                        <option value="0">1 місце (Дошка 1)</option>
                        {details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                          <option key={`b1-1-${p.id}`} value={p.id}>{p.firstName} {p.lastName} ({p.teamName})</option>
                        ))}
                      </select>
                      <select
                        value={winners.board1_2nd || 0}
                        onChange={(e) => setWinners({ ...winners, board1_2nd: Number(e.target.value) })}
                        style={{ flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                      >
                        <option value="0">2 місце (Дошка 1)</option>
                        {details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                          <option key={`b1-2-${p.id}`} value={p.id}>{p.firstName} {p.lastName} ({p.teamName})</option>
                        ))}
                      </select>
                      <select
                        value={winners.board1_3rd || 0}
                        onChange={(e) => setWinners({ ...winners, board1_3rd: Number(e.target.value) })}
                        style={{ flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                      >
                        <option value="0">3 місце (Дошка 1)</option>
                        {details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                          <option key={`b1-3-${p.id}`} value={p.id}>{p.firstName} {p.lastName} ({p.teamName})</option>
                        ))}
                      </select>
                    </div>
                  </div>

                  <div style={{ marginTop: '16px' }}>
                    <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold', fontSize: '14px', color: '#374151' }}>
                      <i className="fas fa-medal" style={{ color: '#f59e0b' }}></i> Кращі на 2 дошці
                    </label>
                    <div style={{ display: 'flex', gap: '8px' }}>
                      <select
                        value={winners.board2_1st || 0}
                        onChange={(e) => setWinners({ ...winners, board2_1st: Number(e.target.value) })}
                        style={{ flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                      >
                        <option value="0">1 місце (Дошка 2)</option>
                        {details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                          <option key={`b2-1-${p.id}`} value={p.id}>{p.firstName} {p.lastName} ({p.teamName})</option>
                        ))}
                      </select>
                      <select
                        value={winners.board2_2nd || 0}
                        onChange={(e) => setWinners({ ...winners, board2_2nd: Number(e.target.value) })}
                        style={{ flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                      >
                        <option value="0">2 місце (Дошка 2)</option>
                        {details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                          <option key={`b2-2-${p.id}`} value={p.id}>{p.firstName} {p.lastName} ({p.teamName})</option>
                        ))}
                      </select>
                      <select
                        value={winners.board2_3rd || 0}
                        onChange={(e) => setWinners({ ...winners, board2_3rd: Number(e.target.value) })}
                        style={{ flex: 1, padding: '10px', borderRadius: '8px', border: '1px solid #d1d5db' }}
                      >
                        <option value="0">3 місце (Дошка 2)</option>
                        {details?.teams?.flatMap((t: any) => (t.players || []).map((p: any) => ({ ...p, teamName: t.name }))).map((p: any) => (
                          <option key={`b2-3-${p.id}`} value={p.id}>{p.firstName} {p.lastName} ({p.teamName})</option>
                        ))}
                      </select>
                    </div>
                  </div>
                </>)}


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
                {(details?.level !== 'Regional' && details?.level !== 'National' && tournament?.level !== 'Regional' && tournament?.level !== 'National') && (
                  <div className={styles.metaItem}>
                    <i className="fas fa-users"></i>
                    <span>
                      {actualParticipants} {participantLabel}
                      {(() => {
                        const cat = (details as any)?.category || tournament?.category;
                        if (!cat) return '';
                        let catLabel = 'змішані';
                        if (cat === 'Men' || cat === 'Чоловіки') catLabel = 'чоловіки';
                        else if (cat === 'Women' || cat === 'Жінки') catLabel = 'жінки';
                        return ` (${catLabel})`;
                      })()}
                    </span>
                  </div>
                )}
                {(() => {
                  const systemLabel = getTournamentSystemLabel();
                  if (!systemLabel) return null;
                  return (
                    <div className={styles.metaItem}>
                      <i className="fas fa-project-diagram"></i>
                      <span>Система проведення: {systemLabel}</span>
                    </div>
                  );
                })()}
                {details?.timeControl && tournament?.sport?.toLowerCase() !== 'armwrestling' && (
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
            <div
              className={styles.tournamentImage}
              onClick={() => {
                if (details?.mainPhotoUrl || (details?.photos && details.photos.length > 0)) {
                  setIsMainPhotoLightboxOpen(true);
                }
              }}
              style={{ cursor: (details?.mainPhotoUrl || (details?.photos && details.photos.length > 0)) ? 'pointer' : 'default' }}
              title={(details?.mainPhotoUrl || (details?.photos && details.photos.length > 0)) ? "Натисніть, щоб збільшити" : ""}
            >
              {details?.mainPhotoUrl ? (
                <img src={getImageUrl(details.mainPhotoUrl)} alt={tournament?.title} loading="lazy" />
              ) : details?.photos && details.photos.length > 0 ? (
                <img src={getImageUrl(details.photos[0].url)} alt={tournament?.title} loading="lazy" />
              ) : (
                <i className={
                  tournament?.sport === 'checkers' ? "fas fa-braille" :
                    tournament?.sport === 'armwrestling' ? "fas fa-hand-rock" :
                      tournament?.sport === 'football' ? "fas fa-futbol" :
                        tournament?.sport === 'volleyball' || tournament?.sport === 'beach_volleyball' ? "fas fa-volleyball-ball" :
                          tournament?.sport === 'basketball_3x3' ? "fas fa-basketball-ball" :
                            tournament?.sport === 'badminton' ? "fas fa-feather-alt" :
                              tournament?.sport === 'table_tennis' || tournament?.sport === 'table-tennis' || tournament?.sport === 'tabletennis' ? "fas fa-table-tennis" :
                                "fas fa-chess-queen"
                }></i>
              )}

              {(details?.mainPhotoUrl || (details?.photos && details.photos.length > 0)) && (
                <div className={styles.zoomIndicator}>
                  <i className="fas fa-search-plus"></i>
                </div>
              )}
            </div>
          </div>

          {details && (details.firstPlace || details.secondPlace || details.thirdPlace || details.firstPlaceTeam || details.secondPlaceTeam || details.thirdPlaceTeam || details.mvpId) && (
            <TournamentPodium
              firstPlace={isTeamSport ? details.firstPlaceTeam : details.firstPlace}
              secondPlace={isTeamSport ? details.secondPlaceTeam : details.secondPlace}
              thirdPlace={isTeamSport ? details.thirdPlaceTeam : details.thirdPlace}
              mvpId={['chess', 'checkers', 'athletics', 'table_tennis', 'tabletennis', 'badminton', 'beach_volleyball', 'beachvolleyball', 'armwrestling'].includes(tournament?.sport?.toLowerCase() || '') ? null : details.mvpId}
              mvpName={['chess', 'checkers', 'athletics', 'table_tennis', 'tabletennis', 'badminton', 'beach_volleyball', 'beachvolleyball', 'armwrestling'].includes(tournament?.sport?.toLowerCase() || '') ? null : details.mvpName}
              isTeam={isTeamSport}
              onViewRoster={(teamId) => setViewingRosterTeamId(teamId)}
            />
          )}

          {['chess', 'checkers'].includes(tournament?.sport?.toLowerCase() || '') && isTeamSport && details && (() => {
            const findBoardForPlayer = (playerId: number) => {
              const team = (details as any).teams?.find((t: any) => t.players?.some((p: any) => p.id === playerId));
              if (!team) return 1;
              const playerIndex = team.players.findIndex((p: any) => p.id === playerId);
              return playerIndex + 1;
            };

            const b1_1st = (details as any).participants?.find((p: any) => p.boardRank === 1 && findBoardForPlayer(p.playerId) === 1);
            const b1_2nd = (details as any).participants?.find((p: any) => p.boardRank === 2 && findBoardForPlayer(p.playerId) === 1);
            const b1_3rd = (details as any).participants?.find((p: any) => p.boardRank === 3 && findBoardForPlayer(p.playerId) === 1);

            const b2_1st = (details as any).participants?.find((p: any) => p.boardRank === 1 && findBoardForPlayer(p.playerId) === 2);
            const b2_2nd = (details as any).participants?.find((p: any) => p.boardRank === 2 && findBoardForPlayer(p.playerId) === 2);
            const b2_3rd = (details as any).participants?.find((p: any) => p.boardRank === 3 && findBoardForPlayer(p.playerId) === 2);

            if (!b1_1st && !b1_2nd && !b1_3rd && !b2_1st && !b2_2nd && !b2_3rd) return null;

            return (
              <div style={{ marginTop: '20px', padding: '15px', background: '#f9fafb', borderRadius: '12px', border: '1px solid #e5e7eb', marginBottom: '20px' }}>
                <h3 style={{ fontSize: '16px', marginBottom: '12px', color: '#374151', display: 'flex', alignItems: 'center' }}>
                  <i className="fas fa-chess" style={{ marginRight: '8px', color: '#4b5563' }}></i> Призери по дошках
                </h3>

                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '15px' }}>
                  {/* Board 1 */}
                  <div>
                    <h4 style={{ fontSize: '14px', marginBottom: '8px', color: '#6b7280' }}>Дошка 1</h4>
                    <ul style={{ listStyle: 'none', padding: 0 }}>
                      {b1_1st && <li style={{ fontSize: '14px', marginBottom: '4px' }}>🥇 1. {b1_1st.playerName}</li>}
                      {b1_2nd && <li style={{ fontSize: '14px', marginBottom: '4px' }}>🥈 2. {b1_2nd.playerName}</li>}
                      {b1_3rd && <li style={{ fontSize: '14px', marginBottom: '4px' }}>🥉 3. {b1_3rd.playerName}</li>}
                    </ul>
                  </div>

                  {/* Board 2 */}
                  <div>
                    <h4 style={{ fontSize: '14px', marginBottom: '8px', color: '#6b7280' }}>Дошка 2</h4>
                    <ul style={{ listStyle: 'none', padding: 0 }}>
                      {b2_1st && <li style={{ fontSize: '14px', marginBottom: '4px' }}>🥇 1. {b2_1st.playerName}</li>}
                      {b2_2nd && <li style={{ fontSize: '14px', marginBottom: '4px' }}>🥈 2. {b2_2nd.playerName}</li>}
                      {b2_3rd && <li style={{ fontSize: '14px', marginBottom: '4px' }}>🥉 3. {b2_3rd.playerName}</li>}
                    </ul>
                  </div>
                </div>
              </div>
            );
          })()}

          <div className={styles.infoCard}>
            <h2><i className="fas fa-info-circle"></i> Про змагання</h2>
            <div className={styles.infoGrid}>
              <div className={styles.infoSection}>
                <h3><i className="fas fa-align-left"></i> Опис</h3>
                <p style={{ whiteSpace: 'pre-line' }}>{tournament?.description}</p>
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
                    <div style={{ color: '#4b5563', fontSize: '0.95rem', whiteSpace: 'pre-line', padding: '4px 0' }}>
                      {(details as any).referees}
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

          {details?.level === 'CollegeGroup' && (
            <div className={styles.statsGridCombined}>
              {isTeamSport ? (
                <>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}><i className="fas fa-users-viewfinder"></i></div>
                    <div className={styles.statValue}>{actualParticipants}</div>
                    <div className={styles.statLabel}>Всього команд</div>
                  </div>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}><i className="fas fa-user-group"></i></div>
                    <div className={styles.statValue}>{details?.teams?.reduce((acc: number, t: any) => acc + (t.players?.length || 0), 0) || 0}</div>
                    <div className={styles.statLabel}>Всього гравців</div>
                  </div>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}><i className="fas fa-calendar-check"></i></div>
                    <div className={styles.statValue}>{allGamesOrMatches.length}</div>
                    <div className={styles.statLabel}>Всього матчів</div>
                  </div>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}><i className="fas fa-chart-line"></i></div>
                    <div className={styles.statValue}>{playedGames}</div>
                    <div className={styles.statLabel}>Зіграно матчів</div>
                  </div>
                </>
              ) : (
                <>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}>
                      <i className={
                        tournament?.sport === 'badminton' ||
                          tournament?.sport === 'table_tennis' ||
                          tournament?.sport === 'tabletennis' ||
                          tournament?.sport === 'armwrestling' ||
                          tournament?.sport === 'athletics'
                          ? (tournament?.sport === 'athletics' ? "fas fa-running" : "fas fa-project-diagram")
                          : "fas fa-chess-board"
                      }></i>
                    </div>
                    <div className={styles.statValue}>{totalRounds || '-'}</div>
                    <div className={styles.statLabel}>Всього турів</div>
                  </div>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}><i className="fas fa-users"></i></div>
                    <div className={styles.statValue}>{actualParticipants}</div>
                    <div className={styles.statLabel}>{participantLabel}</div>
                  </div>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}>
                      <i className={
                        tournament?.sport === 'badminton' ? "fas fa-feather-alt" :
                          tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? "fas fa-table-tennis" :
                            tournament?.sport === 'armwrestling' ? "fas fa-hand-rock" :
                              "fas fa-venus-mars"
                      }></i>
                    </div>
                    <div className={styles.statValue}>
                      {(() => {
                        const cat = (details as any)?.category || tournament?.category;
                        if (cat === 'Men' || cat === 'Чоловіки') return 'Чоловіки';
                        if (cat === 'Women' || cat === 'Жінки') return 'Жінки';
                        return 'Змішане';
                      })()}
                    </div>
                    <div className={styles.statLabel}>Категорія</div>
                  </div>
                  <div className={styles.statItemCombined}>
                    <div className={styles.statIcon}><i className="fas fa-chart-line"></i></div>
                    <div className={styles.statValue}>{playedGames}</div>
                    <div className={styles.statLabel}>Зіграно {tournament?.sport === 'armwrestling' || tournament?.sport === 'badminton' || tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? 'матчів' : 'партій'}</div>
                  </div>
                </>
              )}
            </div>
          )}

          {(() => {
            const mandatoryTeamSports = ['volleyball', 'beach_volleyball', 'football', 'basketball_3_3', 'basketball_3x3', 'basketball3x3'];
            const isTeam = !!(details as any)?.isTeam || mandatoryTeamSports.includes(sportKey);

            if (details && isTeam) {
              return (
                <TeamSportsTournamentView
                  sport={sportKey as any}
                  details={details}
                  isAdmin={isAdmin}
                  isFizorg={isFizorg}
                  userGroup={currentUserPlayer?.group}
                  onRefresh={() => fetchTournament(true)}
                />
              );
            }

            if (sportKey === 'armwrestling' && details) {
              return (
                <ArmwrestlingTournamentView
                  details={details as any}
                  isAdmin={isAdmin}
                  onRefresh={() => fetchTournament(true)}
                />
              );
            }

            if (sportKey === 'athletics' || sportKey === 'chess' || sportKey === 'checkers' || sportKey === 'badminton' || sportKey === 'tabletennis' || sportKey === 'table_tennis') {
              return (
                <>


                  <div className={styles.tabsContainer}>
                    <div className={styles.tabsHeader}>
                      <button
                        className={`${styles.tabButton} ${activeTab === 'standings' ? styles.active : ''}`}
                        onClick={() => setActiveTab('standings')}
                      >
                        <i className="fas fa-chart-simple"></i> {sportKey === 'athletics' ? 'Результати змагань' : 'Таблиця змагань'}
                      </button>
                      {sportKey !== 'athletics' && (
                      <button
                        className={`${styles.tabButton} ${activeTab === 'games' ? styles.active : ''}`}
                        onClick={() => setActiveTab('games')}
                      >
                        <i className={tournament?.sport === 'badminton' ? "fas fa-table-tennis-paddle-ball" : tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? "fas fa-table-tennis" : "fas fa-chess-board"}></i> {tournament?.sport === 'badminton' || tournament?.sport === 'table_tennis' || tournament?.sport === 'tabletennis' ? 'Всі матчі' : 'Всі партії'}
                      </button>
                      )}
                      <button
                        className={`${styles.tabButton} ${activeTab === 'participants' ? styles.active : ''}`}
                        onClick={() => setActiveTab('participants')}
                      >
                        <i className="fas fa-users"></i> Учасники {pendingParticipantsCount > 0 && (
                          <span style={{
                            marginLeft: '6px',
                            background: '#ef4444',
                            color: '#fff',
                            padding: '2px 8px',
                            borderRadius: '12px',
                            fontSize: '11px',
                            fontWeight: 'bold',
                            boxShadow: '0 2px 4px rgba(239,68,68,0.3)'
                          }}>
                            {pendingParticipantsCount}
                          </span>
                        )}
                      </button>
                      {sportKey !== 'athletics' && (
                        <button
                          className={`${styles.tabButton} ${activeTab === 'schedule' ? styles.active : ''}`}
                          onClick={() => setActiveTab('schedule')}
                        >
                          <i className="fas fa-calendar-week"></i> Розклад турів
                        </button>
                      )}
                      {sportKey !== 'athletics' && details?.level !== 'Regional' && details?.level !== 'National' && tournament?.level !== 'Regional' && tournament?.level !== 'National' && (
                        <button
                          className={`${styles.tabButton} ${activeTab === 'voting' ? styles.active : ''}`}
                          onClick={() => setActiveTab('voting')}
                        >
                          <i className="fas fa-poll"></i> Голосування
                        </button>
                      )}
                    </div>

                    {activeTab === 'voting' && (
                      <div className={styles.tabContent}>
                        <div className={styles.votingSection} style={{ background: '#fff', padding: '15px', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.05)' }}>
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
                            {(sortedPlayers || []).map((player: any) => {
                              const stats = voteStats?.optionStats?.find((s: any) => s.id === player.playerId);
                              const votes = stats?.votes || 0;
                              const percentage = stats?.percentage || 0;
                              return (
                                <div key={player.playerId} style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                                  <div style={{ width: '150px', fontSize: '14px', fontWeight: 500, color: '#334155' }}>
                                    {player.playerName}
                                  </div>
                                  <div style={{ flex: 1, height: '8px', background: '#e2e8f0', borderRadius: '4px', overflow: 'hidden' }}>
                                    <div style={{ width: `${percentage}%`, height: '100%', background: '#3b82f6' }}></div>
                                  </div>
                                  <div style={{ width: '100px', fontSize: '12px', color: '#64748b', textAlign: 'right' }}>
                                    {votes} ({percentage}%)
                                  </div>
                                  {!voteStats?.hasVoted && !isVoting && new Date(tournament?.rawDate || tournament?.date || '') > new Date() && (
                                    <button
                                      onClick={() => handleVote(undefined, player.playerId)}
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
                      </div>
                    )}

                    {activeTab === 'standings' && (
                      <div className={styles.tabContent}>
                        {sportKey === 'athletics' ? (
                          <AthleticsTournamentView
                            details={details as any}
                            isAdmin={isAdmin}
                            isFizorg={isFizorg}
                            userGroup={currentUserPlayer?.group ?? undefined}
                            onRefresh={() => fetchTournament(true)}
                          />
                        ) : (
                        <div className={styles.tableWrapper}>
                          <table className={styles.dataTable}>
                            <thead>
                              <tr>
                                <th>Місце</th>
                                {(tournament?.sport?.toLowerCase() === 'chess' || tournament?.sport?.toLowerCase() === 'checkers') && details?.level !== 'CollegeGroup' && (
                                  <th>Дошка</th>
                                )}
                                <th>Учасник</th>
                                <th>Група</th>
                                {isChess ? (
                                  <>
                                    <th>Рейтинг</th>
                                    <th>Очки</th>
                                  </>
                                ) : (
                                  <th>Очки</th>
                                )}
                                <th style={{ textAlign: 'center' }}>Статус</th>
                                {(isAdmin || isFizorg) && <th style={{ textAlign: 'center' }}>Дії</th>}
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
                                      {(tournament?.sport?.toLowerCase() === 'chess' || tournament?.sport?.toLowerCase() === 'checkers') && details?.level !== 'CollegeGroup' && (
                                        <td>
                                          <input type="number" value={editingParticipantData.boardRank || ''} placeholder="Дошка" onChange={(e) => setEditingParticipantData({ ...editingParticipantData, boardRank: e.target.value ? parseInt(e.target.value) : undefined })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                        </td>
                                      )}
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
                                          (p.playerId && !p.isGuest) ? (
                                            <Link to={`/players/${p.playerId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
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
                                          <td>
                                            <input type="number" step="0.5" value={editingParticipantData.points} onChange={(e) => setEditingParticipantData({ ...editingParticipantData, points: parseFloat(e.target.value) || 0 })} style={{ width: '60px', padding: '4px', borderRadius: '4px', border: '1px solid #ccc' }} />
                                          </td>
                                        </>
                                      )}
                                      <td style={{ textAlign: 'center' }}>-</td>
                                      <td style={{ textAlign: 'center' }}>
                                        <button type="button" onClick={() => handleSaveParticipantEdit(p.playerId)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', marginRight: '4px' }}><i className="fas fa-check"></i></button>
                                        <button type="button" onClick={handleCancelParticipantEdit} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}><i className="fas fa-times"></i></button>
                                      </td>
                                    </>
                                  ) : (
                                    <>
                                      <td>{p.rank || idx + 1}</td>
                                      {(tournament?.sport?.toLowerCase() === 'chess' || tournament?.sport?.toLowerCase() === 'checkers') && details?.level !== 'CollegeGroup' && (
                                        <td>{p.boardRank || '-'}</td>
                                      )}
                                      <td>
                                        {p.playerId ? (
                                          !p.isGuest ? (
                                            <Link to={`/players/${p.playerId}`} style={{ color: '#3b82f6', textDecoration: 'none', fontWeight: 'bold' }}>
                                              {p.playerName}
                                            </Link>
                                          ) : (
                                            <span style={{ fontWeight: 'bold' }}>{p.playerName}</span>
                                          )
                                        ) : (
                                          <strong>{p.playerName}</strong>
                                        )}
                                      </td>
                                      <td>{p.groupName || '—'}</td>
                                      {isChess ? (
                                        <>
                                          <td>{p.rating ?? 0}</td>
                                          <td><strong>{p.points ?? 0}</strong></td>
                                        </>
                                      ) : (
                                        <td><strong>{p.points ?? 0}</strong></td>
                                      )}
                                      <td style={{ textAlign: 'center' }}>
                                        {p.status && (
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
                                            {p.status === RegistrationStatus.Approved ? 'Схвалено' : p.status === RegistrationStatus.Pending ? 'Очікує' : 'Відхилено'}
                                          </span>
                                        )}
                                      </td>
                                      {(isAdmin || isFizorg) && (
                                        <td style={{ textAlign: 'center' }}>
                                          <div style={{ display: 'flex', gap: '6px', flexWrap: 'wrap', justifyContent: 'center' }}>
                                            {p.status === RegistrationStatus.Pending && (
                                              <>
                                                <button
                                                  type="button"
                                                  onClick={() => handleApproveParticipant(p.playerId, p.playerName)}
                                                  style={{ padding: '6px 10px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(16,185,129,0.2)' }}
                                                  title="Схвалити"
                                                  onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                                                  onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                                                >
                                                  <i className="fas fa-check" style={{ fontSize: '14px' }}></i>
                                                </button>
                                                <button
                                                  type="button"
                                                  onClick={() => handleRejectParticipant(p.playerId, p.playerName)}
                                                  style={{ padding: '6px 10px', background: '#f59e0b', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(245,158,11,0.2)' }}
                                                  title="Відхилити"
                                                  onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                                                  onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                                                >
                                                  <i className="fas fa-times" style={{ fontSize: '14px' }}></i>
                                                </button>
                                              </>
                                            )}
                                            <button
                                              type="button"
                                              onClick={() => handleEditParticipantClick(p)}
                                              style={{ padding: '6px 10px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(59,130,246,0.2)' }}
                                              title="Редагувати"
                                              onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                                              onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                                            >
                                              <i className="fas fa-edit" style={{ fontSize: '14px' }}></i>
                                            </button>
                                            <button
                                              type="button"
                                              onClick={() => handleRemoveParticipant(p.playerId, p.playerName)}
                                              style={{ padding: '6px 10px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 2px 4px rgba(239,68,68,0.2)' }}
                                              title="Видалити"
                                              onMouseOver={(e) => e.currentTarget.style.transform = 'translateY(-1px)'}
                                              onMouseOut={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                                            >
                                              <i className="fas fa-trash" style={{ fontSize: '14px' }}></i>
                                            </button>
                                          </div>
                                        </td>
                                      )}
                                    </>
                                  )}
                                </tr>
                              )) : (
                                <tr><td colSpan={7}>Ще немає даних</td></tr>
                              )}
                            </tbody>
                          </table>
                        </div>
                        )}
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
                                          {game.whitePlayerId ? (
                                            <Link to={`/players/${game.whitePlayerId}`} style={{ color: '#3b82f6', textDecoration: 'none' }}>
                                              {game.whitePlayerName}
                                            </Link>
                                          ) : (
                                            game.whitePlayerName || '—'
                                          )}
                                        </td>
                                        <td>
                                          {game.blackPlayerId ? (
                                            <Link to={`/players/${game.blackPlayerId}`} style={{ color: '#3b82f6', textDecoration: 'none' }}>
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
                                <div style={{ display: 'flex', flexDirection: 'column' }}>
                                  <label style={{ marginBottom: '4px', fontSize: '12px' }}>Стать</label>
                                  <select
                                    value={newPlayer.gender}
                                    onChange={(e) => setNewPlayer({ ...newPlayer, gender: e.target.value as Gender })}
                                    style={{ padding: '8px', borderRadius: '6px', border: '1px solid #ccc' }}
                                  >
                                    <option value="None">Не вказано</option>
                                    <option value="Male">Чоловік</option>
                                    <option value="Female">Жінка</option>
                                  </select>
                                </div>
                              </div>
                              <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                                <button type="button" onClick={handleAddPlayer} style={{ padding: '10px 24px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}>
                                  {newPlayer.existingPlayerId ? 'Додати існуючого гравця' : newPlayer.userId ? 'Створити профіль гравця та додати' : 'Створити гравця та додати'}
                                </button>
                                {(newPlayer.userId || newPlayer.existingPlayerId) && (
                                  <button
                                    onClick={() => setNewPlayer({ ...newPlayer, userId: null, existingPlayerId: null, firstName: '', lastName: '', group: '', gender: 'None' })}
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
                                <th style={{ textAlign: 'center' }}>Статус</th>
                                {isAdmin && <th style={{ textAlign: 'center' }}>Дії</th>}
                              </tr>
                            </thead>
                            <tbody>
                              {sortedPlayers.length > 0 ? sortedPlayers.map((p: any, idx: number) => (
                                <tr key={p.id}>
                                  <td>{idx + 1}</td>
                                  <td>
                                    {p.playerId && !p.isGuest ? (
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
                                  <td style={{ textAlign: 'center' }}>
                                    <span style={{
                                      fontSize: '11px',
                                      fontWeight: 'bold',
                                      padding: '4px 8px',
                                      borderRadius: '4px',
                                      background: p.status === RegistrationStatus.Approved ? '#ecfdf5' : p.status === RegistrationStatus.Pending ? '#fffbeb' : '#fef2f2',
                                      color: p.status === RegistrationStatus.Approved ? '#10b981' : p.status === RegistrationStatus.Pending ? '#f59e0b' : '#ef4444',
                                      border: `1px solid ${p.status === RegistrationStatus.Approved ? '#bdf4e4' : p.status === RegistrationStatus.Pending ? '#fef3c7' : '#fee2e2'}`,
                                      display: 'inline-block'
                                    }}>
                                      {p.status === RegistrationStatus.Approved ? 'Підтверджено' : p.status === RegistrationStatus.Pending ? 'Очікує' : 'Відхилено'}
                                    </span>
                                  </td>
                                  {isAdmin && (
                                    <td style={{ textAlign: 'center' }}>
                                      <div style={{ display: 'flex', gap: '8px', justifyContent: 'center' }}>
                                        {p.status === RegistrationStatus.Pending && (
                                          <>
                                            <button onClick={() => handleApproveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Підтвердити">
                                              <i className="fas fa-check"></i>
                                            </button>
                                            <button onClick={() => handleRejectParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#f59e0b', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Відхилити">
                                              <i className="fas fa-times"></i>
                                            </button>
                                          </>
                                        )}
                                        <button onClick={() => handleRemoveParticipant(p.playerId, p.playerName)} style={{ padding: '4px 8px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }} title="Видалити зовсім">
                                          <i className="fas fa-trash"></i>
                                        </button>
                                      </div>
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
                <h2>Галерея змагання</h2>
              </div>
              <div className={styles.photoGrid}>
                {details.photos.map((photo: any, index: number) => (
                  <div
                    key={photo.id}
                    className={styles.galleryItem}
                    onClick={() => setSelectedPhotoIndex(index)}
                  >
                    {isVideo(photo.url) ? (
                      <div className={styles.videoThumbnail} style={{ backgroundColor: '#1e293b', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <i className="fas fa-video" style={{ fontSize: '3rem', color: '#475569' }}></i>
                        <div className={styles.videoPlayIcon}><i className="fas fa-play"></i></div>
                      </div>
                    ) : (
                      <img src={getImageUrl(photo.url)} alt={`Фото ${index + 1}`} loading="lazy" decoding="async" />
                    )}
                    <div className={styles.photoOverlay}>
                      <i className="fas fa-search-plus"></i>
                      {isAdmin && (
                        <button
                          className={styles.directDeleteBtn}
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDeletePhoto(photo.id);
                          }}
                          title="Видалити"
                        >
                          <i className="fas fa-trash-alt"></i>
                        </button>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* User Media Section */}
          <div className={styles.gallerySection} style={{ marginTop: '40px' }}>
            <div className={styles.galleryHeader}>
              <i className="fas fa-camera-retro"></i>
              <h2>Галерея від учасників та вболівальників</h2>
            </div>

            {/* Upload Form (only for logged in users) */}
            {user ? (
              !isAdmin && (
                <div style={{ background: '#f8fafc', padding: '20px', borderRadius: '12px', marginBottom: '24px', border: '1px solid #e2e8f0' }}>
                  <h3 style={{ fontSize: '16px', fontWeight: 'bold', marginBottom: '12px', color: '#1e293b' }}>Поділіться своїми фото чи відео</h3>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: '16px', alignItems: 'center' }}>
                    <input
                      type="file"
                      accept="image/*,video/*"
                      onChange={(e) => setUserMediaFile(e.target.files?.[0] || null)}
                      style={{ padding: '8px', borderRadius: '6px', border: '1px solid #cbd5e1', background: '#fff' }}
                    />
                    <input
                      type="text"
                      placeholder="Ваш коментар (опціонально)"
                      value={userMediaComment}
                      onChange={(e) => setUserMediaComment(e.target.value)}
                      style={{ flex: 1, minWidth: '200px', padding: '10px', borderRadius: '6px', border: '1px solid #cbd5e1' }}
                    />
                    <button
                      onClick={async () => {
                        if (!userMediaFile) {
                          toast.error("Будь ласка, оберіть файл");
                          return;
                        }

                        const MAX_FILE_SIZE = 500 * 1024 * 1024; // 500 MB
                        if (userMediaFile.size > MAX_FILE_SIZE) {
                          toast.error("Файл занадто великий. Максимум 500 МБ.");
                          return;
                        }

                        try {
                          setIsUploadingUserMedia(true);
                          await submitUserEventMedia(id!, userMediaFile, userMediaComment);
                          toast.success("Медіа надіслано на модерацію!");
                          setUserMediaFile(null);
                          setUserMediaComment('');
                          // Reset file input
                          const input = document.querySelector('input[type="file"]') as HTMLInputElement;
                          if (input) input.value = '';
                        } catch (err: any) {
                          console.error(err);
                          const errorMsg = typeof err.response?.data === 'string'
                            ? err.response.data
                            : (err.response?.data?.message || "Помилка при відправці");
                          toast.error(errorMsg);
                        } finally {
                          setIsUploadingUserMedia(false);
                        }
                      }}
                      disabled={isUploadingUserMedia || !userMediaFile}
                      style={{
                        padding: '10px 20px',
                        background: '#2563eb',
                        color: '#fff',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer',
                        fontWeight: 'bold',
                        opacity: (isUploadingUserMedia || !userMediaFile) ? 0.5 : 1
                      }}
                    >
                      {isUploadingUserMedia ? "Відправка..." : "Надіслати"}
                    </button>
                  </div>
                  <p style={{ fontSize: '12px', color: '#64748b', marginTop: '8px' }}>
                    * Всі медіа проходять модерацію адміністратором перед публікацією.
                  </p>
                </div>
              )
            ) : (
              <div style={{ background: '#f8fafc', padding: '15px', borderRadius: '12px', marginBottom: '24px', textAlign: 'center', color: '#64748b' }}>
                <i className="fas fa-info-circle" style={{ marginRight: '8px' }}></i>
                Будь ласка, <Link to="/login" style={{ color: '#2563eb', textDecoration: 'underline' }}>авторизуйтесь</Link>, щоб додати свої фото чи відео.
              </div>
            )}

            {/* Display Grid */}
            {userMedia && userMedia.length > 0 ? (
              <div className={styles.photoGrid}>
                {userMedia.map((media: any, index: number) => (
                  <div key={media.id} className={styles.galleryItem} onClick={() => setSelectedUserPhotoIndex(index)} style={{ cursor: 'pointer', height: 'auto', background: '#fff', borderRadius: '12px', overflow: 'hidden', boxShadow: '0 2px 4px rgba(0,0,0,0.05)', border: '1px solid #e2e8f0' }}>
                    <div style={{ height: '200px', overflow: 'hidden', position: 'relative' }}>
                      {media.type === 'video' ? (
                        <div style={{ width: '100%', height: '100%', backgroundColor: '#1e293b', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                          <i className="fas fa-video" style={{ fontSize: '3rem', color: '#475569' }}></i>
                          <div className={styles.videoPlayIcon}><i className="fas fa-play"></i></div>
                        </div>
                      ) : (
                        <img src={getImageUrl(media.url)} alt="User media" loading="lazy" decoding="async" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                      )}

                      {isAdmin && (
                        <div style={{ position: 'absolute', top: '8px', right: '8px' }}>
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              handleDeleteUserMedia(media.id);
                            }}
                            style={{
                              background: 'rgba(239, 68, 68, 0.9)',
                              color: 'white',
                              border: 'none',
                              borderRadius: '50%',
                              width: '32px',
                              height: '32px',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              cursor: 'pointer',
                              boxShadow: '0 2px 4px rgba(0,0,0,0.2)'
                            }}
                            title="Видалити"
                          >
                            <i className="fas fa-trash-alt"></i>
                          </button>
                        </div>
                      )}
                    </div>
                    <div style={{ padding: '12px' }}>
                      <div style={{ fontSize: '14px', fontWeight: 'bold', color: '#1e293b', marginBottom: '4px' }}>
                        <i className="fas fa-user" style={{ marginRight: '6px', color: '#64748b' }}></i>
                        {media.authorName || 'Анонім'}
                      </div>
                      {media.comment && (
                        <div style={{ fontSize: '13px', color: '#475569', fontStyle: 'italic' }}>
                          "{media.comment}"
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div style={{ textAlign: 'center', color: '#64748b', padding: '40px 0' }}>
                <i className="fas fa-camera" style={{ fontSize: '40px', marginBottom: '12px', color: '#cbd5e1' }}></i>
                <p>Ще немає медіа від учасників. Будьте першим!</p>
              </div>
            )}
          </div>

          {/* Admin Moderation Section */}
          {isAdmin && (
            <div className={styles.gallerySection} style={{ marginTop: '40px', background: '#fffbeb', padding: '20px', borderRadius: '12px', border: '1px solid #fef3c7' }}>
              <div className={styles.galleryHeader}>
                <i className="fas fa-tasks" style={{ color: '#f59e0b' }}></i>
                <h2 style={{ color: '#b45309' }}>
                  Модерація медіа від користувачів
                  {pendingUserMedia.length > 0 && (
                    <span style={{ marginLeft: '8px', background: '#ef4444', color: '#fff', padding: '2px 8px', borderRadius: '12px', fontSize: '14px' }}>
                      {pendingUserMedia.length}
                    </span>
                  )}
                </h2>
              </div>

              {pendingUserMedia && pendingUserMedia.length > 0 ? (
                <div className={styles.photoGrid}>
                  {pendingUserMedia.map((media: any) => (
                    <div key={media.id} style={{ background: '#fff', borderRadius: '12px', overflow: 'hidden', boxShadow: '0 2px 4px rgba(0,0,0,0.05)', border: '1px solid #e2e8f0', display: 'flex', flexDirection: 'column' }}>
                      <div style={{ height: '200px', overflow: 'hidden', position: 'relative' }}>
                        {media.type === 'video' ? (
                          <div style={{ width: '100%', height: '100%', backgroundColor: '#1e293b', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                            <i className="fas fa-video" style={{ fontSize: '3rem', color: '#475569' }}></i>
                            <div className={styles.videoPlayIcon}><i className="fas fa-play"></i></div>
                          </div>
                        ) : (
                          <img src={getImageUrl(media.url)} alt="User media" loading="lazy" decoding="async" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                        )}
                      </div>
                      <div style={{ padding: '12px' }}>
                        <div style={{ fontSize: '14px', fontWeight: 'bold', color: '#1e293b', marginBottom: '4px' }}>
                          <i className="fas fa-user" style={{ marginRight: '6px', color: '#64748b' }}></i>
                          {media.authorName || 'Анонім'}
                        </div>
                        {media.comment && (
                          <div style={{ fontSize: '13px', color: '#475569', fontStyle: 'italic', marginBottom: '8px' }}>
                            "{media.comment}"
                          </div>
                        )}
                        <div style={{ display: 'flex', gap: '8px' }}>
                          <button
                            onClick={async () => {
                              try {
                                await updateUserEventMediaStatus(media.id, ModerationStatus.Approved);
                                toast.success("Схвалено!");
                                fetchUserMedia();
                                fetchPendingUserMedia();
                              } catch (err) {
                                toast.error("Помилка");
                              }
                            }}
                            style={{ flex: 1, padding: '6px', background: '#10b981', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
                          >
                            Схвалити
                          </button>
                          <button
                            onClick={async () => {
                              try {
                                await updateUserEventMediaStatus(media.id, ModerationStatus.Rejected);
                                toast.success("Відхилено!");
                                fetchPendingUserMedia();
                              } catch (err) {
                                toast.error("Помилка");
                              }
                            }}
                            style={{ flex: 1, padding: '6px', background: '#ef4444', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
                          >
                            Відхилити
                          </button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div style={{ textAlign: 'center', color: '#64748b', padding: '20px 0' }}>
                  <p>Немає медіафайлів, що потребують модерації.</p>
                </div>
              )}
            </div>
          )}

          {/* Main Photo Lightbox */}
          {isMainPhotoLightboxOpen && (details?.mainPhotoUrl || (details?.photos && details.photos.length > 0)) && (
            <div className={styles.lightboxOverlay} onClick={() => setIsMainPhotoLightboxOpen(false)}>
              <div className={styles.lightboxContent} onClick={(e) => e.stopPropagation()}>
                <button className={styles.lightboxClose} onClick={() => setIsMainPhotoLightboxOpen(false)}>
                  <i className="fas fa-times"></i>
                </button>
                <img
                  src={getImageUrl(details?.mainPhotoUrl || details?.photos[0].url)}
                  alt="Tournament Main Photo Fullscreen"
                  className={styles.lightboxImage}
                />
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

                {isAdmin && (
                  <button
                    className={styles.lightboxDeleteBtn}
                    onClick={(e) => {
                      e.stopPropagation();
                      if (details.photos[selectedPhotoIndex]) {
                        handleDeletePhoto(details.photos[selectedPhotoIndex].id);
                        setSelectedPhotoIndex(null);
                      }
                    }}
                    title="Видалити медіафайл"
                  >
                    <i className="fas fa-trash-alt"></i> Видалити
                  </button>
                )}

                {isVideo(details.photos[selectedPhotoIndex].url) ? (
                  <video
                    src={getImageUrl(details.photos[selectedPhotoIndex].url)}
                    controls
                    preload="none"
                    playsInline
                    className={styles.lightboxVideo}
                  />
                ) : (
                  <img
                    src={getImageUrl(details.photos[selectedPhotoIndex].url)}
                    alt="Tournament Fullscreen"
                    className={styles.lightboxImage}
                  />
                )}

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

          {/* User Media Lightbox / Fullscreen Preview */}
          {selectedUserPhotoIndex !== null && userMedia && (
            <div className={styles.lightboxOverlay} onClick={() => setSelectedUserPhotoIndex(null)}>
              <div className={styles.lightboxContent} onClick={(e) => e.stopPropagation()}>
                <button className={styles.lightboxClose} onClick={() => setSelectedUserPhotoIndex(null)}>
                  <i className="fas fa-times"></i>
                </button>

                {isAdmin && (
                  <button
                    className={styles.lightboxDeleteBtn}
                    onClick={(e) => {
                      e.stopPropagation();
                      if (userMedia[selectedUserPhotoIndex]) {
                        handleDeleteUserMedia(userMedia[selectedUserPhotoIndex].id);
                      }
                    }}
                    title="Видалити медіафайл"
                  >
                    <i className="fas fa-trash-alt"></i> Видалити
                  </button>
                )}

                {userMedia[selectedUserPhotoIndex].type === 'video' ? (
                  <video
                    src={getImageUrl(userMedia[selectedUserPhotoIndex].url)}
                    controls
                    preload="none"
                    playsInline
                    className={styles.lightboxVideo}
                  />
                ) : (
                  <img
                    src={getImageUrl(userMedia[selectedUserPhotoIndex].url)}
                    alt="User Fullscreen"
                    className={styles.lightboxImage}
                  />
                )}

                <div style={{ position: 'absolute', bottom: '20px', left: '0', right: '0', textAlign: 'center', color: '#fff', textShadow: '0 2px 4px rgba(0,0,0,0.8)' }}>
                  <div style={{ fontSize: '18px', fontWeight: 'bold' }}>
                    <i className="fas fa-user" style={{ marginRight: '6px' }}></i>
                    {userMedia[selectedUserPhotoIndex].authorName || 'Анонім'}
                  </div>
                  {userMedia[selectedUserPhotoIndex].comment && (
                    <div style={{ fontSize: '16px', fontStyle: 'italic', marginTop: '8px' }}>
                      "{userMedia[selectedUserPhotoIndex].comment}"
                    </div>
                  )}
                </div>

                <div className={styles.lightboxNav}>
                  <button
                    className={styles.lightboxBtn}
                    onClick={(e) => {
                      e.stopPropagation();
                      setSelectedUserPhotoIndex((selectedUserPhotoIndex - 1 + userMedia.length) % userMedia.length);
                    }}
                  >
                    <i className="fas fa-chevron-left"></i> Назад
                  </button>
                  <button
                    className={styles.lightboxBtn}
                    onClick={(e) => {
                      e.stopPropagation();
                      setSelectedUserPhotoIndex((selectedUserPhotoIndex + 1) % userMedia.length);
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
                        {!isVolleyball && <option value="Mixed">Змішане (Mixed)</option>}
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
                    {['football', 'badminton', 'basketball_3x3', 'basketball3x3', 'table_tennis', 'tabletennis'].includes(sportKey) && (
                      <div className={styles.formGroup}>
                        <label>Система проведення</label>
                        <select value={(editTournamentData as any).tournamentType || 'RoundRobin'} onChange={(e) => setEditTournamentData({ ...editTournamentData, tournamentType: e.target.value } as any)}>
                          <option value="RoundRobin">Кругова система</option>
                          <option value="SingleElimination">Олімпійська система</option>
                          <option value="DoubleElimination">Двоєступінчаста система</option>
                          <option value="GroupsPlayoff">Груповий етап + плей-офф</option>
                        </select>
                      </div>
                    )}
                    {editTournamentData.level === 'CollegeGroup' && (
                      <div className={styles.formGroup} style={{ gridColumn: '1 / -1', display: 'flex', gap: '20px' }}>
                        {!isMandatoryTeamSport && sportKey !== 'athletics' && (
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
                        )}

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
                        {!isMandatoryTeamSport && sportKey !== 'athletics' && (
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
                        )}
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
                      <label>Призи</label>
                      <textarea rows={5} placeholder="Опишіть призовий фонд..." value={editTournamentData.prizes} onChange={(e) => setEditTournamentData({ ...editTournamentData, prizes: e.target.value })} />
                    </div>
                    <div className={styles.formGroup} style={{ gridColumn: '1 / -1' }}>
                      <label>Розклад змагань</label>
                      <textarea rows={5} placeholder="Введіть детальний розклад (кожен пункт з нового рядка)..." value={editTournamentData.schedule} onChange={(e) => setEditTournamentData({ ...editTournamentData, schedule: e.target.value })} />
                    </div>

                    <div className={styles.mainPhotoSection} style={{ gridColumn: '1 / -1' }}>
                      <label><i className="fas fa-star"></i> Головне фото (афіша)</label>
                      <div className={styles.mainPhotoContainer}>
                        {details?.mainPhotoUrl ? (
                          <div className={styles.mainPhotoPreview}>
                            <img src={getImageUrl(details.mainPhotoUrl)} alt="Main" loading="lazy" />
                            <div className={styles.mainPhotoActions}>
                              <button
                                className={styles.changeMainPhotoBtn}
                                onClick={() => {
                                  const input = document.createElement('input');
                                  input.type = 'file';
                                  input.accept = 'image/*';
                                  input.onchange = async (e: any) => {
                                    const file = e.target.files?.[0];
                                    if (file) {
                                      try {
                                        setIsUploading(true);
                                        const response = await setMainPhoto(id!, file);
                                        toast.success("Головне фото оновлено");
                                        setDetails((prev: any) => ({ ...prev, mainPhotoUrl: response.url }));
                                      } catch (err) {
                                        toast.error("Помилка при оновленні");
                                      } finally {
                                        setIsUploading(false);
                                      }
                                    }
                                  };
                                  input.click();
                                }}
                              >
                                <i className="fas fa-edit"></i> Змінити афішу
                              </button>
                              <button
                                className={styles.removeMainPhotoBtn}
                                onClick={async () => {
                                  if (!window.confirm("Видалити афішу? Змагання буде використовувати перше фото з галереї.")) return;
                                  try {
                                    // We can just call an update with null or a dedicated delete
                                    // For now let's just use the markAsMain with null logic if supported or just add a simple setter
                                    await api.post(`/event-media/${id}/mark-as-main/0`); // 0 or null logic
                                    toast.success("Афішу видалено");
                                    setDetails((prev: any) => ({ ...prev, mainPhotoUrl: null }));
                                  } catch (err) {
                                    toast.error("Помилка");
                                  }
                                }}
                              >
                                <i className="fas fa-trash"></i>
                              </button>
                            </div>
                          </div>
                        ) : (
                          <div
                            className={styles.mainPhotoPlaceholder}
                            onClick={() => {
                              const input = document.createElement('input');
                              input.type = 'file';
                              input.accept = 'image/*';
                              input.onchange = async (e: any) => {
                                const file = e.target.files?.[0];
                                if (file) {
                                  try {
                                    setIsUploading(true);
                                    const response = await setMainPhoto(id!, file);
                                    toast.success("Головне фото встановлено");
                                    setDetails((prev: any) => ({ ...prev, mainPhotoUrl: response.url }));
                                  } catch (err) {
                                    toast.error("Помилка при завантаженні");
                                  } finally {
                                    setIsUploading(false);
                                  }
                                }
                              };
                              input.click();
                            }}
                          >
                            <i className="fas fa-plus"></i>
                            <span>Завантажити афішу</span>
                          </div>
                        )}
                      </div>
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
                            <p>Перетягніть файли сюди або <span>виберіть файли</span></p>
                            <input type="file" accept="image/*,video/*" multiple onChange={handlePhotoUpload} className={styles.fileInput} />
                          </div>
                        )}
                      </div>

                      {details?.photos && details.photos.length > 0 && (
                        <div className={styles.currentPhotosGrid}>
                          {details.photos.map((photo: any) => (
                            <div key={photo.id} className={styles.photoPreviewCard}>
                              {isVideo(photo.url) ? (
                                <div style={{ width: '100%', height: '100%', backgroundColor: '#1e293b', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                                  <i className="fas fa-video" style={{ fontSize: '2rem', color: '#475569' }}></i>
                                </div>
                              ) : (
                                <img src={getImageUrl(photo.url)} alt="Tournament" loading="lazy" />
                              )}
                              <div className={styles.adminPhotoActions}>
                                <button onClick={() => handleDeletePhoto(photo.id)} className={styles.deletePhotoBtn}>
                                  <i className="fas fa-times"></i>
                                </button>
                              </div>
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

      {viewingRosterTeamId !== null && (
        <div className={styles.rosterModalOverlay} onClick={() => setViewingRosterTeamId(null)}>
          <div className={styles.rosterModal} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Склад команди: {details?.teams?.find((t: any) => t.id === viewingRosterTeamId)?.name}</h3>
              <button className={styles.closeButton} onClick={() => setViewingRosterTeamId(null)}>&times;</button>
            </div>
            <div className={styles.modalBody} style={{ maxHeight: '400px', overflowY: 'auto', padding: '20px' }}>
              <div className={styles.playersList}>
                {details?.teams?.find((t: any) => t.id === viewingRosterTeamId)?.players?.map((player: any) => (
                  <div key={player.id} className={styles.playerPickItem} style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 16px', background: '#f8fafc', borderRadius: '10px', marginBottom: '8px', border: '1px solid #e2e8f0', cursor: 'default' }}>
                    <span style={{ fontWeight: 600, color: '#1e293b' }}>
                      {player.footballShirtNumber ? `№${player.footballShirtNumber} ` : ''}
                      {player.lastName} {player.firstName}
                    </span>
                    <span style={{ color: '#64748b' }}>{player.group}</span>
                  </div>
                ))}
                {(!details?.teams?.find((t: any) => t.id === viewingRosterTeamId)?.players || details?.teams?.find((t: any) => t.id === viewingRosterTeamId)?.players.length === 0) && (
                  <p style={{ textAlign: 'center', color: '#64748b' }}>У цій команді ще немає гравців</p>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TournamentViewPage;

