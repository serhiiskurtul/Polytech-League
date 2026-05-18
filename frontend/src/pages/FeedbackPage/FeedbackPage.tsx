import React, { useState } from 'react';
import styles from './FeedbackPage.module.css';
import { useAuth } from '@/app/providers/AuthContext';
import { toast } from 'react-hot-toast';
import api from '@/shared/api/axiosInstance';

const FeedbackPage: React.FC = () => {
  const { user } = useAuth();
  const [content, setContent] = useState('');
  const [type, setType] = useState('Suggestion');
  const [file, setFile] = useState<File | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!content.trim()) {
      toast.error('Будь ласка, введіть текст повідомлення');
      return;
    }

    try {
      setIsSubmitting(true);
      const formData = new FormData();
      formData.append('content', content);
      formData.append('type', type);
      if (file) {
        formData.append('file', file);
      }

      await api.post('/feedback', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      toast.success('Дякуємо за ваш відгук!');
      setContent('');
      setType('Suggestion');
      setFile(null);
      
      // Reset file input
      const fileInput = document.getElementById('file') as HTMLInputElement;
      if (fileInput) fileInput.value = '';
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

          <div className={styles.formGroup}>
            <label htmlFor="file">Прикріпити фото</label>
            <input
              type="file"
              id="file"
              accept="image/*"
              onChange={(e) => setFile(e.target.files?.[0] || null)}
              className={styles.fileInput}
            />
          </div>

          <button type="submit" className={styles.submitBtn} disabled={isSubmitting}>
            {isSubmitting ? 'Відправка...' : 'Надіслати'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default FeedbackPage;
