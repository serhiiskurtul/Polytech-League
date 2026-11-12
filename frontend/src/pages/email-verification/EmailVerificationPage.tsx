import { useState, useEffect, useRef } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { CodeInput, VerificationButton, CountdownTimer } from "@/shared/ui";
import { showToast } from '@/shared/lib/toast';
import styles from './EmailVerificationPage.module.css';
import axiosInstance from '@/shared/api/axiosInstance';
import { useAuth } from '@/entities/session';

const EmailVerification = () => {
  const [code, setCode] = useState<string[]>(['', '', '', '', '', '']);
  const [isVerified, setIsVerified] = useState(false);
  const [hasError, setHasError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [canResend, setCanResend] = useState(false);
  const [isResending, setIsResending] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const { user, refreshUser } = useAuth();
  const autoSentRef = useRef(false);

  const emailFromStorage = localStorage.getItem('emailToVerify');
  const displayEmail: string = location.state?.email || user?.email || emailFromStorage || "user@example.com";

  useEffect(() => {
    if (!displayEmail || displayEmail === "user@example.com") {
      navigate('/signup');
    }
  }, [displayEmail, navigate]);

  // Прийшли з банера для непідтверджених — одразу надсилаємо код
  useEffect(() => {
    if (!location.state?.sendOnOpen || autoSentRef.current) return;
    autoSentRef.current = true;

    axiosInstance.post('/email/send-verification-email', { email: displayEmail })
      .then(() => showToast.success('Код надіслано на вашу пошту'))
      .catch(() => showToast.error('Помилка відправки коду. Спробуйте пізніше.'));
  }, [displayEmail, location.state]);

  const handleCodeChange = (newCode: string[]) => {
    setCode(newCode);
    setHasError(false);
  };

  const verifyEmail = async (email: string, code: string) => {
    try {
      await axiosInstance.post('/email/verify', { email, code });
      return;
    } catch (error: any) {
      console.error('Помилка запиту верифікації:', error);
      throw error;
    }
  };

  const resendVerificationEmail = async (email: string) => {
    try {
      const response = await axiosInstance.post('/email/send-verification-email', { email });
      return response.data;
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

      showToast.success('Email успішно підтверджено!');

      if (user) {
        await refreshUser();
      }

      setTimeout(() => {
        navigate(user ? '/' : '/login');
      }, 2000);
    } catch (error) {
      setHasError(true);
      showToast.error('Невірний код підтвердження. Спробуйте ще раз.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleResend = async () => {
    if (!canResend) return;

    setIsResending(true);
    try {
      await resendVerificationEmail(displayEmail);
      showToast.success('Новий код надіслано на вашу пошту');
      setCanResend(false);
    } catch (error) {
      showToast.error('Помилка відправки коду. Спробуйте пізніше.');
    } finally {
      setIsResending(false);
    }
  };

  const handleTimerComplete = () => {
    setCanResend(true);
  };

  return (
    <div className={styles.verificationContainer}>
      <div className={styles.verificationCard}>
        <div className={styles.header}>
          <div className={styles.iconContainer}>
            <i className="fas fa-envelope-open-text"></i>
          </div>
          <h1 className={styles.title}>Підтвердження Email</h1>
          <p className={styles.description}>
            Ми надіслали 6-значний код на адресу: <br />
            <strong>{displayEmail}</strong>
          </p>
          <div className={styles.spamNotice}>
            <i className="fas fa-info-circle"></i>
            <span>Якщо ви не бачите листа, перевірте папку <strong>"Спам"</strong></span>
          </div>
        </div>

        <div className={styles.codeSection}>
          <CodeInput 
            code={code} 
            onChange={handleCodeChange} 
            hasError={hasError}
            autoFocus={true}
          />
          {hasError && <p className={styles.errorMessage}>Невірний код. Перевірте ще раз.</p>}
        </div>

        <div className={styles.actionSection}>
          <VerificationButton 
            onClick={handleVerify} 
            disabled={isLoading || isVerified}
            isLoading={isLoading}
            isVerified={isVerified}
          />
          
          <div className={styles.resendSection}>
            {canResend ? (
              <button 
                className={styles.resendButton} 
                onClick={handleResend}
                disabled={isResending}
              >
                {isResending ? 'Надсилаємо...' : 'Надіслати код повторно'}
              </button>
            ) : (
              <CountdownTimer onTimerComplete={handleTimerComplete} />
            )}
          </div>
        </div>

        <div className={styles.footer}>
          <button className={styles.backButton} onClick={() => navigate(user ? '/' : '/login')}>
            <i className="fas fa-clock"></i> Підтвердити пізніше
          </button>
        </div>
      </div>
    </div>
  );
};

export default EmailVerification;
