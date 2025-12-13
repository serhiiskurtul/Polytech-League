import './TournamentsList.css';
import TournamentItem from '../TournamentsListItem/TournamentsListItem';

export default function TournamentsList({ tournaments }) {
  return (
    <section className="card">
        <div className="header-row header-row-tournaments">
            <h3>Турніри</h3>
            <p className="pill">Список турнірів</p>
        </div>

        <div className="tournament-list">
            {tournaments.map((t) => (
            <TournamentItem
                key={t.id}
                title={t.title}
                teams={t.teams}
                status={t.status}
                id={t.id}
                date={t.date}
            />
            ))}
        </div>
    </section>
  );
}