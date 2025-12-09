import { Routes, Route, Link } from "react-router-dom";
import './Header.css';

export default function Header() {
  return (
    <header>
      <Link to="/" className="logo-link">
        <div className="container-header">
          <div className="logo-icon">
            <div className="field">
              <div className="center-circle"></div>
              <div className="center-line"></div>
            </div>
          </div>
          <div className="logo-text">Campus League</div>
        </div>
      </Link>
      <nav>
        <ul>
          <li><Link to="/match">Календар</Link></li>
          <li><Link to="/profile">Турніри</Link></li>
          <li><Link to="/teams">Команди</Link></li>
          <li><Link to="/ratings">Рейтинги</Link></li>
        </ul>
      </nav>
      <nav>
        <ul>
          <li><Link to="/notifications">Сповіщення</Link></li>
          <li><Link to="/theme">Тема</Link></li>
          <li><Link to="/avatar">Аватар</Link></li>
        </ul>
      </nav>
    </header>
  );
}