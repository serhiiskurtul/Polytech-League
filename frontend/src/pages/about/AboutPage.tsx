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
            <i className="fas fa-medal"></i>
            Твоя спортивна історія
          </h2>
          <p>
            Ставай частиною легендарного руху! Реєструйся на змагання онлайн у кілька кліків,
            стеж за результатами в реальному часі та вкарбовуй своє ім'я в золоті сторінки
            спортивної історії Політеха через систему глобальних рейтингів.
          </p>
        </div>

        <div className={styles.card}>
          <h2>
            <i className="fas fa-chart-line"></i>
            Аналітика та прогрес
          </h2>
          <p>
            Слідкуй за останніми змаганнями, аналізуй гру суперників та відстежуй
            власний прогрес. Наша мета — створити прозору та справедливу систему
            спортивних досягнень для кожного студента.
          </p>
        </div>

        <div className={styles.card}>
          <h2>
            <i className="fas fa-bell"></i>
            Завжди в курсі подій
          </h2>
          <p>
            Отримуй миттєві сповіщення про нові турніри, зміни в розкладі та результати ігор.
            Встановлюй Polytech League як мобільний додаток (PWA) для швидкого доступу
            до матчів та свіжих новин навіть без відкриття браузера.
          </p>
        </div>
      </div>

      <section className={styles.metaSection}>
        <div className={styles.metaGrid}>
          <div className={styles.metaItem}>
            <div className={styles.metaLabel}>Дата запуску</div>
            <div className={styles.metaValue}>Травень 2026</div>
          </div>

          <div className={styles.metaItem}>
            <div className={styles.metaLabel}>Розробник</div>
            <div className={styles.metaValue}>Андрій Думітращук</div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default AboutPage;
