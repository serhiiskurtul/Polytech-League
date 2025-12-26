import React from 'react';
import styles from './VerificationButton.module.css';

const VerificationButton = ({ onClick, disabled, isLoading, isVerified }) => {
  return (
    <button
      className={`${styles.btn} ${isVerified ? styles.btnSuccess : ''}`}
      onClick={onClick}
      disabled={disabled}
    >
      {isLoading ? 'Перевірка...' : 
       isVerified ? 'Підтверджено!' : 
       'Підтвердити email'}
    </button>
  );
};

export default VerificationButton;