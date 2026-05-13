import { useState, useEffect } from 'react';
import CalendarGrid from './CalendarGrid';
import CalendarSidebar from './CalendarSidebar';
import CalendarStats from './CalendarStats';
import CalendarControls from './CalendarControls';
import CalendarFilters from './CalendarFilters';
import { getTournaments } from '@/shared/api/tournaments.api';
import './Calendar.css';

export interface CalendarEventData {
  id: number;
  day: number;
  month: number;
  year: number;
  sport: string;
  title: string;
  time: string;
  location: string;
  description: string;
}

const Calendar = () => {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [activeSportFilter, setActiveSportFilter] = useState('all');
  const [events, setEvents] = useState<CalendarEventData[]>([]);

  // Ініціалізація даних подій
  useEffect(() => {
    // Fetch all tournaments (using a large page size for simplicity in the calendar)
    getTournaments(1, 100)
      .then(response => {
        const mappedEvents: CalendarEventData[] = response.items.map(tournament => {
          const date = new Date(tournament.rawDate || tournament.date);
          return {
            id: Number(tournament.id),
            day: date.getDate(),
            month: date.getMonth(),
            year: date.getFullYear(),
            sport: tournament.sport,
            title: tournament.title,
            time: tournament.time || '00:00',
            location: tournament.location || 'Уточнюється',
            description: tournament.description || ''
          };
        });
        setEvents(mappedEvents);
      })
      .catch(err => console.error('Помилка завантаження турнірів для календаря:', err));
  }, []);

  // Назви місяців
  const monthNames = [
    'Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень',
    'Липень', 'Серпень', 'Вересень', 'Жовтень', 'Листопад', 'Грудень'
  ];

  // Назви видів спорту
  const sportNames: Record<string, string> = {
    'football': 'Футзал',
    'basketball-3x3': 'Баскетбол 3х3',
    'volleyball': 'Волейбол',
    'beach-volleyball': 'Пляжний волейбол',
    'chess': 'Шахи',
    'checkers': 'Шашки',
    'table-tennis': 'Настільний теніс',
    'badminton': 'Бадмінтон',
    'armwrestling': 'Армреслінг'
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
  const handleMonthChange = (direction: 'prev' | 'next') => {
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
  const handleDaySelect = (day: number, month: number, year: number) => {
    setSelectedDate(new Date(year, month, day));
  };

  // Обробка зміни фільтра
  const handleFilterChange = (sport: string) => {
    setActiveSportFilter(sport);
  };

  // Отримати статистику для поточного місяця
  const getMonthStats = () => {
    const monthEvents = getEventsForCurrentMonth();
    
    // Рахуємо дні з подіями
    const daysWithEvents = [...new Set(monthEvents.map(event => event.day))].length;

    return {
      totalEvents: monthEvents.length,
      daysWithEvents
    };
  };

  return (
    <div className="calendar-page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Календар спортивних подій</h1>
          <p className="page-subtitle">Слідкуйте за розкладом матчів та змагань коледжу</p>
        </div>
      </div>

      <CalendarStats stats={getMonthStats()} />

      <div className="calendar-layout">
        <div className="calendar-main-content">
          <CalendarFilters 
            activeFilter={activeSportFilter}
            onFilterChange={handleFilterChange}
          />

          <CalendarControls 
            currentDate={currentDate}
            monthNames={monthNames}
            onMonthChange={handleMonthChange}
          />
          
          <CalendarGrid 
            currentDate={currentDate}
            selectedDate={selectedDate}
            events={events}
            activeSportFilter={activeSportFilter}
            onDaySelect={handleDaySelect}
          />
        </div>

        <CalendarSidebar 
          selectedDate={selectedDate}
          monthNames={monthNames}
          events={getEventsForSelectedDay()}
          sportNames={sportNames}
        />
      </div>
    </div>
  );
};

export default Calendar;
