import './Header.css';

export default function Header() {
  return (
    <header>
      <div className="container topbar">
        <div className="logo-icon">
          <div className="field">
            <div className="center-circle"></div>
            <div className="center-line"></div>
          </div>
        </div>
        <div className="logo-text">Campus League</div>
      </div>
      <nav>
        <ul>
          <li><a href="#match">Календар</a></li>
          <li><a href="#profile">Турніри</a></li>
          <li><a href="#teams">Команди</a></li>
          <li><a href="#ratings">Рейтинги</a></li>
        </ul>
      </nav>
      <nav>
        <ul>
          <li><a href="#home">Сповіщення</a></li>
          <li><a href="#match">Тема</a></li>
          <li><a href="#profile">Аватар</a></li>
        </ul>
      </nav>
    </header>
  );
}