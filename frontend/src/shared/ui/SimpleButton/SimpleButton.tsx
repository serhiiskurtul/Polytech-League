import { Link } from 'react-router-dom';
import styles from './SimpleButton.module.css';
import { ButtonHTMLAttributes } from 'react';

interface SimpleButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  to?: string;
  text: string;
  fullWidth?: boolean;
  insideLink?: boolean;
}

export default function SimpleButton({ 
  to, 
  text, 
  onClick, 
  fullWidth = false, 
  insideLink = false, 
  ...props 
}: SimpleButtonProps) {
  const className = `${styles.viewBtn} ${fullWidth ? styles.fullWidth : ''}`;

  const buttonContent = (
    <button className={className} onClick={onClick} type="button" {...props}>
      {text}
    </button>
  );

  // Якщо кнопка всередині іншого Link або to не передано — просто button
  if (!to || insideLink) {
    return buttonContent;
  }

  // Інакше обгортаємо у Link
  return (
    <Link to={to} className={styles.viewLink}>
      {buttonContent}
    </Link>
  );
}
