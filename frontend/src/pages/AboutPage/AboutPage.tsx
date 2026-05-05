import React from 'react';
import styles from './AboutPage.module.css';

const AboutPage: React.FC = () => {
  return (
    <div className={styles.aboutContainer}>
      <section className={styles.hero}>
        <h1>Polytech League</h1>
        <p>
          Інноваційна екосистема для розвитку студентського спорту, 
          яка об'єднує атлетів, вболівальників та організаторів під одним цифровим дахом.
        </p>
      </section>

      <div className={styles.contentGrid}>
        <div className={styles.card}>
          <h2>
            <i className="fas fa-rocket"></i>
            Про проєкт
          </h2>
          <p>
            Polytech League — це централізована платформа для керування спортивними змаганнями. 
            Ми автоматизували найскладніші процеси: від побудови турнірних сіток 
            до розрахунку динамічних рейтингів гравців на основі їхніх досягнень.
          </p>
        </div>

        <div className={styles.card}>
          <h2>
            <i className="fas fa-shield-alt"></i>
            Наша мета
          </h2>
          <p>
            Створити прозору та справедливу систему оцінки спортивних результатів у Політехніці. 
            Ми прагнемо популяризувати активний спосіб життя через гейміфікацію 
            та надання гравцям детальної статистики їхнього прогресу.
          </p>
        </div>
      </div>

      <section className={styles.metaSection}>
        <div className={styles.metaGrid}>
          <div className={styles.metaItem}>
            <div className={styles.metaLabel}>Створено</div>
            <div className={styles.metaValue}>Вересень 2024</div>
          </div>

          <div className={styles.metaItem}>
            <div className={styles.metaLabel}>Статус проєкту</div>
            <div className={styles.metaValue}>Активна розробка</div>
          </div>

          <div className={styles.metaItem}>
            <div className={styles.metaLabel}>Розробник</div>
            <div className={styles.metaValue}>
              <a 
                href="https://github.com/AndriiDumi" 
                target="_blank" 
                rel="noopener noreferrer"
                className={styles.authorLink}
              >
                Андрій Думіч
              </a>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default AboutPage;
