import { useState } from 'react';
import styles from './Signup.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { InputSign, ButtonSign, Logo } from '@/shared/ui';

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
      console.log('Відправка запиту на:', `${API_URL}/users`);
      
      const response = await fetch(`${API_URL}/users`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
      });

      if (!response.ok) {
        let serverMessage = `Помилка сервера: ${response.status}`;

        try {
          const errorData = await response.json();
          serverMessage =
            errorData.detail ||
            errorData.message ||
            errorData.title ||
            serverMessage;
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
              register={register('firstName')}
              error={errors.firstName?.message}
            />
            <InputSign
              labelInput="Прізвище"
              placeholder="Іванов"
              register={register('lastName')}
              error={errors.lastName?.message}
            />
          </div>

          <InputSign
            labelInput="Email"
            type="email"
            placeholder="student@gmail.com"
            register={register('email')}
            error={errors.email?.message}
          />

          <InputSign
            labelInput="Пароль"
            type="password"
            placeholder="••••••••"
            register={register('password')}
            error={errors.password?.message}
          />

          <InputSign
            labelInput="Підтвердьте пароль"
            type="password"
            placeholder="••••••••"
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
