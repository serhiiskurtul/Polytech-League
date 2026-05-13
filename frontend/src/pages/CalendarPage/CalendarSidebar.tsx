import { useNavigate } from 'react-router-dom';
import { CalendarEventData } from './Calendar';

interface CalendarSidebarProps {
  selectedDate: Date;
  monthNames: string[];
  events: CalendarEventData[];
  sportNames: Record<string, string>;
}

const CalendarSidebar = ({ selectedDate, monthNames, events, sportNames }: CalendarSidebarProps) => {
  const navigate = useNavigate();

  const dayNames = ['Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П\'ятниця', 'Субота', 'Неділя'];
  const dayIndex = selectedDate.getDay();
  const selectedDayName = dayNames[dayIndex === 0 ? 6 : dayIndex - 1];
  
  const getSportClass = (sport: string) => {
    const classes: Record<string, string> = {
      'football': 'event-football',
      'basketball-3x3': 'event-basketball',
      'volleyball': 'event-volleyball',
      'beach-volleyball': 'event-beach-volleyball',
      'chess': 'event-chess',
      'checkers': 'event-checkers',
      'table-tennis': 'event-table-tennis',
      'badminton': 'event-badminton',
      'armwrestling': 'event-armwrestling'
    };
    return classes[sport] || '';
  };

  const handleEventClick = (eventId: number) => {
    navigate(`/tournaments/${eventId}`);
  };

  return (
    <div className="day-details">
      <div className="selected-day-header">
        <div className="selected-day-title">
          <i className="far fa-calendar-alt"></i>
          <span>
            {selectedDate.getDate()} {monthNames[selectedDate.getMonth()]} {selectedDate.getFullYear()}
          </span>
        </div>
        <div className="page-subtitle">{selectedDayName}</div>
      </div>
      
      <div className="events-list">
        {events.length === 0 ? (
          <div className="no-events">
            <i className="far fa-calendar-times"></i>
            <h3>Подій немає</h3>
            <p>На цей день нічого не заплановано</p>
          </div>
        ) : (
          events.map(event => (
            <div key={event.id} className="event-card" onClick={() => handleEventClick(event.id)}>
              <div className="event-card-header">
                <div className={`event-card-sport ${getSportClass(event.sport)}`}>
                  {sportNames[event.sport]}
                </div>
                <div className="event-card-time">{event.time}</div>
              </div>
              
              <div className="event-card-title">{event.title}</div>
              
              <div className="event-card-location">
                <i className="fas fa-map-marker-alt"></i>
                <span>{event.location}</span>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default CalendarSidebar;
