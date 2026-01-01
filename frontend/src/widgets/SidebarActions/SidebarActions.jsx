import React from 'react';
import styles from './SidebarActions.module.css';

const SidebarActions = ({ onActionClick }) => {
  const actions = [
    { icon: 'fas fa-plus', text: 'Створити команду' },
    { icon: 'fas fa-calendar-plus', text: 'Записатись на турнір' },
    { icon: 'fas fa-chart-line', text: 'Переглянути статистику' },
    { icon: 'fas fa-cog', text: 'Налаштування профілю' },
    { icon: 'fas fa-share-alt', text: 'Поділитись профілем' }
  ];

  return (
    <div className={`${styles.sidebarActions} ${styles.card}`}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-bolt"></i> Швидкі дії</h2>
      </div>
      
      <div className={styles.quickActions}>
        {actions.map((action, index) => (
          <button
            key={index}
            className={styles.actionBtn}
            onClick={() => onActionClick(action.text)}
          >
            <i className={action.icon}></i>
            {action.text}
          </button>
        ))}
      </div>
    </div>
  );
};

export default SidebarActions;