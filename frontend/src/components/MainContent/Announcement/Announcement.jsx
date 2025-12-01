import AnnouncementItem from './AnnouncementItem/AnnouncementItem';
import './Announcement.css';

export default function Announcement({ items }) {
  return (
    <div className="announcement-wrapper">
      <h3>Анонси</h3>
      {items.map((item, index) => (
        <AnnouncementItem
          key={index}
          title={item.title}
          date={item.date}
          location={item.location}
          time={item.time}
        />
      ))}
    </div>
  );
}

