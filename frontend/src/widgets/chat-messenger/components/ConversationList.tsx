import React from 'react';
import styles from '../MessengerView.module.css';
import { ConversationDto } from '@/entities/chat/api/chat.api';
import { formatConversationDate } from '@/shared/utils/date.utils';

interface ConversationListProps {
    conversations: ConversationDto[];
    activeConversationId: number | null;
    onConversationClick: (id: number) => void;
}

export const ConversationList: React.FC<ConversationListProps> = ({ 
    conversations, 
    activeConversationId, 
    onConversationClick 
}) => {
    return (
        <div className={styles.conversationList}>
            {conversations.length > 0 ? (
                conversations.map((conv) => (
                    <div 
                        key={conv.id} 
                        className={`${styles.conversationItem} ${activeConversationId === conv.id ? styles.active : ''}`}
                        onClick={() => onConversationClick(conv.id)}
                    >
                        <div className={styles.avatar}>
                            {conv.otherUserName.charAt(0)}
                        </div>
                        <div className={styles.convInfo}>
                            <div className={styles.convHeader}>
                                <span className={styles.convName}>{conv.otherUserName}</span>
                                <span className={styles.convTime}>
                                {conv.lastMessageAt ? formatConversationDate(conv.lastMessageAt) : ''}
                                </span>
                            </div>
                            <div className={styles.convPreview}>
                                <span className={styles.lastMsg}>
                                    {conv.lastMessage ? conv.lastMessage.content : 'Немає повідомлень'}
                                </span>
                                {conv.unreadCount > 0 && (
                                    <span className={styles.itemUnread}>{conv.unreadCount}</span>
                                )}
                            </div>
                        </div>
                    </div>
                ))
            ) : (
                <div className={styles.emptyState}>
                    <i className="fa-solid fa-message"></i>
                    <p>У вас ще немає розмов. Напишіть комусь першим!</p>
                </div>
            )}
        </div>
    );
};
