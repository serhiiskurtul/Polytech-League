import './CurrentEvents.module.css';
import CurrentEventsItem from '../../entities/current-events-item/CurrentEventsItem.jsx';

export default function CurrentEvents({ items }) {
  return (
    <div className="card">
        <div className="header-row">
            <h3>Поточні події</h3>
        </div>
        <div className="leader-list">
        {items.map((item, index) => (
            <CurrentEventsItem
                key={index}
                title={item.title}
                date={item.date}
            />
        ))}
        </div>
    </div>
  );
}
