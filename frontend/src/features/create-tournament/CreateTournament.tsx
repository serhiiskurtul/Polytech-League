import styles from './CreateTournament.module.css';
import SimpleButton from '../../shared/ui/SimpleButton/SimpleButton';

export default function CreateTournament() {
  return (
    <div className="card">
      <div className={`header-row ${styles.headerRowCreateTournament}`}>
        <h3>Створити турнір</h3>
      </div>

      <SimpleButton
        to="/create-tournament"
        text="Створити"
        fullWidth
      />
    </div>
  );
}
