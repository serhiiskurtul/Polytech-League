import { Link } from 'react-router-dom';
import styles from './Footer.module.css';
import { Logo } from '@/shared/ui';

const Footer = () => {

  const sports = [
    { id: 'football', label: 'Футбол' },
    { id: 'basketball-3x3', label: 'Баскетбол 3х3' },
    { id: 'volleyball', label: 'Волейбол' },
    { id: 'beach-volleyball', label: 'Пляжний волейбол' },
    { id: 'chess', label: 'Шахи' },
    { id: 'checkers', label: 'Шашки' },
    { id: 'table-tennis', label: 'Настільний теніс' },
    { id: 'badminton', label: 'Бадмінтон' },
    { id: 'armwrestling', label: 'Армреслінг' },
    { id: 'athletics', label: 'Легка атлетика' }
  ];

  const quickLinks = [
    { name: 'Календар подій', path: '/calendar' },
    { name: 'Рейтинг гравців', path: '/ratings' },
    { name: 'Список змагань', path: '/tournaments' },
    { name: 'Про проєкт', path: '/about' }
  ];

  const socialLinks = [
    { name: 'Instagram', icon: 'fab fa-instagram', url: 'https://www.instagram.com/sportclubpolytech/' }
  ];

  const getSportIcon = (sportId: string) => {
    const icons: Record<string, string> = {
      'football': 'fas fa-futbol',
      'basketball-3x3': 'fas fa-basketball-ball',
      'volleyball': 'fas fa-volleyball-ball',
      'chess': 'fas fa-chess',
      'checkers': 'fas fa-chess-board',
      'table-tennis': 'fas fa-table-tennis',
      'badminton': 'fas fa-feather-alt',
      'armwrestling': 'fas fa-fist-raised',
      'beach-volleyball': 'fas fa-umbrella-beach',
      'athletics': 'fas fa-running'
    };
    const iconClass = icons[sportId] || 'fas fa-trophy';
    return <i className={iconClass} style={{ marginRight: '8px' }}></i>;
  };

  return (
    <footer className={styles.footer}>
      <div className={styles.footerTop}>
        <div className={styles.container}>
          <div className={styles.footerGrid}>
            <div className={styles.footerColumn}>
              <div className={styles.footerLogo}>
                <Logo />
                <span className={styles.logoText}>Polytech League</span>
              </div>
              <p className={styles.footerDescription}>
                Спортивна платформа для студентів та викладачів Чернівецького політехнічного фахового коледжу.
                Організація змагань, ведення статистики та спілкування спортивної спільноти.
              </p>
            </div>

            <div className={styles.footerColumn}>
              <h4 className={styles.footerTitle}>Швидкий доступ</h4>
              <ul className={styles.footerLinks}>
                {quickLinks.map((link, index) => (
                  <li key={index}>
                    <Link to={link.path} className={styles.footerLink}>
                      <i className={`fas fa-chevron-right ${styles.linkIcon}`}></i>
                      {link.name}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>

            <div className={styles.footerColumn}>
              <h4 className={styles.footerTitle}>Спортивні дисципліни</h4>
              <div className={styles.sportsGrid}>
                {sports.map((sport, index) => (
                  <Link 
                    key={index} 
                    to={`/tournaments?sport=${sport.id}`} 
                    className={styles.sportTag}
                    style={{ textDecoration: 'none', color: 'inherit' }}
                  >
                    {getSportIcon(sport.id)}
                    {sport.label}
                  </Link>
                ))}
              </div>
            </div>

            <div className={styles.footerColumn}>
              <h4 className={styles.footerTitle}>Зв'яжіться з нами</h4>
              <div className={styles.socialLinks}>
                <div className={styles.socialIcons}>
                  {socialLinks.map((social, index) => (
                    <a key={index} href={social.url} className={styles.socialIcon} title={social.name} target="_blank" rel="noopener noreferrer">
                      <i className={social.icon}></i>
                    </a>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;

