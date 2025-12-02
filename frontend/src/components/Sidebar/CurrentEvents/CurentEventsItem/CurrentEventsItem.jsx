import './CurrentEventsItem.css';

export default function CurrentEventsItem({ title, date }) {
  return (
    <div className="currentevents card-item" style = {{paddingBottom: '16px'}}>
        <div className="event">
            <div className="bar"></div>
            <div className="event-info">
                <strong className='player-name'>{title}</strong>
                <small style={{color: "var(--muted)"}}>{date}</small>
            </div>
        </div>
    </div>
  );
}
