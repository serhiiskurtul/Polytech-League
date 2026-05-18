import { useState, useEffect } from 'react';
import api from '@/shared/api/axiosInstance';
import styles from './AdminFeedbackPage.module.css';
import toast from 'react-hot-toast';

interface FeedbackResponse {
  id: number;
  content: string;
  type: string;
  createdAt: string;
  isRead: boolean;
  userId: number | null;
  authorName: string;
}

export const AdminFeedbackPage = () => {
  const [feedbacks, setFeedbacks] = useState<FeedbackResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'unread' | 'read'>('all');

  const fetchFeedbacks = async () => {
    setLoading(true);
    try {
      const response = await api.get<FeedbackResponse[]>('/feedback');
      setFeedbacks(response.data);
    } catch (error) {
      console.error('Failed to fetch feedbacks:', error);
      toast.error('Не вдалося завантажити список відгуків');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFeedbacks();
  }, []);

  const handleMarkAsRead = async (id: number) => {
    try {
      await api.put(`/feedback/${id}/read`);
      toast.success('Позначено як прочитане');
      setFeedbacks(feedbacks.map(f => f.id === id ? { ...f, isRead: true } : f));
    } catch (error) {
      toast.error('Помилка при оновленні статусу');
    }
  };

  const filteredFeedbacks = feedbacks.filter(f => {
    if (filter === 'unread') return !f.isRead;
    if (filter === 'read') return f.isRead;
    return true;
  });

  const getTypeLabel = (type: string) => {
    switch (type) {
      case 'Suggestion': return 'Пропозиція';
      case 'Complaint': return 'Скарга';
      case 'Other': return 'Інше';
      default: return type;
    }
  };

  const getTypeClass = (type: string) => {
    switch (type) {
      case 'Suggestion': return styles.suggestionType;
      case 'Complaint': return styles.complaintType;
      default: return styles.otherType;
    }
  };

  return (
    <div className={styles.adminFeedbackPage}>
      <div className={styles.container}>
        <header className={styles.header}>
          <div className={styles.titleSection}>
            <h1>Пропозиції та скарги</h1>
            <p>Перегляд та керування відгуками користувачів</p>
            <div className={styles.statsBadge}>
              Всього повідомлень: <span>{feedbacks.length}</span>
            </div>
          </div>
        </header>

        <div className={styles.controls}>
          <div className={styles.tabs}>
            <button 
              className={`${styles.tab} ${filter === 'all' ? styles.activeTab : ''}`}
              onClick={() => setFilter('all')}
            >
              Всі
            </button>
            <button 
              className={`${styles.tab} ${filter === 'unread' ? styles.activeTab : ''}`}
              onClick={() => setFilter('unread')}
            >
              Непрочитані ({feedbacks.filter(f => !f.isRead).length})
            </button>
            <button 
              className={`${styles.tab} ${filter === 'read' ? styles.activeTab : ''}`}
              onClick={() => setFilter('read')}
            >
              Прочитані ({feedbacks.filter(f => f.isRead).length})
            </button>
          </div>
        </div>

        <div className={styles.tableContainer}>
          {loading ? (
            <div className={styles.loading}>Завантаження...</div>
          ) : (
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Автор</th>
                  <th>Тип</th>
                  <th>Повідомлення</th>
                  <th>Дата</th>
                  <th>Статус</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {filteredFeedbacks.map((feedback) => (
                  <tr key={feedback.id} className={feedback.isRead ? styles.readRow : styles.unreadRow}>
                    <td>
                      <div className={styles.userCell}>
                        {feedback.userId ? (
                          <div className={styles.avatar}>
                            {feedback.authorName[0]}
                          </div>
                        ) : (
                          <div className={`${styles.avatar} ${styles.anonymousAvatar}`}>
                            <i className="fas fa-user-secret"></i>
                          </div>
                        )}
                        <div>
                          <span className={styles.userName}>{feedback.authorName}</span>
                          {feedback.userId && <span className={styles.userEmail}>ID: {feedback.userId}</span>}
                        </div>
                      </div>
                    </td>
                    <td>
                      <span className={`${styles.typeBadge} ${getTypeClass(feedback.type)}`}>
                        {getTypeLabel(feedback.type)}
                      </span>
                    </td>
                    <td>
                      <div className={styles.contentCell}>
                        {feedback.content}
                      </div>
                    </td>
                    <td>
                      {new Intl.DateTimeFormat('uk-UA', { 
                        day: '2-digit', 
                        month: 'short', 
                        year: 'numeric',
                        hour: '2-digit',
                        minute: '2-digit'
                      }).format(new Date(feedback.createdAt))}
                    </td>
                    <td>
                      <span className={`${styles.statusBadge} ${feedback.isRead ? styles.readStatus : styles.unreadStatus}`}>
                        {feedback.isRead ? 'Прочитано' : 'Нове'}
                      </span>
                    </td>
                    <td>
                      <div className={styles.actions}>
                        {!feedback.isRead && (
                          <button 
                            className={`${styles.actionBtn} ${styles.readBtn}`}
                            onClick={() => handleMarkAsRead(feedback.id)}
                            title="Позначити як прочитане"
                          >
                            <i className="fas fa-check"></i>
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
          
          {!loading && filteredFeedbacks.length === 0 && (
            <div className={styles.loading}>Повідомлень не знайдено</div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AdminFeedbackPage;
