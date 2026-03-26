import { Link } from 'react-router-dom';
import styles from './TournamentsListItem.module.css';
import SimpleButton from '../../shared/ui/SimpleButton/SimpleButton';

export type TournamentStatus = 'ongoing' | 'upcoming' | 'finished';

interface TournamentItemProps {
  title: string;
  teams: number;
  status: TournamentStatus;
  id: string;
  date: string;
}

const STATUS_CONFIG: Record<
  TournamentStatus,
  { text: string; className: string }
> = {
  ongoing: { text: 'Триває', className: styles.statusOngoing },
  upcoming: { text: 'Заплановано', className: styles.statusUpcoming },
  finished: { text: 'Завершено', className: styles.statusFinished },
};

export default function TournamentItem({
  title,
  teams,
  status,
  id,
  date,
}: TournamentItemProps) {
  const { text, className } = STATUS_CONFIG[status];

  return (
    <Link to={`/tournament/${id}`} className={styles.tournamentLink}>
      <section className={`${styles.tournamentItem} standard-hover-animation`}>
        <div className={styles.tournamentInfo}>
          <div className={styles.tournamentTitle}>{title}</div>

          <div className={styles.tournamentDetails}>
            <span className={styles.tournamentDate}>📅 {date}</span> • Команд:{' '}
            <strong>{teams}</strong> •{' '}
            <span className={`${styles.status} ${className}`}>
              {text}
            </span>
          </div>
        </div>

        <SimpleButton text="Переглянути" to={`/tournament/${id}`} />
      </section>
    </Link>
  );
}