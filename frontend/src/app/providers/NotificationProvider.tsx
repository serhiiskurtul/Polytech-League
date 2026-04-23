import React, { createContext, useContext, useEffect, useState, useCallback } from 'react';
import { useAuth } from './AuthContext';
import { getMyNotifications, markAsRead as apiMarkAsRead, markAllAsRead as apiMarkAllAsRead, deleteNotification as apiDeleteNotification, clearReadNotifications as apiClearReadNotifications } from '@/shared/api/notifications.api';
import { Notification } from '@/shared/types/notification.types';

interface NotificationContextType {
  notifications: Notification[];
  unreadCount: number;
  refreshNotifications: () => Promise<void>;
  markAsRead: (id: number) => Promise<void>;
  markAllAsRead: () => Promise<void>;
  deleteNotification: (id: number) => Promise<void>;
  clearReadNotifications: () => Promise<void>;
  loading: boolean;
}

const NotificationContext = createContext<NotificationContextType | undefined>(undefined);

export const NotificationProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(false);

  const refreshNotifications = useCallback(async () => {
    if (!isAuthenticated) return;
    try {
      const data = await getMyNotifications();
      setNotifications(data);
      setUnreadCount(data.filter(n => !n.isRead).length);
    } catch (error) {
      console.error('Failed to fetch notifications:', error);
    }
  }, [isAuthenticated]);

  const markAsRead = async (id: number) => {
    try {
      await apiMarkAsRead(id);
      await refreshNotifications();
    } catch (error) {
      console.error('Failed to mark notification as read:', error);
    }
  };

  const markAllAsRead = async () => {
    try {
      await apiMarkAllAsRead();
      await refreshNotifications();
    } catch (error) {
      console.error('Failed to mark all as read:', error);
    }
  };

  const deleteNotification = async (id: number) => {
    try {
      await apiDeleteNotification(id);
      await refreshNotifications();
    } catch (error) {
      console.error('Failed to delete notification:', error);
    }
  };

  const clearReadNotifications = async () => {
    try {
      await apiClearReadNotifications();
      await refreshNotifications();
    } catch (error) {
      console.error('Failed to clear read notifications:', error);
    }
  };

  useEffect(() => {
    if (isAuthenticated) {
      setLoading(true);
      refreshNotifications().finally(() => setLoading(false));
      const interval = setInterval(refreshNotifications, 60000);
      return () => clearInterval(interval);
    } else {
      setNotifications([]);
      setUnreadCount(0);
    }
  }, [isAuthenticated, refreshNotifications]);

  return (
    <NotificationContext.Provider value={{
      notifications,
      unreadCount,
      refreshNotifications,
      markAsRead,
      markAllAsRead,
      deleteNotification,
      clearReadNotifications,
      loading
    }}>
      {children}
    </NotificationContext.Provider>
  );
};

export const useNotifications = () => {
  const context = useContext(NotificationContext);
  if (context === undefined) {
    throw new Error('useNotifications must be used within a NotificationProvider');
  }
  return context;
};
