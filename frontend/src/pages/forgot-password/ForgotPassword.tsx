import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { Link, useNavigate } from 'react-router-dom';
import { InputSign, ButtonSign, Logo } from "@/shared/ui";
import { forgotPassword } from "@/entities/session/api/auth.api";
import styles from '../login/Login.module.css'; // Перевикористовуємо стилі логіну

const schema = yup.object({
  email: yup.string().required('Email є обов’язковим').email('Введіть коректний email'),
}).required();

type FormData = yup.InferType<typeof schema>;

export default function ForgotPassword() {
  const [isSuccess, setIsSuccess] = useState(false);
  const { register, handleSubmit, formState: { errors, isSubmitting }, setError } = useForm<FormData>({
    resolver: yupResolver(schema),
  });

  const navigate = useNavigate();

  const onSubmit = async (data: FormData) => {
    try {
      await forgotPassword(data.email);
      navigate('/reset-password', { state: { email: data.email } });
    } catch (error) {
      setError("root", { message: "Щось пішло не так. Спробуйте пізніше." });
    }
  };

  return (
    <main className={styles.loginContainer}>
      <div className={styles.authCard}>
        <div className={styles.logoSign}>
          <Logo />
        </div>

        <h1>Відновлення пароля</h1>
        
        <div className={styles.subtitle}>
          Введіть свій email, і ми надішлемо вам код для відновлення пароля.
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

          <ButtonSign type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Надсилання...' : 'Надіслати код'}
          </ButtonSign>
        </form>

        <div className={styles.links}>
          <Link to="/login">Повернутися до входу</Link>
        </div>
      </div>
    </main>
  );
}
