import { useState, useEffect } from 'react';
import styles from './CountdownTimer.module.css';

interface CountdownTimerProps {
  initialSeconds?: number;
  onTimerComplete?: () => void;
}

const CountdownTimer = ({ initialSeconds = 60, onTimerComplete }: CountdownTimerProps) => {
  const [seconds, setSeconds] = useState(initialSeconds);
  const [isActive, setIsActive] = useState(true);

  useEffect(() => {
    let interval: ReturnType<typeof setInterval> | null = null;
    
    if (isActive && seconds > 0) {
      interval = setInterval(() => {
        setSeconds(prevSeconds => {
          if (prevSeconds <= 1) {
            setIsActive(false);
            if (onTimerComplete) onTimerComplete();
            return 0;
          }
          return prevSeconds - 1;
        });
      }, 1000);
    }
    
    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isActive, seconds, onTimerComplete]);

  const formatTime = (secs: number) => {
    const minutes = Math.floor(secs / 60);
    const remainingSeconds = secs % 60;
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  return (
    <div className={styles.timer}>
      Надіслати код повторно можна через: <span className={styles.countdown}>{formatTime(seconds)}</span>
    </div>
  );
};

export default CountdownTimer;
