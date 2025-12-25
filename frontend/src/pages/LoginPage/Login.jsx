import styles from './Login.module.css';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

import InputSign from "../../shared/InputSign/InputSign.jsx";
import ButtonSign from '../../shared/ButtonSign/ButtonSign.jsx';
import Logo from '../../shared/Logo/Logo.jsx';

// Схема валідації з Yup
const loginSchema = yup.object({
  email: yup
    .string()
    .required('Email є обов’язковим')
    .email('Введіть коректний email')
    .matches(/@gmail\.com$/, 'Email повинен закінчуватися на @gmail.com'),
  password: yup
    .string()
    .required('Пароль є обов’язковим')
    .min(8, 'Пароль має містити щонайменше 8 символів')
    .max(50, 'Пароль занадто довгий'),
});

export default function Login() {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm({
    resolver: yupResolver(loginSchema),
    mode: 'onChange',
  });

  const onSubmit = (data) => {
    console.log('Дані форми:', data);
    alert('Успішний логін! (демо)');
  };

  return (
    <main className={styles.loginContainer}>
      <div className={styles.authCard}>
        <div className={styles.logoSign}>
          <Logo />
        </div>

        <h1>Вітаємо назад!</h1>
        <div className={styles.subtitle}>
          Увійдіть до свого акаунту коледжу
        </div>

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <InputSign
            labelInput="Email"
            type="email"
            placeholder="student@college.edu.ua"
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

          <ButtonSign type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Вхід...' : 'Увійти'}
          </ButtonSign>
        </form>

        <div className={styles.links}>
          <Link to="/signup">Немає акаунту? Зареєструватись</Link>
          <a href="#">Забули пароль?</a>
        </div>
      </div>
    </main>
  );
}
