import React from 'react';
import shared from '../../TournamentCreatePage.module.css';
import styles from './PreviewSection.module.css';
import { AVAILABLE_SPORTS, TOURNAMENT_TYPES, CHESS_SYSTEMS, CHECKERS_SYSTEMS, TABLE_TENNIS_TOURNAMENT_TYPES, ARMWRESTLING_HANDS, ARMWRESTLING_WEIGHT_CLASSES, ARMWRESTLING_CATEGORIES } from '../../constants';

const PreviewSection = ({ formData, selectedCategories }) => {
  const getFormatName = () => {
    if (['chess', 'checkers'].includes(formData.sport)) {
      const systems = formData.sport === 'chess' ? CHESS_SYSTEMS : CHECKERS_SYSTEMS;
      const system = systems.find(t => t.id === formData.system);
      return system ? system.name : formData.system;
    } else if (formData.sport === 'table_tennis') {
      const type = TABLE_TENNIS_TOURNAMENT_TYPES.find(t => t.id === formData.tournamentType);
      return type ? type.name : formData.tournamentType;
    } else if (formData.sport === 'armwrestling') {
      const hand = ARMWRESTLING_HANDS.find(h => h.id === formData.armHand);
      const wc = ARMWRESTLING_WEIGHT_CLASSES.find(w => w.id === formData.armWeightClass);
      return `${hand?.name || ''} · ${wc?.name || ''}`;
    } else {
      const type = TOURNAMENT_TYPES.find(t => t.id === formData.type);
      return type ? type.name : formData.type;
    }
  };

  return (
    <div className={styles.previewCard}>
      <h3 className={styles.previewTitle}>
        <i className="fas fa-eye"></i> Попередній перегляд
      </h3>
      
      <div className={styles.previewContent}>
        {formData.name ? (
          <>
            {/* Заголовок турніру */}
            <div className={styles.previewHeader}>
              <div className={styles.previewSport}>
                <i className={
                  AVAILABLE_SPORTS.find(s => s.id === formData.sport)?.icon || 'fas fa-trophy'
                }></i>
                <span>{AVAILABLE_SPORTS.find(s => s.id === formData.sport)?.name || 'Спорт'}</span>
              </div>
              <h4 className={styles.previewName}>{formData.name}</h4>
            </div>

            {/* Дати */}
            <div className={styles.previewDates}>
              <div className={styles.dateItem}>
                <i className="fas fa-user-clock"></i>
                <div>
                  <div className={styles.dateLabel}>Реєстрація до:</div>
                  <div className={styles.dateValue}>
                    {formData.registrationDeadline || 'Не вказано'}
                  </div>
                </div>
              </div>
              <div className={styles.dateItem}>
                <i className="fas fa-play-circle"></i>
                <div>
                  <div className={styles.dateLabel}>Початок:</div>
                  <div className={styles.dateValue}>
                    {formData.startDate || 'Не вказано'}
                  </div>
                </div>
              </div>
            </div>

            {/* Деталі */}
            <div className={styles.previewDetails}>
              <div className={styles.detailItem}>
                <i className="fas fa-users"></i>
                {formData.sport === 'armwrestling'
                  ? <span>{ARMWRESTLING_CATEGORIES.find(c => c.id === formData.armCategory)?.name || formData.armCategory}</span>
                  : <span>{formData.minTeams}-{['chess', 'checkers'].includes(formData.sport) ? formData.participantsCount : formData.maxTeams} команд/учасників</span>
                }
              </div>
              <div className={styles.detailItem}>
                <i className="fas fa-chess-board"></i>
                <span>{getFormatName() || 'Формат'}</span>
              </div>
            </div>

            {/* Опис */}
            {formData.description && (
              <div className={styles.previewDescription}>
                <p>{formData.description.substring(0, 150)}{formData.description.length > 150 ? '...' : ''}</p>
              </div>
            )}

            {/* Категорії */}
            {selectedCategories.length > 0 && (
              <div className={styles.previewCategories}>
                {selectedCategories.map(cat => (
                  <span key={cat} className={styles.categoryTag}>{cat}</span>
                ))}
              </div>
            )}
          </>
        ) : (
          <div className={styles.emptyPreview}>
            <i className="fas fa-info-circle"></i>
            <p>Заповніть форму, щоб побачити попередній перегляд</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default PreviewSection;
