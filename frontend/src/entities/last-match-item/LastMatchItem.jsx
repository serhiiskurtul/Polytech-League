import styles from './LastMatchItem.module.css';

export default function LastMatchItem({
  firstOpponent,
  secondOpponent,
  result,
  sportName,
  typeColor,
}) {
  return (
    <section className={`card-item ${styles.matchItem}`}>
      <div
        className={`${styles.match} ${styles[typeColor]} standard-hover-animation`}
      >
        <div className={styles.team}>{firstOpponent}</div>

        <div className={styles.matchInfo}>
          <div className={styles.result}>{result}</div>
          <div className={styles.sportName}>{sportName}</div>
        </div>

        <div className={styles.team}>{secondOpponent}</div>
      </div>
    </section>
  );
}
