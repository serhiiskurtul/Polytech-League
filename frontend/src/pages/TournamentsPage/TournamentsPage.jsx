import TournamentsList from '../../widgets/tournaments-list/TournamentsList.jsx';
import CreateTournament from "../../features/create-tournament/CreateTournament.jsx";
import TournamentFilter from '../../features/tournament-filter/TournamentFilter.jsx';
import styles from './TournamentsPage.module.css';

const tournamentsPage = [
  { id: 1, title: 'Осінній Кубок 2025', teams: 8, status: 'ongoing', date: '15.10.2025 — 20.12.2025' },
  { id: 2, title: 'Студентська Ліга 2025', teams: 12, status: 'upcoming', date: '10.01.2026 — 28.02.2026' },
  { id: 3, title: 'Фестиваль волейболу', teams: 16, status: 'finished', date: '05.09.2025 — 12.09.2025' },
  { id: 4, title: 'Чемпіонат з баскетболу', teams: 10, status: 'ongoing', date: '01.12.2025 — 31.12.2025' },
  { id: 5, title: 'Турнір з настільного тенісу', teams: 24, status: 'finished', date: '20.11.2025 — 25.11.2025' },
];

export default function TournamentsPage() {
  return (
    <div className={styles.container}>
      <div className={styles.layout}>
        <main className={styles.main}>
          <TournamentsList tournaments={tournamentsPage} />
        </main>

        <aside className={styles.sidebar}>
          <CreateTournament />
          <TournamentFilter />
        </aside>
      </div>
    </div>
  );
}
