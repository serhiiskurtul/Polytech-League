import api from '@/shared/api/axiosInstance';
import { Notification } from '@/entities/notification/model/notification.types';

export const getMyNotifications = async (): Promise<Notification[]> => {
    try {
        const response = await api.get<Notification[]>('/notifications');
        return response.data;
    } catch (error) {
        console.error('Помилка при отриманні сповіщень:', error);
        throw error;
    }
};

export const markAsRead = async (id: number): Promise<void> => {
    try {
        await api.put(`/notifications/${id}/read`);
    } catch (error) {
        console.error(`Помилка при позначенні сповіщення ${id} як прочитаного:`, error);
        throw error;
    }
};

export const markAllAsRead = async (): Promise<void> => {
    try {
        await api.put('/notifications/read-all');
    } catch (error) {
        console.error('Помилка при позначенні всіх сповіщень як прочитаних:', error);
        throw error;
    }
};

export const deleteNotification = async (id: number): Promise<void> => {
    try {
        await api.delete(`/notifications/${id}`);
    } catch (error) {
        console.error(`Помилка при видаленні сповіщення ${id}:`, error);
        throw error;
    }
};

export const clearReadNotifications = async (): Promise<void> => {
    try {
        await api.delete('/notifications/clear-read');
    } catch (error) {
        console.error('Помилка при очищенні прочитаних сповіщень:', error);
        throw error;
    }
};
