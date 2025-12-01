import AnnouncementItem from './AnnouncementItem/AnnouncementItem';
import './Announcement.css';

export default function Announcement({ items }) {
  return (
    <section className="card">
        <div className="header-row">
            <h3>Анонси</h3>
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
    </section>
  );
}

