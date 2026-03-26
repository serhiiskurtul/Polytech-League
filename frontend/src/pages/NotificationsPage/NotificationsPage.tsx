import { useState } from 'react';
import styles from './NotificationsPage.module.css';

interface Notification {
  id: number;
  type: string;
  title: string;
  message: string;
  time: string;
  isRead: boolean;
  icon: string;
  color: string;
}

const NotificationsPage = () => {
  const [notifications, setNotifications] = useState<Notification[]>([
    {
      id: 1,
      type: 'match',
      title: 'Новий матч заплановано',
      message: 'Ваша команда "ІТ-31" запланована на матч з "ЕК-22" 15 грудня о 18:00',
      time: '10 хвилин тому',
      isRead: false,
      icon: 'fas fa-futbol',
      color: 'var(--primary)'
    },
    {
      id: 2,
      type: 'tournament',
      title: 'Реєстрація на турнір',
      message: 'Чемпіонат з шахів розпочинає реєстрацію. Термін до 12 грудня',
      time: '2 години тому',
      isRead: false,
      icon: 'fas fa-chess-board',
      color: 'var(--success)'
    },
    {
      id: 3,
      type: 'achievement',
      title: 'Нове досягнення',
      message: 'Ви отримали досягнення "Активний учасник" за участь у 10 матчах',
      time: 'Вчора, 14:30',
      isRead: true,
      icon: 'fas fa-trophy',
      color: 'var(--warning)'
    },
    {
      id: 4,
      type: 'team',
      title: 'Запрошення до команди',
      message: 'Марія Семенова запрошує вас до команди з настільного тенісу',
      time: '2 дні тому',
      isRead: true,
      icon: 'fas fa-user-plus',
      color: 'var(--accent)'
    },
    {
      id: 5,
      type: 'system',
      title: 'Оновлення системи',
      message: 'Заплановано технічні роботи на платформі 14 грудня з 23:00 до 02:00',
      time: '3 дні тому',
      isRead: true,
      icon: 'fas fa-cog',
      color: 'var(--muted)'
    },
    {
      id: 6,
      type: 'friend',
      title: 'Новий друг',
      message: 'Влад Ковальчук прийняв ваш запит на дружбу',
      time: '1 тиждень тому',
      isRead: true,
      icon: 'fas fa-user-friends',
      color: 'var(--primary)'
    }
  ]);

  const [filter, setFilter] = useState<'all' | 'unread' | 'read'>('all');

  // Фільтрація сповіщень
  const filteredNotifications = notifications.filter(notification => {
    if (filter === 'all') return true;
    if (filter === 'unread') return !notification.isRead;
    if (filter === 'read') return notification.isRead;
    return true;
  });

  // Позначити всі як прочитані
  const markAllAsRead = () => {
    setNotifications(prev => prev.map(notification => ({
      ...notification,
      isRead: true
    })));
  };

  // Позначити конкретне сповіщення як прочитане
  const markAsRead = (id: number) => {
    setNotifications(prev => prev.map(notification => 
      notification.id === id ? { ...notification, isRead: true } : notification
    ));
  };

  // Видалити сповіщення
  const deleteNotification = (id: number) => {
    setNotifications(prev => prev.filter(notification => notification.id !== id));
  };

  // Очистити всі прочитані
  const clearAllRead = () => {
    setNotifications(prev => prev.filter(notification => !notification.isRead));
  };

  return (
    <div className={styles.notificationsContainer}>
      <div className={styles.header}>
        <div className={styles.titleSection}>
          <h1 className={styles.title}>
            <i className="fas fa-bell"></i>
            Сповіщення
          </h1>
          <div className={styles.unreadCount}>
            {notifications.filter(n => !n.isRead).length} нових
          </div>
        </div>
        
        <div className={styles.actions}>
          <button className={styles.markAllBtn} onClick={markAllAsRead}>
            Позначити всі як прочитані
          </button>
          <button className={styles.clearReadBtn} onClick={clearAllRead}>
            Очистити прочитані
          </button>
        </div>
      </div>

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
              className={`${styles.notificationCard} ${!notification.isRead ? styles.unread : ''}`}
              onClick={() => markAsRead(notification.id)}
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
                  <span className={styles.notificationTime}>{notification.time}</span>
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
        ) : (
          <div className={styles.emptyState}>
            <div className={styles.emptyIcon}>
              <i className="fas fa-bell-slash"></i>
            </div>
            <p>У вас немає нових сповіщень</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default NotificationsPage;
