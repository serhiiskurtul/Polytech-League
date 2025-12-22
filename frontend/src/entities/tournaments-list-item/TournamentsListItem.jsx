import { Link } from 'react-router-dom';
import './TournamentsListItem.css';
import SimpleButton from "../../shared/SimpleButton/SimpleButton.jsx";

export default function TournamentItem({ title, teams, status, id, date }) {
  const statusConfig = {
    ongoing: { text: 'Триває', class: 'status-ongoing' },
    upcoming: { text: 'Заплановано', class: 'status-upcoming' },
    finished: { text: 'Завершено', class: 'status-finished' },
  };

  const config = statusConfig[status] || statusConfig.finished;

  return (
      <Link to={`/tournament/${id}`} className="tournament-link">
        <section className="tournament-item standard-hover-animation">
          <div className="tournament-info">
            <div className="tournament-title">{title}</div>
            <div className="tournament-details">
              <span className="tournament-date">📅 {date}</span> • Команд: <strong>{teams}</strong> •
              <span className={`status ${config.class}`}>{config.text}</span>
            </div>
          </div>

          <SimpleButton text="Переглянути" insideLink />
        </section>
      </Link>
  );
}
