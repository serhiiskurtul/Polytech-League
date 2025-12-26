import AnnouncementItem from '../../entities/announcement-item/AnnouncementItem';
import styles from './Announcement.module.css';
import BaseCard from '../../shared/BaseCard/BaseCard';

export default function Announcement({ items }) {
  return (
    <BaseCard className={styles.announcementSection}>
        <div className={styles.headerRow}>
            <h3 className={styles.h3Announcements}>Анонси</h3>
            <small className={styles.pill}>Останні</small>
        </div>
        {items.map((item, index) => (
          <AnnouncementItem
            key={index}
            title={item.title}
            date={item.date}
            location={item.location}
            time={item.time}
          />
        ))}
    </BaseCard>
  );
}

