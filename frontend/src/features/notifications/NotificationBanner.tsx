import React, { useState, useEffect } from 'react';
import { subscribeUserToPush } from '@/services/pushService';
import { useAuth } from '@/app/providers/AuthContext';

console.log('--- FILE LOADED: NotificationBanner.tsx ---');

export const NotificationBanner: React.FC = () => {
  console.log('--- COMPONENT RENDERED: NotificationBanner ---');
  const { isAuthenticated, loading } = useAuth();
  const [showBanner, setShowBanner] = useState(false);
  const [isSubscribing, setIsSubscribing] = useState(false);

  useEffect(() => {
    console.log('NotificationBanner state:', { isAuthenticated, loading, permission: Notification.permission });

    if (loading) return;

    if (!isAuthenticated) {
      console.log('Banner NOT showing: user not authenticated');
      return;
    }

    if (!('Notification' in window)) {
      console.log('Banner NOT showing: Notification API not supported');
      return;
    }

    if (Notification.permission === 'default') {
      const hasDismissed = localStorage.getItem('notification_banner_dismissed');
      console.log('Banner dismiss status:', hasDismissed);
      
      if (!hasDismissed) {
        console.log('Setting timer to show banner...');
        const timer = setTimeout(() => {
          console.log('SHOWING BANNER NOW');
          setShowBanner(true);
        }, 2000);
        return () => clearTimeout(timer);
      }
    } else {
      console.log('Banner NOT showing: permission is already', Notification.permission);
    }
  }, [isAuthenticated, loading]);

  const handleEnable = async () => {
    setIsSubscribing(true);
    try {
      const subscription = await subscribeUserToPush();
      if (subscription) {
        setShowBanner(false);
      }
    } catch (error) {
      console.error('Failed to enable notifications', error);
    } finally {
      setIsSubscribing(false);
    }
  };

  const handleDismiss = () => {
    localStorage.setItem('notification_banner_dismissed', 'true');
    setShowBanner(false);
  };

  if (!showBanner) return null;

  return (
    <div className="notification-banner-container">
      <div className="notification-banner-content">
        <div className="notification-banner-icon">
          <i className="fas fa-bell"></i>
        </div>
        <div className="notification-banner-text">
          <h3>Будьте в курсі подій!</h3>
          <p>Ввімкніть сповіщення, щоб миттєво дізнаватися про результати матчів та нові повідомлення.</p>
        </div>
        <div className="notification-banner-actions">
          <button 
            className="btn-enable" 
            onClick={handleEnable}
            disabled={isSubscribing}
          >
            {isSubscribing ? 'Підключення...' : 'Ввімкнути'}
          </button>
          <button 
            className="btn-dismiss" 
            onClick={handleDismiss}
            disabled={isSubscribing}
          >
            Пізніше
          </button>
        </div>
      </div>

      <style>{`
        .notification-banner-container {
          position: fixed;
          bottom: 24px;
          left: 50%;
          transform: translateX(-50%);
          width: calc(100% - 48px);
          max-width: 500px;
          z-index: 9999;
          animation: slideUp 0.5s cubic-bezier(0.16, 1, 0.3, 1);
        }

        .notification-banner-content {
          background: rgba(255, 255, 255, 0.85);
          backdrop-filter: blur(12px);
          -webkit-backdrop-filter: blur(12px);
          border: 1px solid rgba(255, 255, 255, 0.3);
          border-radius: 20px;
          padding: 20px;
          box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
          display: flex;
          align-items: center;
          gap: 20px;
        }

        .notification-banner-icon {
          width: 48px;
          height: 48px;
          background: linear-gradient(135deg, #0066cc 0%, #004499 100%);
          border-radius: 14px;
          display: flex;
          align-items: center;
          justify-content: center;
          color: white;
          font-size: 20px;
          flex-shrink: 0;
          box-shadow: 0 4px 12px rgba(0, 102, 204, 0.3);
        }

        .notification-banner-text {
          flex-grow: 1;
        }

        .notification-banner-text h3 {
          margin: 0 0 4px 0;
          font-size: 1.1rem;
          font-weight: 700;
          color: #1a1a1a;
        }

        .notification-banner-text p {
          margin: 0;
          font-size: 0.9rem;
          color: #4a4a4a;
          line-height: 1.4;
        }

        .notification-banner-actions {
          display: flex;
          flex-direction: column;
          gap: 8px;
        }

        .btn-enable {
          background: #0066cc;
          color: white;
          border: none;
          padding: 10px 20px;
          border-radius: 10px;
          font-weight: 600;
          font-size: 0.9rem;
          cursor: pointer;
          transition: all 0.2s ease;
          white-space: nowrap;
        }

        .btn-enable:hover {
          background: #0055aa;
          transform: translateY(-1px);
          box-shadow: 0 4px 12px rgba(0, 102, 204, 0.2);
        }

        .btn-enable:active {
          transform: translateY(0);
        }

        .btn-enable:disabled {
          opacity: 0.7;
          cursor: not-allowed;
        }

        .btn-dismiss {
          background: transparent;
          color: #666;
          border: none;
          padding: 6px;
          font-size: 0.85rem;
          font-weight: 500;
          cursor: pointer;
          transition: color 0.2s ease;
        }

        .btn-dismiss:hover {
          color: #1a1a1a;
        }

        @keyframes slideUp {
          from {
            transform: translate(-50%, 100%);
            opacity: 0;
          }
          to {
            transform: translate(-50%, 0);
            opacity: 1;
          }
        }

        @media (max-width: 600px) {
          .notification-banner-content {
            flex-direction: column;
            text-align: center;
            gap: 16px;
          }
          
          .notification-banner-actions {
            width: 100%;
            flex-direction: row;
          }
          
          .btn-enable {
            flex-grow: 1;
          }
          
          .btn-dismiss {
            flex-grow: 0;
          }
        }
      `}</style>
    </div>
  );
};
