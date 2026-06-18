import './App.css'
import { useEffect } from 'react';
import { Header } from '@/widgets/header';
import { Footer } from '@/widgets/footer';
import { LoginPage } from '@/pages/login';
import { Routes, Route, useLocation, useNavigate } from 'react-router-dom'
import { SignupPage } from '@/pages/signup';
import { CalendarPage } from '@/pages/calendar';
import { TournamentsPage } from '@/pages/tournaments';
import { TournamentViewPage } from '@/pages/tournament-view';
import { HomePage } from '@/pages/home';
import { EmailVerificationPage } from '@/pages/email-verification';
import { ForgotPasswordPage } from '@/pages/forgot-password';
import { ResetPasswordPage } from '@/pages/reset-password';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { PlayersRatingPage } from '@/pages/players-rating';
import { ProfilePage } from '@/pages/profile';
import { NotificationsPage } from '@/pages/notifications';
import { TournamentCreatePage } from '@/pages/tournament-create';
import { PlayerViewPage } from '@/pages/player-view';
import { PlayerComparePage } from '@/pages/player-compare/index'; // Player comparison page
import NotFoundPage from '@/pages/not-found/NotFoundPage';
import { useNotifications } from '@/entities/notification';
import InternalServerErrorPage from '@/pages/internal-server-error/InternalServerErrorPage';
import { RatingsPage } from '@/pages/ratings';
import { AdminMergesPage } from '@/pages/admin-merges/AdminMergesPage';
import { AdminUsersPage } from '@/pages/admin-users';
import { AdminAnalyticsPage } from '@/pages/admin-analytics/AdminAnalyticsPage';
import { NationalRatingsPage } from '@/pages/national-ratings';
import { AdminCollegesPage } from '@/pages/admin-colleges';
import { RegionalRatingsPage } from '@/pages/regional-ratings';
import { CollegesPage } from '@/pages/colleges';
import ChampionsPage from '@/pages/champions/ChampionsPage';
import { AboutPage } from '@/pages/about';
import StudentNewsPage from '@/pages/student-news/StudentNewsPage';
import { FeedbackPage } from '@/pages/feedback';
import { AdminFeedbackPage } from '@/pages/admin-feedback';
import { ApiDocsPage } from '@/pages/api-docs';

import ProtectedRoute from '../providers/ProtectedRoute';
import ScrollToTop from '../providers/ScrollToTop';
import { Toaster } from 'react-hot-toast';
import { NotificationBanner } from '@/features/notifications/NotificationBanner';
import api from '@/shared/api/axiosInstance';



