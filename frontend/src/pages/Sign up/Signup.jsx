import './Signup.css';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

import InputSign from '../../components/UI/InputSign/InputSign';
import ButtonSign from '../../components/UI/ButtonSign/ButtonSign';
import RoleSign from '../../components/UI/RoleSign/RoleSign';
import Logo from '../../components/UI/Logo/Logo';

// Схема валідації
const signupSchema = yup.object({
  firstName: yup.string().required("Ім'я є обов’язковим").min(2, 'Мінімум 2 символи'),
  lastName: yup.string().required('Прізвище є обов’язковим').min(2, 'Мінімум 2 символи'),
  role: yup.string().required('Оберіть роль'),
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
  confirmPassword: yup
    .string()
    .required('Підтвердіть пароль')
    .oneOf([yup.ref('password'), null], 'Паролі не співпадають'),
});

export default function Signup() {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: yupResolver(signupSchema),
    mode: 'onChange',
  });

  const onSubmit = (data) => {
    console.log('Дані реєстрації:', data);
    alert('Реєстрація успішна! (демо)');
    // Тут буде реальний запит на сервер
  };

  return (
    <>
      <main className="signup-container">
        <div className="auth-card">
          <div className="logo-sign"><Logo /></div>

          <h1>Створити акаунт</h1>
          <div className="subtitle">Приєднуйся до спортивного життя коледжу</div>

          <form onSubmit={handleSubmit(onSubmit)} noValidate>
            <InputSign
              labelInput="Ім'я"
              type="text"
              placeholder="Іван"
              register={register('firstName')}
              error={errors.firstName?.message}
            />

            <InputSign
              labelInput="Прізвище"
              type="text"
              placeholder="Шевченко"
              register={register('lastName')}
              error={errors.lastName?.message}
            />

            <RoleSign
              labelInput="Роль"
              options={[
                { value: 'User', label: 'Студент / Гравець' },
                { value: 'Admin', label: 'Адміністратор' },
              ]}
              register={register('role')}
              error={errors.role?.message}
            />

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
              placeholder="Мінімум 8 символів"
              register={register('password')}
              error={errors.password?.message}
            />

            <InputSign
              labelInput="Повторіть пароль"
              type="password"
              placeholder="••••••••"
              register={register('confirmPassword')}
              error={errors.confirmPassword?.message}
            />

            <ButtonSign type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Реєстрація...' : 'Зареєструватись'}
            </ButtonSign>
          </form>

          <div className="links">
            <Link to="/login">Вже є акаунт? Увійти</Link>
          </div>
        </div>
      </main>
    </>
  );
}