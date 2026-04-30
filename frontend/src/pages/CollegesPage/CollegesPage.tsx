import React, { useState, useEffect } from 'react';
import styles from './CollegesPage.module.css';
import { getColleges, College } from '@/shared/api/colleges.api';
import toast from 'react-hot-toast';

const CollegesPage: React.FC = () => {
  const [colleges, setColleges] = useState<College[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    fetchColleges();
  }, []);

  const fetchColleges = async () => {
    try {
      const data = await getColleges();
      setColleges(data);
    } catch (error) {
      toast.error('Не вдалося завантажити список коледжів');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className={styles.container}>
        <div className={styles.loading}>
          <div className={styles.spinner}></div>
          <p>Завантаження списку коледжів...</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <h1>Навчальні заклади</h1>
        <p>Учасники всеукраїнських змагань Polytech League</p>
      </header>

      {colleges.length === 0 ? (
        <div className={styles.emptyState}>
          <i className="fas fa-university"></i>
          <h3>Список порожній</h3>
          <p>Наразі жодного коледжу не додано до системи.</p>
        </div>
      ) : (
        <div className={styles.grid}>
          {colleges.map(college => (
            <div key={college.id} className={styles.card}>
              <div className={styles.logoContainer}>
                {college.logoUrl ? (
                  <img src={college.logoUrl} alt={college.name} />
                ) : (
                  <i className="fas fa-university"></i>
                )}
              </div>
              <div className={styles.info}>
                <span className={styles.shortName}>{college.shortName}</span>
                <h3 className={styles.fullName}>{college.name}</h3>
                <div className={styles.city}>
                  <i className="fas fa-map-marker-alt"></i>
                  <span>{college.city || 'Місто не вказано'}</span>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default CollegesPage;
