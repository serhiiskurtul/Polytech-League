import shared from '../form.module.css';
import styles from './DatesLocationSection.module.css';
import { TournamentFormData } from '../../model/useTournamentForm';
import { ChangeEvent } from 'react';

interface DatesLocationSectionProps {
  formData: TournamentFormData;
  handleInputChange: (e: ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => void;
  calculateDuration: () => number;
}

const DatesLocationSection = ({ formData, handleInputChange, calculateDuration }: DatesLocationSectionProps) => {
  return (
    <div className={shared.formBlock}>
      <h3 className={shared.blockTitle}>
        <i className="fas fa-calendar-alt"></i> Дати та місце проведення
      </h3>
      
      <div className={shared.formGrid}>
        {/* Початок реєстрації */}
        {formData.level === 'CollegeGroup' && (
          <>
            <div className={shared.formGroup}>
              <label className={shared.formLabel}>
                <i className="fas fa-sign-in-alt"></i> Початок реєстрації
              </label>
              <div className={styles.dateTimeRow}>
                <input
                  type="date"
                  name="registrationStartDate"
                  value={formData.registrationStartDate}
                  onChange={handleInputChange}
                  className={shared.formInput}
                />
                <input
                  type="time"
                  name="registrationStartTime"
                  value={formData.registrationStartTime}
                  onChange={handleInputChange}
                  className={shared.formInput}
                  style={{ width: '120px' }}
                />
              </div>
            </div>

            {/* Кінець реєстрації */}
            <div className={shared.formGroup}>
              <label className={shared.formLabel}>
                <i className="fas fa-user-clock"></i> Кінець реєстрації *
              </label>
              <div className={styles.dateTimeRow}>
                <input
                  type="date"
                  name="registrationDeadline"
                  value={formData.registrationDeadline}
                  onChange={handleInputChange}
                  className={shared.formInput}
                  required
                />
                <input
                  type="time"
                  name="registrationDeadlineTime"
                  value={formData.registrationDeadlineTime}
                  onChange={handleInputChange}
                  className={shared.formInput}
                  required
                  style={{ width: '120px' }}
                />
              </div>
            </div>
          </>
        )}

        {/* Дата та час початку */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-play-circle"></i> Дата та час початку *
          </label>
          <div className={styles.dateTimeRow}>
            <input
              type="date"
              name="startDate"
              value={formData.startDate}
              onChange={handleInputChange}
              className={shared.formInput}
              required
            />
            <input
              type="time"
              name="startTime"
              value={formData.startTime}
              onChange={handleInputChange}
              className={shared.formInput}
              required
              style={{ width: '120px' }}
            />
          </div>
        </div>

        {/* Дата завершення */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-flag-checkered"></i> Дата завершення
          </label>
          <div className={styles.dateTimeRow}>
            <input
              type="date"
              name="endDate"
              value={formData.endDate}
              onChange={handleInputChange}
              className={shared.formInput}
            />
            <input
              type="time"
              name="endTime"
              value={formData.endTime}
              onChange={handleInputChange}
              className={shared.formInput}
              style={{ width: '120px' }}
            />
          </div>
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
