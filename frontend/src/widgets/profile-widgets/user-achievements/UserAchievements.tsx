import { Link } from 'react-router-dom';
import styles from './UserAchievements.module.css';

interface Achievement {
  tournamentId: number;
  tournamentName: string;
  sportType: string;
  result?: string;
  date: string;
  rank?: number;
}

interface UserAchievementsProps {
  achievements: Achievement[];
}

const UserAchievements = ({ achievements }: UserAchievementsProps) => {
  const getSportIcon = (sportType: string) => {
    switch (sportType.toLowerCase()) {
      case 'chess': return 'fas fa-chess';
      case 'checkers': return 'fas fa-square';
      case 'futsal': return 'fas fa-futbol';
      case 'volleyball':
      case 'beach_volleyball': return 'fas fa-volleyball-ball';
      case 'basketball_3x3': return 'fas fa-basketball-ball';
      case 'badminton': return 'fas fa-shuttlecock';
      case 'table_tennis': return 'fas fa-table-tennis';
      case 'armwrestling': return 'fas fa-fist-raised';
      default: return 'fas fa-medal';
    }
  };

  const getRankBadgeClass = (rank?: number) => {
    if (rank === 1) return styles.gold;
    if (rank === 2) return styles.silver;
    if (rank === 3) return styles.bronze;
    return '';
  };

  return (
    <section className={styles.card}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-trophy"></i> Досягнення та виступи</h2>
        <span className={styles.count}>{achievements.length}</span>
      </div>
      
      {achievements.length === 0 ? (
        <div className={styles.emptyState}>
          <p>Немає записів про участь у турнірах</p>
        </div>
      ) : (
        <div className={styles.achievementsGrid}>
          {achievements.map((achievement, index) => (
            <Link 
              key={`${achievement.tournamentId}-${index}`} 
              to={`/tournaments/${achievement.tournamentId}`}
              className={`${styles.achievementCard} ${getRankBadgeClass(achievement.rank)}`}
            >
              <div className={styles.iconContainer}>
                <div className={styles.achievementIcon}>
                  <i className={getSportIcon(achievement.sportType)}></i>
                </div>
                {achievement.rank !== undefined && achievement.rank > 0 && (
                  <div className={styles.rankBadge}>
                    <span className={styles.rankNumber}>{achievement.rank}</span>
                    <span className={styles.rankLabel}>місце</span>
                  </div>
                )}
              </div>
              <div className={styles.achievementInfo}>
                <div className={styles.achievementName}>{achievement.tournamentName}</div>
                <div className={styles.achievementResult}>
                  {achievement.result?.replace(`${achievement.rank} місце `, '').replace(`${achievement.rank} місце`, '') || 'Учасник'}
                </div>
                <div className={styles.achievementDate}>
                  {new Date(achievement.date).toLocaleDateString('uk-UA', { year: 'numeric', month: 'short' })}
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}
    </section>
  );
};

export default UserAchievements;
