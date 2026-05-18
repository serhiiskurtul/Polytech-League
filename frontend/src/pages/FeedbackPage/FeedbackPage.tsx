import React, { useState } from 'react';
import styles from './FeedbackPage.module.css';
import { useAuth } from '@/app/providers/AuthContext';
import { toast } from 'react-hot-toast';
import api from '@/shared/api/axiosInstance';

const FeedbackPage: React.FC = () => {
  const { user } = useAuth();
  const [content, setContent] = useState('');
  const [type, setType] = useState('Suggestion');
  const [isAnonymous, setIsAnonymous] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!content.trim()) {
      toast.error('Будь ласка, введіть текст повідомлення');
      return;
    }

    try {
      setIsSubmitting(true);
      await api.post('/feedback', {
        content,
        type,
        isAnonymous: !user ? true : isAnonymous
      });
      toast.success('Дякуємо за ваш відгук!');
      setContent('');
      setType('Suggestion');
      setIsAnonymous(false);
    } catch (error) {
      toast.error('Помилка при відправці відгуку');
      console.error(error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <h1 className={styles.title}>Пропозиції та скарги</h1>
        <p className={styles.subtitle}>Допоможіть нам зробити сайт кращим</p>
      </header>

      <div className={styles.formWrapper}>
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.formGroup}>
            <label htmlFor="type">Тип повідомлення</label>
            <select
              id="type"
              value={type}
              onChange={(e) => setType(e.target.value)}
              className={styles.select}
            >
              <option value="Suggestion">Пропозиція</option>
              <option value="Complaint">Скарга</option>
              <option value="Other">Інше</option>
            </select>
          </div>

          <div className={styles.formGroup}>
            <label htmlFor="content">Повідомлення</label>
            <textarea
              id="content"
              placeholder="Опишіть вашу пропозицію або скаргу..."
              value={content}
              onChange={(e) => setContent(e.target.value)}
              required
              rows={6}
              className={styles.textarea}
            />
          </div>

          {user && (
            <div className={styles.checkboxGroup}>
              <input
                type="checkbox"
                id="isAnonymous"
                checked={isAnonymous}
                onChange={(e) => setIsAnonymous(e.target.checked)}
              />
              <label htmlFor="isAnonymous">Надіслати анонімно</label>
            </div>
          )}

          {!user && (
            <p className={styles.anonymousNote}>
              * Ви не авторизовані, тому повідомлення буде надіслано анонімно.
            </p>
          )}

          <button type="submit" className={styles.submitBtn} disabled={isSubmitting}>
            {isSubmitting ? 'Відправка...' : 'Надіслати'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default FeedbackPage;
