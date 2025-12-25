import styles from './HomePage.module.css';
import Announcement from '../../widgets/announcement/Announcement.jsx';
import LastMatch from '../../widgets/last-match/LastMatch.jsx';
import TopPlayers from '../../widgets/top-players/TopPlayers.jsx';
import CurrentEvents from '../../widgets/current-events/CurrentEvents.jsx';
import GlobalStatisticSeson from '../../widgets/global-statistic-seson/GlobalStatisticSeson.jsx';

const announcements = [
  {
    title: "Фінал між «Університет А» та «Коледж Б»",
    date: "01.11.2025",
    location: "Стадіон Центральний",
    time: "18:00"
  },
  {
    title: "Фінал між «Університет C» та «Коледж B»",
    date: "01.07.2025",
    location: "Стадіон Центральний",
    time: "8:00"
  },
  {
    title: "Чемпіонат коледжу з шахів (швейцарка)",
    date: "18.12.2025",
    location: "Актова зала",
    time: "11:00"
  }
];

const lastmatches = [
  {
    firstOpponent: "Університет А",
    secondOpponent: "Коледж Б",
    result: "2 — 1",
    sportName: "Футбол",
    typeColor: "isSuccess"
  },
  {
    firstOpponent: "Кафедра IТ",
    secondOpponent: "Коледж С",
    result: "0 — 1",
    sportName: "Волейбол",
    typeColor: "isFail"
  },
  {
    firstOpponent: "Університет D",
    secondOpponent: "Коледж A",
    result: "0 — 0",
    sportName: "Шахи",
    typeColor: "isDraw"
  }
];

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


export default function HomePage() {
  return (
    <div className={styles.homePage}>
      <main className={styles.main}>
        <Announcement items={announcements} />
        <LastMatch items={lastmatches} />
      </main>

      <aside className={styles.sidebar}>
        <TopPlayers items={TopPlayersList} />
        <CurrentEvents items={CurrentEventsList} />
        <GlobalStatisticSeson
          activePlayers={GlobalStatisticSesonList.activePlayers}
          tournaments={GlobalStatisticSesonList.tournaments}
          matchesPlayed={GlobalStatisticSesonList.matchesPlayed}
        />
      </aside>
    </div>
  );
}
