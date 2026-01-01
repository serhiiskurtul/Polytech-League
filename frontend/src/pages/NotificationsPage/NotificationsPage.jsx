import React, { useState, useEffect } from 'react';
import styles from './NotificationsPage.module.css';

const NotificationsPage = () => {
  const [notifications, setNotifications] = useState([
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

  const [filter, setFilter] = useState('all'); // all, unread, read
  const [isLoading, setIsLoading] = useState(false);

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
  const markAsRead = (id) => {
    setNotifications(prev => prev.map(notification => 
      notification.id === id ? { ...notification, isRead: true } : notification
    ));
  };

  // Видалити сповіщення
  const deleteNotification = (id) => {
    setNotifications(prev => prev.filter(notification => notification.id !== id));
  };

  // Очистити всі прочитані
  const clearAllRead = () => {
    setNotifications(prev => prev.filter(notification => !notification.isRead));
  };

  // Завантажити більше сповіщень (імітація)
  const loadMore = () => {
    setIsLoading(true);
    setTimeout(() => {
      const newNotifications = [
        {
          id: notifications.length + 1,
          type: 'match',
          title: 'Результат матчу',
          message: 'Ваша команда виграла матч з рахунком 3:1. Так тримати!',
          time: '2 тижні тому',
          isRead: true,
          icon: 'fas fa-medal',
          color: 'var(--success)'
        },
        {
          id: notifications.length + 2,
          type: 'system',
          title: 'Новий функціонал',
          message: 'Додано можливість створення власних турнірів. Спробуйте!',
          time: '2 тижні тому',
          isRead: true,
          icon: 'fas fa-star',
          color: 'var(--primary)'
        }
      ];
      setNotifications(prev => [...prev, ...newNotifications]);
      setIsLoading(false);
    }, 1000);
  };

  // Кількість непрочитаних сповіщень
  const unreadCount = notifications.filter(n => !n.isRead).length;

  useEffect(() => {
    // Анімація появи сповіщень
    const cards = document.querySelectorAll(`.${styles.notificationCard}`);
    cards.forEach((card, index) => {
      card.style.animationDelay = `${index * 0.1}s`;
    });
  }, [filteredNotifications]);

  return (
    <div className={styles.notificationsPage}>
      <div className={styles.container}>
        {/* Заголовок сторінки */}
        <div className={styles.pageHeader}>
          <div className={styles.headerContent}>
            <h1 className={styles.pageTitle}>
              <i className="fas fa-bell"></i> Сповіщення
            </h1>
            {unreadCount > 0 && (
              <span className={styles.unreadBadge}>
                {unreadCount} нових
              </span>
            )}
          </div>
          <p className={styles.pageSubtitle}>
            Керуйте своїми сповіщеннями та залишайтесь в курсі подій
          </p>
        </div>

        {/* Панель управління */}
        <div className={styles.controlPanel}>
          <div className={styles.filterButtons}>
            <button 
              className={`${styles.filterBtn} ${filter === 'all' ? styles.active : ''}`}
              onClick={() => setFilter('all')}
            >
              <i className="fas fa-list"></i> Всі
            </button>
            <button 
              className={`${styles.filterBtn} ${filter === 'unread' ? styles.active : ''}`}
              onClick={() => setFilter('unread')}
            >
              <i className="fas fa-envelope"></i> Непрочитані
              {unreadCount > 0 && (
                <span className={styles.filterBadge}>{unreadCount}</span>
              )}
            </button>
            <button 
              className={`${styles.filterBtn} ${filter === 'read' ? styles.active : ''}`}
              onClick={() => setFilter('read')}
            >
              <i className="fas fa-envelope-open"></i> Прочитані
            </button>
          </div>

          <div className={styles.actionButtons}>
            <button 
              className={styles.actionBtn}
              onClick={markAllAsRead}
              disabled={unreadCount === 0}
            >
              <i className="fas fa-check-double"></i> Прочитати всі
            </button>
            <button 
              className={`${styles.actionBtn} ${styles.actionBtnDanger}`}
              onClick={clearAllRead}
            >
              <i className="fas fa-trash"></i> Очистити прочитані
            </button>
          </div>
        </div>

        {/* Список сповіщень */}
        <div className={styles.notificationsList}>
          {filteredNotifications.length === 0 ? (
            <div className={styles.emptyState}>
              <div className={styles.emptyIcon}>
                <i className="fas fa-bell-slash"></i>
              </div>
              <h3>Немає сповіщень</h3>
              <p>Тут будуть з'являтися ваші сповіщення</p>
            </div>
          ) : (
            <>
              {filteredNotifications.map(notification => (
                <div 
                  key={notification.id} 
                  className={`${styles.notificationCard} ${notification.isRead ? styles.read : styles.unread}`}
                >
                  <div className={styles.notificationIcon} style={{ color: notification.color }}>
                    <i className={notification.icon}></i>
                  </div>
                  
                  <div className={styles.notificationContent}>
                    <div className={styles.notificationHeader}>
                      <h4 className={styles.notificationTitle}>{notification.title}</h4>
                      <span className={styles.notificationTime}>{notification.time}</span>
                    </div>
                    <p className={styles.notificationMessage}>{notification.message}</p>
                    
                    <div className={styles.notificationActions}>
                      {!notification.isRead && (
                        <button 
                          className={styles.notificationAction}
                          onClick={() => markAsRead(notification.id)}
                        >
                          <i className="fas fa-check"></i> Прочитати
                        </button>
                      )}
                      <button 
                        className={styles.notificationAction}
                        onClick={() => deleteNotification(notification.id)}
                      >
                        <i className="fas fa-times"></i> Видалити
                      </button>
                    </div>
                  </div>
                  
                  {!notification.isRead && (
                    <div className={styles.unreadIndicator}></div>
                  )}
                </div>
              ))}

              {/* Кнопка завантаження більше */}
              <div className={styles.loadMoreContainer}>
                <button 
                  className={styles.loadMoreBtn}
                  onClick={loadMore}
                  disabled={isLoading}
                >
                  {isLoading ? (
                    <>
                      <i className="fas fa-spinner fa-spin"></i> Завантаження...
                    </>
                  ) : (
                    <>
                      <i className="fas fa-redo"></i> Завантажити ще
                    </>
                  )}
                </button>
              </div>
            </>
          )}
        </div>

        {/* Статистика сповіщень */}
        <div className={styles.statsPanel}>
          <div className={styles.statCard}>
            <div className={styles.statIcon} style={{ background: 'rgba(59, 130, 246, 0.1)', color: 'var(--primary)' }}>
              <i className="fas fa-bell"></i>
            </div>
            <div className={styles.statInfo}>
              <div className={styles.statValue}>{notifications.length}</div>
              <div className={styles.statLabel}>Всього сповіщень</div>
            </div>
          </div>
          
          <div className={styles.statCard}>
            <div className={styles.statIcon} style={{ background: 'rgba(244, 63, 94, 0.1)', color: 'var(--accent)' }}>
              <i className="fas fa-envelope"></i>
            </div>
            <div className={styles.statInfo}>
              <div className={styles.statValue}>{unreadCount}</div>
              <div className={styles.statLabel}>Непрочитаних</div>
            </div>
          </div>
          
          <div className={styles.statCard}>
            <div className={styles.statIcon} style={{ background: 'rgba(16, 185, 129, 0.1)', color: 'var(--success)' }}>
              <i className="fas fa-envelope-open"></i>
            </div>
            <div className={styles.statInfo}>
              <div className={styles.statValue}>{notifications.length - unreadCount}</div>
              <div className={styles.statLabel}>Прочитаних</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NotificationsPage;