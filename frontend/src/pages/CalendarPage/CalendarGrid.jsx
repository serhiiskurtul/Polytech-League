import React from 'react';

const CalendarGrid = ({ 
  currentDate, 
  selectedDate, 
  events, 
  activeSportFilter, 
  onDaySelect 
}) => {
  const currentMonth = currentDate.getMonth();
  const currentYear = currentDate.getFullYear();
  
  const dayNames = ['Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П\'ятниця', 'Субота', 'Неділя'];
  
  // Іконки для видів спорту
  const sportIcons = {
    'football': 'fas fa-futbol',
    'basketball': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'chess': 'fas fa-chess',
    'table-tennis': 'fas fa-table-tennis'
  };

  // Кольори для видів спорту
  const sportColors = {
    'football': 'event-football',
    'basketball': 'event-basketball',
    'volleyball': 'event-volleyball',
    'chess': 'event-chess',
    'table-tennis': 'event-table-tennis'
  };

  // Отримати події для дня
  const getEventsForDay = (dayNumber) => {
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
    
    const days = [];
    
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

  const calendarDays = getCalendarDays();

  return (
    <div className="calendar-container">
      <div className="calendar-weekdays">
        {dayNames.map(dayName => (
          <div key={dayName} className="weekday">{dayName}</div>
        ))}
      </div>
      
      <div className="calendar-grid">
        {calendarDays.map((day, index) => (
          <div
            key={index}
            className={`calendar-day ${!day.isCurrentMonth ? 'empty' : ''} ${day.isToday ? 'today' : ''} ${day.isSelected ? 'selected' : ''} ${day.isWeekend ? 'weekend' : ''}`}
            onClick={() => {
              if (day.isCurrentMonth) {
                onDaySelect(day.number, currentMonth, currentYear);
              }
            }}
          >
            {day.isCurrentMonth && (
              <>
                <div className="day-number">{day.number}</div>
                <div className="day-events">
                  {getEventsForDay(day.number)
                    .slice(0, 3)
                    .map(event => (
                      <div
                        key={event.id}
                        className={`calendar-event ${sportColors[event.sport]}`}
                        onClick={(e) => {
                          e.stopPropagation();
                          alert(`Деталі події:\n\n${event.title}\n\nЧас: ${event.time}\nМісце: ${event.location}\n\n${event.description}`);
                        }}
                      >
                        <div className="event-sport">
                          <i className={sportIcons[event.sport]}></i>
                          {event.title.substring(0, 15)}{event.title.length > 15 ? '...' : ''}
                        </div>
                        <div className="event-time">{event.time}</div>
                      </div>
                    ))}
                  
                  {getEventsForDay(day.number).length > 3 && (
                    <div className="calendar-event more-events">
                      +{getEventsForDay(day.number).length - 3} ще
                    </div>
                  )}
                </div>
              </>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default CalendarGrid;