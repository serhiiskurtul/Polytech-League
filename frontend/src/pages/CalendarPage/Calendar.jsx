import React, { useState, useEffect } from 'react';
import CalendarGrid from './CalendarGrid';
import CalendarSidebar from './CalendarSidebar';
import CalendarStats from './CalendarStats';
import CalendarControls from './CalendarControls';
import CalendarFilters from './CalendarFilters';
import './Calendar.css';

const Calendar = () => {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [activeSportFilter, setActiveSportFilter] = useState('all');
  const [calendarView, setCalendarView] = useState('month');
  const [events, setEvents] = useState([]);

  // Ініціалізація даних подій
  useEffect(() => {
    const eventsData = [
      { id: 1, day: 15, month: 11, year: 2025, sport: 'football', title: 'Чемпіонат коледжу з футзалу', time: '14:00', location: 'Спортивний зал №1', description: 'Чвертьфінальний матч між командами ІТ-31 та ЕК-22' },
      { id: 2, day: 15, month: 11, year: 2025, sport: 'table-tennis', title: 'Турнір з настільного тенісу', time: '16:30', location: 'Спортивний зал №3', description: 'Груповий етап турніру' },
      { id: 3, day: 18, month: 11, year: 2025, sport: 'basketball', title: 'Кубок декана: баскетбол', time: '15:00', location: 'Спортивний зал №2', description: 'Півфінальні матчі' },
      { id: 4, day: 20, month: 11, year: 2025, sport: 'chess', title: 'Чемпіонат з шахів', time: '13:00', location: 'Бібліотека, зал №4', description: 'Фінальний тур' },
      { id: 5, day: 22, month: 11, year: 2025, sport: 'volleyball', title: 'Волейбольний турнір', time: '16:00', location: 'Спортивний зал №1', description: 'Фінальний матч' },
      { id: 6, day: 25, month: 11, year: 2025, sport: 'football', title: 'Футзал: півфінал', time: '14:30', location: 'Спортивний зал №1', description: 'МК-41 vs ПР-23' },
      { id: 7, day: 28, month: 11, year: 2025, sport: 'table-tennis', title: 'Фінал настільного тенісу', time: '17:00', location: 'Спортивний зал №3', description: 'Визначення чемпіона' },
      { id: 8, day: 5, month: 0, year: 2026, sport: 'basketball', title: 'Новорічний турнір', time: '12:00', location: 'Спортивний зал №2', description: 'Відкритий турнір з баскетболу' },
    ];
    setEvents(eventsData);
  }, []);

  // Назви місяців
  const monthNames = [
    'Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень',
    'Липень', 'Серпень', 'Вересень', 'Жовтень', 'Листопад', 'Грудень'
  ];

  // Назви видів спорту
  const sportNames = {
    'football': 'Футзал',
    'basketball': 'Баскетбол',
    'volleyball': 'Волейбол',
    'chess': 'Шахи',
    'table-tennis': 'Настільний теніс'
  };

  // Отримати події для вибраного дня
  const getEventsForSelectedDay = () => {
    return events.filter(event => 
      event.day === selectedDate.getDate() && 
      event.month === selectedDate.getMonth() && 
      event.year === selectedDate.getFullYear()
    );
  };

  // Отримати події для поточного місяця
  const getEventsForCurrentMonth = () => {
    return events.filter(event => 
      event.month === currentDate.getMonth() && 
      event.year === currentDate.getFullYear()
    );
  };

  // Обробка зміни місяця
  const handleMonthChange = (direction) => {
    setCurrentDate(prevDate => {
      const newDate = new Date(prevDate);
      if (direction === 'prev') {
        newDate.setMonth(newDate.getMonth() - 1);
      } else {
        newDate.setMonth(newDate.getMonth() + 1);
      }
      return newDate;
    });
  };

  // Обробка вибору дня
  const handleDaySelect = (day, month, year) => {
    setSelectedDate(new Date(year, month, day));
  };

  // Обробка зміни фільтра
  const handleFilterChange = (sport) => {
    setActiveSportFilter(sport);
  };

  // Обробка зміни виду календаря
  const handleViewChange = (view) => {
    setCalendarView(view);
  };

  // Отримати статистику для поточного місяця
  const getMonthStats = () => {
    const monthEvents = getEventsForCurrentMonth();
    const footballEvents = monthEvents.filter(event => event.sport === 'football').length;
    const basketballEvents = monthEvents.filter(event => event.sport === 'basketball').length;
    const volleyballEvents = monthEvents.filter(event => event.sport === 'volleyball').length;
    
    // Рахуємо дні з подіями
    const daysWithEvents = [...new Set(monthEvents.map(event => event.day))].length;

    return {
      totalEvents: monthEvents.length,
      footballEvents,
      basketballEvents,
      volleyballEvents,
      daysWithEvents
    };
  };

  return (
    <div className="calendar-page">
      {/* Заголовок сторінки */}
      <div className="page-header">
        <div>
          <h1 className="page-title">Календар подій</h1>
          <p className="page-subtitle">Слідкуйте за розкладом турнірів та матчів</p>
        </div>
        
        <CalendarControls 
          currentDate={currentDate}
          monthNames={monthNames}
          onMonthChange={handleMonthChange}
          calendarView={calendarView}
          onViewChange={handleViewChange}
        />
      </div>

      {/* Фільтри */}
      <CalendarFilters 
        activeFilter={activeSportFilter}
        onFilterChange={handleFilterChange}
      />

      <div className="calendar-sidebar">
        {/* Основний календар */}
        <CalendarGrid 
          currentDate={currentDate}
          selectedDate={selectedDate}
          events={events}
          activeSportFilter={activeSportFilter}
          onDaySelect={handleDaySelect}
        />

        {/* Деталі вибраного дня */}
        <CalendarSidebar 
          selectedDate={selectedDate}
          monthNames={monthNames}
          events={getEventsForSelectedDay()}
          sportNames={sportNames}
        />
      </div>

      {/* Статистика */}
      <CalendarStats stats={getMonthStats()} />
    </div>
  );
};

export default Calendar;