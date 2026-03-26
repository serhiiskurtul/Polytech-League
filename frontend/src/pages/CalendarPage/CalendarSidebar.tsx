import { useState } from 'react';
import { CalendarEventData } from './Calendar';

interface CalendarSidebarProps {
  selectedDate: Date;
  monthNames: string[];
  events: CalendarEventData[];
  sportNames: Record<string, string>;
}

const CalendarSidebar = ({ selectedDate, monthNames, events, sportNames }: CalendarSidebarProps) => {
  const [modalEvent, setModalEvent] = useState<CalendarEventData | null>(null);

  const dayNames = ['Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П\'ятниця', 'Субота', 'Неділя'];
  const dayIndex = selectedDate.getDay();
  const selectedDayName = dayNames[dayIndex === 0 ? 6 : dayIndex - 1];
  
  // Іконки для видів спорту
  const sportIcons: Record<string, string> = {
    'football': 'fas fa-futbol',
    'basketball': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'chess': 'fas fa-chess',
    'table-tennis': 'fas fa-table-tennis'
  };

  // Кольори для видів спорту
  const getSportColor = (sport: string) => {
    const colors: Record<string, string> = {
      'football': '#16a34a',
      'basketball': '#f97316',
      'volleyball': '#3b82f6',
      'chess': '#8b5cf6',
      'table-tennis': '#ec4899'
    };
    return colors[sport] || '#64748b';
  };

  const getSportBackground = (sport: string) => {
    const backgrounds: Record<string, string> = {
      'football': 'rgba(34, 197, 94, 0.15)',
      'basketball': 'rgba(249, 115, 22, 0.15)',
      'volleyball': 'rgba(59, 130, 246, 0.15)',
      'chess': 'rgba(139, 92, 246, 0.15)',
      'table-tennis': 'rgba(236, 72, 153, 0.15)'
    };
    return backgrounds[sport] || 'rgba(100, 116, 139, 0.15)';
  };

  const handleViewEventDetails = (event: CalendarEventData) => {
    setModalEvent(event);
  };

  const handleCloseModal = () => {
    setModalEvent(null);
  };

  const handleRegisterForEvent = (eventId: number) => {
    alert('Реєстрація на подію успішна!');
    handleCloseModal();
  };

  const handleAddToCalendar = (eventId: number) => {
    alert('Подія додана до вашого особистого календаря!');
  };

  return (
    <div className="calendar-sidebar">
      <div className="day-details">
        <div className="selected-day">
          <i className="far fa-calendar-alt"></i>
          <span>
            {selectedDate.getDate()} {monthNames[selectedDate.getMonth()]} {selectedDate.getFullYear()}, {selectedDayName}
          </span>
        </div>
        
        <div className="events-list">
          {events.length === 0 ? (
            <div className="no-events">
              <i className="far fa-calendar-times"></i>
              <h3>Немає подій</h3>
              <p>На цей день не заплановано жодних подій</p>
            </div>
          ) : (
            events.map(event => (
              <div key={event.id} className="event-card" onClick={() => handleViewEventDetails(event)}>
                <div className="event-header">
                  <div>
                    <div className="event-title">{event.title}</div>
                    <div 
                      className="event-sport-badge"
                      style={{ 
                        background: getSportBackground(event.sport), 
                        color: getSportColor(event.sport) 
                      }}
                    >
                      {sportNames[event.sport]}
                    </div>
                  </div>
                  <div className="event-time-display">{event.time}</div>
                </div>
                
                <p className="event-description">{event.description}</p>
                
                <div className="event-details">
                  <div className="event-detail">
                    <i className="fas fa-map-marker-alt"></i>
                    <span>{event.location}</span>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {modalEvent && (
        <div className="event-modal-overlay" onClick={handleCloseModal}>
          <div className="event-modal" onClick={e => e.stopPropagation()}>
            <button className="close-modal" onClick={handleCloseModal}>&times;</button>
            
            <div className="modal-header">
              <div className="modal-sport-icon" style={{ color: getSportColor(modalEvent.sport) }}>
                <i className={sportIcons[modalEvent.sport]}></i>
              </div>
              <h2 className="modal-title">{modalEvent.title}</h2>
            </div>
            
            <div className="modal-body">
              <div className="modal-info-grid">
                <div className="info-item">
                  <i className="far fa-calendar-alt"></i>
                  <div>
                    <div className="info-label">Дата</div>
                    <div className="info-value">{modalEvent.day} {monthNames[modalEvent.month]} {modalEvent.year}</div>
                  </div>
                </div>
                <div className="info-item">
                  <i className="far fa-clock"></i>
                  <div>
                    <div className="info-label">Час</div>
                    <div className="info-value">{modalEvent.time}</div>
                  </div>
                </div>
                <div className="info-item">
                  <i className="fas fa-map-marker-alt"></i>
                  <div>
                    <div className="info-label">Локація</div>
                    <div className="info-value">{modalEvent.location}</div>
                  </div>
                </div>
                <div className="info-item">
                  <i className="fas fa-tag"></i>
                  <div>
                    <div className="info-label">Вид спорту</div>
                    <div className="info-value">{sportNames[modalEvent.sport]}</div>
                  </div>
                </div>
              </div>
              
              <div className="modal-description">
                <h3>Про подію</h3>
                <p>{modalEvent.description}</p>
              </div>
            </div>
            
            <div className="modal-footer">
              <button className="btn-add-calendar" onClick={() => handleAddToCalendar(modalEvent.id)}>
                <i className="far fa-calendar-plus"></i> В календар
              </button>
              <button className="btn-register-event" onClick={() => handleRegisterForEvent(modalEvent.id)}>
                Зареєструватись
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CalendarSidebar;
