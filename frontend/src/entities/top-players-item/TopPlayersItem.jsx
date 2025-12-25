import styles from './TopPlayersItem.module.css';

export default function TopPlayersItem({
  avatar,
  name,
  achievement,
  sport,
  grade,
}) {
  return (
    <div className={`card-item standard-hover-animation ${styles.topPlayers}`}>
      <div className={styles.avatar}>{avatar}</div>

      <div className={styles.playerInfo}>
        <strong className={styles.playerName}>{name}</strong>
        <small style={{ color: 'var(--muted)' }}>
          {achievement} • {sport} • {grade}
        </small>
      </div>
    </div>
  );
}
