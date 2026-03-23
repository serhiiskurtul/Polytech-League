import React from 'react';
import shared from '../../TournamentCreatePage.module.css';
import styles from './BasicInfoSection.module.css';
import { AVAILABLE_SPORTS, TOURNAMENT_TYPES, CHESS_SYSTEMS, CHECKERS_SYSTEMS, TABLE_TENNIS_TOURNAMENT_TYPES } from '../../constants';

const BasicInfoSection = ({ formData, handleInputChange }) => {
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

        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-chess-board"></i> Формат змагання *
          </label>
          <select
            name={['chess', 'checkers'].includes(formData.sport) ? 'system' : formData.sport === 'table_tennis' ? 'tournamentType' : 'type'}
            value={['chess', 'checkers'].includes(formData.sport) ? formData.system : formData.sport === 'table_tennis' ? formData.tournamentType : formData.type}
            onChange={handleInputChange}
            className={shared.formSelect}
            required
          >
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
                  : TOURNAMENT_TYPES.map(type => (
                      <option key={type.id} value={type.id}>{type.name} - {type.description}</option>
                    ))}
          </select>
        </div>

        {/* Кількість команд/учасників */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-users"></i> Кількість команд/учасників
          </label>
          <div className={styles.rangeGroup}>
            <div className={styles.rangeInput}>
              <span className={styles.rangeLabel}>Мін:</span>
              <input
                type="number"
                name="minTeams"
                value={formData.minTeams}
                onChange={handleInputChange}
                className={shared.numberInput}
                min="2"
                max="64"
              />
            </div>
            <div className={styles.rangeSeparator}>-</div>
            <div className={styles.rangeInput}>
              <span className={styles.rangeLabel}>Макс:</span>
              <input
                type="number"
                name={['chess', 'checkers'].includes(formData.sport) ? 'participantsCount' : 'maxTeams'}
                value={['chess', 'checkers'].includes(formData.sport) ? formData.participantsCount : formData.maxTeams}
                onChange={handleInputChange}
                className={shared.numberInput}
                min={formData.minTeams}
                max={formData.sport === 'table_tennis' ? "256" : "128"}
              />
            </div>
          </div>
          <div className={styles.rangeHint}>
            Оптимально: 8, 16, 32 для олімпійської системи
          </div>
        </div>

        {/* Шахові / шашкові специфічні поля */}
        {['chess', 'checkers'].includes(formData.sport) && (
          <>
            <div className={shared.formGroup}>
              <label className={shared.formLabel}>
                <i className="fas fa-clock"></i> Контроль часу *
              </label>
              <input
                type="text"
                name="timeControl"
                value={formData.timeControl}
                onChange={handleInputChange}
                className={shared.formInput}
                placeholder="Наприклад: 10+5 або Classical"
                required
              />
            </div>
          </>
        )}

        {/* Опис */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-align-left"></i> Опис змагання *
          </label>
          <textarea
            name="description"
            value={formData.description}
            onChange={handleInputChange}
            className={shared.formTextarea}
            placeholder="Детальний опис змагання, його цілей та особливостей..."
            rows="4"
            required
          />
        </div>
      </div>
    </div>
  );
};

export default BasicInfoSection;
