import React, { useState, useEffect } from 'react';
import CodeInput from "../../shared/CodeInput/CodeInput";
import VerificationButton from "../../shared/VerificationButton/VerificationButton";
import CountdownTimer from "../../shared/CountdownTimer/CountdownTimer";

import styles from './EmailVerificationPage.module.css';
const EmailVerification = ({ email = "user@example.com", redirectUrl = "/" }) => {
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [isVerified, setIsVerified] = useState(false);
  const [hasError, setHasError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [correctCode, setCorrectCode] = useState('123456');
  const [canResend, setCanResend] = useState(false);
  const [isResending, setIsResending] = useState(false);
  
  const emailFromStorage = localStorage.getItem('emailToVerify');
  const displayEmail = email || emailFromStorage || "user@example.com";

  useEffect(() => {
    if (email) {
      localStorage.setItem('emailToVerify', email);
    }
  }, [email]);

  const handleCodeChange = (newCode) => {
    setCode(newCode);
    setHasError(false);
  };

  const handleVerify = async () => {
    const enteredCode = code.join('');
    
    if (enteredCode.length !== 6) return;
    
    setIsLoading(true);
    
    setTimeout(() => {
      if (enteredCode === correctCode) {
        setIsVerified(true);
        setHasError(false);
        
        setTimeout(() => {
          window.location.href = redirectUrl;
        }, 2000);
      } else {
        setHasError(true);
      }
      setIsLoading(false);
    }, 500);
  };

  const handleResendCode = () => {
    if (!canResend || isResending) return;
    
    setIsResending(true);
    
    setTimeout(() => {
      const newCode = Math.floor(100000 + Math.random() * 900000).toString();
      setCorrectCode(newCode);
      console.log(`Новий код: ${newCode}`);
      
      alert(`Новий код надіслано на ${displayEmail}`);
      
      setCode(['', '', '', '', '', '']);
      setHasError(false);
      
      setIsResending(false);
      setCanResend(false);
    }, 800);
  };

  const isCodeComplete = code.every(digit => digit !== '');

  return (
    <div className={styles.container}>
        <div className={styles.card}>
        <div className={styles.icon}>📧</div>
        <h2>Підтвердження email</h2>
        
        <div className={styles.description}>
            Ми надіслали 6-значний код підтвердження на адресу 
            <span className={styles.emailHighlight}> {displayEmail}</span>.
            Введіть його нижче:
        </div>
        
        {isVerified && (
            <div className={`${styles.message} ${styles.successMessage}`}>
            ✓ Email успішно підтверджено! Перенаправляємо на головну сторінку...
            </div>
        )}
        
        {hasError && (
            <div className={`${styles.message} ${styles.errorMessage}`}>
            Неправильний код. Спробуйте ще раз або запросіть новий код.
            </div>
        )}
        
        <CodeInput 
            code={code} 
            onChange={handleCodeChange} 
            hasError={hasError}
            autoFocus={true}
        />
        
        <CountdownTimer 
            initialSeconds={60}
            onTimerComplete={() => setCanResend(true)}
        />
        
        <button 
            className={styles.resendLink} 
            onClick={handleResendCode}
            disabled={!canResend || isResending}
        >
            {isResending ? 'Надсилання...' : 'Надіслати код повторно'}
        </button>
        
        <VerificationButton 
            onClick={handleVerify}
            disabled={!isCodeComplete || isLoading || isVerified}
            isLoading={isLoading}
            isVerified={isVerified}
        />
        
        <a href="/login" className={styles.backLink}>← Повернутися до входу</a>
        </div>
    </div>
  );
};

export default EmailVerification;