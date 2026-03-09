import React from 'react';
import shared from '../../TournamentCreatePage.module.css';
import styles from './DatesLocationSection.module.css';

const DatesLocationSection = ({ formData, handleInputChange, calculateDuration }) => {
  return (
    <div className={shared.formBlock}>
      <h3 className={shared.blockTitle}>
        <i className="fas fa-calendar-alt"></i> Дати та місце проведення
      </h3>
      
      <div className={shared.formGrid}>
        {/* Термін реєстрації */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-user-clock"></i> Термін реєстрації *
          </label>
          <input
            type="date"
            name="registrationDeadline"
            value={formData.registrationDeadline}
            onChange={handleInputChange}
            className={shared.formInput}
            required
          />
        </div>

        {/* Дата початку */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-play-circle"></i> Дата початку *
          </label>
          <input
            type="date"
            name="startDate"
            value={formData.startDate}
            onChange={handleInputChange}
            className={shared.formInput}
            required
          />
        </div>

        {/* Дата завершення */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-flag-checkered"></i> Дата завершення
          </label>
          <input
            type="date"
            name="endDate"
            value={formData.endDate}
            onChange={handleInputChange}
            className={shared.formInput}
          />
          {formData.startDate && formData.endDate && (
            <div className={styles.durationBadge}>
              Тривалість: {calculateDuration()} днів
            </div>
          )}
        </div>

        {/* Місце проведення */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-map-marker-alt"></i> Місто/Локація
          </label>
          <input
            type="text"
            name="location"
            value={formData.location}
            onChange={handleInputChange}
            className={shared.formInput}
            placeholder="Наприклад: Київ"
          />
        </div>

        {/* Місце проведення (детально) */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-building"></i> Спортивний об'єкт
          </label>
          <input
            type="text"
            name="venue"
            value={formData.venue}
            onChange={handleInputChange}
            className={shared.formInput}
            placeholder="Наприклад: Зал №3, Спорткомплекс 'Університетський'"
          />
        </div>
      </div>
    </div>
  );
};

export default DatesLocationSection;
