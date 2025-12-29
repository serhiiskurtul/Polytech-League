import React from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './QuickActions.module.css';

const QuickActions = () => {
  const navigate = useNavigate();

  const actions = [
    {
      id: 1,
      icon: 'fa-plus',
      title: 'Створити команду',
      description: 'Зареєструйте нову команду для участі в турнірах',
      color: '#0066cc',
      path: '/teams/create'
    },
    {
      id: 2,
      icon: 'fa-calendar-plus',
      title: 'Записатись на турнір',
      description: 'Оберіть турнір та подайте заявку',
      color: '#10b981',
      path: '/tournaments'
    },
    {
      id: 3,
      icon: 'fa-chart-bar',
      title: 'Моя статистика',
      description: 'Перегляньте ваші результати та прогрес',
      color: '#8b5cf6',
      path: '/profile/stats'
    },
    {
      id: 4,
      icon: 'fa-trophy',
      title: 'Мої досягнення',
      description: 'Всі нагороди та медалі',
      color: '#f59e0b',
      path: '/profile/achievements'
    },
    {
      id: 5,
      icon: 'fa-users',
      title: 'Знайти команду',
      description: 'Шукаєте партнерів для гри?',
      color: '#f43f5e',
      path: '/teams/find'
    },
    {
      id: 6,
      icon: 'fa-calendar-check',
      title: 'Мої події',
      description: 'Майбутні матчі та турніри',
      color: '#3b82f6',
      path: '/my-events'
    }
  ];

  const handleActionClick = (path) => {
    navigate(path);
  };

  return (
    <div className={styles.quickActions}>
      <div className={styles.header}>
        <h3 className={styles.title}>
          <i className="fas fa-bolt"></i>
          Швидкі дії
        </h3>
        <div className={styles.badge}>{actions.length}</div>
      </div>

      <div className={styles.actionsGrid}>
        {actions.map((action) => (
          <button
            key={action.id}
            className={styles.actionButton}
            onClick={() => handleActionClick(action.path)}
            style={{ '--action-color': action.color }}
          >
            <div 
              className={styles.actionIcon}
              style={{ backgroundColor: `${action.color}20` }}
            >
              <i className={`fas ${action.icon}`} style={{ color: action.color }}></i>
            </div>
            
            <div className={styles.actionContent}>
              <div className={styles.actionTitle}>{action.title}</div>
              <div className={styles.actionDescription}>{action.description}</div>
            </div>
            
            <div className={styles.actionArrow}>
              <i className="fas fa-chevron-right"></i>
            </div>
          </button>
        ))}
        </div>
    </div>
  );
};

export default QuickActions;