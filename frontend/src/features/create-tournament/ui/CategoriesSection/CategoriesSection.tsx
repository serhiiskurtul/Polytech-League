import shared from '../form.module.css';
import styles from './CategoriesSection.module.css';
import { CATEGORIES } from '../../model/constants';

interface CategoriesSectionProps {
  selectedCategories: string[];
  handleCategoryToggle: (category: string) => void;
  sport: string;
  isTeam: boolean;
}

const CategoriesSection = ({ selectedCategories, handleCategoryToggle, sport, isTeam }: CategoriesSectionProps) => {
  const filteredCategories = CATEGORIES.filter(cat => {
    if (cat === 'Змішані') {
      const sportsWithoutMixed = [
        'futsal', 'basketball-3x3', 'volleyball', 'beach-volleyball', 
        'armwrestling', 'table_tennis', 'badminton'
      ];
      if (sportsWithoutMixed.includes(sport)) return false;
      if ((sport === 'chess' || sport === 'checkers') && isTeam) return false;
    }
    return true;
  });

  return (
    <div className={shared.formBlock}>
      <h3 className={shared.blockTitle}>
        <i className="fas fa-filter"></i> Категорії та обмеження
      </h3>
      
      <div className={shared.formGrid}>
        {/* Категорії */}
        <div className={shared.formGroup}>
          <label className={shared.formLabel}>
            <i className="fas fa-tags"></i> Категорії учасників *
          </label>
          <div className={styles.categoriesGrid}>
            {filteredCategories.map(category => (
              <button
                key={category}
                type="button"
                className={`${styles.categoryBtn} ${
                  selectedCategories.includes(category) ? styles.selected : ''
                }`}
                onClick={() => handleCategoryToggle(category)}
              >
                {category}
                {selectedCategories.includes(category) && (
                  <i className="fas fa-check"></i>
                )}
              </button>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default CategoriesSection;
