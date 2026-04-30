import './App.css'
import { Header } from '@/widgets/header';
import { Footer } from '@/widgets/footer';
import { LoginPage } from '@/pages/LoginPage';
import { Routes, Route, useLocation } from 'react-router-dom'
import { SignupPage } from '@/pages/SignupPage';
import { CalendarPage } from '@/pages/CalendarPage';
import { TournamentsPage } from '@/pages/TournamentsPage';
import { TournamentViewPage } from '@/pages/TournamentViewPage';
import { HomePage } from '@/pages/HomePage';
import { EmailVerificationPage } from '@/pages/EmailVerificationPage';
import '@fortawesome/fontawesome-free/css/all.min.css';
import { PlayersRatingPage } from '@/pages/PlayersRatingPage';
import { ProfilePage } from '@/pages/ProfilePage';
import { NotificationsPage } from '@/pages/NotificationsPage';
import { TournamentCreatePage } from '@/pages/TournamentCreatePage';
import { PlayerViewPage } from '@/pages/PlayerViewPage';
import { PlayerComparePage } from '@/pages/PlayerComparePage/index'; // Player comparison page
import NotFoundPage from '@/pages/NotFoundPage/NotFoundPage';
import InternalServerErrorPage from '@/pages/InternalServerErrorPage/InternalServerErrorPage';
import { RatingsPage } from '@/pages/RatingsPage';
import { AdminMergesPage } from '@/pages/AdminMergesPage/AdminMergesPage';
import { AdminUsersPage } from '@/pages/AdminUsersPage';
import { NationalRatingsPage } from '@/pages/NationalRatingsPage';

import ProtectedRoute from '../providers/ProtectedRoute';
import ScrollToTop from '../providers/ScrollToTop';
import { Toaster } from 'react-hot-toast';



export default function App() {
  const location = useLocation();
  const hideLayout = location.pathname === "/login" ||
    location.pathname === "/signup" ||
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

        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignupPage />} />
        <Route path="/email-verification" element={<EmailVerificationPage />} />

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


        {/* Помилки */}
        <Route path="/404" element={<NotFoundPage />} />
        <Route path="/500" element={<InternalServerErrorPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>

      {!hideLayout && <Footer />}
    </>
  );
}
