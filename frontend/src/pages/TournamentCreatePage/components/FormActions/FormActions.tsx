import styles from './FormActions.module.css';

interface FormActionsProps {
  isSubmitting: boolean;
  fillTestData: () => void;
}

const FormActions = ({ isSubmitting, fillTestData }: FormActionsProps) => {
  return (
    <div className={styles.formActions}>
      <button
        type="button"
        className={styles.testDataBtn}
        onClick={fillTestData}
      >
        <i className="fas fa-vial"></i> Заповнити тестові дані
      </button>
      
      <div className={styles.submitActions}>
        <button
          type="button"
          className={styles.cancelBtn}
          onClick={() => window.history.back()}
        >
          <i className="fas fa-times"></i> Скасувати
        </button>
        <button
          type="submit"
          className={styles.submitBtn}
          disabled={isSubmitting}
        >
          {isSubmitting ? (
            <>
              <i className="fas fa-spinner fa-spin"></i> Створення...
            </>
          ) : (
            <>
              <i className="fas fa-check"></i> Створити змагання
            </>
          )}
        </button>
      </div>
    </div>
  );
};

export default FormActions;
