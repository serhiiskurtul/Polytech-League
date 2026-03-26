import styles from './SidebarActions.module.css';

interface SidebarActionsProps {
  onActionClick: (text: string) => void;
}

const SidebarActions = ({ onActionClick }: SidebarActionsProps) => {
  const actions = [
    { icon: 'fas fa-plus', text: 'Створити команду' },
    { icon: 'fas fa-calendar-plus', text: 'Записатись на турнір' },
    { icon: 'fas fa-chart-line', text: 'Переглянути статистику' },
    { icon: 'fas fa-cog', text: 'Налаштування профілю' },
    { icon: 'fas fa-share-alt', text: 'Поділитись профілем' }
  ];

  return (
    <section className={styles.card}>
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
            <span>{action.text}</span>
          </button>
        ))}
      </div>
    </section>
  );
};

export default SidebarActions;
