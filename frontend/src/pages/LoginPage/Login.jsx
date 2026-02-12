import styles from './Login.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useAuth } from '../../auth/AuthContext.jsx';
import { login as loginApi } from "../../api/auth.api.js";
import api from "../../api/axiosInstance.js";

import InputSign from "../../shared/InputSign/InputSign.jsx";
import ButtonSign from '../../shared/ButtonSign/ButtonSign.jsx';
import Logo from '../../shared/Logo/Logo.jsx';

// Yup schema
const loginSchema = yup.object({
  email: yup
    .string()
    .required('Email є обов’язковим')
    .email('Введіть коректний email'),
  password: yup
    .string()
    .required('Пароль є обов’язковим')
    .min(8, 'Пароль має містити щонайменше 8 символів'),
});

export default function Login() {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm({
    resolver: yupResolver(loginSchema),
  });

  const { login } = useAuth();

  const onSubmit = async (data) => {
    try {
      await loginApi(data.email, data.password); 

      const meResponse = await api.get("/users/me");  
      login(meResponse.data);
      navigate("/");
    } catch (error) {
      console.error("Login error:", error);
      if (error.response?.status === 401) {
        setError("root", { message: "Невірний email або пароль" });
      } else {
        setError("root", { message: "Щось пішло не так" });
      }
    }
  };

  return (
    <main className={styles.loginContainer}>
      <div className={styles.authCard}>
        <div className={styles.logoSign}>
          <Logo />
        </div>

        <h1>Вітаємо назад!</h1>
        <div className={styles.subtitle}>
          Увійдіть до свого акаунту
        </div>

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
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
