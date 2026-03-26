import './App.css'
import Header from '../../widgets/header/Header';
import Footer from '../../widgets/footer/Footer';
import Login from '../../pages/LoginPage/Login';
import { Routes, Route, useLocation } from 'react-router-dom'
import Signup from '../../pages/SignupPage/Signup';
import CalendarPage from '../../pages/CalendarPage/CalendarPage';
import Tournaments from '../../pages/TournamentsPage/TournamentsPage';
import HomePage from '../../pages/HomePage/HomePage';
import EmailVerification from '../../pages/EmailVerificationPage/EmailVerificationPage';
import '@fortawesome/fontawesome-free/css/all.min.css';
import PlayersRatingPage from '../../pages/PlayersRatingPage/PlayersRatingPage';
import ProfilePage from '../../pages/ProfilePage/ProfilePage';
import NotificationsPage from '../../pages/NotificationsPage/NotificationsPage';
import TournamentCreatePage from '../../pages/TournamentCreatePage/TournamentCreatePage';
import ProtectedRoute from '../providers/ProtectedRoute';
import { Toaster } from 'react-hot-toast';

export default function App() {
  const location = useLocation();
  const hideLayout = location.pathname === "/login" || 
                     location.pathname === "/signup" || 
                     location.pathname === "/email-verification";

  return (
    <>
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
              <CalendarPage />
            </ProtectedRoute>
          } 
        />

        {/* Турніри */}
        <Route 
          path="/tournaments-page" 
          element={
            <ProtectedRoute requireAuth={false}>
              <div className="page-wrapper">
                <Tournaments />
              </div>
            </ProtectedRoute>
          }
        />

        {/* Рейтинги */}
        <Route 
          path="/ratings" 
          element={
            <ProtectedRoute requireAuth={false}>
              <PlayersRatingPage />
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
        
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<Signup />} />
        <Route path="/email-verification" element={<EmailVerification />} />
        
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

        {/* Створення турніру */}
        <Route 
          path="/create-tournament" 
          element={
            <ProtectedRoute requireAuth={true}>
              <TournamentCreatePage />
            </ProtectedRoute>
          } 
        />
      </Routes>

      {!hideLayout && <Footer />}
    </>
  );
}
