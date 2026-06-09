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
  goalsCount: number;
  yellowCardsCount?: number;
  redCardsCount?: number;
  globalRank?: number;
  hatTricksCount?: number;
  pokersCount?: number;
  pentatricksCount?: number;
  sportStats?: SportStat[];
  chessTotalPoints?: number | null;
  chessTotalPossiblePoints?: number | null;
  isChessUndefeatedFourSeasons?: boolean;
  footballGoalMatches?: Array<{
    matchId: number;
    tournamentId: number;
    tournamentName: string;
    homeTeamName: string;
    awayTeamName: string;
    goalsScored: number;
    date: string;
  }>;
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
    case 'athletics': return 'fa-running';
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
    case 'basketball_3x3': return 'Баскетбол';
    case 'beach_volleyball': return 'Пляжний волейбол';
    case 'athletics': return 'Легка атлетика';
    default: return type;
  }
};

const ProfileStats = ({ stats }: ProfileStatsProps) => {
  const isFootballer = stats.sportStats?.some(s => 
    s.sportType.toLowerCase() === 'football' || 
    s.sportType.toLowerCase() === 'футбол' ||
    s.sportType.toLowerCase() === 'futsal'
  ) || 
  (stats.goalsCount !== undefined && stats.goalsCount > 0) ||
  (stats.yellowCardsCount !== undefined && stats.yellowCardsCount > 0) ||
  (stats.redCardsCount !== undefined && stats.redCardsCount > 0);

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
        {(isFootballer || (stats.goalsCount !== undefined && stats.goalsCount > 0)) && (
          <div className={styles.statCard} title="Кількість забитих голів у футбольних матчах">
            <div className={styles.statValue}>{stats.goalsCount || 0}</div>
            <div className={styles.statLabel}>Забитих голів</div>
          </div>
        )}
        {(isFootballer || (stats.hatTricksCount !== undefined && stats.hatTricksCount > 0)) && (
          <div className={styles.statCard} title="Кількість забитих хет-триків у футбольних матчах (3+ голи за матч)">
            <div className={styles.statValue}>{stats.hatTricksCount || 0}</div>
            <div className={styles.statLabel}>Хет-триків</div>
          </div>
        )}
        {(isFootballer || (stats.pokersCount !== undefined && stats.pokersCount > 0)) && (
          <div className={styles.statCard} title="Кількість забитих покерів у футбольних матчах (4+ голи за матч)">
            <div className={styles.statValue}>{stats.pokersCount || 0}</div>
            <div className={styles.statLabel}>Покерів</div>
          </div>
        )}
        {(isFootballer || (stats.pentatricksCount !== undefined && stats.pentatricksCount > 0)) && (
          <div className={styles.statCard} title="Кількість забитих пентатриків у футбольних матчах (5+ голів за матч)">
            <div className={styles.statValue}>{stats.pentatricksCount || 0}</div>
            <div className={styles.statLabel}>Пентатриків</div>
          </div>
        )}
        {(isFootballer || (stats.yellowCardsCount !== undefined && stats.yellowCardsCount > 0)) && (
          <div className={`${styles.statCard} ${styles.noHover}`} title="Жовті картки отримані у футбольних матчах">
            <div className={`${styles.statValue} ${styles.yellowCardValue}`}>
              {stats.yellowCardsCount ?? stats.sportStats?.find(s => s.sportType.toLowerCase() === 'football')?.yellowCardsCount ?? 0}
            </div>
            <div className={styles.statLabel}>Жовті картки</div>
          </div>
        )}
        {(isFootballer || (stats.redCardsCount !== undefined && stats.redCardsCount > 0)) && (
          <div className={`${styles.statCard} ${styles.noHover}`} title="Червоні картки отримані у футбольних матчах">
            <div className={`${styles.statValue} ${styles.redCardValue}`}>
              {stats.redCardsCount ?? stats.sportStats?.find(s => s.sportType.toLowerCase() === 'football')?.redCardsCount ?? 0}
            </div>
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

      {stats.footballGoalMatches && stats.footballGoalMatches.length > 0 && (
        <div className={styles.scoringHistory}>
          <h3 className={styles.subTitle}><i className="fas fa-futbol"></i> Бомбардирська історія</h3>
          <div className={styles.matchesList}>
            {stats.footballGoalMatches.map((match, index) => (
              <div key={index} className={styles.matchItem}>
                <div className={styles.matchDate}>
                  {new Date(match.date).toLocaleDateString('uk-UA', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                </div>
                <div className={styles.matchInfo}>
                  <div className={styles.tournamentName}>{match.tournamentName}</div>
                  <div className={styles.teams}>
                    {match.homeTeamName} <span className={styles.vs}>vs</span> {match.awayTeamName}
                  </div>
                </div>
                <div className={styles.goalsScored}>
                  <span className={styles.goalCount}>{match.goalsScored}</span>
                  <i className="fas fa-futbol"></i>
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
