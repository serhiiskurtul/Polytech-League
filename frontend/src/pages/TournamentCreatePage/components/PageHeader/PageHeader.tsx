import styles from './PageHeader.module.css';

const PageHeader = () => {
  return (
    <div className={styles.pageHeader}>
      <div className={styles.headerContent}>
        <h1 className={styles.pageTitle}>
          <i className="fas fa-plus-circle"></i> Створення змагання
        </h1>
        <button 
          type="button"
          className={styles.previewBtn}
          onClick={() => alert('Попередній перегляд тимчасово недоступний')}
        >
          <i className="fas fa-eye"></i> Попередній перегляд
        </button>
      </div>
      <p className={styles.pageSubtitle}>
        Заповніть форму для створення нового змагання. Всі поля позначені * є обов'язковими. Наразі підтримуються тільки шахи.
      </p>
    </div>
  );
};

export default PageHeader;
