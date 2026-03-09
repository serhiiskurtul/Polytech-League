import React from 'react';
import shared from '../../TournamentCreatePage.module.css';
import styles from './CategoriesSection.module.css';
import { CATEGORIES } from '../../constants';

const CategoriesSection = ({ selectedCategories, handleCategoryToggle }) => {
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
            {CATEGORIES.map(category => (
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
