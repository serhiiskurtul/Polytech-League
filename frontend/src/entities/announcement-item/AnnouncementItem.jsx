import styles from './AnnouncementItem.module.css';

export default function AnnouncementItem({ title, date, location, time }) {
  return (
    <div className={`${styles.announcement} card-item standard-hover-animation`}>
      <div className={styles.announcementBar}></div>

      <div className={styles.announcementContent}>
        <strong className={styles.announcementTitle}>{title}</strong>

        <div className={styles.announcementDetails}>
          {date} — {location} • Початок {time}
        </div>
      </div>
    </div>
  );
}
