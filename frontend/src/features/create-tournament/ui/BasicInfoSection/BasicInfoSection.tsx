import shared from '../form.module.css';
import styles from './BasicInfoSection.module.css';
import { 
  AVAILABLE_SPORTS, 
  TOURNAMENT_TYPES, 
  CHESS_SYSTEMS, 
  CHECKERS_SYSTEMS, 
  TABLE_TENNIS_TOURNAMENT_TYPES, 
  BASKETBALL3X3_TOURNAMENT_TYPES, 
  BADMINTON_TOURNAMENT_TYPES 
} from '../../model/constants';
import { TournamentFormData } from '../../model/useTournamentForm';
import { ChangeEvent } from 'react';

interface BasicInfoSectionProps {
  formData: TournamentFormData;
  handleInputChange: (e: ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => void;
}

const BasicInfoSection = ({ formData, handleInputChange }: BasicInfoSectionProps) => {
  return (
    <div className={shared.formBlock}>
      <h3 className={shared.blockTitle}>
        <i className="fas fa-info-circle"></i> Основна інформація
      </h3>
      
      <div className={shared.formGrid}>
        {/* Назва турніру */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-trophy"></i> Назва змагання *
          </label>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            className={shared.formInput}
            placeholder="Наприклад: Чемпіонат коледжу з шахів 2024"
            required
          />
        </div>

        {/* Вид спорту */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-running"></i> Вид спорту *
          </label>
          <div className={styles.sportsGrid}>
            {AVAILABLE_SPORTS.map(sport => (
              <label 
                key={sport.id} 
                className={`${styles.sportOption} ${
                  formData.sport === sport.id ? styles.selected : ''
                }`}
              >
                <input
                  type="radio"
                  name="sport"
                  value={sport.id}
                  checked={formData.sport === sport.id}
                  onChange={handleInputChange}
                  className={styles.radioInput}
                />
                <div className={styles.sportIcon}>
                  <i className={sport.icon}></i>
                </div>
                <span className={styles.sportName}>{sport.name}</span>
              </label>
            ))}
          </div>
        </div>

        {/* Тип участі (тільки для гібридних видів) */}
        {['chess', 'checkers', 'badminton'].includes(formData.sport) && (
          <div className={shared.formGroup}>
            <label className={shared.formLabel}>
              <i className="fas fa-users-cog"></i> Формат участі *
            </label>
            <div className={styles.sportsGrid}>
              <label className={`${styles.sportOption} ${!formData.isTeam ? styles.selected : ''}`}>
                <input
                  type="radio"
                  name="isTeam"
                  value="false"
                  checked={!formData.isTeam}
                  onChange={() => handleInputChange({ target: { name: 'isTeam', value: false } } as any)}
                  className={styles.radioInput}
                />
                <div className={styles.sportIcon}>
                  <i className="fas fa-user"></i>
                </div>
                <span className={styles.sportName}>Індивідуальний</span>
              </label>
              <label className={`${styles.sportOption} ${formData.isTeam ? styles.selected : ''}`}>
                <input
                  type="radio"
                  name="isTeam"
                  value="true"
                  checked={formData.isTeam}
                  onChange={() => handleInputChange({ target: { name: 'isTeam', value: true } } as any)}
                  className={styles.radioInput}
                />
                <div className={styles.sportIcon}>
                  <i className="fas fa-users"></i>
                </div>
                <span className={styles.sportName}>Командний</span>
              </label>
            </div>
          </div>
        )}

        {/* Формат змагань (не для армреслінгу) */}
        {formData.sport !== 'armwrestling' && (
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-chess-board"></i> Система проведення
          </label>

          <select
            name={['chess', 'checkers'].includes(formData.sport) ? 'system' : formData.sport === 'table_tennis' ? 'tournamentType' : 'type'}
            value={['chess', 'checkers'].includes(formData.sport) ? formData.system : formData.sport === 'table_tennis' ? formData.tournamentType : formData.type}
            onChange={handleInputChange}
            className={shared.formSelect}
          >
            <option value="">Не вказано</option>

            {formData.sport === 'chess' 
              ? CHESS_SYSTEMS.map(type => (
                  <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                ))
              : formData.sport === 'checkers'
                ? CHECKERS_SYSTEMS.map(type => (
                    <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                  ))
                : formData.sport === 'table_tennis'
                  ? TABLE_TENNIS_TOURNAMENT_TYPES.map(type => (
                      <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                    ))
                    : formData.sport === 'basketball-3x3'
                      ? BASKETBALL3X3_TOURNAMENT_TYPES.map(type => (
                          <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                        ))
                      : formData.sport === 'badminton'
                        ? BADMINTON_TOURNAMENT_TYPES.map(type => (
                            <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                          ))
                        : TOURNAMENT_TYPES.map(type => (
                            <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                          ))}
          </select>
        </div>
        )}

        {/* Опис */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-align-left"></i> Опис змагання
          </label>

          <textarea
            name="description"
            value={formData.description}
            onChange={handleInputChange}
            className={shared.formTextarea}
            placeholder="Розкажіть про формат, учасників та важливі деталі..."
            rows={4}
          />

        </div>
      </div>
    </div>
  );
};

export default BasicInfoSection;
