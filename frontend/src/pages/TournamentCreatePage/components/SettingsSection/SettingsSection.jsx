import React from 'react';
import shared from '../../TournamentCreatePage.module.css';
import styles from './SettingsSection.module.css';

const SettingsSection = ({ formData, handleInputChange }) => {
  return (
    <>
      {/* Рівень змагання */}
      <div className={shared.formGroup}>
        <label className={shared.formLabel}>Рівень змагання *</label>
        <select
          name="level"
          value={formData.level || "CollegeGroup"}
          onChange={handleInputChange}
          className={shared.formSelect}
          required
        >
          <option value="CollegeGroup">Міжгрупові (коледж)</option>
          <option value="Regional">Регіональні</option>
          <option value="National">Всеукраїнські</option>
        </select>
      </div>

      {/* Деталі турніру */}
      <div className={shared.formBlock}>
        <h3 className={shared.blockTitle}>
          <i className="fas fa-cog"></i> Деталі та налаштування
        </h3>
        
        <div className={shared.formGrid}>
          {/* Правила */}
          <div className={shared.formGroup}>
            <label className={shared.formLabel}>
              <i className="fas fa-book"></i> Правила змагання
            </label>
            <textarea
              name="rules"
              value={formData.rules}
              onChange={handleInputChange}
              className={shared.formTextarea}
              placeholder="Основні правила та регламент..."
              rows="4"
            />
          </div>

          {/* Призи */}
          <div className={shared.formGroup}>
            <label className={shared.formLabel}>
              <i className="fas fa-gift"></i> Призи та нагороди
            </label>
            <textarea
              name="prizes"
              value={formData.prizes}
              onChange={handleInputChange}
              className={shared.formTextarea}
              placeholder="Опишіть призовий фонд..."
              rows="4"
            />
          </div>

          {/* Налаштування видимості */}
          <div className={shared.formGroup}>
            <label className={shared.formLabel}>
              <i className="fas fa-eye"></i> Видимість
            </label>
            <div className={styles.checkboxGroup}>
              <label className={styles.checkboxLabel}>
                <input
                  type="checkbox"
                  name="isPublic"
                  checked={formData.isPublic}
                  onChange={handleInputChange}
                  className={styles.checkboxInput}
                />
                <span className={styles.checkboxText}>Публічне змагання</span>
              </label>
              <label className={styles.checkboxLabel}>
                <input
                  type="checkbox"
                  name="requireApproval"
                  checked={formData.requireApproval}
                  onChange={handleInputChange}
                  className={styles.checkboxInput}
                />
                <span className={styles.checkboxText}>Модерація заявок</span>
              </label>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default SettingsSection;
