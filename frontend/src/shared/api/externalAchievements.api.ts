import api from './axiosInstance';

export interface ExternalAchievementMedia {
    id: number;
    url: string;
    type: 'photo' | 'video';
}

export interface ExternalAchievement {
    id: number;
    studentName: string;
    title: string;
    content: string;
    media: ExternalAchievementMedia[];
    externalLink?: string;
    date: string;
    createdAt: string;
}

export const getExternalAchievements = async (): Promise<ExternalAchievement[]> => {
    const response = await api.get<ExternalAchievement[]>('/externalachievements');
    return response.data;
};

export const createExternalAchievement = async (data: { 
    studentName: string; 
    title: string; 
    content: string; 
    date: string; 
    externalLink?: string; 
    photos?: File[];
    videos?: File[];
}) => {
    const formData = new FormData();
    formData.append('studentName', data.studentName);
    formData.append('title', data.title);
    formData.append('content', data.content);
    formData.append('date', data.date);
    if (data.externalLink) formData.append('externalLink', data.externalLink);
    
    if (data.photos) {
        data.photos.forEach(file => formData.append('photos', file));
    }
    if (data.videos) {
        data.videos.forEach(file => formData.append('videos', file));
    }

    const response = await api.post('/externalachievements', formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data;
};

export const deleteExternalAchievement = async (id: number): Promise<void> => {
    await api.delete(`/externalachievements/${id}`);
};
