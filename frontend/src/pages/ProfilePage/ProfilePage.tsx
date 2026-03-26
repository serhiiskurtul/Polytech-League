import { useState, useEffect } from 'react';
import ProfileHeader from '../../widgets/profile-widgets/profile-header/ProfileHeader';
import ProfileStats from '../../widgets/profile-widgets/profile-stats/ProfileStats';
import UserTeams from '../../widgets/profile-widgets/user-teams/UserTeams';
import UserAchievements from '../../widgets/profile-widgets/user-achievements/UserAchievements';
import SidebarActions from '../../widgets/profile-widgets/sidebar-actions/SidebarActions';
import RecentActivity from '../../widgets/profile-widgets/recent-activity/RecentActivity';
import styles from './ProfilePage.module.css';

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
  const [userData] = useState<UserData>({
    name: 'Артем Іванов',
    studentId: 'IT-31-2022',
    faculty: 'Інформаційних технологій',
    email: 'artem.ivanov@college.edu',
    phone: '+380 67 123 4567',
    city: 'Київ',
    joinDate: 'Вересень 2022',
    bio: 'Студент 3 курсу, ІТ-спеціальність. Капітан команди з футзалу "ІТ-31". Активно беру участь у шахових турнірах та чемпіонатах з настільного тенісу.',
    badges: ['Капітан', 'Активний', 'ТОП-5']
  });

  const [stats] = useState<Stats>({
    matchesPlayed: 42,
    wins: 28,
    eloRating: 2184,
    goalsScored: 15
  });

  const [teams] = useState<Team[]>([
    { id: 1, logo: 'ІТ', name: 'ІТ-31', sport: 'Футзал', role: 'Капітан' },
    { id: 2, logo: 'ШХ', name: 'Шаховий клуб', sport: 'Шахи', role: 'Учасник' },
    { id: 3, logo: 'НТ', name: 'Настільний теніс', sport: 'Теніс', role: 'Учасник' }
  ]);

  const [achievements] = useState<Achievement[]>([
    { id: 1, icon: 'fas fa-chess-queen', name: 'Чемпіон з шахів', year: '2024' },
    { id: 2, icon: 'fas fa-futbol', name: 'Кращий бомбардир', year: '2023' },
    { id: 3, icon: 'fas fa-medal', name: 'ТОП-3 теніс', year: '2024' },
    { id: 4, icon: 'fas fa-star', name: 'Активний учасник', year: '2023' }
  ]);

  const [activities] = useState<Activity[]>([
    { id: 1, icon: 'fas fa-futbol', title: 'Перемога у матчі з футзалу', time: 'Сьогодні, 15:30' },
    { id: 2, icon: 'fas fa-chess-board', title: 'Нова перемога у шахах', time: 'Вчора, 19:15' },
    { id: 3, icon: 'fas fa-user-plus', title: 'Приєднався до команди з тенісу', time: '2 дні тому' },
    { id: 4, icon: 'fas fa-trophy', title: 'Отримав досягнення "Активний учасник"', time: '3 дні тому' }
  ]);

  const [activityChart] = useState<ActivityChartItem[]>([
    { day: 'Пн', value: 40 },
    { day: 'Вт', value: 70 },
    { day: 'Ср', value: 90 },
    { day: 'Чт', value: 110 },
    { day: 'Пт', value: 80 },
    { day: 'Сб', value: 60 },
    { day: 'Нд', value: 30 }
  ]);

  const handleEditProfile = () => {
    alert('Редагування профілю - в розробці');
  };

  const handleAvatarUpload = () => {
    alert('Функція завантаження фото буде доступна найближчим часом!');
  };

  const handleTeamClick = (teamName: string) => {
    alert(`Інформація про команду "${teamName}" буде доступна найближчим часом!`);
  };

  const handleActionClick = (actionName: string) => {
    alert(`Функція "${actionName}" буде доступна найближчим часом!`);
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

  return (
    <div className={styles.profilePage}>
      <ProfileHeader 
        userData={userData} 
        onEdit={handleEditProfile}
        onAvatarUpload={handleAvatarUpload}
      />
      
      <div className={styles.profileGrid}>
        <div className={styles.profileMain}>
          <ProfileStats stats={stats} />
          <div className={styles.profileTwoColumn}>
            <UserTeams teams={teams} onTeamClick={handleTeamClick} />
            <UserAchievements achievements={achievements} />
          </div>
          <RecentActivity activities={activities} activityChart={activityChart} />
        </div>
        
        <aside className={styles.profileSidebar}>
          <SidebarActions onActionClick={handleActionClick} />
        </aside>
      </div>
    </div>
  );
};

export default ProfilePage;
