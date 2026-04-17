export interface Notification {
    id: number;
    title: string;
    message: string;
    isRead: boolean;
    createdAt: string;
    link?: string;
    type: string;
    icon: string;
    color: string;
}
