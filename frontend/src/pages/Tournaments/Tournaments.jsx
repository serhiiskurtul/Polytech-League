import TournamentsList from '../../components/Tournaments/TournamentsList/TournamentsList';
import CreateTournaments from '../../components/Tournaments/TournamentsSidebar/CreateTournament/CreateTournament';
import TournamentFilter from '../../components/Tournaments/TournamentsSidebar/TournamentFilter/TournamentFilter';

const tournaments = [
  { id: 1, title: 'Осінній Кубок 2025', teams: 8, status: 'ongoing', date: '15.10.2025 — 20.12.2025' },
  { id: 2, title: 'Студентська Ліга 2025', teams: 12, status: 'upcoming', date: '10.01.2026 — 28.02.2026' },
  { id: 3, title: 'Фестиваль волейболу', teams: 16, status: 'finished', date: '05.09.2025 — 12.09.2025' },
  { id: 4, title: 'Чемпіонат з баскетболу', teams: 10, status: 'ongoing', date: '01.12.2025 — 31.12.2025' },
  { id: 5, title: 'Турнір з настільного тенісу', teams: 24, status: 'finished', date: '20.11.2025 — 25.11.2025' },
];

export default function Tournaments() {
  return (
    <div className="container">
      <div className="Layout">
        {/* Основний контент — ліворуч */}
        <TournamentsList tournaments={tournaments} />

        {/* Сайдбар — праворуч */}
        <aside className="tournaments-sidebar">
          <CreateTournaments />
          <TournamentFilter />
        </aside>
      </div>
    </div>
  );
}