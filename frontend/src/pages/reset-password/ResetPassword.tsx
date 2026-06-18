import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { InputSign, ButtonSign, Logo } from "@/shared/ui";
import { resetPassword, verifyResetCode } from "@/entities/session/api/auth.api";
import toast from 'react-hot-toast';
import styles from '../login/Login.module.css';

const codeSchema = yup.object({
  code: yup.string().required('Код є обов’язковим'),
}).required();

const passwordSchema = yup.object({
  password: yup.string().required('Пароль є обов’язковим').min(8, 'Пароль має містити щонайменше 8 символів'),
  confirmPassword: yup.string()
    .oneOf([yup.ref('password')], 'Паролі повинні співпадати')
    .required('Підтвердження пароля є обов’язковим'),
}).required();

export default function ResetPassword() {
  const navigate = useNavigate();
  const location = useLocation();
  const [step, setStep] = useState<1 | 2>(1);
  const [email, setEmail] = useState('');
  const [verifiedCode, setVerifiedCode] = useState('');

  useEffect(() => {
    if (location.state?.email) {
      setEmail(location.state.email);
    } else {
      navigate('/forgot-password');
    }
  }, [location, navigate]);

  const { register: registerCode, handleSubmit: handleSubmitCode, formState: { errors: codeErrors, isSubmitting: isSubmittingCode }, setError: setCodeError } = useForm({
    resolver: yupResolver(codeSchema),
  });

  const { register: registerPassword, handleSubmit: handleSubmitPassword, formState: { errors: passwordErrors, isSubmitting: isSubmittingPassword }, setError: setPasswordError } = useForm({
    resolver: yupResolver(passwordSchema),
  });

  const onVerifyCode = async (data: any) => {
    try {
      await verifyResetCode({ email, code: data.code });
      setVerifiedCode(data.code);
      setStep(2);
      toast.success("Код підтверджено!");
    } catch (error: any) {
      const detail = error.response?.data?.detail;
      setCodeError("root", { message: detail || "Невірний код" });
    }
  };

  const onResetPassword = async (data: any) => {
    try {
      await resetPassword({
        email: email,
        code: verifiedCode,
        newPassword: data.password,
      });
      toast.success("Пароль успішно змінено!");
      navigate('/login');
    } catch (error: any) {
      const detail = error.response?.data?.detail;
      setPasswordError("root", { message: detail || "Сталася помилка" });
    }
  };

  return (
    <main className={styles.loginContainer}>
      <div className={styles.authCard}>
        <div className={styles.logoSign}>
          <Logo />
        </div>

        <h1>Новий пароль</h1>
        <div className={styles.subtitle}>
          {step === 1 ? `Введіть код підтвердження, який ми надіслали на ${email}` : "Введіть новий пароль"}
        </div>

        {step === 1 ? (
          <form onSubmit={handleSubmitCode(onVerifyCode)} noValidate>
            {codeErrors.root && <div className={styles.errorMessage}>{codeErrors.root.message}</div>}

            <InputSign
              labelInput="Код підтвердження"
              type="text"
              placeholder="123456"
              register={registerCode('code')}
              error={codeErrors.code?.message}
            />

            <ButtonSign type="submit" disabled={isSubmittingCode}>
              {isSubmittingCode ? 'Перевірка...' : 'Підтвердити код'}
            </ButtonSign>
          </form>
        ) : (
          <form onSubmit={handleSubmitPassword(onResetPassword)} noValidate>
            {passwordErrors.root && <div className={styles.errorMessage}>{passwordErrors.root.message}</div>}

            <InputSign
              labelInput="Новий пароль"
              type="password"
              placeholder="••••••••"
              register={registerPassword('password')}
              error={passwordErrors.password?.message}
            />

            <InputSign
              labelInput="Підтвердження пароля"
              type="password"
              placeholder="••••••••"
              register={registerPassword('confirmPassword')}
              error={passwordErrors.confirmPassword?.message}
            />

            <ButtonSign type="submit" disabled={isSubmittingPassword}>
              {isSubmittingPassword ? 'Збереження...' : 'Змінити пароль'}
            </ButtonSign>
          </form>
        )}

        <div className={styles.links}>
          <Link to="/login">Повернутися до входу</Link>
        </div>
      </div>
    </main>
  );
}
