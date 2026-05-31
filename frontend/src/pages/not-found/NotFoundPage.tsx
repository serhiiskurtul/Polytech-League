import styles from './NotFoundPage.module.css';
import { useNavigate } from "react-router-dom";

export default function NotFound() {
  const navigate = useNavigate();

  return (
    <div className={styles.spacer}>
      <div className={styles.card}>
        <h1 className={styles.code}>404</h1>
        <p className={styles.subtitle}>Сторінку не знайдено</p>

        <button
          className={styles.button}
          onClick={() => navigate('/')}
        >
          На головну
        </button>
      </div>
    </div>
  );
}
