import { Link } from "react-router-dom";
import styles from './Header.module.css';
import { Logo } from "@/shared/ui";
import { useAuth } from "@/app/providers/AuthContext";

export default function Header() {
  const { user, isAuthenticated } = useAuth();

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
          <button className={styles.notificationBtn}>
            <i className="fas fa-bell"></i>
            <span className={styles.notificationBadge}></span>
          </button>

          {isAuthenticated && user ? (
            <Link to="/profile" className={styles.userProfileLink}>
              <div className={styles.userProfile}>
                <div className={styles.userAvatar}>{initials || <i className="fas fa-user"></i>}</div>
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
