import shared from '../form.module.css';
import styles from './StatsCard.module.css';
import { CATEGORIES } from '../../model/constants';
import { TournamentFormData } from '../../model/useTournamentForm';

interface StatsCardProps {
  formData: TournamentFormData;
  selectedCategories: string[];
  calculateDuration: () => number;
}

const StatsCard = ({ formData, selectedCategories, calculateDuration }: StatsCardProps) => {
  return (
    <div className={styles.statsCard}>
      <h4 className={styles.statsTitle}>Статистика заповнення</h4>
      <div className={styles.statsGrid}>
        <div className={styles.statItem}>
          <div className={styles.statValue}>
            {Object.values(formData).filter(v => v !== null && v !== undefined && v.toString().trim() !== '').length}
          </div>
          <div className={styles.statLabel}>Заповнених полів</div>
        </div>
        <div className={styles.statItem}>
          <div className={styles.statValue}>{selectedCategories.length}</div>
          <div className={styles.statLabel}>Категорій</div>
        </div>
        <div className={styles.statItem}>
          <div className={styles.statValue}>
            {calculateDuration() || '?'}
          </div>
          <div className={styles.statLabel}>Днів тривалість</div>
        </div>
      </div>
    </div>
  );
};

export default StatsCard;
