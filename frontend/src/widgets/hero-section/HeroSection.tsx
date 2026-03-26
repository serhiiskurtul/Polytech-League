import styles from './HeroSection.module.css';

const HeroSection = () => {
  return (
    <section className={styles.heroSection}>
      <div className={styles.heroCard}>
        <h1 className={styles.heroTitle}>Спортивна платформа ЧПФК</h1>
        <p className={styles.heroDescription}>
          Приєднуйтесь до турнірів, стежте за результатами матчів та ведіть статистику
        </p>
        
        <div className={styles.heroStats}>
          <div className={styles.stat}>
            <div className={styles.statValue}>24</div>
            <div className={styles.statLabel}>Активні турніри</div>
          </div>
          <div className={styles.stat}>
            <div className={styles.statValue}>156</div>
            <div className={styles.statLabel}>Зареєстровані гравці</div>
          </div>
          <div className={styles.stat}>
            <div className={styles.statValue}>8</div>
            <div className={styles.statLabel}>Спортивних дисциплін</div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;
