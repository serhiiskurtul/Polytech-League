import { Link } from 'react-router-dom';
import styles from './TournamentsListItem.module.css';
import SimpleButton from '../../shared/SimpleButton/SimpleButton.jsx';

export default function TournamentItem({ title, teams, status, id, date }) {
  const statusConfig = {
    ongoing: { text: 'Триває', class: styles.statusOngoing },
    upcoming: { text: 'Заплановано', class: styles.statusUpcoming },
    finished: { text: 'Завершено', class: styles.statusFinished },
  };

  const config = statusConfig[status] || statusConfig.finished;

  return (
    <Link to={`/tournament/${id}`} className={styles.tournamentLink}>
      <section className={`${styles.tournamentItem} standard-hover-animation`}>
        <div className={styles.tournamentInfo}>
          <div className={styles.tournamentTitle}>{title}</div>

          <div className={styles.tournamentDetails}>
            <span className={styles.tournamentDate}>📅 {date}</span> • Команд:{' '}
            <strong>{teams}</strong> •{' '}
            <span className={`${styles.status} ${config.class}`}>
              {config.text}
            </span>
          </div>
        </div>

        <SimpleButton text="Переглянути" insideLink />
      </section>
    </Link>
  );
}
