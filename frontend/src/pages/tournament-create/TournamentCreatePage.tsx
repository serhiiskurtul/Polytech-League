import styles from './TournamentCreatePage.module.css';
import { TournamentCreateForm } from '@/features/create-tournament';

const TournamentCreatePage = () => {
  return (
    <div className={styles.tournamentCreatePage}>
      <div className={styles.container}>
        <TournamentCreateForm />
      </div>
    </div>
  );
};

export default TournamentCreatePage;
