import React, { useState, useEffect } from 'react';
import ProfileHeader from '../../widgets/profile-widgets/profile-header/ProfileHeader.jsx';
import ProfileStats from '../../widgets/profile-widgets/profile-stats/ProfileStats.jsx';
import UserTeams from '../../widgets/profile-widgets/user-teams/UserTeams.jsx';
import UserAchievements from '../../widgets/profile-widgets/user-achievements/UserAchievements.jsx';
import SidebarActions from '../../widgets/profile-widgets/sidebar-actions/SidebarActions.jsx';
import RecentActivity from '../../widgets/profile-widgets/recent-activity/RecentActivity.jsx';
import styles from './ProfilePage.module.css';

const ProfilePage = () => {
  const [userData] = useState({
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

  const [stats] = useState({
    matchesPlayed: 42,
    wins: 28,
    eloRating: 2184,
    goalsScored: 15
  });

  const [teams] = useState([
    { id: 1, logo: 'ІТ', name: 'ІТ-31', sport: 'Футзал', role: 'Капітан' },
    { id: 2, logo: 'ШХ', name: 'Шаховий клуб', sport: 'Шахи', role: 'Учасник' },
    { id: 3, logo: 'НТ', name: 'Настільний теніс', sport: 'Теніс', role: 'Учасник' }
  ]);

  const [achievements] = useState([
    { id: 1, icon: 'fas fa-chess-queen', name: 'Чемпіон з шахів', year: '2024' },
    { id: 2, icon: 'fas fa-futbol', name: 'Кращий бомбардир', year: '2023' },
    { id: 3, icon: 'fas fa-medal', name: 'ТОП-3 теніс', year: '2024' },
    { id: 4, icon: 'fas fa-star', name: 'Активний учасник', year: '2023' }
  ]);

  const [activities] = useState([
    { id: 1, icon: 'fas fa-futbol', title: 'Перемога у матчі з футзалу', time: 'Сьогодні, 15:30' },
    { id: 2, icon: 'fas fa-chess-board', title: 'Нова перемога у шахах', time: 'Вчора, 19:15' },
    { id: 3, icon: 'fas fa-user-plus', title: 'Приєднався до команди з тенісу', time: '2 дні тому' },
    { id: 4, icon: 'fas fa-trophy', title: 'Отримав досягнення "Активний учасник"', time: '3 дні тому' }
  ]);

  const [activityChart] = useState([
    { day: 'Пн', value: 40 },
    { day: 'Вт', value: 70 },
    { day: 'Ср', value: 90 },
    { day: 'Чт', value: 110 },
    { day: 'Пт', value: 80 },
    { day: 'Сб', value: 60 },
    { day: 'Нд', value: 30 }
  ]);

  const [isVisible, setIsVisible] = useState(false);

  const handleEditProfile = () => {
    alert('Редагування профілю - в розробці');
  };

  const handleAvatarUpload = () => {
    alert('Функція завантаження фото буде доступна найближчим часом!');
  };

  const handleTeamClick = (teamName) => {
    alert(`Інформація про команду "${teamName}" буде доступна найближчим часом!`);
  };

  const handleActionClick = (actionName) => {
    alert(`Функція "${actionName}" буде доступна найближчим часом!`);
  };

  // Анімація появи при завантаженні
  useEffect(() => {
    setIsVisible(true);
    
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
    <div className={`${styles.profilePage} ${isVisible ? styles.visible : ''}`}>
      <div className={styles.container}>
        {/* Шапка профілю */}
        <ProfileHeader 
          userData={userData}
          onEdit={handleEditProfile}
          onAvatarUpload={handleAvatarUpload}
        />

        <div className={styles.profileContent}>
          <div className={styles.mainContent}>
            {/* Статистика */}
            <ProfileStats stats={stats} />

            {/* Мої команди */}
            <UserTeams 
              teams={teams}
              onTeamClick={handleTeamClick}
            />

            {/* Досягнення */}
            <UserAchievements achievements={achievements} />
          </div>

          <div className={styles.sidebar}>
            {/* Швидкі дії */}
            <SidebarActions onActionClick={handleActionClick} />

            {/* Остання активність */}
            <RecentActivity 
              activities={activities}
              activityChart={activityChart}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;