import React, { useState, useEffect } from 'react';
import styles from './CountdownTimer.module.css';

const CountdownTimer = ({ initialSeconds = 60, onTimerComplete }) => {
  const [seconds, setSeconds] = useState(initialSeconds);
  const [isActive, setIsActive] = useState(true);

  useEffect(() => {
    let interval = null;
    
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

  const formatTime = (secs) => {
    const minutes = Math.floor(secs / 60);
    const seconds = secs % 60;
    return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  };

  return (
    <div className={styles.timer}>
      Надіслати код повторно можна через: <span className={styles.countdown}>{formatTime(seconds)}</span>
    </div>
  );
};

export default CountdownTimer;