import styles from './ButtonSign.module.css';

export default function ButtonSign({ children, onClick, type = "button", disabled = false }) {
  return (
    <button 
      className={styles.btnPrimary} 
      onClick={onClick}
      type={type}
      disabled={disabled}
    >
      {children}
    </button>
  );
}
