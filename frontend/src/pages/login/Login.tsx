import styles from './Login.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { login as loginApi } from "@/entities/session/api/auth.api";
import api from "@/shared/api/axiosInstance";
import { InputSign, ButtonSign, Logo } from "@/shared/ui";
import { useAuth } from '@/entities/session';

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
}).required();

type LoginFormInputs = yup.InferType<typeof loginSchema>;

export default function Login() {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<LoginFormInputs>({
    resolver: yupResolver(loginSchema),
  });

  const { login } = useAuth();

  const onSubmit = async (data: LoginFormInputs) => {
    try {
      const response = await loginApi({ email: data.email, password: data.password });

      // Save tokens to localStorage
      localStorage.setItem('token', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);

      // Set header explicitly for the current instance to be sure
      api.defaults.headers.common['Authorization'] = `Bearer ${response.accessToken}`;

      const meResponse = await api.get("/users/me");
      login(meResponse.data);
      
      try {
        let guestId = localStorage.getItem('guestId');
        if (!guestId) {
          guestId = crypto.randomUUID();
          localStorage.setItem('guestId', guestId);
        }
        await api.post('/SiteVisits', { guestId });
      } catch (e) {
        console.error('Failed to record visit after login', e);
      }
      
      navigate("/");
    } catch (error: any) {
      console.error("Login error:", error);
      const detail = error.response?.data?.detail;

      if (detail && detail.toLowerCase().includes("blocked")) {
        setError("root", { message: "Ваш акаунт заблокований адміністратором" });
      } else if (error.response?.status === 401) {
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

        <h1>З поверненням!</h1>
        <div className={styles.subtitle}>
          Увійдіть до свого акаунту
        </div>

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          {errors.root && <div className={styles.errorMessage}>{errors.root.message}</div>}

          <InputSign
            labelInput="Email"
            type="email"
            placeholder="student@gmail.com"
            autoComplete="username email"
            register={register('email')}
            error={errors.email?.message}
          />

          <InputSign
            labelInput="Пароль"
            type="password"
            placeholder="••••••••"
            autoComplete="current-password"
            register={register('password')}
            error={errors.password?.message}
          />



          <ButtonSign type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Вхід...' : 'Увійти'}
          </ButtonSign>
        </form>

        <div className={styles.links}>
          <Link to="/signup">Немає акаунту? Зареєструватись</Link>
          <Link to="/forgot-password">Забули пароль?</Link>
        </div>
      </div>
    </main>
  );
}
