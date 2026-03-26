import styles from './PreviewSection.module.css';
import { 
  AVAILABLE_SPORTS, 
  TOURNAMENT_TYPES, 
  CHESS_SYSTEMS, 
  CHECKERS_SYSTEMS, 
  TABLE_TENNIS_TOURNAMENT_TYPES, 
  BASKETBALL3X3_TOURNAMENT_TYPES, 
  ARMWRESTLING_HANDS, 
  ARMWRESTLING_WEIGHT_CLASSES, 
  ARMWRESTLING_CATEGORIES, 
  BADMINTON_TOURNAMENT_TYPES 
} from '../../constants';
import { TournamentFormData } from '../../hooks/useTournamentForm';

interface PreviewSectionProps {
  formData: TournamentFormData;
  selectedCategories: string[];
}

const PreviewSection = ({ formData, selectedCategories }: PreviewSectionProps) => {
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
    } else if (formData.sport === 'basketball-3x3') {
      const type = BASKETBALL3X3_TOURNAMENT_TYPES.find(t => t.id === formData.type);
      return type ? type.name : formData.type;
    } else if (formData.sport === 'badminton') {
      const type = BADMINTON_TOURNAMENT_TYPES.find(t => t.id === formData.type);
      return type ? type.name : formData.type;
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
          </>
        ) : (
          <div className={styles.previewEmpty}>
            <i className="fas fa-edit"></i>
            <p>Заповніть форму, щоб побачити попередній перегляд</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default PreviewSection;
