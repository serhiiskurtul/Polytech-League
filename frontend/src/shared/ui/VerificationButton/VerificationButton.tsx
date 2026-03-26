import styles from './VerificationButton.module.css';
import { ButtonHTMLAttributes } from 'react';

interface VerificationButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  isLoading?: boolean;
  isVerified?: boolean;
}

const VerificationButton = ({ onClick, disabled, isLoading, isVerified, ...props }: VerificationButtonProps) => {
  return (
    <button
      className={`${styles.btn} ${isVerified ? styles.btnSuccess : ''}`}
      onClick={onClick}
      disabled={disabled}
      {...props}
    >
      {isLoading ? 'Перевірка...' : 
       isVerified ? 'Підтверджено!' : 
       'Підтвердити email'}
    </button>
  );
};

export default VerificationButton;
