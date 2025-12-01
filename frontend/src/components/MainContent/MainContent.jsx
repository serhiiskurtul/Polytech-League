import Announcement from './Announcement/Announcement';
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

export default function MainContent() {
  return (
    <main>
      <Announcement items={announcements} />
    </main>
  );
}
