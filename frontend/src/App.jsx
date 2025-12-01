import './App.css'
import Header from './components/Header/Header.jsx'
import { useState } from 'react'
import MainContent from './components/MainContent/MainContent.jsx'
import Sidebar from './components/Sidebar/Sidebar.jsx'

function App() {

  return (
    <>
      <Header />
      <div>
        <MainContent />
        <Sidebar />
      </div>
    </>
  )
}

export default App
