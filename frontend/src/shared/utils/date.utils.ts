export const formatMessageDate = (dateStr: string | Date) => {
    const date = new Date(dateStr);
    const now = new Date();
    
    const isToday = date.toDateString() === now.toDateString();
    
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    const isYesterday = date.toDateString() === yesterday.toDateString();
    
    const timeStr = date.toLocaleTimeString('uk-UA', { hour: '2-digit', minute: '2-digit' });
    
    if (isToday) {
        return timeStr;
    }
    
    if (isYesterday) {
        return `Вчора, ${timeStr}`;
    }
    
    const currentYear = now.getFullYear();
    const messageYear = date.getFullYear();
    
    if (currentYear === messageYear) {
        return `${date.toLocaleDateString('uk-UA', { day: '2-digit', month: '2-digit' })} ${timeStr}`;
    }

    return `${date.toLocaleDateString('uk-UA', { day: '2-digit', month: '2-digit', year: 'numeric' })} ${timeStr}`;
};

export const formatConversationDate = (dateStr: string | Date) => {
    const date = new Date(dateStr);
    const now = new Date();
    const isToday = date.toDateString() === now.toDateString();
    
    if (isToday) {
        return date.toLocaleTimeString('uk-UA', { hour: '2-digit', minute: '2-digit' });
    }
    
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    const isYesterday = date.toDateString() === yesterday.toDateString();
    
    if (isYesterday) {
        return 'Вчора';
    }
    
    return date.toLocaleDateString('uk-UA', { day: '2-digit', month: '2-digit' });
};
