
import './CreateTournament.css';
import SimpleButton from "../../shared/SimpleButton/SimpleButton.jsx";

export default function CreateTournament() {
  return (
    <div className="card">
      <div className="header-row header-row-create-tournament">
        <h3>Створити турнір</h3>
      </div>
      <SimpleButton to="/create-tournament" text="Створити" fullWidth />
    </div>
  );
}