import styles from './TournamentsList.module.css';
import TournamentItem from "../../entities/tournaments-list-item/TournamentsListItem.jsx";

export default function TournamentsList({ tournaments }) {
  return (
    <section className={styles.card}>
      <h3>Список Турнірів</h3>

      <div className={styles.tournamentList}>
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
