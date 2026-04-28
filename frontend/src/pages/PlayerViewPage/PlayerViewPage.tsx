import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  ProfileHeader, 
  ProfileStats, 
  UserAchievements 
} from '@/widgets/profile-widgets';
import styles from './PlayerViewPage.module.css';
import { PlayerDto } from '@/shared/types/player.types';
import { getPlayerById, updatePlayer } from '@/shared/api/players.api';
import EditProfileModal from '@/widgets/profile-widgets/edit-profile-modal/EditProfileModal';
import { toast } from 'react-hot-toast';
import { useChat } from '@/app/providers/ChatProvider';
import { chatApi } from '@/shared/api/chat.api';
import { useAuth } from '@/app/providers/AuthContext';


const PlayerViewPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [playerProfile, setPlayerProfile] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { setActiveConversationId, refreshConversations } = useChat();
  const { user } = useAuth();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';


  useEffect(() => {

    const fetchPlayer = async () => {
      if (!id) return;
      
      setLoading(true);
      try {
        const data = await getPlayerById(id);
        setPlayerProfile(data);
        setError(null);
      } catch (err: any) {
        console.error("Failed to fetch player profile:", err);
        setError("Гравець не знайдений або сталася помилка завантаження");
        toast.error("Не вдалося завантажити профіль гравця");
      } finally {
        setLoading(false);
      }
    };

    fetchPlayer();
  }, [id]);

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
        <p>Завантаження профілю...</p>
      </div>
    );
  }

  if (error || !playerProfile) {
    return (
      <div className={styles.errorContainer}>
        <i className="fas fa-user-slash"></i>
        <h2>{error || "Профіль не знайдений"}</h2>
        <button className={styles.backBtn} onClick={() => navigate('/ratings')}>
          До рейтингу
        </button>
      </div>
    );
  }

  const userData = {
    name: `${playerProfile.firstName} ${playerProfile.lastName}`,
    group: playerProfile.group || '—',
    studentId: playerProfile.studentId || '—',
    faculty: playerProfile.faculty || '—',
    email: '—', // Hide email for public profiles
    phone: '—',
    city: playerProfile.city || '—',
    joinDate: '—', // Could fetch from user data if needed
    bio: playerProfile.bio || 'Інформація відсутня',
    badges: playerProfile.isExternal ? ['Гість'] : ['Гравець'],
    instagramUrl: playerProfile.instagramUrl,
    photoUrl: playerProfile.photoUrl,
    globalRank: playerProfile.globalRank
  };


  const stats = {
    matchesPlayed: playerProfile.matchesPlayed || 0,
    wins: playerProfile.winsCount || 0,
    eloRating: playerProfile.chessRating || 0,
    goalsScored: playerProfile.goalsScored || 0,
    globalRank: playerProfile.globalRank,
    sportStats: playerProfile.sportStats
  };

  const handleCompare = () => {
    navigate(`/compare?p1=${playerProfile.id}`);
  };

  const handleMessage = async () => {
    if (!playerProfile.userId) {
      toast.error("Цей користувач ще не зареєстрував акаунт для спілкування");
      return;
    }
    
    try {
      const convId = await chatApi.getConversationIdWithUser(playerProfile.userId);
      setActiveConversationId(convId);
      await refreshConversations();
      navigate('/notifications?tab=messages');
    } catch (err) {
      console.error("Failed to start conversation:", err);
      toast.error("Не вдалося почати чат");
    }
  };

  const handleSaveProfile = async (data: Partial<PlayerDto>) => {
    if (!playerProfile?.id) return;
    try {
      // Create a clean update payload by excluding read-only/calculated fields
      // This prevents 400 errors from backend validation of complex nested objects
      const { 
        achievements, 
        sportStats, 
        pointsBreakdown, 
        globalRank, 
        performanceScore, 
        winsCount, 
        matchesPlayed, 
        goalsScored,
        sports,
        armwrestlingWeightClasses,
        ...updatableFields 
      } = playerProfile as any;

      const payload = { ...updatableFields, ...data };
      
      await updatePlayer(playerProfile.id, payload);
      const updatedProfile = await getPlayerById(id!);
      setPlayerProfile(updatedProfile);
      toast.success('Профіль успішно оновлено');
    } catch (error) {
      console.error('Failed to update profile:', error);
      toast.error('Не вдалося оновити профіль');
    }
  };


  return (
    <div className={styles.playerViewPage}>
      <div className={styles.container}>
        <ProfileHeader 
          userData={userData} 
          onCompare={handleCompare} 
          onMessage={user?.id !== playerProfile?.userId ? handleMessage : undefined} 
          onEdit={isAdmin ? () => setIsEditModalOpen(true) : undefined}
          isAdmin={playerProfile.isExternal} // Show as external if flagged
        />
        
        <div className={styles.profileContent}>
          <div className={styles.mainContent}>
            <ProfileStats stats={stats} />
            <UserAchievements achievements={playerProfile.achievements || []} />
          </div>
        </div>
      </div>

      {playerProfile && (
        <EditProfileModal
          isOpen={isEditModalOpen}
          onClose={() => setIsEditModalOpen(false)}
          onSave={handleSaveProfile}
          initialData={playerProfile}
          isAdmin={isAdmin}
        />
      )}
    </div>
  );
};


export default PlayerViewPage;
