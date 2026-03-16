import { useState } from 'react';
import styles from './Signup.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import InputSign from '../../shared/ui/InputSign/InputSign.jsx';
import ButtonSign from '../../shared/ui/ButtonSign/ButtonSign.jsx';
import Logo from '../../shared/ui/Logo/Logo.jsx';

const API_URL = 'http://localhost:5000';

// Схема валідації
const signupSchema = yup.object({
  firstName: yup
    .string()
    .required("Ім'я є обов'язковим")
    .min(2, 'Мінімум 2 символи')
    .max(50, 'Максимум 50 символів'),

  lastName: yup
    .string()
    .required("Прізвище є обов'язковим")
    .min(2, 'Мінімум 2 символи')
    .max(50, 'Максимум 50 символів'),

  email: yup
    .string()
    .required('Email є обовʼязковим')
    .email('Введіть коректний email')
    .max(100, 'Максимум 100 символів'),

  password: yup
    .string()
    .required('Пароль є обовʼязковим')
    .min(8, 'Пароль має містити щонайменше 8 символів')
    .max(50, 'Пароль занадто довгий')
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
      'Пароль повинен містити великі, малі літери, цифри та спецсимволи'
    ),

  confirmPassword: yup
    .string()
    .required('Підтвердіть пароль')
    .oneOf([yup.ref('password'), null], 'Паролі не співпадають'),
});

export default function Signup() {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm({
    resolver: yupResolver(signupSchema),
    mode: 'onChange',
  });

  const registerUser = async (userData) => {
    try {
      console.log('Відправка запиту на:', `${API_URL}/users`);
      console.log('Дані:', JSON.stringify(userData, null, 2));

      const response = await fetch(`${API_URL}/users`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
      });

      console.log('Статус відповіді:', response.status);

      if (!response.ok) {
        let serverMessage = `Помилка сервера: ${response.status}`;

        try {
          const errorData = await response.json();
          // беремо найточніше поле з Problem Details
          serverMessage =
            errorData.detail ||
            errorData.message ||
            errorData.title ||
            serverMessage;

          console.log('Помилка від сервера:', errorData);
        } catch (e) {
          console.log('Не вдалося розпарсити помилку як JSON');
        }

        throw new Error(serverMessage);
      }

      return { success: true };
    } catch (error) {
      throw error;
    }
  };

  const sendVerificationEmail = async (email) => {
    try {
      console.log('Відправка верифікаційного email на:', email);

      const response = await fetch(`${API_URL}/email/send-verification-email`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email }),
      });

      console.log('Статус відповіді email:', response.status);

      if (!response.ok) {
        let errorMessage = `Помилка відправки email: ${response.status}`;

        try {
          const text = await response.text();
          if (text) errorMessage += ` - ${text}`;
        } catch {
          // ігноруємо
        }

        throw new Error(errorMessage);
      }

      return { success: true };
    } catch (error) {
      console.error('Помилка відправки email:', error);
      throw error;
    }
  };

  const onSubmit = async (data) => {
    setServerError(null); // очищаємо попередню помилку

    try {
      const userData = {
        firstName: data.firstName.trim(),
        lastName: data.lastName.trim(),
        email: data.email.trim(),
        password: data.password,
      };

      await registerUser(userData);
      await sendVerificationEmail(data.email);

      localStorage.setItem('emailToVerify', data.email);
      sessionStorage.setItem('pendingEmailVerification', data.email);
      sessionStorage.setItem('userFirstName', data.firstName);

      navigate('/email-verification', {
        state: { email: data.email, firstName: data.firstName },
        replace: true,
      });
    } catch (error) {
      console.error('Повна помилка реєстрації:', error);

      let message = 'Щось пішло не так. Спробуйте ще раз або зверніться до підтримки.';

      const errText = error.message?.toLowerCase() || '';

      if (errText.includes('already exist') || errText.includes('існує') || errText.includes('користувач з такою')) {
        message = 'Користувач з такою email-адресою вже зареєстрований.';
      } else if (errText.includes('401') || errText.includes('unauthorized')) {
        message = 'Помилка авторизації. Можливо, проблема на сервері.';
      }

      setServerError(message);
    }
  };

  return (
    <main className={styles.signupContainer}>
      <div className={styles.authCard}>
        <div className={styles.logoSign}>
          <Logo />
        </div>

        <h1>Створити акаунт</h1>
        <div className={styles.subtitle}>
          Приєднуйся до спортивного життя коледжу
        </div>

        {serverError && (
          <div className={styles.serverErrorMessage}>
            {serverError}
            {serverError.includes('вже зареєстрований') && (
              <div className={styles.errorExtra}>
                <Link to="/login" className={styles.loginLink}>
                  Увійти в існуючий акаунт →
                </Link>
              </div>
            )}
          </div>
        )}

        {Object.keys(errors).length > 0 && (
          <div className={styles.errorSummary}>
            <h3>Будь ласка, виправте помилки:</h3>
            <ul>
              {errors.firstName?.message && <li>{errors.firstName.message}</li>}
              {errors.lastName?.message && <li>{errors.lastName.message}</li>}
              {errors.email?.message && <li>{errors.email.message}</li>}
              {errors.password?.message && <li>{errors.password.message}</li>}
              {errors.confirmPassword?.message && (
                <li>{errors.confirmPassword.message}</li>
              )}
            </ul>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <InputSign
            labelInput="Ім'я"
            type="text"
            placeholder="Іван"
            register={register('firstName')}
            error={errors.firstName?.message}
            disabled={isSubmitting}
          />

          <InputSign
            labelInput="Прізвище"
            type="text"
            placeholder="Шевченко"
            register={register('lastName')}
            error={errors.lastName?.message}
            disabled={isSubmitting}
          />

          <InputSign
            labelInput="Email"
            type="email"
            placeholder="student@gmail.com"
            register={register('email')}
            error={errors.email?.message}
            disabled={isSubmitting}
          />

          <InputSign
            labelInput="Пароль"
            type="password"
            placeholder="Мінімум 8 символів"
            register={register('password')}
            error={errors.password?.message}
            disabled={isSubmitting}
          />

          <InputSign
            labelInput="Повторіть пароль"
            type="password"
            placeholder="••••••••"
            register={register('confirmPassword')}
            error={errors.confirmPassword?.message}
            disabled={isSubmitting}
          />

          <ButtonSign
            type="submit"
            disabled={isSubmitting}
            loading={isSubmitting}
          >
            {isSubmitting ? 'Реєстрація...' : 'Зареєструватись'}
          </ButtonSign>
        </form>

        <div className={styles.links}>
          <Link to="/login">Вже є акаунт? Увійти</Link>
        </div>
      </div>
    </main>
  );
}