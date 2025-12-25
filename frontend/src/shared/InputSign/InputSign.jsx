import styles from './InputSign.module.css';

export default function InputSign({
  labelInput,
  type = 'text',
  placeholder,
  register,
  error,
  ...rest
}) {
  return (
    <div className={styles.inputGroup}>
      <label className={styles.inputLabel}>{labelInput}</label>
      
      <input
        type={type}
        placeholder={placeholder}
        className={`${styles.customInput} ${error ? styles.inputError : ''}`}
        {...register}
        {...rest}
      />

      {error && (
        <span className={styles.errorMessage}>{error}</span>
      )}
    </div>
  );
}
