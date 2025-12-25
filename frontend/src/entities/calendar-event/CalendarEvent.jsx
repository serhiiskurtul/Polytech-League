import styles from './CalendarEvent.module.css';

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
  const statusClass =
    status === 'upcoming' ? styles.upcoming : styles.finished;

  const outcomeClass = outcome ? styles[`outcome-${outcome}`] : '';

  return (
    <section
      className={`${styles.calendarItem} ${statusClass} ${outcomeClass} standard-hover-animation`}
    >
      <div className={styles.dateTime}>
        <div className={styles.date}>{date}</div>
        <div className={styles.time}>{time}</div>
      </div>

      <div className={styles.eventInfo}>
        <div className={styles.sportName}>{sport}</div>
        <div className={styles.eventTitle}>{title}</div>
        <div className={styles.opponent}>
          {status === 'upcoming' ? 'Проти' : 'Проти'} {opponent}
        </div>
        <div className={styles.location}>{location}</div>
      </div>

      <div className={`${styles.result} ${outcomeClass}`}>
        {status === 'finished' ? (result || '—') : 'Скоро'}
      </div>
    </section>
  );
}
