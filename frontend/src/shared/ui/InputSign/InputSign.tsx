import { InputHTMLAttributes, useState } from 'react';
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
  const [showPassword, setShowPassword] = useState(false);
  const isPassword = type === 'password';

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  return (
    <div className={styles.inputGroup}>
      <label className={styles.inputLabel}>{labelInput}</label>
      
      <div className={styles.inputWrapper}>
        <input
          type={isPassword ? (showPassword ? 'text' : 'password') : type}
          placeholder={placeholder}
          className={`${styles.customInput} ${error ? styles.inputError : ''} ${isPassword ? styles.passwordInput : ''}`}
          {...(register || {})}
          {...rest}
        />
        
        {isPassword && (
          <button
            type="button"
            className={styles.passwordToggle}
            onClick={togglePasswordVisibility}
            aria-label={showPassword ? 'Сховати пароль' : 'Показати пароль'}
          >
            <i className={`fas ${showPassword ? 'fa-eye-slash' : 'fa-eye'}`}></i>
          </button>
        )}
      </div>

      {error && (
        <span className={styles.errorMessage}>{error}</span>
      )}
    </div>
  );
}
