import styles from './TournamentFilter.module.css';
import FilterInput from '../../shared/FilterInput/FilterInput.jsx';

export default function TournamentFilter() {
  return (
    <div className="card">
      <div className={`header-row ${styles.headerRowTournamentFilter}`}>
        <h3>Фільтри</h3>
      </div>

      <FilterInput />
    </div>
  );
}
