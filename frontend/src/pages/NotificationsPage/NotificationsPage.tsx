import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import styles from './NotificationsPage.module.css';
import { 
  getMyNotifications, 
  markAsRead as apiMarkAsRead, 
  markAllAsRead as apiMarkAllAsRead, 
  deleteNotification as apiDeleteNotification, 
  clearReadNotifications as apiClearReadNotifications 
} from '@/shared/api/notifications.api';
import { Notification } from '@/shared/types/notification.types';
import toast from 'react-hot-toast';
import { MessengerView } from '@/widgets/chat-messenger/MessengerView';
import { useChat } from '@/app/providers/ChatProvider';


const NotificationsPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'unread' | 'read'>('all');
  const [activeTab, setActiveTab] = useState<'notifications' | 'messages'>('notifications');

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const tabParam = params.get('tab');
    if (tabParam === 'messages') {
      setActiveTab('messages');
    } else if (tabParam === 'notifications') {
      setActiveTab('notifications');
    }
  }, [location.search]);


  const fetchNotifications = async () => {
    try {
      setLoading(true);
      const data = await getMyNotifications();
      setNotifications(data);
    } catch (error) {
      console.error('Failed to fetch notifications:', error);
      toast.error('Помилка при завантаженні сповіщень');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchNotifications();
  }, []);

  // Фільтрація сповіщень
  const filteredNotifications = notifications.filter(notification => {
    if (filter === 'all') return true;
    if (filter === 'unread') return !notification.isRead;
    if (filter === 'read') return notification.isRead;
    return true;
  });

  // Позначити всі як прочитані
  const markAllAsRead = async () => {
    try {
      await apiMarkAllAsRead();
      setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
      toast.success('Всі сповіщення позначено як прочитані');
    } catch (error) {
      toast.error('Не вдалося оновити сповіщення');
    }
  };

  // Позначити конкретне сповіщення як прочитане
  const markAsRead = async (notification: Notification) => {
    if (notification.isRead) {
      if (notification.link) navigate(notification.link);
      return;
    }

    try {
      await apiMarkAsRead(notification.id);
      setNotifications(prev => prev.map(n => 
        n.id === notification.id ? { ...n, isRead: true } : n
      ));
      if (notification.link) navigate(notification.link);
    } catch (error) {
      console.error('Failed to mark as read:', error);
    }
  };

  // Видалити сповіщення
  const deleteNotification = async (id: number) => {
    try {
      await apiDeleteNotification(id);
      setNotifications(prev => prev.filter(n => n.id !== id));
      toast.success('Сповіщення видалено');
    } catch (error) {
      toast.error('Не вдалося видалити сповіщення');
    }
  };

  // Очистити всі прочитані
  const clearAllRead = async () => {
    try {
      await apiClearReadNotifications();
      setNotifications(prev => prev.filter(n => !n.isRead));
      toast.success('Прочитані сповіщення видалено');
    } catch (error) {
      toast.error('Не вдалося очистити сповіщення');
    }
  };

  const formatNotificationTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (isNaN(date.getTime())) return 'Невідомо';
    if (diffMins < 1) return 'Щойно';
    if (diffMins < 60) return `${diffMins} хв. тому`;
    if (diffHours < 24) return `${diffHours} год. тому`;
    if (diffDays === 1) return 'Вчора';
    if (diffDays < 7) return `${diffDays} дн. тому`;
    
    return date.toLocaleDateString('uk-UA', {
      day: 'numeric',
      month: 'long',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className={styles.notificationsPage}>
      <div className={styles.notificationsContainer}>
        <div className={styles.header}>
          <div className={styles.titleSection}>
            <h1 className={styles.title}>
              <i className="fas fa-bell"></i>
              Сповіщення
            </h1>
            <p className={styles.pageSubtitle}>Будьте в курсі всіх подій вашого спортивного життя</p>
            <div className={styles.unreadCount}>
              {notifications.filter(n => !n.isRead).length} нових сповіщень
            </div>
          </div>

          <div className={styles.tabSwitcher}>
            <button 
              className={`${styles.tabBtn} ${activeTab === 'notifications' ? styles.active : ''}`}
              onClick={() => setActiveTab('notifications')}
            >
              <i className="fas fa-bell"></i>
              Сповіщення
            </button>
            <button 
              className={`${styles.tabBtn} ${activeTab === 'messages' ? styles.active : ''}`}
              onClick={() => setActiveTab('messages')}
            >
              <i className="fas fa-comment-dots"></i>
              Повідомлення
            </button>
          </div>
          
          {activeTab === 'notifications' && (
            <div className={styles.actions}>
              <button className={styles.markAllBtn} onClick={markAllAsRead}>
                <i className="fas fa-check-double"></i>
                Прочитати все
              </button>
              <button className={styles.clearReadBtn} onClick={clearAllRead}>
                <i className="fas fa-broom"></i>
                Очистити
              </button>
            </div>
          )}
        </div>

      {activeTab === 'notifications' ? (
        <>
          <div className={styles.filters}>
            <button 
              className={`${styles.filterBtn} ${filter === 'all' ? styles.active : ''}`}
              onClick={() => setFilter('all')}
            >
              Всі
            </button>
            <button 
              className={`${styles.filterBtn} ${filter === 'unread' ? styles.active : ''}`}
              onClick={() => setFilter('unread')}
            >
              Непрочитані
            </button>
            <button 
              className={`${styles.filterBtn} ${filter === 'read' ? styles.active : ''}`}
              onClick={() => setFilter('read')}
            >
              Прочитані
            </button>
          </div>

          <div className={styles.notificationsList}>
            {filteredNotifications.length > 0 ? (
              filteredNotifications.map(notification => (
                <div 
                  key={notification.id} 
                  className={`${styles.notificationCard} ${!notification.isRead ? styles.unread : ''} ${notification.link ? styles.clickable : ''}`}
                  onClick={() => markAsRead(notification)}
                >
                  <div 
                    className={styles.notificationIcon}
                    style={{ backgroundColor: `${notification.color}15`, color: notification.color }}
                  >
                    <i className={notification.icon}></i>
                  </div>
                  
                  <div className={styles.notificationContent}>
                    <div className={styles.notificationHeader}>
                      <h3 className={styles.notificationTitle}>{notification.title}</h3>
                      <span className={styles.notificationTime}>{formatNotificationTime(notification.createdAt)}</span>
                    </div>
                    <p className={styles.notificationMessage}>{notification.message}</p>
                  </div>
                  
                  <div className={styles.notificationActions}>
                    {!notification.isRead && <div className={styles.unreadDot}></div>}
                    <button 
                      className={styles.deleteBtn}
                      onClick={(e) => {
                        e.stopPropagation();
                        deleteNotification(notification.id);
                      }}
                      title="Видалити"
                    >
                      <i className="fas fa-trash-alt"></i>
                    </button>
                  </div>
                </div>
              ))
            ) : !loading ? (
              <div className={styles.emptyState}>
                <div className={styles.emptyIcon}>
                  <i className="fas fa-bell-slash"></i>
                </div>
                <p>У вас немає сповіщень</p>
              </div>
            ) : (
              <div className={styles.emptyState}>
                <p>Завантаження...</p>
              </div>
            )}
          </div>
        </>
      ) : (
        <MessengerView />
      )}
      </div>
    </div>
  );
};

export default NotificationsPage;
