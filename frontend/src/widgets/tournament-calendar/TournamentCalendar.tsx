 import styles from './TournamentCalendar.module.css';

interface Tournament {
  id: number;
  sport: string;
  name: string;
  date: string;
  location: string;
  tags: string[];
  participants: number;
  status: string;
  color: string;
}

const TournamentCalendar = () => {
  // Дані турнірів
  const tournaments: Tournament[] = [
    {
      id: 1,
      sport: 'Шахи',
      name: 'Чемпіонат коледжу',
      date: '15 грудня, 14:00',
      location: 'Бібліотека, 3 поверх',
      tags: ['Шахи', 'Індивідуальний'],
      participants: 24,
      status: 'Реєстрація',
      color: 'var(--success)'
    },
    {
      id: 2,
      sport: 'Волейбол',
      name: 'Кубок декана',
      date: '18-20 грудня',
      location: 'Спортзал №1',
      tags: ['Волейбол', 'Командний'],
      participants: 12,
      status: 'У процесі',
      color: 'var(--primary)'
    },
    {
      id: 3,
      sport: 'Футзал',
      name: 'Осінній кубок',
      date: '22 грудня, 16:00',
      location: 'Спортзал №2',
      tags: ['Футзал', 'Командний'],
      participants: 16,
      status: 'Скоро',
      color: 'var(--accent)'
    },
    {
      id: 4,
      sport: 'Настільний теніс',
      name: 'Турнір першості',
      date: '25 грудня, 11:00',
      location: 'Спортзал №3',
      tags: ['Теніс', 'Індивідуальний'],
      participants: 32,
      status: 'Реєстрація',
      color: 'var(--warning)'
    }
  ];

  const handleRegister = (tournamentId: number) => {
    console.log(`Реєстрація на турнір ${tournamentId}`);
    // Тут можна додати логіку реєстрації
    alert(`Ви зареєструвались на турнір!`);
  };

  const handleDetails = (tournamentId: number) => {
    console.log(`Деталі турніру ${tournamentId}`);
    // Тут можна додати перехід на сторінку турніру
  };

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Шахи': 'fa-chess',
      'Волейбол': 'fa-volleyball-ball',
      'Футзал': 'fa-futbol',
      'Настільний теніс': 'fa-table-tennis'
    };
    return icons[sport] || 'fa-trophy';
  };

  const getStatusClass = (status: string) => {
    const classes: Record<string, string> = {
      'Реєстрація': styles.statusRegistration,
      'У процесі': styles.statusOngoing,
      'Скоро': styles.statusSoon
    };
    return classes[status] || '';
  };

  return (
    <section className={styles.tournamentCalendar}>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <h2 className={styles.cardTitle}>
            <i className={`fas fa-calendar-day ${styles.cardIcon}`}></i>
            Найближчі події
          </h2>
          <div className={`${styles.badge} ${styles.badgePrimary}`}>
            {tournaments.length} подій
          </div>
        </div>
        
        <div className={styles.calendarGrid}>
          {tournaments.map((tournament) => (
            <div key={tournament.id} className={tournament.status === 'У процесі' ? `${styles.tournamentCard} ${styles.active}` : styles.tournamentCard}>
              <div className={styles.tournamentHeader}>
                <div className={styles.tournamentSport} style={{ color: tournament.color }}>
                  <i className={`fas ${getSportIcon(tournament.sport)} ${styles.sportIcon}`}></i>
                  {tournament.sport}
                </div>
                <div className={`${styles.statusBadge} ${getStatusClass(tournament.status)}`}>
                  {tournament.status}
                </div>
              </div>
              
              <h3 className={styles.tournamentName}>{tournament.name}</h3>
              
              <div className={styles.tournamentDetails}>
                <div className={styles.detailItem}>
                  <i className="far fa-calendar"></i>
                  <span>{tournament.date}</span>
                </div>
                <div className={styles.detailItem}>
                  <i className="fas fa-map-marker-alt"></i>
                  <span>{tournament.location}</span>
                </div>
                <div className={styles.detailItem}>
                   <i className="fas fa-users"></i>
                   <span>{tournament.participants} учасників</span>
                </div>
              </div>

              <div className={styles.tournamentTags}>
                {tournament.tags.map((tag, index) => (
                  <span key={index} className={styles.tag}>{tag}</span>
                ))}
              </div>

              <div className={styles.tournamentActions}>
                <button 
                  className={styles.registerBtn}
                  onClick={() => handleRegister(tournament.id)}
                >
                  Реєстрація
                </button>
                <button 
                  className={styles.detailsBtn}
                  onClick={() => handleDetails(tournament.id)}
                >
                  Деталі
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default TournamentCalendar;
