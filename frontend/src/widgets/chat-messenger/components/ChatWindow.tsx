import React, { useState, useRef, useEffect } from 'react';
import styles from '../MessengerView.module.css';
import { useChat } from '@/app/providers/ChatProvider';
import { useAuth } from '@/app/providers/AuthContext';

interface ChatWindowProps {
    onBack?: () => void;
}

import { formatMessageDate, formatDateGroup } from '@/shared/utils/date.utils';

export const ChatWindow: React.FC<ChatWindowProps> = ({ onBack }) => {
    const { user } = useAuth();
    const { 
        conversations, 
        activeConversationId, 
        messages,
        sendMessage
    } = useChat();

    const [inputValue, setInputValue] = useState('');
    const messageListRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (messageListRef.current) {
            messageListRef.current.scrollTop = messageListRef.current.scrollHeight;
        }
    }, [messages, activeConversationId]);

    const handleSend = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!inputValue.trim() || !activeConversationId) return;

        const activeConv = conversations.find(c => c.id === activeConversationId);
        if (activeConv) {
            await sendMessage(activeConv.otherUserId, inputValue.trim());
            setInputValue('');
        }
    };

    const getOtherParticipant = (convId: number) => {
        return conversations.find(c => c.id === convId);
    };



    if (!activeConversationId) return null;

    const otherParticipant = getOtherParticipant(activeConversationId);

    return (
        <div className={styles.chatWindow}>
            <div className={styles.chatHeader}>
                {onBack && (
                    <button onClick={onBack} className={styles.backButton}>
                        <i className="fa-solid fa-arrow-left"></i>
                    </button>
                )}
                <div className={styles.headerAvatar}>
                    {otherParticipant?.otherUserName.charAt(0)}
                </div>
                <div className={styles.headerInfo}>
                    <h3 className={styles.headerName}>{otherParticipant?.otherUserName}</h3>
                </div>
            </div>
            
            <div className={styles.messageList} ref={messageListRef}>
                {messages.length > 0 ? (
                    messages.map((msg, index) => {
                        const currentDateGroup = formatDateGroup(msg.sentAt);
                        const prevDateGroup = index > 0 ? formatDateGroup(messages[index - 1].sentAt) : null;
                        const showDateGroup = currentDateGroup !== prevDateGroup;

                        return (
                            <React.Fragment key={msg.id}>
                                {showDateGroup && (
                                    <div className={styles.dateSeparator}>
                                        <span className={styles.dateSeparatorText}>{currentDateGroup}</span>
                                    </div>
                                )}
                                <div 
                                    className={`${styles.messageBubble} ${msg.senderId === user?.id ? styles.msgSent : styles.msgReceived}`}
                                >
                                    <div className={styles.msgContent}>{msg.content}</div>
                                    <span className={styles.msgTime}>{formatMessageDate(msg.sentAt)}</span>
                                </div>
                            </React.Fragment>
                        );
                    })
                ) : (
                    <div className={styles.noMessages}>
                        <p>Почніть розмову...</p>
                    </div>
                )}
            </div>

            <form className={styles.inputArea} onSubmit={handleSend}>
                <input 
                    type="text" 
                    className={styles.chatInput}
                    placeholder="Напишіть повідомлення..."
                    value={inputValue}
                    onChange={(e) => setInputValue(e.target.value)}
                />
                <button type="submit" className={styles.sendButton} disabled={!inputValue.trim()}>
                    <i className="fa-solid fa-paper-plane"></i>
                </button>
            </form>
        </div>
    );
};
