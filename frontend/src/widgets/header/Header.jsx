import { Link } from "react-router-dom";
import styles from './Header.module.css';
import Logo from "../../shared/Logo/Logo.jsx";

export default function Header() {
  return (
    <header className={styles.header}>
      <div className={styles.containerHeader}>
        <Logo />
        <Link to="/" className={styles.logoLink}>
          <div className={styles.logoText}>Campus League</div>
        </Link>
      </div>

      <nav className={styles.navPrimary}>
        <ul className={styles.navList}>
          <li><Link to="/calendar" className={styles.navLink}>Календар</Link></li>
          <li><Link to="/tournamentsPage" className={styles.navLink}>Турніри</Link></li>
          <li><Link to="/teams" className={styles.navLink}>Команди</Link></li>
          <li><Link to="/ratings" className={styles.navLink}>Рейтинги</Link></li>
        </ul>
      </nav>

      <nav className={styles.navSecondary}>
        <ul className={styles.navList}>
          <li><Link to="/notifications" className={styles.navLink}>Сповіщення</Link></li>
          <li><Link to="/login" className={styles.navLink}>Увійти</Link></li>
        </ul>
      </nav>
    </header>
  );
}
