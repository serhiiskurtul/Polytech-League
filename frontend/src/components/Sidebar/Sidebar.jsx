import TopPlayers from './TopPlayers/TopPlayers.jsx';
import CurrentEvents from './CurrentEvents/CurrentEvents.jsx';
import GlobalStatisticSeson from './GlobalStatisticSeson/GlobalStatisticSeson.jsx';

const TopPlayersList = [
  {
    avatar: 'АІ',
    name: 'Іванов Артем',
    achievement: '2184 Elo',
    sport: 'Шахи',
    grade: '3 курс',
  },
  {
    avatar: 'KB',
    name: 'Козак Богдан',
    achievement: '200m у фіналі',
    sport: 'Легка атлетика',
    grade: '2 курс',
  },
  {
    avatar: 'MS',
    name: 'Мельник Світлана',
    achievement: 'Чемпіонка області',
    sport: 'Волейбол',
    grade: '1 курс',
  },
  {
    avatar: 'DN',
    name: 'Дмитро Нікітін',
    achievement: 'Переможець турніру',
    sport: 'Футбол',
    grade: '4 курс',
  },
  {
    avatar: 'OL',
    name: 'Олена Литвин',
    achievement: 'Рекордсменка університету',
    sport: 'Плавання',
    grade: '2 курс',
  },
];

// Поточні події - події, які 
// були протягом минулого тижня, 
// сьогодні будуть або найближчим часом (7 днів)
const CurrentEventsList = [
  {
    title: "Весняний турнір з футболу",
    date: "15.04.2025",
  },
  {
    title: "Обласні змагання з волейболу",
    date: "22.05.2025",
  }
];

const GlobalStatisticSesonList = {
  activePlayers: 428,
  tournaments: 18,
  matchesPlayed: 1247
}

export default function Sidebar() {
  return (
    <div className="sidebar">
      <TopPlayers items={TopPlayersList} />
      <CurrentEvents items={CurrentEventsList} />
      <GlobalStatisticSeson 
        activePlayers={GlobalStatisticSesonList.activePlayers} 
        tournaments={GlobalStatisticSesonList.tournaments} 
        matchesPlayed={GlobalStatisticSesonList.matchesPlayed} 
      />
    </div>
  );
}