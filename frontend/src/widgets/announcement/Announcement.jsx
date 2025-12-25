import AnnouncementItem from '../../entities/announcement-item/AnnouncementItem';
import styles from './Announcement.module.css';
import BaseCard from '../../shared/BaseCard/BaseCard';

export default function Announcement({ items }) {
  return (
    <BaseCard className={styles.announcementSection}>
        <div className="header-row">
            <h3 id="h3-announcements">Анонси</h3>
            <small className="pill">Останні</small>
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

