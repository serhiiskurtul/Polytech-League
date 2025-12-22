import './TournamentsList.css';
import TournamentItem from "../../entities/tournaments-list-item/TournamentsListItem.jsx";

export default function TournamentsList({ tournaments }) {
    return (
        <section className="card">
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
