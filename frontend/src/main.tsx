import { StrictMode } from 'react'
import { BrowserRouter } from "react-router-dom";
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './app/App/App'
import './app/styles/globals.css';
import { AuthProvider } from './app/providers/AuthContext';
import { NotificationProvider } from './app/providers/NotificationProvider';
import { ChatProvider } from './app/providers/ChatProvider';
import { Analytics } from "@vercel/analytics/react";
import { SpeedInsights } from "@vercel/speed-insights/react";

const rootElement = document.getElementById('root');
if (!rootElement) throw new Error('Failed to find the root element');

createRoot(rootElement).render(
  <StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <ChatProvider>
          <NotificationProvider>
            <App />
            <Analytics />
            <SpeedInsights />
          </NotificationProvider>
        </ChatProvider>
      </AuthProvider>
    </BrowserRouter>
  </StrictMode>
)
