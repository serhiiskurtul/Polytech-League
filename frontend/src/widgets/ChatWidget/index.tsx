import React, { useState, useRef, useEffect } from 'react';
import styles from './ChatWidget.module.css';
import { useChat } from '@/app/providers/ChatProvider';
import { useAuth } from '@/app/providers/AuthContext';

export const ChatWidget: React.FC = () => {
    const { isAuthenticated, user } = useAuth();
    const { 
        conversations, 
        unreadTotal, 
        activeConversationId, 
        setActiveConversationId,
        messages,
        sendMessage,
        markAsRead,
        isChatOpen,
        setIsChatOpen
    } = useChat();

    const [inputValue, setInputValue] = useState('');
    const messageListRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (messageListRef.current) {
            messageListRef.current.scrollTop = messageListRef.current.scrollHeight;
        }
    }, [messages, activeConversationId]);

    if (!isAuthenticated) return null;

    const toggleChat = () => {
        setIsChatOpen(!isChatOpen);
        if (activeConversationId) {
            markAsRead(activeConversationId);
        }
    };

    const handleConversationClick = (id: number) => {
        setActiveConversationId(id);
        markAsRead(id);
    };

    const handleBack = () => {
        setActiveConversationId(null);
    };

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

    const formatTime = (dateStr: string) => {
        const date = new Date(dateStr);
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    };

    return (
        <div className={styles.chatWidget}>
            {isChatOpen && (
                <div className={styles.chatPanel}>
                    {activeConversationId ? (
                        <div className={styles.chatWindow}>
                            <div className={styles.panelHeader}>
                                <h3>
                                    <button onClick={handleBack} className={styles.backButton}>
                                        <i className="fa-solid fa-arrow-left"></i>
                                    </button>
                                    {getOtherParticipant(activeConversationId)?.otherUserName}
                                </h3>
                                <button onClick={toggleChat} className={styles.closeButton}>
                                    <i className="fa-solid fa-xmark"></i>
                                </button>
                            </div>
                            
                            <div className={styles.messageList} ref={messageListRef}>
                                {messages.map((msg) => (
                                    <div 
                                        key={msg.id} 
                                        className={`${styles.messageBubble} ${msg.senderId === user?.id ? styles.msgSent : styles.msgReceived}`}
                                    >
                                        {msg.content}
                                        <span className={styles.msgTime}>{formatTime(msg.sentAt)}</span>
                                    </div>
                                ))}
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
                    ) : (
                        <>
                            <div className={styles.panelHeader}>
                                <h3><i className="fa-solid fa-comments"></i> Повідомлення</h3>
                                <button onClick={toggleChat} className={styles.closeButton}>
                                    <i className="fa-solid fa-xmark"></i>
                                </button>
                            </div>
                            <div className={styles.conversationList}>
                                {conversations.length > 0 ? (
                                    conversations.map((conv) => (
                                        <div 
                                            key={conv.id} 
                                            className={`${styles.conversationItem} ${activeConversationId === conv.id ? styles.active : ''}`}
                                            onClick={() => handleConversationClick(conv.id)}
                                        >
                                            <div className={styles.avatar}>
                                                {conv.otherUserName.charAt(0)}
                                            </div>
                                            <div className={styles.convInfo}>
                                                <div className={styles.convHeader}>
                                                    <span className={styles.convName}>{conv.otherUserName}</span>
                                                    <span className={styles.convTime}>
                                                        {conv.lastMessageAt ? new Date(conv.lastMessageAt).toLocaleDateString() : ''}
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
                        </>
                    )}
                </div>
            )}
            
            <button className={styles.chatButton} onClick={toggleChat}>
                {isChatOpen ? (
                    <i className="fa-solid fa-chevron-down"></i>
                ) : (
                    <i className="fa-solid fa-message"></i>
                )}
                {unreadTotal > 0 && !isChatOpen && (
                    <span className={styles.unreadBadge}>{unreadTotal}</span>
                )}
            </button>
        </div>
    );
};
