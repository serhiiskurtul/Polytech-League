import { Link } from "react-router-dom";
import styles from './Header.module.css';
import Logo from "../../shared/ui/Logo/Logo.jsx";

export default function Header() {
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
              <Link to="/tournaments-page" className={styles.navLink}>
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


          <div className={styles.userProfile}>
            <div className={styles.userAvatar}>АІ</div>
            <span className={styles.userName}>Артем І.</span>
          </div>
        </div>
      </div>
    </header>
  );
};