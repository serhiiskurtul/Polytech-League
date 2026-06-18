import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate, Link } from 'react-router-dom';
import { InputSign, ButtonSign, Logo } from "@/shared/ui";
import { resetPassword } from "@/entities/session/api/auth.api";
import toast from 'react-hot-toast';
import styles from '../login/Login.module.css';

const schema = yup.object({
  email: yup.string().required('Email є обов’язковим').email('Введіть коректний email'),
  code: yup.string().required('Код є обов’язковим'),
  password: yup.string().required('Пароль є обов’язковим').min(8, 'Пароль має містити щонайменше 8 символів'),
  confirmPassword: yup.string()
    .oneOf([yup.ref('password')], 'Паролі повинні співпадати')
    .required('Підтвердження пароля є обов’язковим'),
}).required();

type FormData = yup.InferType<typeof schema>;

export default function ResetPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting }, setError } = useForm<FormData>({
    resolver: yupResolver(schema),
  });

  const onSubmit = async (data: FormData) => {
    try {
      await resetPassword({
        email: data.email,
        code: data.code,
        newPassword: data.password,
      });
      toast.success("Пароль успішно змінено!");
      navigate('/login');
    } catch (error: any) {
      const detail = error.response?.data?.detail;
      setError("root", { message: detail || "Невірний код або сталася помилка" });
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
          Введіть email, код з листа та новий пароль.
        </div>

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          {errors.root && <div className={styles.errorMessage}>{errors.root.message}</div>}

          <InputSign
            labelInput="Email"
            type="email"
            placeholder="student@gmail.com"
            register={register('email')}
            error={errors.email?.message}
          />

          <InputSign
            labelInput="Код підтвердження"
            type="text"
            placeholder="123456"
            register={register('code')}
            error={errors.code?.message}
          />

          <InputSign
            labelInput="Новий пароль"
            type="password"
            placeholder="••••••••"
            register={register('password')}
            error={errors.password?.message}
          />

          <InputSign
            labelInput="Підтвердження пароля"
            type="password"
            placeholder="••••••••"
            register={register('confirmPassword')}
            error={errors.confirmPassword?.message}
          />

          <ButtonSign type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Збереження...' : 'Змінити пароль'}
          </ButtonSign>
        </form>

        <div className={styles.links}>
          <Link to="/login">Повернутися до входу</Link>
        </div>
      </div>
    </main>
  );
}
