import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/entities/session';
import styles from './VerifyEmailBanner.module.css';

const VerifyEmailBanner = () => {
  const { user, loading } = useAuth();
  const navigate = useNavigate();

  if (loading || !user || user.isEmailConfirmed !== false) return null;

  return (
    <div className={styles.banner} role="status">
      <span className={styles.text}>
        Підтвердіть пошту <strong>{user.email}</strong>, щоб мати змогу відновити пароль.
      </span>
      <button
        type="button"
        className={styles.button}
        onClick={() => navigate('/email-verification', { state: { email: user.email, sendOnOpen: true } })}
      >
        Підтвердити
      </button>
    </div>
  );
};

export default VerifyEmailBanner;
