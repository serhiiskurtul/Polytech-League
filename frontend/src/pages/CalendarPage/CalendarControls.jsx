import React from 'react';

const CalendarControls = ({ 
  currentDate, 
  monthNames, 
  onMonthChange, 
  calendarView, 
  onViewChange 
}) => {
  const currentMonth = currentDate.getMonth();
  const currentYear = currentDate.getFullYear();

  return (
    <div className="calendar-controls">
      <div className="month-navigation">
        <button 
          className="month-nav-btn" 
          onClick={() => onMonthChange('prev')}
          aria-label="Попередній місяць"
        >
          <i className="fas fa-chevron-left"></i>
        </button>
        <div className="current-month">
          {monthNames[currentMonth]} {currentYear}
        </div>
        <button 
          className="month-nav-btn" 
          onClick={() => onMonthChange('next')}
          aria-label="Наступний місяць"
        >
          <i className="fas fa-chevron-right"></i>
        </button>
      </div>
      
      <div className="view-toggle">
        <button 
          className={`view-btn ${calendarView === 'month' ? 'active' : ''}`}
          onClick={() => onViewChange('month')}
          data-view="month"
        >
          <i className="fas fa-calendar-alt"></i> Місяць
        </button>
        <button 
          className={`view-btn ${calendarView === 'week' ? 'active' : ''}`}
          onClick={() => onViewChange('week')}
          data-view="week"
        >
          <i className="fas fa-calendar-week"></i> Тиждень
        </button>
        <button 
          className={`view-btn ${calendarView === 'list' ? 'active' : ''}`}
          onClick={() => onViewChange('list')}
          data-view="list"
        >
          <i className="fas fa-list"></i> Список
        </button>
      </div>
    </div>
  );
};

export default CalendarControls;