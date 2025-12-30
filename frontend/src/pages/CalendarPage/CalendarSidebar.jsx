import React, { useState } from 'react';

const CalendarSidebar = ({ selectedDate, monthNames, events, sportNames }) => {
  const [modalEvent, setModalEvent] = useState(null);

  const dayNames = ['Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П\'ятниця', 'Субота', 'Неділя'];
  const selectedDayName = dayNames[selectedDate.getDay() === 0 ? 6 : selectedDate.getDay() - 1];
  
  // Іконки для видів спорту
  const sportIcons = {
    'football': 'fas fa-futbol',
    'basketball': 'fas fa-basketball-ball',
    'volleyball': 'fas fa-volleyball-ball',
    'chess': 'fas fa-chess',
    'table-tennis': 'fas fa-table-tennis'
  };

  // Кольори для видів спорту
  const getSportColor = (sport) => {
    const colors = {
      'football': '#16a34a',
      'basketball': '#f97316',
      'volleyball': '#3b82f6',
      'chess': '#8b5cf6',
      'table-tennis': '#ec4899'
    };
    return colors[sport] || '#64748b';
  };

  const getSportBackground = (sport) => {
    const backgrounds = {
      'football': 'rgba(34, 197, 94, 0.15)',
      'basketball': 'rgba(249, 115, 22, 0.15)',
      'volleyball': 'rgba(59, 130, 246, 0.15)',
      'chess': 'rgba(139, 92, 246, 0.15)',
      'table-tennis': 'rgba(236, 72, 153, 0.15)'
    };
    return backgrounds[sport] || 'rgba(100, 116, 139, 0.15)';
  };

  const handleViewEventDetails = (event) => {
    setModalEvent(event);
  };

  const handleCloseModal = () => {
    setModalEvent(null);
  };

  const handleRegisterForEvent = (eventId) => {
    alert('Реєстрація на подію успішна!');
    handleCloseModal();
  };

  const handleAddToCalendar = (eventId) => {
    alert('Подія додана до вашого особистого календаря!');
  };

  return (
    <>
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
              <div key={event.id} className="event-card">
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
                  <div className="event-detail">
                    <i className={sportIcons[event.sport]}></i>
                    <span>{sportNames[event.sport]}</span>
                  </div>
                  <div className="event-detail">
                    <i className="fas fa-calendar-day"></i>
                    <span>{selectedDate.getDate()} {monthNames[selectedDate.getMonth()]}</span>
                  </div>
                </div>
                
                <div className="event-actions">
                  <button 
                    className="btn btn-primary"
                    onClick={() => handleViewEventDetails(event)}
                  >
                    <i className="fas fa-eye"></i> Деталі
                  </button>
                  <button 
                    className="btn btn-secondary"
                    onClick={() => handleAddToCalendar(event.id)}
                  >
                    <i className="far fa-calendar-plus"></i> Додати
                  </button>
                </div>
              </div>
            ))
          )}
        </div>
        
        <div className="add-event-container">
          <button className="btn btn-secondary add-event-btn">
            <i className="fas fa-plus-circle"></i> Додати нову подію
          </button>
        </div>
      </div>

      {/* Модальне вікно для деталей події */}
      {modalEvent && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>{modalEvent.title}</h2>
              <button className="modal-close" onClick={handleCloseModal}>
                &times;
              </button>
            </div>
            
            <div className="modal-body">
              <div className="modal-sport-info">
                <div 
                  className="modal-sport-badge"
                  style={{ 
                    background: getSportBackground(modalEvent.sport), 
                    color: getSportColor(modalEvent.sport) 
                  }}
                >
                  {sportNames[modalEvent.sport]}
                </div>
                <div className="modal-date">
                  <i className="far fa-calendar"></i> 
                  {modalEvent.day} {monthNames[modalEvent.month]} {modalEvent.year}
                </div>
              </div>
              
              <div className="modal-description">
                <h3>Деталі події:</h3>
                <p>{modalEvent.description}</p>
              </div>
              
              <div className="modal-details-grid">
                <div className="modal-detail">
                  <i className="fas fa-clock" style={{ color: '#3b82f6' }}></i>
                  <div>
                    <div className="detail-label">Час</div>
                    <div className="detail-value">{modalEvent.time}</div>
                  </div>
                </div>
                
                <div className="modal-detail">
                  <i className="fas fa-map-marker-alt" style={{ color: '#3b82f6' }}></i>
                  <div>
                    <div className="detail-label">Місце</div>
                    <div className="detail-value">{modalEvent.location}</div>
                  </div>
                </div>
              </div>
            </div>
            
            <div className="modal-footer">
              <button 
                className="btn btn-primary modal-register-btn"
                onClick={() => handleRegisterForEvent(modalEvent.id)}
              >
                <i className="fas fa-user-plus"></i> Зареєструватись
              </button>
              <button 
                className="btn btn-secondary"
                onClick={() => handleAddToCalendar(modalEvent.id)}
              >
                <i className="far fa-calendar-plus"></i> Додати в календар
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default CalendarSidebar;