import { useState } from 'react';
import styles from './Signup.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { InputSign, ButtonSign, Logo } from '@/shared/ui';
import axiosInstance from '@/shared/api/axiosInstance';

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
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/,
      'Пароль повинен містити великі, малі літери та цифри'
    ),

  confirmPassword: yup
    .string()
    .required('Підтвердіть пароль')
    .oneOf([yup.ref('password')], 'Паролі не співпадають'),
}).required();

type SignupFormInputs = yup.InferType<typeof signupSchema>;

export default function Signup() {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<SignupFormInputs>({
    resolver: yupResolver(signupSchema),
    mode: 'onChange',
  });

  const registerUser = async (userData: any) => {
    try {
      console.log('Відправка запиту на реєстрацію...');
      
      const response = await axiosInstance.post('/users', userData);
      return { success: true, data: response.data };
    } catch (error: any) {
      let serverMessage = 'Помилка сервера';
      
      if (error.response) {
        const errorData = error.response.data;
        serverMessage =
          errorData.detail ||
          errorData.message ||
          errorData.title ||
          `Помилка ${error.response.status}`;
      } else if (error.request) {
        serverMessage = 'Сервер не відповідає';
      }

      throw new Error(serverMessage);
    }
  };

  const onSubmit = async (data: SignupFormInputs) => {
    setServerError(null);

    const userData = {
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      password: data.password,
    };

    try {
      await registerUser(userData);
      
      // Відправляємо лист для підтвердження
      try {
        await axiosInstance.post('/email/send-verification-email', { email: data.email });
      } catch (emailError) {
        console.error("Не вдалося відправити лист для підтвердження", emailError);
        // Не перериваємо процес, якщо лист не відправився (може бути Debug Mode)
      }
      
      localStorage.setItem('emailToVerify', data.email);
      sessionStorage.setItem('pendingEmailVerification', 'true');
      sessionStorage.setItem('userFirstName', data.firstName);
      
      navigate('/email-verification', { 
        state: { email: data.email } 
      });
    } catch (error: any) {
      setServerError(error.message);
      if (error.message.includes('email')) {
        setError('email', { type: 'manual', message: 'Цей email вже використовується' });
      }
    }
  };

  return (
    <main className={styles.signupContainer}>
      <div className={styles.authCard}>
        <div className={styles.logoSign}>
          <Logo />
        </div>

        <h1>Створіть акаунт</h1>
        <div className={styles.subtitle}>
          Приєднуйтесь до спортивної спільноти коледжу
        </div>

        {serverError && <div className={styles.serverError}>{serverError}</div>}

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <div className={styles.row}>
            <InputSign
              labelInput="Ім'я"
              placeholder="Артем"
              autoComplete="given-name"
              register={register('firstName')}
              error={errors.firstName?.message}
            />
            <InputSign
              labelInput="Прізвище"
              placeholder="Іванов"
              autoComplete="family-name"
              register={register('lastName')}
              error={errors.lastName?.message}
            />
          </div>

          <InputSign
            labelInput="Email"
            type="email"
            placeholder="student@gmail.com"
            autoComplete="email"
            register={register('email')}
            error={errors.email?.message}
          />

          <InputSign
            labelInput="Пароль"
            type="password"
            placeholder="••••••••"
            autoComplete="new-password"
            register={register('password')}
            error={errors.password?.message}
          />

          <InputSign
            labelInput="Підтвердьте пароль"
            type="password"
            placeholder="••••••••"
            autoComplete="new-password"
            register={register('confirmPassword')}
            error={errors.confirmPassword?.message}
          />

          <ButtonSign type="submit" disabled={isSubmitting}>
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
