import { StrictMode } from 'react'
import { BrowserRouter } from "react-router-dom";
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './pages/App/App.jsx'
import './styles/globals.css';
import { AuthProvider } from './auth/AuthContext.jsx';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <App />\
      </AuthProvider>
    </BrowserRouter>
  </StrictMode>
)