export default function App() {
  const location = useLocation();
  const navigate = useNavigate();
  const { notifications, markAsRead } = useNotifications();

  useEffect(() => {
    const recordVisit = async () => {
      try {
        let guestId = localStorage.getItem('guestId');
        if (!guestId) {
          guestId = crypto.randomUUID();
          localStorage.setItem('guestId', guestId);
        }
        await api.post('/SiteVisits', { guestId });
      } catch (e) {
        console.error('Failed to record visit:', e);
      }
    };
    recordVisit();
  }, []);

  useEffect(() => {
    const handleMessage = (event: MessageEvent) => {
      if (event.data && event.data.type === 'NAVIGATE') {
        const url = event.data.url;
        navigate(url);
        
        try {
          const urlPath = new URL(url).pathname;
          const matchingNotif = notifications.find(n => !n.isRead && n.link === urlPath);
          if (matchingNotif) {
            markAsRead(matchingNotif.id).catch(console.error);
          }
        } catch (e) {
          console.error('Failed to parse URL in message listener:', e);
        }
      }
    };

    navigator.serviceWorker?.addEventListener('message', handleMessage);
    return () => navigator.serviceWorker?.removeEventListener('message', handleMessage);
  }, [navigate, notifications, markAsRead]);

  const hideLayout = location.pathname === "/login" ||
    location.pathname === "/signup" ||
    location.pathname === "/forgot-password" ||
    location.pathname === "/reset-password" ||
    location.pathname === "/email-verification";

  return (
    <>
      <ScrollToTop />
      <Toaster
        position="top-center"
        toastOptions={{
          duration: 4000,
          style: {
            borderRadius: '10px',
            background: '#333',
            color: '#fff',
            padding: '16px 24px',
            fontSize: '16px',
          },
          success: {
            iconTheme: {
              primary: '#10b981',
              secondary: '#fff',
            },
          },
        }}
      />
      <NotificationBanner />

      {!hideLayout && <Header />}

      <Routes>
        {/* Головна */}
        <Route
          path="/"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <HomePage />
              </div>
            </ProtectedRoute>
          }
        />

        {/* Календар */}
        <Route
          path="/calendar"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <CalendarPage />
              </div>
            </ProtectedRoute>
          }
        />

        {/* Змагання */}
        <Route
          path="/tournaments"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <TournamentsPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/tournaments/:id"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <TournamentViewPage />
              </div>
            </ProtectedRoute>
          }
        />

        {/* Рейтинги */}
        <Route
          path="/ratings"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <PlayersRatingPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/group-ratings"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <RatingsPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/national-ratings"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <NationalRatingsPage />
              </div>
            </ProtectedRoute>
          }
        />
        
        <Route
          path="/regional-ratings"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <RegionalRatingsPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/colleges"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <CollegesPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/players/:id"
          element={
            <ProtectedRoute requireAuth={false}>
              <PlayerViewPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/compare"
          element={
            <ProtectedRoute requireAuth={false}>
              <PlayerComparePage />
            </ProtectedRoute>
          }
        />

        {/* Команди */}
        <Route
          path="/teams"
          element={
            <ProtectedRoute requireAuth={false}>
              <div>Список команд</div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/hall-of-fame"
          element={
            <ProtectedRoute requireAuth={false}>
              <ChampionsPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/about"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <AboutPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/api-docs"
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <ApiDocsPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/feedback"
          element={
            <ProtectedRoute requireAuth={true}>
              <div className="page-wrapper">
                <FeedbackPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignupPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />
        <Route path="/email-verification" element={<EmailVerificationPage />} />
        <Route path="/student-news" element={<StudentNewsPage />} />

        {/* Профіль */}
        <Route
          path="/profile"
          element={
            <ProtectedRoute requireAuth={true}>
              <ProfilePage />
            </ProtectedRoute>
          }
        />

        {/* Сповіщення */}
        <Route
          path="/notifications"
          element={
            <ProtectedRoute requireAuth={true}>
              <NotificationsPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/create-tournament"
          element={
            <ProtectedRoute requireAuth={true}>
              <TournamentCreatePage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/merges"
          element={
            <ProtectedRoute requireAuth={true} adminOnly={true}>
              <AdminMergesPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/users"
          element={
            <ProtectedRoute requireAuth={true} adminOnly={true}>
              <div className="page-wrapper">
                <AdminUsersPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/analytics"
          element={
            <ProtectedRoute requireAuth={true} adminOnly={true}>
              <div className="page-wrapper">
                <AdminAnalyticsPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/colleges"
          element={
            <ProtectedRoute requireAuth={true} adminOnly={true}>
              <div className="page-wrapper">
                <AdminCollegesPage />
              </div>
            </ProtectedRoute>
          }
        />

        <Route
          path="/admin/feedback"
          element={
            <ProtectedRoute requireAuth={true} adminOnly={true}>
              <div className="page-wrapper">
                <AdminFeedbackPage />
              </div>
            </ProtectedRoute>
          }
        />


        {/* Помилки */}
        <Route path="/404" element={<NotFoundPage />} />
        <Route path="/500" element={<InternalServerErrorPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>

      {!hideLayout && <Footer />}
    </>
  );
}
