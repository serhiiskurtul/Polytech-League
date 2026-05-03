import styles from './Login.module.css';
import { Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { login as loginApi, axiosInstance as api } from "@/shared/api";
import { InputSign, ButtonSign, Logo } from "@/shared/ui";
import { useAuth } from "@/app/providers/AuthContext";

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

      const meResponse = await api.get("/users/me");
      login(meResponse.data);
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

        <h1>Вітаємо назад!</h1>
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
          <a href="#">Забули пароль?</a>
        </div>
      </div>
    </main>
  );
}
