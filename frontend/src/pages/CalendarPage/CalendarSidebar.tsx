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

  const getSportClass = (sport: string) => {
    const classes: Record<string, string> = {
      'football': 'event-football',
      'basketball': 'event-basketball',
      'volleyball': 'event-volleyball',
      'chess': 'event-chess',
      'table-tennis': 'event-table-tennis'
    };
    return classes[sport] || '';
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
            <div key={event.id} className="event-card" onClick={() => handleViewEventDetails(event)}>
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

              <div className="event-card-actions">
                <button className="btn btn-primary" onClick={(e) => {
                  e.stopPropagation();
                  handleRegisterForEvent(event.id);
                }}>
                  Реєстрація
                </button>
                <button className="btn btn-secondary" onClick={(e) => {
                  e.stopPropagation();
                  handleViewEventDetails(event);
                }}>
                  Деталі
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      {modalEvent && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={e => e.stopPropagation()}>
            <div className="modal-banner" style={{ background: getSportColor(modalEvent.sport) }}>
              <div className="modal-header-info">
                <div className="modal-header-sport">
                  <i className={sportIcons[modalEvent.sport]}></i> {sportNames[modalEvent.sport]}
                </div>
                <h2>{modalEvent.title}</h2>
              </div>
              <button className="modal-close" onClick={handleCloseModal} style={{ color: 'white' }}>&times;</button>
            </div>
            
            <div className="modal-body">
              <div className="modal-info-section">
                <div className="modal-info-item">
                  <i className="far fa-calendar-alt"></i>
                  <div className="info-text">
                    <div className="label">Дата</div>
                    <div className="info-value">{modalEvent.day} {monthNames[modalEvent.month]} {modalEvent.year}</div>
                  </div>
                </div>
                <div className="modal-info-item">
                  <i className="far fa-clock"></i>
                  <div className="info-text">
                    <div className="label">Час</div>
                    <div className="info-value">{modalEvent.time}</div>
                  </div>
                </div>
                <div className="modal-info-item">
                  <i className="fas fa-map-marker-alt"></i>
                  <div className="info-text">
                    <div className="label">Локація</div>
                    <div className="info-value">{modalEvent.location}</div>
                  </div>
                </div>
                <div className="modal-info-item">
                  <i className="fas fa-trophy"></i>
                  <div className="info-text">
                    <div className="label">Дисципліна</div>
                    <div className="info-value">{sportNames[modalEvent.sport]}</div>
                  </div>
                </div>
              </div>
              
              <div className="modal-description-box">
                <h3>Про подію</h3>
                <p>{modalEvent.description}</p>
              </div>
            </div>
            
            <div className="modal-footer">
              <button className="btn btn-primary" style={{ width: '100%' }} onClick={() => handleRegisterForEvent(modalEvent.id)}>
                Зареєструватись на подію
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CalendarSidebar;
