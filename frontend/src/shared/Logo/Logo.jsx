import { Link } from 'react-router-dom';
import styles from './Logo.module.css';

export default function Logo() {
  return (
    <Link to="/" className={styles.logoLink}>
      <div className={styles.logoIcon}>
        <div className={styles.field}>
          <div className={styles.centerCircle}></div>
          <div className={styles.centerLine}></div>
        </div>
      </div>
    </Link>
  );
}
