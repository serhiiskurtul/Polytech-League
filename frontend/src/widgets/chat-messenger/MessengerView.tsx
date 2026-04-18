import React from 'react';
import styles from './MessengerView.module.css';
import { useChat } from '@/app/providers/ChatProvider';
import { ConversationList } from './components/ConversationList';
import { ChatWindow } from './components/ChatWindow';

export const MessengerView: React.FC = () => {
    const { 
        conversations, 
        activeConversationId, 
        setActiveConversationId,
        markAsRead
    } = useChat();

    const handleConversationClick = (id: number) => {
        setActiveConversationId(id);
        markAsRead(id);
    };

    const handleBack = () => {
        setActiveConversationId(null);
    };

    // Full-screen mobile-like behavior: 
    // If activeConversationId is set, show ChatWindow. Otherwise show ConversationList.
    return (
        <div className={styles.messengerView}>
            {activeConversationId ? (
                <ChatWindow onBack={handleBack} />
            ) : (
                <ConversationList 
                    conversations={conversations} 
                    activeConversationId={activeConversationId}
                    onConversationClick={handleConversationClick}
                />
            )}
        </div>
    );
};
