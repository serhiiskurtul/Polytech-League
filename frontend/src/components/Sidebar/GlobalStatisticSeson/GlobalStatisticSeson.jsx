import './GlobalStatisticSeson.css';

export default function GlobalStatisticSeson({ activePlayers, tournaments, matchesPlayed }) {
  return (
    <div className="card">
        <div className="header-row"><h3 style = {{marginBottom: "1px"}}>Статистика сезону</h3></div>
            <div className="stats">
            <div className="stat">
                <div className="stat-num">428</div>
                <div>активних гравців</div>
            </div>
            <div className="stat">
                <div className="stat-num">18</div>
                <div>турнірів</div>
            </div>
            <div className="stat">
                <div className="stat-num">1 247</div>
                <div>зіграно матчів</div>
            </div>
            <div className="stat">
                <div className="stat-num">8</div>
                <div>видів спорту</div>
            </div>
        </div>
    </div>
  );
}
