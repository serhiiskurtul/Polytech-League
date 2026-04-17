import { Link } from "react-router-dom";
import styles from './Header.module.css';
import { Logo } from "@/shared/ui";
import { useAuth } from "@/app/providers/AuthContext";
import { useState, useEffect } from "react";
import { getMyNotifications } from "@/shared/api/notifications.api";
import { Notification } from "@/shared/types/notification.types";

export default function Header() {
  const { user, isAuthenticated } = useAuth();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    if (isAuthenticated) {
      const fetchNotifications = async () => {
        try {
          const data = await getMyNotifications();
          setNotifications(data);
          setUnreadCount(data.filter(n => !n.isRead).length);
        } catch (error) {
          console.error('Failed to fetch notifications in header:', error);
        }
      };

      fetchNotifications();
      // Опитування кожні 60 секунд (опціонально)
      const interval = setInterval(fetchNotifications, 60000);
      return () => clearInterval(interval);
    }
  }, [isAuthenticated]);

  const initials = user ? `${user.firstName[0] || ''}${user.lastName[0] || ''}`.toUpperCase() : '';

  return (
    <header className={styles.header}>
      <div className={styles.container}>
        <div className={styles.logoContainer}>
          <Logo />
          <Link to="/" className={styles.logoLink}>
            <span className={styles.logoText}>Polytech League</span>
          </Link>
        </div>

        <nav className={styles.navPrimary}>
          <ul className={styles.navList}>
            <li>
              <Link to="/" className={`${styles.navLink} ${styles.active}`}>
                <i className={`fas fa-home ${styles.navIcon}`}></i>
                Головна
              </Link>
            </li>
            <li>
              <Link to="/calendar" className={styles.navLink}>
                <i className={`fas fa-calendar-alt ${styles.navIcon}`}></i>
                Календар
              </Link>
            </li>
            <li>
              <Link to="/tournaments" className={styles.navLink}>
                <i className={`fas fa-trophy ${styles.navIcon}`}></i>
                Турніри
              </Link>
            </li>
            <li>
              <Link to="/ratings" className={styles.navLink}>
                <i className={`fas fa-chart-line ${styles.navIcon}`}></i>
                Рейтинги
              </Link>
            </li>
          </ul>
        </nav>

        <div className={styles.userActions}>
          <Link to="/notifications" className={styles.notificationBtn}>
            <i className="fas fa-bell"></i>
            {unreadCount > 0 && (
              <span className={styles.notificationBadge}>{unreadCount}</span>
            )}
          </Link>

          {isAuthenticated && user ? (
            <Link to="/profile" className={styles.userProfileLink}>
              <div className={styles.userProfile}>
                <div className={styles.userAvatar}>
                  {user.photoUrl && user.photoUrl.trim() !== '' ? (
                    <img 
                      src={`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'}${user.photoUrl}`} 
                      alt={`${user.firstName} ${user.lastName}`} 
                      className={styles.avatarImg}
                    />
                  ) : (
                    initials || <i className="fas fa-user"></i>
                  )}
                </div>
                <span className={styles.userName}>{user.firstName} {user.lastName[0]}.</span>
              </div>
            </Link>
          ) : (
            <Link to="/login" className={styles.loginBtn}>
              <i className="fas fa-sign-in-alt"></i>
              <span>Увійти</span>
            </Link>
          )}
        </div>
      </div>
    </header>
  );
}
