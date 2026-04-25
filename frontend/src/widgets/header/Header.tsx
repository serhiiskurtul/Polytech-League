import { Link } from "react-router-dom";
import styles from './Header.module.css';
import { Logo } from "@/shared/ui";
import { useAuth } from "@/app/providers/AuthContext";
import { useState } from "react";
import { useNotifications } from "@/app/providers/NotificationProvider";
import { useChat } from "@/app/providers/ChatProvider";

export default function Header() {
  const { user, isAuthenticated } = useAuth();
  const { unreadCount } = useNotifications();
  const { unreadTotal } = useChat();

  const totalUnread = unreadCount + unreadTotal;

  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const initials = user ? `${user.firstName[0] || ''}${user.lastName[0] || ''}`.toUpperCase() : '';

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
    if (!isMenuOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
  };

  const closeMenu = () => {
    setIsMenuOpen(false);
    document.body.style.overflow = 'unset';
  };

  return (
    <header className={`${styles.header} ${isMenuOpen ? styles.menuOpen : ''}`}>
      <div className={styles.container}>
        <div className={styles.logoContainer}>
          <Logo />
          <Link to="/" className={styles.logoLink} onClick={closeMenu}>
            <span className={styles.logoText}>Polytech League</span>
          </Link>
        </div>

        <nav className={`${styles.navPrimary} ${isMenuOpen ? styles.navActive : ''}`}>
          <ul className={styles.navList}>
            <li>
              <Link to="/" className={`${styles.navLink} ${styles.active}`} onClick={closeMenu}>
                <i className={`fas fa-home ${styles.navIcon}`}></i>
                <span>Головна</span>
              </Link>
            </li>
            <li>
              <Link to="/calendar" className={styles.navLink} onClick={closeMenu}>
                <i className={`fas fa-calendar-alt ${styles.navIcon}`}></i>
                <span>Календар</span>
              </Link>
            </li>
            <li>
              <Link to="/tournaments" className={styles.navLink} onClick={closeMenu}>
                <i className={`fas fa-trophy ${styles.navIcon}`}></i>
                <span>Змагання</span>
              </Link>
            </li>
            <li>
              <Link to="/ratings" className={styles.navLink} onClick={closeMenu}>
                <i className={`fas fa-user-friends ${styles.navIcon}`}></i>
                <span>Рейтинги гравців</span>
              </Link>
            </li>
            <li>
              <Link to="/group-ratings" className={styles.navLink} onClick={closeMenu}>
                <i className={`fas fa-users ${styles.navIcon}`}></i>
                <span>Рейтинги груп</span>
              </Link>
            </li>
          </ul>
        </nav>

        <div className={styles.userActions}>
          <Link to="/notifications" className={styles.notificationBtn} onClick={closeMenu}>
            <i className="fas fa-bell"></i>
            {totalUnread > 0 && (
              <span className={styles.notificationBadge}>{totalUnread}</span>
            )}
          </Link>

          {isAuthenticated && user ? (
            <Link to="/profile" className={styles.userProfileLink} onClick={closeMenu}>
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
            <Link to="/login" className={styles.loginBtn} onClick={closeMenu}>
              <i className="fas fa-sign-in-alt"></i>
              <span>Увійти</span>
            </Link>
          )}

          <button 
            className={`${styles.burgerBtn} ${isMenuOpen ? styles.burgerActive : ''}`} 
            onClick={toggleMenu}
            aria-label="Toggle menu"
          >
            <div className={styles.burgerIcon}>
              <span className={styles.burgerLine}></span>
              <span className={styles.burgerLine}></span>
              <span className={styles.burgerLine}></span>
            </div>
            <span className={styles.burgerText}>Меню</span>
          </button>
        </div>
      </div>
      {isMenuOpen && <div className={styles.overlay} onClick={closeMenu}></div>}
    </header>
  );
}
