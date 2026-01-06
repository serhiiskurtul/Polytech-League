import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import CodeInput from "../../shared/CodeInput/CodeInput";
import VerificationButton from "../../shared/VerificationButton/VerificationButton";
import CountdownTimer from "../../shared/CountdownTimer/CountdownTimer";
import { showToast } from '../../utils/toast';
import styles from './EmailVerificationPage.module.css';

const API_URL = 'http://localhost:5000';

const EmailVerification = () => {
  const [code, setCode] = useState(['', '', '', '', '', '']);
  const [isVerified, setIsVerified] = useState(false);
  const [hasError, setHasError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [canResend, setCanResend] = useState(false);
  const [isResending, setIsResending] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  const emailFromStorage = localStorage.getItem('emailToVerify');
  const displayEmail = location.state?.email || emailFromStorage || "user@example.com";

  useEffect(() => {
    if (!displayEmail || displayEmail === "user@example.com") {
      navigate('/signup');
    }
  }, [displayEmail, navigate]);

  const handleCodeChange = (newCode) => {
    setCode(newCode);
    setHasError(false);
  };

  const verifyEmail = async (email, code) => {
    try {
      const response = await fetch(`${API_URL}/email/verify`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: email,
          code: code
        }),
      });

      if (!response.ok) {
        throw new Error(`Помилка верифікації: ${response.status}`);
      }

      return;
    } catch (error) {
      console.error('Помилка запиту верифікації:', error);
      throw error;
    }
  };

  const resendVerificationEmail = async (email) => {
    try {
      const response = await fetch(`${API_URL}/email/send-verification-email`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email }),
      });

      if (!response.ok) {
        throw new Error(`Помилка відправки email: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('Помилка відправки email:', error);
      throw error;
    }
  };

  const handleVerify = async () => {
    const enteredCode = code.join('');
    
    if (enteredCode.length !== 6) {
      showToast.error('Введіть повний 6-значний код');
      return;
    }
    
    setIsLoading(true);
    
    try {
      await verifyEmail(displayEmail, enteredCode);
      
      setIsVerified(true);
      setHasError(false);
      
      // Видаляємо email з localStorage
      localStorage.removeItem('emailToVerify');
      sessionStorage.removeItem('pendingEmailVerification');
      sessionStorage.removeItem('userFirstName');
      
      // Перенаправляємо на логін через 3 секунди
      setTimeout(() => {
        navigate('/login', { replace: true });
      }, 1500);

      
    } catch (error) {
      setHasError(true);
      showToast.error('Невірний код підтвердження');
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendCode = async () => {
    if (!canResend || isResending) return;
    
    setIsResending(true);
    
    try {
      await resendVerificationEmail(displayEmail);
      showToast.success(`Новий код надіслано на ${displayEmail}`);
      
      setCode(['', '', '', '', '', '']);
      setHasError(false);
      setCanResend(false);
      
    } catch (error) {
      showToast.error('Не вдалося надіслати код. Спробуйте пізніше.');
    } finally {
      setIsResending(false);
    }
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
            ✓ Email успішно підтверджено! Перенаправляємо на сторінку входу...
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
          disabled={isVerified || isLoading}
        />
        
        <CountdownTimer 
          initialSeconds={60}
          onTimerComplete={() => setCanResend(true)}
          disabled={isVerified}
        />
        
        <button 
          className={styles.resendLink} 
          onClick={handleResendCode}
          disabled={!canResend || isResending || isVerified}
        >
          {isResending ? 'Надсилання...' : 'Надіслати код повторно'}
        </button>
        
        <VerificationButton 
          onClick={handleVerify}
          disabled={!isCodeComplete || isLoading || isVerified}
          isLoading={isLoading}
          isVerified={isVerified}
        />
        
        <button 
          onClick={() => navigate('/login')} 
          className={styles.backLink}
        >
          ← Повернутися до входу
        </button>
      </div>
    </div>
  );
};

export default EmailVerification;