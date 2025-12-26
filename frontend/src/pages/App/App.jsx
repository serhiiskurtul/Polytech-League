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

export default function App() {
  const location = useLocation();
  const hideLayout = location.pathname === "/login" || location.pathname === "/signup"; // тут вирішуємо, чи ховати Header/Footer

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

        {/* Логін та реєстрація — без хедера і футера */}
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<Signup />} />

        {/* Інші сторінки */}
        {!hideLayout && (
          <>
            <Route path="/calendar" element={<CalendarPage />} />
            <Route 
              path="/tournamentsPage" 
              element={
                <div className="page-wrapper">
                  <Tournaments />
                </div>
              }
            />
            <Route path="/profile" element={<div>Профіль користувача</div>} />
            <Route path="/teams" element={<div>Список команд</div>} />
            <Route path="/ratings" element={<div>Рейтинги гравців</div>} />
            <Route path="/notifications" element={<div>Сповіщення користувача</div>} />
            <Route path="/theme" element={<div>Налаштування теми</div>} />
            <Route path="/email-verification" element={<EmailVerification />} />
          </>
        )}
      </Routes>

      {!hideLayout && <Footer />}
    </>
  )
}
