import { ReactNode, ButtonHTMLAttributes } from 'react';
import styles from './ButtonSign.module.css';

interface ButtonSignProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
}

export default function ButtonSign({ children, onClick, type = "button", disabled = false, ...props }: ButtonSignProps) {
  return (
    <button 
      className={styles.btnPrimary} 
      onClick={onClick}
      type={type}
      disabled={disabled}
      {...props}
    >
      {children}
    </button>
  );
}
