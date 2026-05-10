import React, { useState, useEffect } from 'react';
import { subscribeUserToPush } from '@/services/pushService';
import { useAuth } from '@/app/providers/AuthContext';

export const NotificationBanner: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [showBanner, setShowBanner] = useState(false);

  useEffect(() => {
    if (isAuthenticated && 'Notification' in window) {
      if (Notification.permission === 'default') {
        const hasDismissed = localStorage.getItem('notification_banner_dismissed');
        if (!hasDismissed) {
          setShowBanner(true);
        }
      }
    }
  }, [isAuthenticated]);

  const handleEnable = async () => {
    try {
      await subscribeUserToPush();
      setShowBanner(false);
    } catch (error) {
      console.error('Failed to enable notifications', error);
      // Even if it fails, we might want to hide the banner or show an error
    }
  };

  const handleDismiss = () => {
    localStorage.setItem('notification_banner_dismissed', 'true');
    setShowBanner(false);
  };

  if (!showBanner) return null;

  return (
    <div style={{
      position: 'fixed',
      bottom: '20px',
      left: '20px',
      right: '20px',
      backgroundColor: '#0066cc',
      color: 'white',
      padding: '16px',
      borderRadius: '12px',
      boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
      display: 'flex',
      flexDirection: 'column',
      gap: '12px',
      zIndex: 1000,
      maxWidth: '500px',
      margin: '0 auto'
    }}>
      <div style={{ fontWeight: 'bold', fontSize: '1.1rem' }}>
        Ввімкніть сповіщення
      </div>
      <div style={{ fontSize: '0.9rem', opacity: 0.9 }}>
        Отримуйте миттєві повідомлення про нові результати змагань та повідомлення від інших гравців.
      </div>
      <div style={{ display: 'flex', gap: '8px', marginTop: '4px' }}>
        <button 
          onClick={handleEnable}
          style={{
            backgroundColor: 'white',
            color: '#0066cc',
            border: 'none',
            padding: '8px 16px',
            borderRadius: '6px',
            fontWeight: '600',
            cursor: 'pointer'
          }}
        >
          Ввімкнути
        </button>
        <button 
          onClick={handleDismiss}
          style={{
            backgroundColor: 'transparent',
            color: 'white',
            border: '1px solid rgba(255,255,255,0.3)',
            padding: '8px 16px',
            borderRadius: '6px',
            cursor: 'pointer'
          }}
        >
          Пізніше
        </button>
      </div>
    </div>
  );
};
