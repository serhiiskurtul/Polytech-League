import './Sidebar.css';
import TopPlayers from './TopPlayers/TopPlayers.jsx';

const items = [
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

export default function Sidebar() {
  return (
    <div className="sidebar">
      <TopPlayers items={items} />
    </div>
  );
}