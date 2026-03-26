import { Link } from 'react-router-dom';
import styles from './Footer.module.css';
import Logo from '../../shared/ui/Logo/Logo';

const Footer = () => {

  const sports = [
    'Футзал', 'Баскетбол', 'Волейбол', 'Шахи', 'Шашки',
    'Настільний теніс', 'Бадмінтон', 'Армреслінг'
  ];

  const quickLinks = [
    { name: 'Календар подій', path: '/calendar' },
    { name: 'Результати матчів', path: '/results' },
    { name: 'Рейтинг гравців', path: '/ratings' },
    { name: 'Статистика команд', path: '/teams/stats' },
    { name: 'Правила турнірів', path: '/rules' },
    { name: 'FAQ', path: '/faq' }
  ];

  const socialLinks = [
    { name: 'Telegram', icon: 'fab fa-telegram', url: 'https://t.me/unileague' },
    { name: 'Instagram', icon: 'fab fa-instagram', url: 'https://instagram.com/unileague' },
    { name: 'Facebook', icon: 'fab fa-facebook', url: 'https://facebook.com/unileague' },
    { name: 'YouTube', icon: 'fab fa-youtube', url: 'https://youtube.com/unileague' }
  ];

  const handleNewsletterSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.currentTarget;
    const emailInput = form.elements.namedItem('email') as HTMLInputElement;
    const email = emailInput.value;
    console.log('Підписка на розсилку:', email);
    alert('Дякуємо за підписку на розсилку!');
    form.reset();
  };

  const getSportIcon = (sport: string) => {
    const icons: Record<string, string> = {
      'Футзал': 'fas fa-futbol',
      'Баскетбол': 'fas fa-basketball-ball',
      'Волейбол': 'fas fa-volleyball-ball',
      'Шахи': 'fas fa-chess',
      'Шашки': 'fas fa-chess-board',
      'Настільний теніс': 'fas fa-table-tennis',
      'Бадмінтон': 'fas fa-feather-alt',
      'Армреслінг': 'fas fa-fist-raised',
      'Пляжний волейбол': 'fas fa-umbrella-beach'
    };
    const iconClass = icons[sport] || 'fas fa-trophy';
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
                Організація турнірів, ведення статистики та спілкування спортивної спільноти.
              </p>

              <div className={styles.newsletter}>
                <h4 className={styles.newsletterTitle}>Слідкуйте за оновленнями</h4>
                <form className={styles.newsletterForm} onSubmit={handleNewsletterSubmit}>
                  <input
                    type="email"
                    name="email"
                    placeholder="Ваш email"
                    className={styles.newsletterInput}
                    required
                  />
                  <button type="submit" className={styles.newsletterButton}>
                    <i className="fas fa-paper-plane"></i>
                  </button>
                </form>
              </div>
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
                  <div key={index} className={styles.sportTag}>
                    {getSportIcon(sport)}
                    {sport}
                  </div>
                ))}
              </div>
            </div>

            <div className={styles.footerColumn}>
              <h4 className={styles.footerTitle}>Зв'яжіться з нами</h4>
              <ul className={styles.footerLinks}>
                <li>
                   <a href="#" className={styles.footerLink}>
                    <i className="fas fa-envelope"></i> info@cpfc.edu.ua
                   </a>
                </li>
              </ul>
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
      <div className={styles.footerBottom}>
        <div className={styles.container}>
          <p className={styles.copyright}>&copy; 2025 Polytech League. Всі права захищені.</p>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
