import { InputHTMLAttributes } from 'react';
import styles from './InputSign.module.css';
import { UseFormRegisterReturn } from 'react-hook-form';

interface InputSignProps extends InputHTMLAttributes<HTMLInputElement> {
  labelInput: string;
  register?: UseFormRegisterReturn;
  error?: string;
}

export default function InputSign({
  labelInput,
  type = 'text',
  placeholder,
  register,
  error,
  ...rest
}: InputSignProps) {
  return (
    <div className={styles.inputGroup}>
      <label className={styles.inputLabel}>{labelInput}</label>
      
      <input
        type={type}
        placeholder={placeholder}
        className={`${styles.customInput} ${error ? styles.inputError : ''}`}
        {...(register || {})}
        {...rest}
      />

      {error && (
        <span className={styles.errorMessage}>{error}</span>
      )}
    </div>
  );
}
