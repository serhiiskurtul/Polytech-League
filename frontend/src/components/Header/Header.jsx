import { Routes, Route, Link } from "react-router-dom";
import './Header.css';
import Logo from "../UI/Logo/Logo.jsx";

export default function Header() {
  return (
    <header>
      <div className="container-header">
        <Logo />
        <div className="logo-text">Campus League</div>
      </div>
      <nav>
        <ul>
          <li><Link to="/calendar">Календар</Link></li>
          <li><Link to="/tournaments">Турніри</Link></li>
          <li><Link to="/teams">Команди</Link></li>
          <li><Link to="/ratings">Рейтинги</Link></li>
        </ul>
      </nav>
      <nav>
        <ul>
          <li><Link to="/notifications">Сповіщення</Link></li>
          <li><Link to="/login">Увійти</Link></li>
        </ul>
      </nav>
    </header>
  );
}