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
        {!hideLayout && (
          <Route
            path="/"
            element={
              <div className="page-wrapper">
                <HomePage />
              </div>
            }
          />
        )}

        {/* Логін, реєстрація та верифікація — без хедера і футера */}
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<Signup />} />
        <Route path="/email-verification" element={<EmailVerification />} />

        {/* Інші сторінки */}
        {!hideLayout && (
          <>
            <Route path="/calendar" element={<CalendarPage />} />
            <Route 
              path="/tournaments-page" 
              element={
                <div className="page-wrapper">
                  <Tournaments />
                </div>
              }
            />
            <Route path="/teams" element={<div>Список команд</div>} />
            <Route path="/ratings" element={<PlayersRatingPage />} />
            <Route path="/notifications" element={<NotificationsPage />} />
            <Route path="/profile" element={<ProfilePage />} />
            <Route path="/create-tournament" element={<TournamentCreatePage />} />
          </>
        )}
      </Routes>

      {!hideLayout && <Footer />}
    </>
  );
}