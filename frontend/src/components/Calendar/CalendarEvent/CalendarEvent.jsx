import './CalendarEvent.css';

export default function CalendarEvent({
  date,
  time,
  sport,
  title,
  opponent,
  location,
  status,
  result,
  outcome,
}) {
  const statusClass = status === "upcoming" ? "upcoming" : "finished";
  const outcomeClass = outcome ? `outcome-${outcome}` : "";

  return (
    <section className={`calendar-item ${statusClass} ${outcomeClass} standard-hover-animation`}>
      <div className="date-time">
        <div className="date">{date}</div>
        <div className="time">{time}</div>
      </div>

      <div className="event-info">
        <div className="sport-name">{sport}</div>
        <div className="event-title">{title}</div>
        <div className="opponent">
          {status === "upcoming" ? "Проти" : "Проти"} {opponent}
        </div>
        <div className="location">{location}</div>
      </div>

      <div className={`result ${outcomeClass}`}>
        {status === "finished" ? (result || "—") : "Скоро"}
      </div>
    </section>
  );
}