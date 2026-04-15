import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  ProfileHeader, 
  ProfileStats, 
  UserAchievements 
} from '@/widgets/profile-widgets';
import styles from './PlayerViewPage.module.css';
import { PlayerDto } from '@/shared/types/player.types';
import { getPlayerById } from '@/shared/api/players.api';
import { toast } from 'react-hot-toast';

const PlayerViewPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [playerProfile, setPlayerProfile] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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
    studentId: playerProfile.group || '—',
    faculty: playerProfile.faculty || '—',
    email: '—', // Hide email for public profiles
    phone: '—',
    city: playerProfile.city || '—',
    joinDate: '—', // Could fetch from user data if needed
    bio: playerProfile.bio || 'Інформація відсутня',
    badges: ['Гравець', ...(playerProfile.sports || [])],
    instagramUrl: playerProfile.instagramUrl,
    photoUrl: playerProfile.photoUrl
  };

  const stats = {
    matchesPlayed: playerProfile.matchesPlayed || 0,
    wins: playerProfile.winsCount || 0,
    eloRating: playerProfile.chessRating || 0,
    goalsScored: playerProfile.goalsScored || 0
  };

  return (
    <div className={styles.playerViewPage}>
      <div className={styles.container}>
        <ProfileHeader userData={userData} />
        
        <div className={styles.profileContent}>
          <div className={styles.mainContent}>
            <ProfileStats stats={stats} />
            <UserAchievements achievements={playerProfile.achievements || []} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default PlayerViewPage;
