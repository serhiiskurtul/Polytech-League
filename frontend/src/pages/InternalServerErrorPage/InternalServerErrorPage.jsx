import styles from './InternalServerErrorPage.module.css';

export default function InternalServerError() {
  return (
    <div className={styles.spacer500}>
      <div className={styles.card500}>
        <h1 style={{ margin: 0 }}>500</h1>
        <p style={{ color: "var(--muted)" }}>
          Внутрішня помилка сервера
        </p>

        <button
          className={styles.btn500}
          onClick={() => location.reload()}
        >
          Оновити сторінку
        </button>
      </div>
    </div>
  );
}
