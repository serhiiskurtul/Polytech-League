import styles from './GlobalStatisticSeson.module.css';

export default function GlobalStatisticSeson({
  activePlayers = 428,
  tournaments = 18,
  matchesPlayed = 1247,
  sportsCount = 8,
}) {
  const stats = [
    { value: activePlayers, label: 'активних гравців' },
    { value: tournaments, label: 'турнірів' },
    { value: matchesPlayed, label: 'зіграно матчів' },
    { value: sportsCount, label: 'видів спорту' },
  ];

  return (
    <div className={styles.card}>
      <h3 className={styles.header}>Статистика сезону</h3>
      <div className={styles.stats}>
        {stats.map((stat, index) => (
          <div key={index} className={styles.stat}>
            <div className={styles.statNum}>{stat.value.toLocaleString()}</div>
            <div>{stat.label}</div>
          </div>
        ))}
      </div>
    </div>
  );
}
