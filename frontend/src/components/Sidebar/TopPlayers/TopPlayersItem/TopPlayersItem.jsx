import './TopPlayersItem.css';

export default function TopPlayersItem({ avatar, name, achievement, sport, grade }) {
  return (
    <div className="topplayers card-item standard-hover-animation">
      <div className="avatar">{avatar}</div>
      <div className="player-info">
        <strong className="player-name">{name}</strong>
        <small style={{ color: "var(--muted)" }}>
          {achievement} • {sport} • {grade}
        </small>
      </div>
    </div>

  );
}
