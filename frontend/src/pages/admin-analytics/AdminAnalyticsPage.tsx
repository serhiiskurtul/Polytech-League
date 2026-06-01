import React, { useState, useEffect } from 'react';
import api from '@/shared/api/axiosInstance';
import styles from './AdminAnalyticsPage.module.css';
import toast from 'react-hot-toast';

interface DailyVisitStats {
  date: string;
  totalVisits: number;
  registeredVisits: number;
  guestVisits: number;
}

interface VisitedUser {
  id: number;
  firstName: string;
  lastName: string;
  email?: string;
  role: string;
  lastVisitAt?: string;
}

export const AdminAnalyticsPage = () => {
  const [dailyStats, setDailyStats] = useState<DailyVisitStats[]>([]);
  const [loading, setLoading] = useState(true);
  
  const [selectedDate, setSelectedDate] = useState<string | null>(null);
  const [visitedUsers, setVisitedUsers] = useState<VisitedUser[]>([]);
  const [loadingUsers, setLoadingUsers] = useState(false);

  const fetchUsersForDate = async (date: string) => {
    if (selectedDate === date) {
      setSelectedDate(null);
      return;
    }
    setSelectedDate(date);
    setLoadingUsers(true);
    try {
      const response = await api.get(`/SiteVisits/users-by-date?date=${date}`);
      setVisitedUsers(response.data);
    } catch (e) {
      console.error('Failed to load users for date', e);
      toast.error('Не вдалося завантажити користувачів');
    } finally {
      setLoadingUsers(false);
    }
  };

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const response = await api.get('/SiteVisits/daily-stats');
        setDailyStats(response.data);
      } catch (e) {
        console.error('Failed to load analytics', e);
        toast.error('Не вдалося завантажити статистику візитів');
      } finally {
        setLoading(false);
      }
    };
    fetchStats();
  }, []);

  const todayDateString = new Date().toISOString().split('T')[0];
  const todayStats = dailyStats.find(s => s.date.startsWith(todayDateString)) || {
    totalVisits: 0,
    registeredVisits: 0,
    guestVisits: 0,
  };

  return (
    <div className={styles.adminAnalyticsPage}>
      <div className={styles.container}>
        <header className={styles.header}>
          <div className={styles.titleSection}>
            <h1>Аналітика відвідувань</h1>
            <p>Статистика візитів за останні 30 днів</p>
            <div style={{ display: 'flex', gap: '10px', marginTop: '16px', flexWrap: 'wrap' }}>
              <div className={styles.statsBadge} style={{ background: 'rgba(59, 130, 246, 0.1)', color: '#3b82f6', border: '1px solid rgba(59, 130, 246, 0.2)' }}>
                <i className="fas fa-calendar-day" style={{ marginRight: '6px' }}></i>
                Сьогодні візитів: <span style={{ color: '#111827' }}>{todayStats.totalVisits}</span>
              </div>
              <div className={styles.statsBadge} style={{ background: 'rgba(16, 185, 129, 0.1)', color: '#10b981', border: '1px solid rgba(16, 185, 129, 0.2)' }}>
                <i className="fas fa-user-check" style={{ marginRight: '6px' }}></i>
                Зареєстрованих: <span style={{ color: '#111827' }}>{todayStats.registeredVisits}</span>
              </div>
              <div className={styles.statsBadge} style={{ background: 'rgba(245, 158, 11, 0.1)', color: '#f59e0b', border: '1px solid rgba(245, 158, 11, 0.2)' }}>
                <i className="fas fa-user-secret" style={{ marginRight: '6px' }}></i>
                Гостей: <span style={{ color: '#111827' }}>{todayStats.guestVisits}</span>
              </div>
            </div>
          </div>
        </header>

        <div className={styles.tableContainer}>
          {loading ? (
            <div className={styles.loading}>Завантаження...</div>
          ) : dailyStats.length === 0 ? (
            <div className={styles.loading}>Немає даних для відображення</div>
          ) : (
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Дата</th>
                  <th>Усі візити</th>
                  <th>Зареєстровані</th>
                  <th>Гості</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {dailyStats.map((stat, idx) => (
                  <React.Fragment key={idx}>
                    <tr onClick={() => fetchUsersForDate(stat.date)} style={{ cursor: 'pointer', backgroundColor: selectedDate === stat.date ? 'var(--surface-hover, #f9fafb)' : 'inherit' }}>
                      <td>{new Intl.DateTimeFormat('uk-UA', { day: '2-digit', month: 'long', year: 'numeric' }).format(new Date(stat.date))}</td>
                      <td style={{ fontWeight: 'bold' }}>{stat.totalVisits}</td>
                      <td style={{ color: '#10b981' }}>{stat.registeredVisits}</td>
                      <td style={{ color: '#f59e0b' }}>{stat.guestVisits}</td>
                      <td>
                        <button className={styles.detailsBtn}>
                          {selectedDate === stat.date ? 'Приховати' : 'Хто заходив?'}
                        </button>
                      </td>
                    </tr>
                    {selectedDate === stat.date && (
                      <tr className={styles.expandedRow}>
                        <td colSpan={5} style={{ padding: 0 }}>
                          <div className={styles.expandedContent}>
                            <h3 className={styles.expandedTitle}>Зареєстровані користувачі ({stat.registeredVisits})</h3>
                            {loadingUsers ? (
                              <div style={{ padding: '20px', color: '#6b7280' }}>Завантаження списку користувачів...</div>
                            ) : visitedUsers.length === 0 ? (
                              <div style={{ padding: '20px', color: '#6b7280' }}>Немає зареєстрованих користувачів за цей день</div>
                            ) : (
                              <div className={styles.userGrid}>
                                {visitedUsers.map(u => (
                                  <div key={u.id} className={styles.userCard}>
                                    <div className={styles.userAvatar}>{u.firstName[0]}{u.lastName[0]}</div>
                                    <div className={styles.userInfo}>
                                      <div className={styles.userName}>{u.firstName} {u.lastName}</div>
                                      <div className={styles.userEmail}>{u.email || 'Без email'} • ID: {u.id}</div>
                                    </div>
                                  </div>
                                ))}
                              </div>
                            )}
                          </div>
                        </td>
                      </tr>
                    )}
                  </React.Fragment>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
};
