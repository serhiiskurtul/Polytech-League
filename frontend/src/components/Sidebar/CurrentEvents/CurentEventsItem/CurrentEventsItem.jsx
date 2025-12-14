import './CurrentEventsItem.css';

export default function CurrentEventsItem({ title, date }) {
  return (
    <div className="currentevents card-item standard-hover-animation">
      <div className="bar"></div>

      <div className="event-info">
        <strong className="player-name">{title}</strong>
        <small style={{ color: "var(--muted)" }}>{date}</small>
      </div>
    </div>
  );
}

