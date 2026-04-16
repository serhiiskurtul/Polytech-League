import styles from './HomePage.module.css';
import { TopPlayersCompact } from '@/widgets/top-players-compact';
import { SeasonStats } from '@/widgets/season-stats';
import { HeroSection } from '@/widgets/hero-section';
import { TournamentCalendar } from '@/widgets/tournament-calendar';
import { RecentMatches } from '@/widgets/recent-matches';

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
          <TopPlayersCompact />
          <SeasonStats />
        </aside>
      </div>
    </>
  );
}
