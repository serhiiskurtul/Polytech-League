import styles from './TournamentCreateForm.module.css';
import { useTournamentForm } from '../model/useTournamentForm';

import PageHeader from './PageHeader/PageHeader';
import BasicInfoSection from './BasicInfoSection/BasicInfoSection';
import DatesLocationSection from './DatesLocationSection/DatesLocationSection';
import CategoriesSection from './CategoriesSection/CategoriesSection';
import SettingsSection from './SettingsSection/SettingsSection';
import FormActions from './FormActions/FormActions';
import PreviewSection from './PreviewSection/PreviewSection';
import StatsCard from './StatsCard/StatsCard';

const TournamentCreateForm = () => {
  const {
    formData,
    selectedCategories,
    isSubmitting,
    handleInputChange,
    handleCategoryToggle,
    handleSubmit,
    calculateDuration
  } = useTournamentForm();

  return (
    <div className={styles.formContainer}>
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
  );
};

export default TournamentCreateForm;
