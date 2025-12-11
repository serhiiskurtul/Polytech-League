import './404.css';
import { useNavigate } from "react-router-dom";

export default function NotFound() {

  const navigate = useNavigate();

  return (
    <div className="spacer-404">
      <div className="card-404">
        <h1 style={{ margin: 0 }}>404</h1>
        <p style={{ color: "var(--muted)" }}>Сторінку не знайдено</p>

        <button 
          className="btn-404" 
          onClick={() => navigate('/')}
        >
          На головну
        </button>
      </div>
    </div>
  );
}
