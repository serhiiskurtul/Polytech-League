import './Calendar.css';
import CalendarEvent from "../../entities/calendar-event/CalendarEvent.jsx";

const events = [
  {
    date: "15.12.2025",
    time: "14:00",
    sport: "Футбол",
    title: "Товариський матч",
    opponent: "Коледж А",
    location: "Стадіон коледжу",
    status: "upcoming",
  },
  {
    date: "18.12.2025",
    time: "11:00",
    sport: "Шахи",
    title: "Чемпіонат коледжу (швейцарська система)",
    opponent: "Всі команди коледжу",
    location: "Актова зала",
    status: "upcoming",
  },
  {
    date: "20.12.2025",
    time: "16:00",
    sport: "Волейбол",
    title: "Матч 1/4 фіналу чемпіонату",
    opponent: "Коледж Б",
    location: "Спортзал №1",
    status: "upcoming",
  },
  {
    date: "05.12.2025",
    time: "18:00",
    sport: "Баскетбол",
    title: "Чвертьфінал кубка",
    opponent: "Університет D",
    location: "Стадіон Центральний",
    result: "78 — 65",
    status: "finished",
    outcome: "win",
  },
  {
    date: "02.12.2025",
    time: "17:30",
    sport: "Футбол",
    title: "Груповий етап",
    opponent: "Коледж C",
    location: "Стадіон коледжу",
    result: "1 — 3",
    status: "finished",
    outcome: "loss",
  },
  {
    date: "28.11.2025",
    time: "19:00",
    sport: "Теніс",
    title: "Турнір з тенісу",
    opponent: "Коледж А",
    location: "Корт №2",
    result: "6:4, 7:5",
    status: "finished",
    outcome: "win",
  },
];

export default function Calendar() {
  return (
    <div className="calendar-container">
      <div className="card calendar-card">
        <div className="header-row">
          <h3>Календар подій</h3>
        </div>

        <div className="calendar-list">
          {events.map((event, index) => (
            <CalendarEvent
              key={index}
              date={event.date}
              time={event.time}
              sport={event.sport}
              title={event.title}
              opponent={event.opponent}
              location={event.location}
              status={event.status}
              result={event.result ?? ""}
              outcome={event.outcome}
            />
          ))}
        </div>
      </div>
    </div>
  );
}