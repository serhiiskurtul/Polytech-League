import './App.css'
import Header from './components/Header/Header.jsx'
import Footer from './components/Footer/Footer.jsx'
import MainContent from './components/MainContent/MainContent.jsx'
import Sidebar from './components/Sidebar/Sidebar.jsx'
import { Routes, Route } from 'react-router-dom'

export default function App() {
  return (
    <>
      <Header />

      <Routes>
        {/* Головна сторінка */}
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

        {/* Інші сторінки */}
        <Route path="/match" element={<div>Календар матчів</div>} />
        <Route path="/profile" element={<div>Профіль користувача</div>} />
        <Route path="/teams" element={<div>Список команд</div>} />
        <Route path="/ratings" element={<div>Рейтинги гравців</div>} />
        <Route path="/notifications" element={<div>Сповіщення користувача</div>} />
        <Route path="/theme" element={<div>Налаштування теми</div>} />
        <Route path="/avatar" element={<div>Зміна аватара</div>} />
      </Routes>

      <Footer />
    </>
  )
}
