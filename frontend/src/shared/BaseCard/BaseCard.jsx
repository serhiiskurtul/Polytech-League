import styles from './BaseCard.module.css';

export default function BaseCard({ children, className = '' }) {
  return (
    <div className={`${styles.card} ${className}`}>
      {children}
    </div>
  );
}
