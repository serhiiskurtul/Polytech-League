import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel, HubConnectionState } from '@microsoft/signalr';
import { useAuth } from './AuthContext';
import { chatApi, ConversationDto, MessageDto } from '@/shared/api/chat.api';

interface ChatContextType {
    conversations: ConversationDto[];
    activeConversationId: number | null;
    setActiveConversationId: (id: number | null) => void;
    messages: MessageDto[];
    sendMessage: (recipientId: number, content: string) => Promise<void>;
    unreadTotal: number;
    markAsRead: (conversationId: number) => Promise<void>;
    refreshConversations: () => Promise<void>;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

export const ChatProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { user, isAuthenticated } = useAuth();
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [conversations, setConversations] = useState<ConversationDto[]>([]);
    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [activeConversationId, setActiveConversationId] = useState<number | null>(null);
    const [unreadTotal, setUnreadTotal] = useState(0);
    const messagesRef = useRef<MessageDto[]>([]);

    useEffect(() => {
        messagesRef.current = messages;
    }, [messages]);

    const refreshConversations = useCallback(async () => {
        if (!isAuthenticated) return;
        try {
            const data = await chatApi.getConversations();
            setConversations(data);
            setUnreadTotal(data.reduce((acc, conv) => acc + conv.unreadCount, 0));
        } catch (error) {
            console.error("Failed to fetch conversations", error);
        }
    }, [isAuthenticated]);

    useEffect(() => {
        if (isAuthenticated) {
            refreshConversations();
        } else {
            setConversations([]);
            setUnreadTotal(0);
        }
    }, [isAuthenticated, refreshConversations]);

    const loadMessages = useCallback(async (conversationId: number) => {
        try {
            const data = await chatApi.getMessages(conversationId);
            setMessages(data);
        } catch (error) {
            console.error("Failed to load messages", error);
        }
    }, []);

    useEffect(() => {
        if (activeConversationId) {
            loadMessages(activeConversationId);
        } else {
            setMessages([]);
        }
    }, [activeConversationId, loadMessages]);

    useEffect(() => {
        if (!isAuthenticated || !user) {
            if (connection) {
                connection.stop();
                setConnection(null);
            }
            return;
        }

        const newConnection = new HubConnectionBuilder()
            .withUrl(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/hubs/chat`, {
                accessTokenFactory: () => localStorage.getItem('access_token') || ''
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        setConnection(newConnection);
    }, [isAuthenticated, user]);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected to ChatHub');
                    
                    connection.on('ReceiveMessage', (message: MessageDto) => {
                        // Update messages if this belongs to active conversation
                        if (messagesRef.current.length > 0 && messagesRef.current[0].conversationId === message.conversationId) {
                           setMessages(prev => [...prev, message]);
                        } else if (activeConversationId === message.conversationId) {
                            setMessages(prev => [...prev, message]);
                        }

                        // Refresh conversations to update last message and unread count
                        refreshConversations();
                    });
                })
                .catch(e => console.log('Connection failed: ', e));

            return () => {
                connection.stop();
            };
        }
    }, [connection, refreshConversations, activeConversationId]);

    const sendMessage = async (recipientId: number, content: string) => {
        if (connection && connection.state === HubConnectionState.Connected) {
            try {
                await connection.invoke('SendMessage', recipientId, content);
            } catch (err) {
                console.error("SignalR SendMessage failed:", err);
                throw err;
            }
        } else {
            console.error("Cannot send message: SignalR is not connected. Current state:", connection?.state);
            throw new Error("З'єднання з сервером втрачено. Спробуйте пізніше.");
        }
    };

    const markAsRead = async (conversationId: number) => {
        try {
            await chatApi.markAsReadAsync(conversationId);
            if (connection) {
                await connection.invoke('MarkAsRead', conversationId);
            }
            refreshConversations();
        } catch (error) {
            console.error("Failed to mark messages as read", error);
        }
    };

    return (
        <ChatContext.Provider value={{
            conversations,
            activeConversationId,
            setActiveConversationId,
            messages,
            sendMessage,
            unreadTotal,
            markAsRead,
            refreshConversations
        }}>
            {children}
        </ChatContext.Provider>
    );
};

export const useChat = () => {
    const context = useContext(ChatContext);
    if (context === undefined) {
        throw new Error('useChat must be used within a ChatProvider');
    }
    return context;
};
