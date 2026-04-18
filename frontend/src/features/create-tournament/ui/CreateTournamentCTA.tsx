import styles from '../CreateTournament.module.css';
import { SimpleButton } from '@/shared/ui';

export default function CreateTournamentCTA() {
  return (
    <div className="card">
      <div className={`header-row ${styles.headerRowCreateTournament}`}>
        <h3>Створити змагання</h3>
      </div>

      <SimpleButton
        to="/create-tournament"
        text="Створити"
      />
    </div>
  );
}
