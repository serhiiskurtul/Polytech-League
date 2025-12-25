import styles from './TopPlayers.module.css';
import TopPlayersItem from '../../entities/top-players-item/TopPlayersItem.jsx';

export default function TopPlayers({ items }) {
  return (
    <div className={styles.card}>
      <div className={styles.headerRow}>
        <h3>ТОП-10 гравців коледжу</h3>
        <span className={styles.pillSuccess}>Сезон 2025/26</span>
      </div>

      <div className={styles.leaderList}>
        {items.map((item, index) => (
          <TopPlayersItem
            key={index}
            avatar={item.avatar}
            name={item.name}
            achievement={item.achievement}
            sport={item.sport}
            grade={item.grade}
          />
        ))}
      </div>
    </div>
  );
}
