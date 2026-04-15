import { useState, useEffect, useRef } from 'react';
import { 
  ProfileHeader, 
  ProfileStats, 
  UserAchievements 
} from '@/widgets/profile-widgets';
import EditProfileModal from '@/widgets/profile-widgets/edit-profile-modal/EditProfileModal';
import styles from './ProfilePage.module.css';
import { useAuth } from '@/app/providers/AuthContext';
import { PlayerDto } from '@/shared/types/player.types';
import { updatePlayer, getProfile, uploadAvatar } from '@/shared/api/players.api';
import { toast } from 'react-hot-toast';

interface UserData {
  name: string;
  studentId: string;
  faculty: string;
  email: string;
  phone: string;
  city: string;
  joinDate: string;
  bio: string;
  badges: string[];
}

interface Stats {
  matchesPlayed: number;
  wins: number;
  eloRating: number;
  goalsScored: number;
}

interface Achievement {
  id: number;
  icon: string;
  name: string;
  year: string;
}

const ProfilePage = () => {
  const { user, logout } = useAuth();
  const [playerProfile, setPlayerProfile] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const data = await getProfile();
        setPlayerProfile(data);
      } catch (error) {
        console.error("Failed to fetch player profile:", error);
        toast.error("Не вдалося завантажити спортивний профіль");
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, []);

  const userData = {
    name: user ? `${user.firstName} ${user.lastName}` : 'Користувач',
    studentId: playerProfile?.group || '-',
    faculty: playerProfile?.faculty || '-',
    email: user?.email || '-',
    phone: '-', 
    city: playerProfile?.city || '-',
    joinDate: (user?.createdAt || (user as any)?.CreatedAt)
      ? (() => {
          const date = new Date(user?.createdAt || (user as any)?.CreatedAt);
          return isNaN(date.getTime()) 
            ? '-' 
            : new Intl.DateTimeFormat('uk-UA', { month: 'long', year: 'numeric' }).format(date);
        })()
      : '-',
    bio: playerProfile?.bio || '-',
    badges: ['Студент', 'Учасник'],
    instagramUrl: playerProfile?.instagramUrl,
    photoUrl: playerProfile?.photoUrl
  };

  const stats = {
    matchesPlayed: playerProfile?.matchesPlayed || 0,
    wins: playerProfile?.winsCount || 0,
    eloRating: playerProfile?.chessRating || 0,
    goalsScored: playerProfile?.goalsScored || 0
  };

  const handleEditProfile = () => {
    setIsEditModalOpen(true);
  };

  const handleSaveProfile = async (data: Partial<PlayerDto>) => {
    if (!playerProfile?.id) return;
    try {
      await updatePlayer(playerProfile.id, { ...playerProfile, ...data });
      const updatedProfile = await getProfile();
      setPlayerProfile(updatedProfile);
      toast.success('Профіль успішно оновлено');
    } catch (error) {
      console.error('Failed to update profile:', error);
      toast.error('Не вдалося оновити профіль');
    }
  };

  const handleAvatarUpload = () => {
    fileInputRef.current?.click();
  };

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      const photoUrl = await uploadAvatar(file);
      const updatedProfile = await getProfile();
      setPlayerProfile(updatedProfile);
      toast.success('Фото профілю оновлено');
    } catch (error) {
      console.error('Failed to upload avatar:', error);
      toast.error('Не вдалося завантажити фото');
    }
  };

  // Анімація появи при завантаженні
  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add(styles.visible);
          }
        });
      },
      {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
      }
    );

    const cards = document.querySelectorAll(`.${styles.card}`);
    cards.forEach(card => {
      card.classList.add(styles.fadeIn);
      observer.observe(card);
    });

    return () => {
      cards.forEach(card => observer.unobserve(card));
    };
  }, []);

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
        <p>Завантаження профілю...</p>
      </div>
    );
  }

  return (
    <div className={styles.profilePage}>
      <div className={styles.container}>
        <ProfileHeader 
          userData={userData} 
          onEdit={handleEditProfile}
          onAvatarUpload={handleAvatarUpload}
          onLogout={logout}
        />
        
        <div className={styles.profileContent}>
          <div className={styles.mainContent}>
            <UserAchievements achievements={playerProfile?.achievements || []} />
            <ProfileStats stats={stats} />
          </div>
        </div>
      </div>

      <input 
        type="file" 
        ref={fileInputRef} 
        style={{ display: 'none' }} 
        accept="image/*"
        onChange={handleFileChange}
      />
      
      {playerProfile && (
        <EditProfileModal
          isOpen={isEditModalOpen}
          onClose={() => setIsEditModalOpen(false)}
          onSave={handleSaveProfile}
          initialData={playerProfile}
        />
      )}
    </div>
  );
};

export default ProfilePage;
