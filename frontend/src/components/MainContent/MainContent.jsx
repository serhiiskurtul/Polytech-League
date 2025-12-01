import Announcement from './Announcement/Announcement';
import LastMatch from './LastMatch/LastMatch';
import './MainContent.css';

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
  }
];

const lastmatches = [
  {
    firstOpponent: "Університет А",
    secondOpponent: "Коледж Б",
    result: "2 — 1",
  },
  {
    firstOpponent: "Кафедра IТ",
    secondOpponent: "Коледж С",
    result: "0 — 0",
  }
];

export default function MainContent() {
  return (
    <main>
      <Announcement items={announcements} />
      <LastMatch items={lastmatches}/>
    </main>
  );
}
