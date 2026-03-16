import styles from './RoleSign.module.css';

export default function RoleSign({
  labelInput = 'Роль',
  options = [],
  register,
  error,
}) {
  return (
    <div className={styles.inputGroup}>
      <label className={styles.inputLabel}>{labelInput}</label>

      <select
        className={`${styles.customSelect} ${error ? styles.inputError : ''}`}
        {...register}
        defaultValue=""
      >
        <option value="" disabled>
          Оберіть роль
        </option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

      {error && <span className={styles.errorMessage}>{error}</span>}
    </div>
  );
}
