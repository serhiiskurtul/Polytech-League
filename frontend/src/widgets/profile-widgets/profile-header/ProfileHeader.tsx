import styles from './ProfileHeader.module.css';

interface UserData {
  name: string;
  badges: string[];
  bio: string;
  studentId: string;
  faculty: string;
  email: string;
  phone: string;
  city: string;
  joinDate: string;
}

interface ProfileHeaderProps {
  userData: UserData;
  onEdit?: () => void;
  onAvatarUpload?: () => void;
}

const ProfileHeader = ({ userData, onEdit, onAvatarUpload }: ProfileHeaderProps) => {
  return (
    <div className={styles.profileHeader}>
      <div className={styles.profileHeaderCard}>
        <div className={styles.profileAvatarSection}>
          <div className={styles.profileAvatar}>
            АІ
            <div className={styles.avatarStatus}></div>
          </div>
          <button className={styles.avatarUpload} onClick={onAvatarUpload}>
            <i className="fas fa-camera"></i> Змінити фото
          </button>
        </div>
        
        <div className={styles.profileInfo}>
          <div className={styles.profileNameRow}>
            <div>
              <h1 className={styles.profileName}>{userData.name}</h1>
              <div className={styles.profileBadges}>
                {userData.badges.map((badge, index) => (
                  <span key={index} className={`${styles.badge} ${
                    index === 0 ? styles.badgePrimary : 
                    index === 1 ? styles.badgeSuccess : 
                    styles.badgeAccent
                  }`}>
                    {badge}
                  </span>
                ))}
              </div>
            </div>
            <button className={styles.editProfileBtn} onClick={onEdit}>
              <i className="fas fa-edit"></i> Редагувати профіль
            </button>
          </div>
          
          <p className={styles.profileBio}>
            {userData.bio}
          </p>
          
          <div className={styles.profileDetails}>
            <div className={styles.detailItem}>
              <span className={styles.detailLabel}><i className="fas fa-id-card"></i> Студентський квиток</span>
              <span className={styles.detailValue}>{userData.studentId}</span>
            </div>
            <div className={styles.detailItem}>
              <span className={styles.detailLabel}><i className="fas fa-graduation-cap"></i> Факультет</span>
              <span className={styles.detailValue}>{userData.faculty}</span>
            </div>
            <div className={styles.detailItem}>
              <span className={styles.detailLabel}><i className="fas fa-envelope"></i> Email</span>
              <span className={styles.detailValue}>{userData.email}</span>
            </div>
            <div className={styles.detailItem}>
              <span className={styles.detailLabel}><i className="fas fa-phone"></i> Телефон</span>
              <span className={styles.detailValue}>{userData.phone}</span>
            </div>
            <div className={styles.detailItem}>
              <span className={styles.detailLabel}><i className="fas fa-map-marker-alt"></i> Місто</span>
              <span className={styles.detailValue}>{userData.city}</span>
            </div>
            <div className={styles.detailItem}>
              <span className={styles.detailLabel}><i className="fas fa-calendar"></i> На платформі з</span>
              <span className={styles.detailValue}>{userData.joinDate}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfileHeader;
