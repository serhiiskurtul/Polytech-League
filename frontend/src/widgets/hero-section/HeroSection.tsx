import styles from './HeroSection.module.css';

const HeroSection = () => {
  return (
    <section className={styles.heroSection}>
      <div className={styles.heroCard}>
        <h1 className={styles.heroTitle}>Спортивна платформа ЧПФК</h1>
        <p className={styles.heroDescription}>
          Приєднуйтесь до змагань, стежте за результатами матчів та ведіть статистику
        </p>
      </div>
    </section>
  );
};

export default HeroSection;
