import React from 'react';
import styles from './RecentActivity.module.css';

const RecentActivity = ({ activities, activityChart }) => {
  const maxValue = Math.max(...activityChart.map(item => item.value));
  
  return (
    <div className={`${styles.recentActivity} ${styles.card}`}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-history"></i> Остання активність</h2>
      </div>
      
      <div className={styles.activityList}>
        {activities.map(activity => (
          <div key={activity.id} className={styles.activityItem}>
            <div className={styles.activityIcon}>
              <i className={activity.icon}></i>
            </div>
            <div className={styles.activityContent}>
              <div className={styles.activityTitle}>{activity.title}</div>
              <div className={styles.activityTime}>{activity.time}</div>
            </div>
          </div>
        ))}
      </div>
      
      <div className={styles.activityChart}>
        <div className={styles.chartTitle}>Активність за тиждень</div>
        <div className={styles.chartBars}>
          {activityChart.map((item, index) => (
            <div key={index} className={styles.chartBarContainer}>
              <div 
                className={styles.chartBar}
                style={{ height: `${(item.value / maxValue) * 100}px` }}
              ></div>
              <div className={styles.chartLabel}>{item.day}</div>
            </div>
          ))}
        </div>
        <div className={styles.chartInfo}>
          <i className="fas fa-info-circle"></i> Найактивніший день - четвер
        </div>
      </div>
    </div>
  );
};

export default RecentActivity;