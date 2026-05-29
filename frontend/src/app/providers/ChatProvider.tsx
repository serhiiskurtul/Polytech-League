import React, { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel, HubConnectionState } from '@microsoft/signalr';
import { useAuth } from './AuthContext';
import { chatApi, ConversationDto, MessageDto } from '@/shared/api/chat.api';
import { BASE_URL } from '@/shared/api/axiosInstance';

interface ChatContextType {
    conversations: ConversationDto[];
    activeConversationId: number | null;
    setActiveConversationId: (id: number | null) => void;
    messages: MessageDto[];
    sendMessage: (recipientId: number, content: string) => Promise<void>;
    unreadTotal: number;
    markAsRead: (conversationId: number) => Promise<void>;
    refreshConversations: () => Promise<void>;
    connection: HubConnection | null;
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
            const total = (data || []).reduce((acc, conv) => acc + (conv.unreadCount || 0), 0);
            setUnreadTotal(total);
        } catch (error) {
            console.error("Failed to fetch conversations", error);
        }
    }, [isAuthenticated]);

    useEffect(() => {
        if (isAuthenticated) {
            refreshConversations();

            // Set up polling interval as a fallback for SignalR
            const interval = setInterval(refreshConversations, 60000);

            // Refresh when tab becomes visible
            const handleVisibilityChange = () => {
                if (document.visibilityState === 'visible') {
                    refreshConversations();
                }
            };
            document.addEventListener('visibilitychange', handleVisibilityChange);

            return () => {
                clearInterval(interval);
                document.removeEventListener('visibilitychange', handleVisibilityChange);
            };
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
            markAsRead(activeConversationId);
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

        const baseUrl = BASE_URL;
        const newConnection = new HubConnectionBuilder()
            .withUrl(`${baseUrl}/hubs/chat`, {
                accessTokenFactory: () => localStorage.getItem('token') || ''
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.None)
            .build();

        setConnection(newConnection);
    }, [isAuthenticated, user]);

    const activeConversationIdRef = useRef<number | null>(null);
    useEffect(() => {
        activeConversationIdRef.current = activeConversationId;
    }, [activeConversationId]);

    useEffect(() => {
        if (!connection) return;

        let isMounted = true;

        const startConnection = async () => {
            try {
                if (connection.state === HubConnectionState.Disconnected) {
                    await connection.start();
                    // Connection started successfully
                }
            } catch (err: any) {
                // Ignore AbortError caused by rapid re-mounting in dev mode
                if (err.name !== 'AbortError' && !err.message?.includes('stopped during negotiation')) {
                    console.error('Connection failed: ', err);
                }
            }
        };

        startConnection();

        connection.on('ReceiveMessage', (message: MessageDto) => {
            // Update messages if this belongs to active conversation
            if (activeConversationIdRef.current === message.conversationId) {
                setMessages(prev => [...prev, message]);
                // Mark as read immediately if the conversation is active
                markAsRead(message.conversationId);
            }
            // Refresh conversations list for all incoming messages
            refreshConversations();
        });

        return () => {
            isMounted = false;
            connection.off('ReceiveMessage');
            if (connection.state !== HubConnectionState.Disconnected) {
                connection.stop();
            }
        };
    }, [connection, refreshConversations]);

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
            await chatApi.markAsRead(conversationId);
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
            refreshConversations,
            connection
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
