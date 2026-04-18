import axiosInstance from "./axiosInstance";

export interface MessageDto {
    id: number;
    conversationId: number;
    senderId: number;
    senderName: string;
    content: string;
    sentAt: string;
    isRead: boolean;
}

export interface ConversationDto {
    id: number;
    otherUserId: number;
    otherUserName: string;
    otherUserPhoto?: string;
    lastMessage?: MessageDto;
    unreadCount: number;
    lastMessageAt: string;
}

export const chatApi = {
    getConversations: async () => {
        const response = await axiosInstance.get<ConversationDto[]>("/Chat/conversations");
        return response.data;
    },
    getMessages: async (conversationId: number, skip = 0, take = 50) => {
        const response = await axiosInstance.get<MessageDto[]>(`/Chat/conversations/${conversationId}/messages`, {
            params: { skip, take }
        });
        return response.data;
    },
    getConversationIdWithUser: async (otherUserId: number) => {
        const response = await axiosInstance.get<number>(`/Chat/conversations/with-user/${otherUserId}`);
        return response.data;
    },
    markAsRead: async (conversationId: number) => {
        await axiosInstance.post(`/Chat/mark-read/${conversationId}`);
    }
};
