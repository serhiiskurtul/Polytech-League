import { CalendarEventData } from './Calendar';

interface CalendarGridProps {
  currentDate: Date;
  selectedDate: Date;
  events: CalendarEventData[];
  activeSportFilter: string;
  onDaySelect: (day: number, month: number, year: number) => void;
}

interface CalendarDay {
  number: number;
  isCurrentMonth: boolean;
  isToday: boolean;
  isSelected: boolean;
  isWeekend: boolean;
}

const CalendarGrid = ({ 
  currentDate, 
  selectedDate, 
  events, 
  activeSportFilter, 
  onDaySelect 
}: CalendarGridProps) => {
  const currentMonth = currentDate.getMonth();
  const currentYear = currentDate.getFullYear();
  
  const dayNames = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Нд'];
  
  // Іконки для видів спорту
  const sportIcons: Record<string, string> = {
    'football': 'fas fa-futbol',
    'basketball-3x3': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'beach-volleyball': 'fas fa-umbrella-beach',
    'chess': 'fas fa-chess',
    'checkers': 'fas fa-chess-board',
    'table-tennis': 'fas fa-table-tennis',
    'badminton': 'fas fa-feather-alt',
    'armwrestling': 'fas fa-fist-raised'
  };

  // Отримати події для дня
  const getEventsForDay = (dayNumber: number) => {
    return events.filter(event => 
      event.day === dayNumber && 
      event.month === currentMonth && 
      event.year === currentYear
    ).filter(event => 
      activeSportFilter === 'all' || event.sport === activeSportFilter
    );
  };

  // Створити масив днів для календаря
  const getCalendarDays = () => {
    const firstDay = new Date(currentYear, currentMonth, 1);
    const lastDay = new Date(currentYear, currentMonth + 1, 0);
    
    let firstDayIndex = firstDay.getDay();
    firstDayIndex = firstDayIndex === 0 ? 6 : firstDayIndex - 1;
    
    const prevMonthLastDay = new Date(currentYear, currentMonth, 0).getDate();
    const daysInMonth = lastDay.getDate();
    
    const days: CalendarDay[] = [];
    
    // Дні попереднього місяця
    for (let i = 0; i < firstDayIndex; i++) {
      const dayNumber = prevMonthLastDay - firstDayIndex + i + 1;
      days.push({
        number: dayNumber,
        isCurrentMonth: false,
        isToday: false,
        isSelected: false,
        isWeekend: false
      });
    }
    
    // Дні поточного місяця
    const today = new Date();
    const isCurrentMonth = today.getMonth() === currentMonth && today.getFullYear() === currentYear;
    
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(currentYear, currentMonth, day);
      const dayOfWeek = date.getDay();
      const isToday = isCurrentMonth && day === today.getDate();
      const isSelected = day === selectedDate.getDate() && 
                        currentMonth === selectedDate.getMonth() && 
                        currentYear === selectedDate.getFullYear();
      const isWeekend = dayOfWeek === 0 || dayOfWeek === 6;
      
      days.push({
        number: day,
        isCurrentMonth: true,
        isToday,
        isSelected,
        isWeekend
      });
    }
    
    // Дні наступного місяця
    const totalCells = 42;
    const remainingCells = totalCells - days.length;
    
    for (let i = 1; i <= remainingCells; i++) {
      days.push({
        number: i,
        isCurrentMonth: false,
        isToday: false,
        isSelected: false,
        isWeekend: false
      });
    }
    
    return days;
  };

  const days = getCalendarDays();

  return (
    <div className="calendar-container">
      <div className="calendar-weekdays">
        {dayNames.map(day => (
          <div key={day} className="weekday">{day}</div>
        ))}
      </div>
      
      <div className="calendar-grid">
        {days.map((day, index) => {
          const dayEvents = day.isCurrentMonth ? getEventsForDay(day.number) : [];
          
          return (
            <div 
              key={index} 
              className={`calendar-day ${!day.isCurrentMonth ? 'empty' : ''} 
                         ${day.isToday ? 'today' : ''} 
                         ${day.isSelected ? 'selected' : ''} 
                         ${day.isWeekend ? 'weekend' : ''}`}
              onClick={() => day.isCurrentMonth && onDaySelect(day.number, currentMonth, currentYear)}
            >
              <span className="day-number">{day.number}</span>
              
              <div className="day-events">
                {dayEvents.map(event => (
                  <div 
                    key={event.id} 
                    className={`calendar-event event-${event.sport}`}
                    title={event.title}
                  >
                    <div className="event-sport">
                      <i className={sportIcons[event.sport]}></i>
                      <span>{event.time}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default CalendarGrid;
