import styles from './HomePage.module.css';
import TopPlayersCompact from '../../widgets/top-players-compact/TopPlayersCompact';
import SeasonStats from '../../widgets/season-stats/SeasonStats';
import HeroSection from '../../widgets/hero-section/HeroSection';
import TournamentCalendar from '../../widgets/tournament-calendar/TournamentCalendar';
import RecentMatches from '../../widgets/recent-matches/RecentMatches';
import QuickActions from '../../widgets/quick-actions/QuickActions';

export default function HomePage() {
  return (
    <>
      <HeroSection />
      <div className={styles.homePage}> 
        <main className={styles.main}>
          <TournamentCalendar />
          <RecentMatches />
        </main>

        <aside className={styles.sidebar}>
          <QuickActions />
          <TopPlayersCompact />
          <SeasonStats />
        </aside>
      </div>
    </>
  );
}
