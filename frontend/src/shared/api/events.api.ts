import api from './axiosInstance';
import { ModerationStatus } from '../types/tournament.types';

/**
 * Upload multiple photos or videos for an event
 * @param eventId The ID of the event
 * @param files Array of files to upload
 */
export const uploadEventMedia = async (eventId: number | string, files: File[]): Promise<{ urls: string[] }> => {
    const formData = new FormData();
    files.forEach(file => {
        formData.append('files', file);
    });

    const response = await api.post<{ urls: string[] }>(`/event-media/${eventId}/upload-multiple`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data;
};

/**
 * Delete a specific media item (photo or video) from an event
 * @param mediaId The ID of the media item
 */
export const deleteEventMedia = async (mediaId: number | string): Promise<void> => {
    await api.delete(`/event-media/${mediaId}`);
};

export const setMainPhoto = async (eventId: number | string, file: File): Promise<{ url: string }> => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post<{ url: string }>(`/event-media/${eventId}/set-main-photo`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
};

export const markAsMainPhoto = async (eventId: number | string, photoId: number | string): Promise<{ url: string }> => {
    const response = await api.post<{ url: string }>(`/event-media/${eventId}/mark-as-main/${photoId}`);
    return response.data;
};

/**
 * Submit media for an event by a student (requires moderation)
 */
export const submitUserEventMedia = async (eventId: number | string, file: File, comment?: string): Promise<{ message: string, id: number }> => {
    const formData = new FormData();
    if (comment) {
        formData.append('comment', comment);
    }
    formData.append('file', file);

    const response = await api.post<{ message: string, id: number }>(`/user-event-media/${eventId}`, formData);
    return response.data;
};

/**
 * Get approved media for an event
 */
export const getApprovedUserEventMedia = async (eventId: number | string): Promise<any[]> => {
    const response = await api.get<any[]>(`/user-event-media/${eventId}`);
    return response.data;
};

/**
 * Get pending media for moderation (Admin only)
 */
export const getPendingUserEventMedia = async (): Promise<any[]> => {
    const response = await api.get<any[]>('/user-event-media/pending');
    return response.data;
};

/**
 * Update moderation status of user media (Admin only)
 */
export const updateUserEventMediaStatus = async (id: number | string, status: ModerationStatus): Promise<void> => {
    await api.put(`/user-event-media/${id}/status`, { status });
};
