import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { Link } from 'react-router-dom';
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

  const onSubmit = async (data: FormData) => {
    try {
      await forgotPassword(data.email);
      setIsSuccess(true);
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
        
        {isSuccess ? (
          <div style={{ textAlign: 'center', marginTop: '20px' }}>
            <p style={{ color: 'var(--success-color)', marginBottom: '20px' }}>
              Інструкції з відновлення пароля були надіслані на вашу пошту.
            </p>
            <Link to="/reset-password">
              <ButtonSign>Ввести код відновлення</ButtonSign>
            </Link>
          </div>
        ) : (
          <>
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
          </>
        )}
      </div>
    </main>
  );
}
