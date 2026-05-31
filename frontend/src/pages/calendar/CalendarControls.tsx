interface CalendarControlsProps {
  currentDate: Date;
  monthNames: string[];
  onMonthChange: (direction: 'prev' | 'next') => void;
}

const CalendarControls = ({ 
  currentDate, 
  monthNames, 
  onMonthChange
}: CalendarControlsProps) => {
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
    </div>
  );
};

export default CalendarControls;
