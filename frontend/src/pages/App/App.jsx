import './App.css'
import Header from "../../components/Header/Header.jsx";
import Footer from '../../components/Footer/Footer.jsx'
import MainContent from '../../components/MainContent/MainContent.jsx'
import Sidebar from '../../components/Sidebar/Sidebar.jsx'
import Login from '../Login/Login.jsx';
import { Routes, Route, useLocation } from 'react-router-dom'
import Signup from '../Sign up/Signup.jsx';
import NotFound from '../404/404.jsx';

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
              <div className="container">
                <div className="Layout">
                  <MainContent />
                  <Sidebar />
                </div>
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
            <Route path="/match" element={<div>Календар матчів</div>} />
            <Route path="/profile" element={<div>Профіль користувача</div>} />
            <Route path="/teams" element={<div>Список команд</div>} />
            <Route path="/ratings" element={<div>Рейтинги гравців</div>} />
            <Route path="/notifications" element={<div>Сповіщення користувача</div>} />
            <Route path="/theme" element={<div>Налаштування теми</div>} />
          </>
        )}
      </Routes>

      {!hideLayout && <Footer />}
    </>
  )
}
