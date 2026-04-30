import api from './axiosInstance';

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
