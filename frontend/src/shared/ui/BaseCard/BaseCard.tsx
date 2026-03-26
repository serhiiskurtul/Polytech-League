import { ReactNode } from 'react';
import styles from './BaseCard.module.css';

interface BaseCardProps {
  children: ReactNode;
  className?: string;
}

export default function BaseCard({ children, className = '' }: BaseCardProps) {
  return (
    <div className={`${styles.card} ${className}`}>
      {children}
    </div>
  );
}
