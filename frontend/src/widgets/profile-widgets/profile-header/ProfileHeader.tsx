import styles from './ProfileHeader.module.css';

interface UserData {
  name: string;
  badges: string[];
  bio: string;
  group: string;
  studentId: string;
  faculty: string;
  phone: string;
  city: string;
  instagramUrl?: string | null;
  photoUrl?: string | null;
  globalRank?: number;
  futsalShirtNumber?: number | null;
  volleyballShirtNumber?: number | null;
  basketballShirtNumber?: number | null;
}


interface ProfileHeaderProps {
  userData: UserData;
  isAdmin?: boolean;
  onEdit?: () => void;
  onAvatarUpload?: () => void;
  onLogout?: () => void;
  onCompare?: () => void;
  onMessage?: () => void;
}

const ProfileHeader = ({ userData, isAdmin, onEdit, onAvatarUpload, onLogout, onCompare, onMessage }: ProfileHeaderProps) => {
  return (
    <div className={styles.profileHeader}>
      <div className={styles.profileHeaderCard}>
        <div className={styles.profileAvatarSection}>
          <div className={styles.profileAvatar}>
            {userData.photoUrl ? (
              <img 
                src={`${import.meta.env.VITE_API_BASE_URL || 'https://polytech-league-production.up.railway.app'}${userData.photoUrl}`} 
                alt={userData.name} 
                className={styles.avatarImage} 
              />
            ) : (
              userData.name.split(' ').map(n => n[0]).join('')
            )}
          </div>
          {onAvatarUpload && (
            <button className={styles.avatarUpload} onClick={onAvatarUpload}>
              <i className="fas fa-camera"></i> Змінити фото
            </button>
          )}
        </div>
        
        <div className={styles.profileInfo}>
          <div className={styles.profileNameRow}>
            <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '15px', flexWrap: 'wrap' }}>
              <h1 className={styles.profileName}>{userData.name}</h1>
            </div>
            <div className={styles.profileBadges}>
              {userData.badges.map((badge, index) => (
                <span key={index} className={`${styles.badge} ${
                  badge === 'Адміністратор' ? styles.badgeAccent :
                  index === 0 ? styles.badgePrimary : 
                  index === 1 ? styles.badgeSuccess : 
                  styles.badgeAccent
                }`}>
                  {badge}
                </span>
              ))}
            </div>
            </div>
            <div className={styles.socialLinks}>
              {userData.instagramUrl && (
                <a href={userData.instagramUrl} target="_blank" rel="noopener noreferrer" className={styles.socialIcon} title="Instagram">
                  <i className="fab fa-instagram"></i>
                </a>
              )}
            </div>
            <div className={styles.profileActions}>
              {onEdit && (
                <button className={styles.editProfileBtn} onClick={onEdit}>
                  <i className="fas fa-edit"></i> Редагувати профіль
                </button>
              )}
              {!isAdmin && onCompare && (
                <button className={styles.compareBtn} onClick={onCompare}>
                  <i className="fas fa-arrows-left-right"></i> Порівняти
                </button>
              )}
              {onMessage && (
                <button className={styles.messageBtn} onClick={onMessage}>
                  <i className="fas fa-comment-dots"></i> Написати
                </button>
              )}
              {onLogout && (
                <button className={styles.logoutBtn} onClick={onLogout}>
                  <i className="fas fa-sign-out-alt"></i> Вийти
                </button>
              )}
            </div>
          </div>
          
          <p className={styles.profileBio}>
            {userData.bio}
          </p>
          
          {!isAdmin && (
            <div className={styles.profileDetails}>
              <div className={styles.detailItem}>
                <span className={styles.detailLabel}><i className="fas fa-users-rectangle"></i> Група</span>
                <span className={styles.detailValue}>{userData.group}</span>
              </div>
              <div className={styles.detailItem}>
                <span className={styles.detailLabel}><i className="fas fa-id-card"></i> Студ. квиток</span>
                <span className={styles.detailValue}>{userData.studentId}</span>
              </div>
              <div className={styles.detailItem}>
                <span className={styles.detailLabel}><i className="fas fa-graduation-cap"></i> Факультет</span>
                <span className={styles.detailValue}>{userData.faculty}</span>
              </div>
              <div className={styles.detailItem}>
                <span className={styles.detailLabel}><i className="fas fa-map-marker-alt"></i> Місто</span>
                <span className={styles.detailValue}>{userData.city}</span>
              </div>
              {userData.futsalShirtNumber !== null && userData.futsalShirtNumber !== undefined && (
                <div className={styles.detailItem}>
                  <span className={styles.detailLabel}><i className="fas fa-tshirt" style={{ color: '#e11d48' }}></i> № Футзал</span>
                  <span className={styles.detailValue} style={{ fontWeight: 'bold' }}>{userData.futsalShirtNumber}</span>
                </div>
              )}
              {userData.volleyballShirtNumber !== null && userData.volleyballShirtNumber !== undefined && (
                <div className={styles.detailItem}>
                  <span className={styles.detailLabel}><i className="fas fa-tshirt" style={{ color: '#2563eb' }}></i> № Волейбол</span>
                  <span className={styles.detailValue} style={{ fontWeight: 'bold' }}>{userData.volleyballShirtNumber}</span>
                </div>
              )}
              {userData.basketballShirtNumber !== null && userData.basketballShirtNumber !== undefined && (
                <div className={styles.detailItem}>
                  <span className={styles.detailLabel}><i className="fas fa-tshirt" style={{ color: '#f59e0b' }}></i> № Баскетбол</span>
                  <span className={styles.detailValue} style={{ fontWeight: 'bold' }}>{userData.basketballShirtNumber}</span>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};


export default ProfileHeader;
