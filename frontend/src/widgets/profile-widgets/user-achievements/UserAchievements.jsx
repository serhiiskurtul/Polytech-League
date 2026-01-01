import React from 'react';
import styles from './UserAchievements.module.css';

const UserAchievements = ({ achievements }) => {
  return (
    <section className={styles.card}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-trophy"></i> Досягнення</h2>
      </div>
      
      <div className={styles.achievementsGrid}>
        {achievements.map(achievement => (
          <div key={achievement.id} className={styles.achievementCard}>
            <div className={styles.achievementIcon}>
              <i className={achievement.icon}></i>
            </div>
            <div className={styles.achievementName}>{achievement.name}</div>
            <div className={styles.achievementDate}>{achievement.year}</div>
          </div>
        ))}
      </div>
    </section>
  );
};

export default UserAchievements;