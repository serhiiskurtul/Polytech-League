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
import { updatePlayer, updateProfile, getProfile, uploadAvatar, checkArchivalMatches, createMergeRequest } from '@/shared/api/players.api';

import { toast } from 'react-hot-toast';

interface UserData {
  name: string;
  studentId: string;
  studyYears: string;
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
  const { user, logout, refreshUser } = useAuth();
  const [playerProfile, setPlayerProfile] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [archivalMatches, setArchivalMatches] = useState<PlayerDto[]>([]);
  const [isMergeRequestSent, setIsMergeRequestSent] = useState(false);
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

  useEffect(() => {
    if (playerProfile && user && archivalMatches.length === 0 && !isMergeRequestSent) {
      // If profile is fresh (Group is Guest) or just has no achievements, check for historical data
      const checkHistory = async () => {
        try {
          const matches = await checkArchivalMatches(user.firstName, user.lastName, playerProfile.group || '');
          // Filter out current profile
          setArchivalMatches(matches.filter(m => m.id !== playerProfile.id));
        } catch (error) {
          console.error("Failed to check archival matches:", error);
        }
      };
      checkHistory();
    }
  }, [playerProfile, user]);

  const handleRequestMerge = async (targetId: number) => {
    try {
      await createMergeRequest(targetId);
      setIsMergeRequestSent(true);
      setArchivalMatches([]);
      toast.success('Запит на об\'єднання надіслано адміністратору');
    } catch (error) {
      toast.error('Не вдалося надіслати запит');
    }
  };


  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';
  
  const userData = {
    name: user ? `${user.firstName} ${user.lastName}` : 'Користувач',
    group: playerProfile?.group || '-',
    studentId: playerProfile?.studentId || '-',
    studyYears: playerProfile?.studyYears || '-',
    phone: '-', 
    city: playerProfile?.city || '-',
    bio: playerProfile?.bio ?? '',
    badges: (() => {
      if (isAdmin) return ['Адміністратор'];
      const b = [];
      if (playerProfile?.group && playerProfile.group !== '-' && playerProfile.group.trim() !== '') {
        b.push('Студент');
      }
      if ((playerProfile?.matchesPlayed || 0) > 0) {
        b.push('Спортсмен');
      }
      return b;
    })(),
    instagramUrl: playerProfile?.instagramUrl && playerProfile.instagramUrl.trim() !== ''
      ? (playerProfile.instagramUrl.startsWith('http') 
          ? playerProfile.instagramUrl 
          : `https://instagram.com/${playerProfile.instagramUrl.replace('@', '').trim()}`)
      : null,
    photoUrl: playerProfile?.photoUrl,

    globalRank: playerProfile?.globalRank,
    isFounder: playerProfile?.isFounder,
    automaticBadges: playerProfile?.automaticBadges
  };

  const stats = {
    matchesPlayed: playerProfile?.matchesPlayed || 0,
    wins: playerProfile?.winsCount || 0,
    competitionsCount: playerProfile?.competitionsCount || 0,
    eloRating: playerProfile?.chessRating || 0,
    goalsScored: playerProfile?.goalsScored || 0,
    globalRank: playerProfile?.globalRank,
    sportStats: playerProfile?.sportStats
  };

  const handleEditProfile = () => {
    setIsEditModalOpen(true);
  };

  const handleSaveProfile = async (data: Partial<PlayerDto>) => {
    if (!playerProfile?.id) return;
    try {
      const updatedProfile = await updateProfile({ ...playerProfile, ...data });
      console.log('Profile updated successfully:', updatedProfile);
      setPlayerProfile(updatedProfile);
      await refreshUser();
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
      await refreshUser(); // Update global user state for Navbar/Header
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
          isAdmin={isAdmin}
          onEdit={handleEditProfile}
          onAvatarUpload={handleAvatarUpload}
          onLogout={logout}
        />
        
        {archivalMatches.length > 0 && !isAdmin && (
          <div className={`${styles.card} ${styles.archivalMatchCard}`} style={{ borderLeft: '4px solid #3b82f6', background: '#eff6ff' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
              <div style={{ fontSize: '24px', color: '#3b82f6' }}>
                <i className="fas fa-history"></i>
              </div>
              <div style={{ flex: 1 }}>
                <h4 style={{ margin: 0, color: '#1e40af' }}>Ми знайшли ваші можливі архівні досягнення!</h4>
                <p style={{ margin: '5px 0 0 0', fontSize: '14px', color: '#1e3a8a' }}>
                  В системі є профіль "{archivalMatches[0].firstName} {archivalMatches[0].lastName}" (Група: {archivalMatches[0].group}), який не прив'язаний до аккаунту. Це ви?
                </p>
              </div>
              <button 
                onClick={() => handleRequestMerge(archivalMatches[0].id!)}
                className="btn-primary"
                style={{ padding: '8px 16px', fontSize: '13px' }}
              >
                Так, це я
              </button>
              <button 
                onClick={() => setArchivalMatches([])}
                style={{ background: 'transparent', border: 'none', color: '#6b7280', cursor: 'pointer' }}
              >
                Ні
              </button>
            </div>
          </div>
        )}

        {isMergeRequestSent && (
          <div className={`${styles.card}`} style={{ borderLeft: '4px solid #10b981', background: '#ecfdf5' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
              <div style={{ fontSize: '24px', color: '#10b981' }}>
                <i className="fas fa-clock"></i>
              </div>
              <div>
                <h4 style={{ margin: 0, color: '#065f46' }}>Запит на об'єднання профілю надіслано</h4>
                <p style={{ margin: '5px 0 0 0', fontSize: '14px', color: '#064e3b' }}>
                  Адміністратор перевірить ваші дані. Після схвалення ваші старі досягнення з'являться тут.
                </p>
              </div>
            </div>
          </div>
        )}
        
        {!isAdmin && (
          <div className={styles.profileContent}>
            <div className={styles.mainContent}>
              <UserAchievements achievements={playerProfile?.achievements || []} />
              <ProfileStats stats={stats} />
            </div>
          </div>
        )}
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
          isAdmin={isAdmin}
        />
      )}

    </div>
  );
};

export default ProfilePage;
