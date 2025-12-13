import FilterInput from '../../../UI/FilterInput/FilterInput';
import './TournamentFilter.css';

export default function TournamentFilter() {
  return (
    <div className="card" >
        <div className="header-row header-row-tournament-filter">
            <h3>Фільтри</h3>
        </div>
        <FilterInput />
    </div>
  );
}