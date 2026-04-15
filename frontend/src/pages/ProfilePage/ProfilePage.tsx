import { useState, useEffect } from 'react';
import { 
  ProfileHeader, 
  ProfileStats, 
  UserTeams, 
  UserAchievements, 
  RecentActivity 
} from '@/widgets/profile-widgets';
import styles from './ProfilePage.module.css';
import { useAuth } from '@/app/providers/AuthContext';
import { getProfile } from '@/shared/api/players.api';
import { PlayerDto } from '@/shared/types/player.types';
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

interface Team {
  id: number;
  logo: string;
  name: string;
  sport: string;
  role: string;
}

interface Achievement {
  id: number;
  icon: string;
  name: string;
  year: string;
}

interface Activity {
  id: number;
  icon: string;
  title: string;
  time: string;
}

interface ActivityChartItem {
  day: string;
  value: number;
}

const ProfilePage = () => {
  const { user } = useAuth();
  const [playerProfile, setPlayerProfile] = useState<PlayerDto | null>(null);
  const [loading, setLoading] = useState(true);

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
    studentId: playerProfile?.group || 'IT-ХХ-202X',
    faculty: 'Політехнічний інститут',
    email: user?.email || '',
    phone: '+380 XX XXX XXXX',
    city: playerProfile?.city || 'Київ',
    joinDate: 'Вересень 2024',
    bio: playerProfile?.chessFideId ? `FIDE ID: ${playerProfile.chessFideId}` : 'Активний учасник спортивної ліги Політехніки.',
    badges: ['Студент', 'Учасник']
  };

  const stats = {
    matchesPlayed: 0,
    wins: 0,
    eloRating: playerProfile?.chessRating || 0,
    goalsScored: 0
  };

  const teams: any[] = [];
  
  const achievements = [
    { id: 1, icon: 'fas fa-star', name: 'Новий учасник', year: '2024' },
    { id: 2, icon: 'fas fa-user-check', name: 'Профіль створено', year: '2024' }
  ];

  const activities: any[] = [
    { id: 1, icon: 'fas fa-user-plus', title: 'Реєстрація в системі', time: 'Нещодавно' }
  ];

  const activityChart = [
    { day: 'Пн', value: 10 },
    { day: 'Вт', value: 20 },
    { day: 'Ср', value: 15 },
    { day: 'Чт', value: 30 },
    { day: 'Пт', value: 25 },
    { day: 'Сб', value: 40 },
    { day: 'Нд', value: 20 }
  ];

  const handleEditProfile = () => {
    alert('Редагування профілю - в розробці');
  };

  const handleAvatarUpload = () => {
    alert('Функція завантаження фото буде доступна найближчим часом!');
  };

  const handleTeamClick = (teamName: string) => {
    alert(`Інформація про команду "${teamName}" буде доступна найближчим часом!`);
  };



  // Анімація появи при завантаженні
  useEffect(() => {
    // Додаємо анімацію для карток при скролі
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

    // Спостерігаємо за всіма картками
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
        />
        
        <div className={styles.profileContent}>
          <div className={styles.mainContent}>
            <ProfileStats stats={stats} />
            <div className={styles.profileTwoColumn}>
              <UserTeams teams={teams} onTeamClick={handleTeamClick} />
              <UserAchievements achievements={achievements} />
            </div>
            <RecentActivity activities={activities} activityChart={activityChart} />
          </div>
          

        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
