import api from './axiosInstance';

export interface ExternalAchievement {
    id: number;
    studentName: string;
    title: string;
    content: string;
    photoUrl?: string;
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
    file?: File | null 
}) => {
    const formData = new FormData();
    formData.append('studentName', data.studentName);
    formData.append('title', data.title);
    formData.append('content', data.content);
    formData.append('date', data.date);
    if (data.externalLink) formData.append('externalLink', data.externalLink);
    if (data.file) formData.append('file', data.file);

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
