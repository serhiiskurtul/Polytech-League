import React from 'react';
import styles from './TournamentCreatePage.module.css';
import { useTournamentForm } from './hooks/useTournamentForm';

import PageHeader from './components/PageHeader/PageHeader';
import BasicInfoSection from './components/BasicInfoSection/BasicInfoSection';
import DatesLocationSection from './components/DatesLocationSection/DatesLocationSection';
import CategoriesSection from './components/CategoriesSection/CategoriesSection';
import SettingsSection from './components/SettingsSection/SettingsSection';
import FormActions from './components/FormActions/FormActions';
import PreviewSection from './components/PreviewSection/PreviewSection';
import StatsCard from './components/StatsCard/StatsCard';

const TournamentCreatePage = () => {
  const {
    formData,
    selectedCategories,
    isSubmitting,
    handleInputChange,
    handleCategoryToggle,
    handleSubmit,
    fillTestData,
    calculateDuration
  } = useTournamentForm();

  return (
    <div className={styles.tournamentCreatePage}>
      <div className={styles.container}>
        <PageHeader />

        <div className={styles.contentWrapper}>
          <div className={styles.formSection}>
            <form onSubmit={handleSubmit} className={styles.tournamentForm}>
              <BasicInfoSection 
                formData={formData} 
                handleInputChange={handleInputChange} 
              />
              
              <DatesLocationSection 
                formData={formData} 
                handleInputChange={handleInputChange} 
                calculateDuration={calculateDuration} 
              />
              
              <CategoriesSection 
                selectedCategories={selectedCategories} 
                handleCategoryToggle={handleCategoryToggle} 
              />

              <SettingsSection 
                formData={formData} 
                handleInputChange={handleInputChange} 
              />

              <FormActions 
                isSubmitting={isSubmitting} 
                fillTestData={fillTestData} 
              />
            </form>
          </div>

          <div className={styles.previewSection}>
            <PreviewSection 
              formData={formData} 
              selectedCategories={selectedCategories} 
            />
            
            <StatsCard 
              formData={formData} 
              selectedCategories={selectedCategories} 
              calculateDuration={calculateDuration} 
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default TournamentCreatePage;