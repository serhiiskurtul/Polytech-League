import './AnnouncementItem.css';

export default function AnnouncementItem({ title, date, location, time }) {
  return (
    <div className="announcement card-item standard-hover-animation">
      <div className="announcement-bar"></div>
      <div className="announcement-content">
        <strong className="announcement-title">{title}</strong>
        <div className="announcement-details">
          {date} — {location} • Початок {time}
        </div>
      </div>
    </div>

  );
}
