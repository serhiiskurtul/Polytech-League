import { useState, useEffect } from 'react';
import api from '@/shared/api/axiosInstance';
import styles from './AdminAnalyticsPage.module.css';
import toast from 'react-hot-toast';

interface DailyVisitStats {
  date: string;
  totalVisits: number;
  registeredVisits: number;
  guestVisits: number;
}

export const AdminAnalyticsPage = () => {
  const [dailyStats, setDailyStats] = useState<DailyVisitStats[]>([]);
  const [loading, setLoading] = useState(true);

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

  return (
    <div className={styles.adminAnalyticsPage}>
      <div className={styles.container}>
        <header className={styles.header}>
          <div className={styles.titleSection}>
            <h1>Аналітика відвідувань</h1>
            <p>Статистика візитів за останні 30 днів</p>
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
                </tr>
              </thead>
              <tbody>
                {dailyStats.map((stat, idx) => (
                  <tr key={idx}>
                    <td>{new Intl.DateTimeFormat('uk-UA', { day: '2-digit', month: 'long', year: 'numeric' }).format(new Date(stat.date))}</td>
                    <td style={{ fontWeight: 'bold' }}>{stat.totalVisits}</td>
                    <td style={{ color: '#10b981' }}>{stat.registeredVisits}</td>
                    <td style={{ color: '#f59e0b' }}>{stat.guestVisits}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
};
