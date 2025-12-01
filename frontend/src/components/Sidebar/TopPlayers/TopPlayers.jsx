import './TopPlayers.css';
import TopPlayersItem from './TopPlayersItem/TopPlayersItem.jsx';

export default function TopPlayers({ items }) {
  return (
    <div className="card">
        <div className="header-row">
            <h3>ТОП-10 гравців коледжу</h3>
            <span className="pill pill-success">Сезон 2025/26</span>
        </div>
        <div className="leader-list">
        {items.map((item, index) => (
            <TopPlayersItem
                key={index}
                avatar={item.avatar}
                name={item.name}
                achievement={item.achievement}
                sport={item.sport}
                grade={item.grade}
            />
        ))}
        </div>
    </div>
  );
}
