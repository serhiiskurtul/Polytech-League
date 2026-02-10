import './App.css'
import Header from '../../widgets/header/Header.jsx';
import Footer from '../../widgets/footer/Footer.jsx';
import Login from '../LoginPage/Login.jsx';
import { Routes, Route, useLocation } from 'react-router-dom'
import Signup from '../SignupPage/Signup.jsx';
import CalendarPage from '../CalendarPage/CalendarPage.jsx';
import Tournaments from '../TournamentsPage/TournamentsPage.jsx';
import HomePage from '../HomePage/HomePage.jsx';
import EmailVerification from '../EmailVerificationPage/EmailVerificationPage.jsx';
import '@fortawesome/fontawesome-free/css/all.min.css';
import PlayersRatingPage from '../PlayersRatingPage/PlayersRatingPage.jsx';
import ProfilePage from '../ProfilePage/ProfilePage.jsx';
import NotificationsPage from '../NotificationsPage/NotificationsPage.jsx';
import TournamentCreatePage from '../TournamentCreatePage/TournamentCreatePage.jsx';
import ProtectedRoute from '../../auth/ProtectedRoute.jsx';

export default function App() {
  const location = useLocation();
  const hideLayout = location.pathname === "/login" || 
                     location.pathname === "/signup" || 
                     location.pathname === "/email-verification";

  return (
    <>
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