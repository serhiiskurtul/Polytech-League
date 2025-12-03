import './GlobalStatisticSeson.css';

export default function GlobalStatisticSeson({ activePlayers, tournaments, matchesPlayed }) {
  return (
    <div className="card">
        <div class="header-row"><h3 style = {{marginBottom: "1px"}}>Статистика сезону</h3></div>
            <div class="stats">
            <div class="stat">
                <div class="stat-num">428</div>
                <div>активних гравців</div>
            </div>
            <div class="stat">
                <div class="stat-num">18</div>
                <div>турнірів</div>
            </div>
            <div class="stat">
                <div class="stat-num">1 247</div>
                <div>зіграно матчів</div>
            </div>
            <div class="stat">
                <div class="stat-num">8</div>
                <div>видів спорту</div>
            </div>
        </div>
    </div>
  );
}
