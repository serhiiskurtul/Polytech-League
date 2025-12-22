
import './TournamentFilter.css';
import FilterInput from "../../shared/FilterInput/FilterInput.jsx";

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