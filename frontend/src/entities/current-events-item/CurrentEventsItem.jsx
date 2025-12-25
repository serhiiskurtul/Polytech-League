import styles from './CurrentEventsItem.module.css';

export default function CurrentEventsItem({ title, date }) {
  return (
    <div className={`${styles.currentEvents} card-item standard-hover-animation`}>
      <div className={styles.bar}></div>

      <div className={styles.eventInfo}>
        <strong className={styles.playerName}>{title}</strong>
        <small className={styles.date}>{date}</small>
      </div>
    </div>
  );
}
