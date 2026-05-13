import styles from './ProfileStats.module.css';

interface SportStat {
  sportType: string;
  rank: number;
  performanceScore: number;
  goalsCount?: number;
  yellowCardsCount?: number;
  redCardsCount?: number;
}

interface Stats {
  matchesPlayed: number;
  wins: number;
  competitionsCount: number;
  eloRating: number;
  goalsScored: number;
  yellowCards?: number;
  redCards?: number;
  globalRank?: number;
  sportStats?: SportStat[];
  chessTotalPoints?: number | null;
  chessTotalPossiblePoints?: number | null;
  isChessUndefeatedFourSeasons?: boolean;
}

interface ProfileStatsProps {
  stats: Stats;
}

const getSportIcon = (type: string) => {
  switch (type.toLowerCase()) {
    case 'chess': return 'fa-chess';
    case 'checkers': return 'fa-dice';
    case 'table_tennis': return 'fa-table-tennis';
    case 'armwrestling': return 'fa-hand-rock';
    case 'badminton': return 'fa-shuttlecock';
    case 'football': return 'fa-futbol';
    case 'volleyball': return 'fa-volleyball-ball';
    case 'basketball_3x3': return 'fa-basketball-ball';
    case 'beach_volleyball': return 'fa-umbrella-beach';
    default: return 'fa-medal';
  }
};

const getSportName = (type: string) => {
  switch (type.toLowerCase()) {
    case 'chess': return 'Шахи';
    case 'checkers': return 'Шашки';
    case 'table_tennis': return 'Настільний теніс';
    case 'armwrestling': return 'Армрестлинг';
    case 'badminton': return 'Бадмінтон';
    case 'football': return 'Футбол';
    case 'volleyball': return 'Волейбол';
    case 'basketball_3x3': return 'Баскетбол 3x3';
    case 'beach_volleyball': return 'Пляжний волейбол';
    default: return type;
  }
};

const ProfileStats = ({ stats }: ProfileStatsProps) => {
  const isFootballer = stats.sportStats?.some(s => s.sportType.toLowerCase() === 'football') || 
                       (stats.goalsScored !== undefined && stats.goalsScored > 0) ||
                       (stats.yellowCards !== undefined && stats.yellowCards > 0) ||
                       (stats.redCards !== undefined && stats.redCards > 0);

  return (
    <section className={styles.card}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-chart-bar"></i> Статистика</h2>
      </div>

      <div className={styles.statsGrid}>
        <div className={styles.statCard} title="Загальна кількість зіграних офіційних матчів">
          <div className={styles.statValue}>{stats.matchesPlayed}</div>
          <div className={styles.statLabel}>Зіграних матчів</div>
        </div>
        <div className={styles.statCard} title="Загальна кількість перемог">
          <div className={styles.statValue}>{stats.wins}</div>
          <div className={styles.statLabel}>Перемог</div>
        </div>
        <div className={styles.statCard} title="Кількість турнірів, у яких брав участь гравець">
          <div className={styles.statValue}>{stats.competitionsCount}</div>
          <div className={styles.statLabel}>Змагань</div>
        </div>
        {stats.eloRating > 0 && (
          <div className={styles.statCard}>
            <div className={styles.statValue}>{stats.eloRating}</div>
            <div className={styles.statLabel}>Шаховий рейтинг Elo</div>
          </div>
        )}
        {(isFootballer || (stats.goalsScored !== undefined && stats.goalsScored > 0)) && (
          <div className={styles.statCard} title="Загальна кількість забитих голів">
            <div className={styles.statValue}>{stats.goalsScored || 0}</div>
            <div className={styles.statLabel}>Забитих голів</div>
          </div>
        )}
        {(isFootballer || (stats.yellowCards !== undefined && stats.yellowCards > 0)) && (
          <div className={styles.statCard} title="Жовті картки отримані у футбольних матчах">
            <div className={`${styles.statValue} ${styles.yellowCardValue}`}>{stats.yellowCards || 0}</div>
            <div className={styles.statLabel}>Жовті картки</div>
          </div>
        )}
        {(isFootballer || (stats.redCards !== undefined && stats.redCards > 0)) && (
          <div className={styles.statCard} title="Червоні картки отримані у футбольних матчах">
            <div className={`${styles.statValue} ${styles.redCardValue}`}>{stats.redCards || 0}</div>
            <div className={styles.statLabel}>Червоні картки</div>
          </div>
        )}
        {stats.chessTotalPoints !== null && stats.chessTotalPoints !== undefined && (
          <div 
            className={`${styles.statCard} ${styles.hasTooltip}`} 
            data-tooltip="Загальна кількість набраних очок у міжгрупових змаганнях з шахів"
          >
            <div className={styles.statValue}>
              {stats.chessTotalPoints}
              {stats.chessTotalPossiblePoints !== null && stats.chessTotalPossiblePoints !== undefined && `/${stats.chessTotalPossiblePoints}`}
            </div>
            <div className={styles.statLabel}>
              Очки в міжгрупових шахах <i className="fas fa-info-circle"></i>
            </div>
          </div>
        )}
        {stats.isChessUndefeatedFourSeasons && (
          <div 
            className={`${styles.statCard} ${styles.undefeatedCard} ${styles.hasTooltip}`} 
            data-tooltip="Не програв жодної партії протягом 4 сезонів"
          >
            <div className={styles.statValue}><i className="fas fa-shield-alt"></i></div>
            <div className={styles.statLabel}>Непереможний (4 сезони)</div>
          </div>
        )}
      </div>

      {((stats.globalRank && stats.globalRank > 0) || (stats.sportStats && stats.sportStats.length > 0)) && (
        <div className={styles.sportRankings}>
          <h3 className={styles.subTitle}><i className="fas fa-trophy"></i> Місце в рейтингах</h3>
          <div className={styles.sportGrid}>
            {stats.globalRank && stats.globalRank > 0 && (
              <div className={`${styles.sportItem} ${styles.globalRankItem}`}>
                <div className={styles.sportInfo}>
                  <i className="fas fa-globe"></i>
                  <span>Загальний рейтинг</span>
                </div>
                <div className={styles.sportRank}>
                  <span className={styles.rankNumber}>#{stats.globalRank}</span>
                </div>
              </div>
            )}
            {stats.sportStats?.map((sport, index) => (
              <div key={index} className={styles.sportItem}>
                <div className={styles.sportInfo}>
                  <i className={`fas ${getSportIcon(sport.sportType)}`}></i>
                  <span>{getSportName(sport.sportType)}</span>
                </div>
                <div className={styles.sportRank}>
                  <span className={styles.rankNumber}>#{sport.rank}</span>
                  <span className={styles.score}>{sport.performanceScore} pts</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </section>
  );
};

export default ProfileStats;
