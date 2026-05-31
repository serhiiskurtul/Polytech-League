import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './TournamentCalendar.module.css';
import { getTournaments } from '@/entities/tournament/api/tournaments.api';
import { Tournament } from '@/entities/tournament/model/tournament.types';

const SPORT_CONFIG: Record<string, { label: string; icon: string; color: string }> = {
  'football': { label: 'Футбол', icon: 'fa-futbol', color: 'var(--accent)' },
  'basketball-3x3': { label: 'Баскетбол', icon: 'fa-basketball-ball', color: 'var(--primary)' },
  'volleyball': { label: 'Волейбол', icon: 'fa-volleyball-ball', color: 'var(--primary)' },
  'beach-volleyball': { label: 'Пляжний волейбол', icon: 'fa-umbrella-beach', color: 'var(--warning)' },
  'chess': { label: 'Шахи', icon: 'fa-chess', color: 'var(--text)' },
  'checkers': { label: 'Шашки', icon: 'fa-chess-board', color: 'var(--text)' },
  'table-tennis': { label: 'Настільний теніс', icon: 'fa-table-tennis', color: 'var(--warning)' },
  'armwrestling': { label: 'Армрестлінг', icon: 'fa-fist-raised', color: 'var(--accent)' },
  'badminton': { label: 'Бадмінтон', icon: 'fa-feather-alt', color: 'var(--success)' },
  'other': { label: 'Спорт', icon: 'fa-trophy', color: 'var(--primary)' }
};

const STATUS_CONFIG: Record<string, { label: string; class: string }> = {
  'upcoming': { label: 'Скоро', class: styles.statusUpcoming },
  'ongoing': { label: 'У процесі', class: styles.statusLive },
  'completed': { label: 'Завершено', class: styles.statusEnded },
  'registration': { label: 'Реєстрація', class: styles.statusOpen }
};

const TournamentCalendar = () => {
  const [tournaments, setTournaments] = useState<Tournament[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchTournaments = async () => {
      try {
        setLoading(true);
        // Завантажуємо тільки майбутні події, обмежимо 4-ма для віджета
        const response = await getTournaments(1, 4, 'upcoming');
        setTournaments(response.items);
      } catch (error) {
        console.error('Помилка при завантаженні турнірів:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchTournaments();
  }, []);

  const getSportInfo = (sport: string) => {
    return SPORT_CONFIG[sport] || SPORT_CONFIG['other'];
  };

  const getStatusInfo = (status: string, dateStr: string) => {
    // Якщо дата сьогодні, може бути 'ongoing'
    // Backend вже повертає статус, але ми можемо додати логіку для "Реєстрація"
    if (status === 'upcoming') return STATUS_CONFIG['upcoming'];
    if (status === 'ongoing') return STATUS_CONFIG['ongoing'];
    if (status === 'completed') return STATUS_CONFIG['completed'];
    return STATUS_CONFIG['registration'];
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
          {loading ? (
            Array(4).fill(0).map((_, index) => (
              <div key={index} className={`${styles.tournamentCard} ${styles.skeleton}`}>
                <div className={styles.skeletonHeader}></div>
                <div className={styles.skeletonTitle}></div>
                <div className={styles.skeletonText}></div>
                <div className={styles.skeletonText}></div>
              </div>
            ))
          ) : tournaments.length > 0 ? (
            tournaments.map((tournament) => {
              const sportInfo = getSportInfo(tournament.sport);
              const statusInfo = getStatusInfo(tournament.status, tournament.date);
              const tags = [sportInfo.label, tournament.isTeam ? 'Командний' : 'Індивідуальний'];

              return (
                <div 
                  key={tournament.id} 
                  className={styles.tournamentCard}
                  onClick={() => navigate(`/tournaments/${tournament.id}`)}
                  style={{ cursor: 'pointer' }}
                >
                  <div className={styles.tournamentHeader}>
                    <div className={styles.tournamentSport} style={{ color: sportInfo.color }}>
                      <i className={`fas ${sportInfo.icon} ${styles.sportIcon}`}></i>
                      {sportInfo.label}
                    </div>
                    <div className={`${styles.statusBadge} ${statusInfo.class}`}>
                      {tournament.currentRound || statusInfo.label}
                    </div>
                  </div>
                  
                  <h3 className={styles.tournamentName}>{tournament.title}</h3>
                  
                  <div className={styles.tournamentDetails}>
                    <div className={styles.detailItem}>
                      <i className="far fa-calendar"></i>
                      <span>
                        {tournament.rawDate ? new Date(tournament.rawDate).toLocaleString('uk-UA', {
                          day: '2-digit',
                          month: '2-digit',
                          year: 'numeric',
                          hour: '2-digit',
                          minute: '2-digit'
                        }) : `${tournament.date} ${tournament.time || ''}`}
                      </span>
                    </div>
                    <div className={styles.detailItem}>
                      <i className="fas fa-map-marker-alt"></i>
                      <span>{tournament.location}</span>
                    </div>
                  </div>

                  <div className={styles.tagsContainer}>
                    {tags.map((tag, index) => (
                      <span key={index} className={styles.sportTag}>{tag}</span>
                    ))}
                  </div>
                </div>
              );
            })
          ) : (
            <div className={styles.noTournaments}>
              <i className="fas fa-calendar-times"></i>
              <p>Наразі немає запланованих подій</p>
            </div>
          )}
        </div>

        <div className={styles.viewAllContainer}>
          <button className={styles.viewAllButton} onClick={() => navigate('/tournaments')}>
            Всі змагання
            <i className="fas fa-arrow-right"></i>
          </button>
        </div>
      </div>
    </section>
  );
};

export default TournamentCalendar;
