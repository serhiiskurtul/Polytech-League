import styles from './TournamentsList.module.css';
import TournamentItem from "../../entities/tournament/TournamentsListItem";

interface Tournament {
  id: string;
  title: string;
  teams: number;
  status: 'ongoing' | 'upcoming' | 'finished';
  date: string;
}

interface TournamentsListProps {
  tournaments: Tournament[];
}

export default function TournamentsList({ tournaments }: TournamentsListProps) {
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
