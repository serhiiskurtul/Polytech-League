import './500.css';
import { useNavigate } from "react-router-dom";

export default function InternalServerError() {

  const navigate = useNavigate();

  return (
    <div className="spacer-500">
      <div className="card-500">
        <h1 style={{ margin: 0 }}>500</h1>
        <p style={{ color: "var(--muted)" }}>Внутрішня помилка сервера</p>

        <button 
          className="btn-500" 
          onClick={() => location.reload()}
        >
          Оновити сторінку
        </button>
      </div>
    </div>
  );
}
