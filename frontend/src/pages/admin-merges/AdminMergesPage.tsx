import { useState, useEffect } from 'react';
import { getPendingMergeRequests, approveMergeRequest, rejectMergeRequest, getAllMergeRequests, mergePlayersManually } from '@/entities/player/api/players.api';
import { toast } from 'react-hot-toast';
import styles from './AdminMergesPage.module.css';

export const AdminMergesPage = () => {
    const [requests, setRequests] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showHistory, setShowHistory] = useState(false);

    const fetchRequests = async () => {
        try {
            setLoading(true);
            const data = showHistory ? await getAllMergeRequests() : await getPendingMergeRequests();
            setRequests(data);
        } catch (error) {
            toast.error('Не вдалося завантажити запити');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRequests();
    }, [showHistory]);

    const handleApprove = async (id: number) => {
        try {
            await approveMergeRequest(id);
            toast.success('Запит схвалено');
            fetchRequests();
        } catch (error) {
            toast.error('Помилка при схваленні');
        }
    };

    const handleReject = async (id: number) => {
        const comment = prompt('Вкажіть причину відхилення:');
        if (comment === null) return;
        try {
            await rejectMergeRequest(id, comment);
            toast.success('Запит відхилено');
            fetchRequests();
        } catch (error) {
            toast.error('Помилка при відхиленні');
        }
    };

    const handleManualMerge = async () => {
        const sourceIdStr = prompt("Введіть ID вихідного гравця (який буде видалений, а його дані перенесені):");
        if (!sourceIdStr) return;
        const sourceId = parseInt(sourceIdStr.trim(), 10);
        if (isNaN(sourceId)) {
            toast.error("Некоректний ID вихідного гравця");
            return;
        }

        const targetIdStr = prompt("Введіть ID цільового гравця (який залишиться і отримає всі досягнення та прив'язку до акаунту):");
        if (!targetIdStr) return;
        const targetId = parseInt(targetIdStr.trim(), 10);
        if (isNaN(targetId)) {
            toast.error("Некоректний ID цільового гравця");
            return;
        }

        if (sourceId === targetId) {
            toast.error("Неможливо об'єднати гравця з самим собою");
            return;
        }

        if (!window.confirm(`Ви впевнені, що хочете об'єднати гравця ${sourceId} з гравцем ${targetId}?\nВсі дані з ${sourceId} перейдуть до ${targetId}, а профіль ${sourceId} буде видалено.`)) {
            return;
        }

        try {
            await toast.promise(
                mergePlayersManually(sourceId, targetId),
                {
                    loading: 'Об\'єднання гравців... Будь ласка, зачекайте, це може зайняти деякий час.',
                    success: 'Гравців успішно об\'єднано!',
                    error: (error: any) => {
                        console.error(error);
                        const data = error.response?.data;
                        return typeof data === 'string'
                            ? data
                            : (data?.detail || data?.title || "Не вдалося об'єднати гравців");
                    }
                }
            );
            fetchRequests();
        } catch (error: any) {
            // Error is handled by toast.promise
        }
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'Approved': return <span className={styles.statusApproved}>Схвалено</span>;
            case 'Rejected': return <span className={styles.statusRejected}>Відхилено</span>;
            case 'Ignored': return <span className={styles.statusIgnored}>Проігноровано</span>;
            default: return <span className={styles.statusPending}>В очікуванні</span>;
        }
    };

    return (
        <div className="page-wrapper">
            <div className="container">
                <div className={styles.headerRow}>
                    <h1 style={{ color: '#1f2937', margin: 0 }}>
                        <i className="fas fa-users-cog" style={{ marginRight: '15px', color: '#3b82f6' }}></i>
                        Запити на об'єднання профілів
                    </h1>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
                        <button className={styles.manualMergeBtn} onClick={handleManualMerge}>
                            <i className="fas fa-compress-alt"></i> Об'єднати вручну
                        </button>
                        <div className={styles.viewToggle}>
                            <button 
                                className={!showHistory ? styles.activeTab : ''} 
                                onClick={() => setShowHistory(false)}
                            >
                                Активні
                            </button>
                            <button 
                                className={showHistory ? styles.activeTab : ''} 
                                onClick={() => setShowHistory(true)}
                            >
                                Історія
                            </button>
                        </div>
                    </div>
                </div>

                {loading ? (
                   <div style={{ textAlign: 'center', padding: '50px' }}>Завантаження...</div>
                ) : requests.length === 0 ? (
                    <div style={{ textAlign: 'center', padding: '100px 20px', background: 'white', borderRadius: '15px', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}>
                        <i className="fas fa-check-circle" style={{ fontSize: '64px', color: '#10b981', marginBottom: '20px' }}></i>
                        <p style={{ fontSize: '18px', color: '#6b7280' }}>
                            {showHistory ? 'Історія порожня' : 'Немає активних запитів на об\'єднання'}
                        </p>
                    </div>
                ) : (
                    <div className={styles.requestList}>
                        {requests.map(req => (
                            <div key={req.id} className={styles.requestCard}>
                                <div className={styles.cardHeader}>
                                    <span className={styles.date}>{new Date(req.createdAt).toLocaleDateString()}</span>
                                    {getStatusBadge(req.status)}
                                </div>
                                <div className={styles.requestInfo}>
                                    <div className={styles.party}>
                                        <span className={styles.label}>Користувач (Аккаунт):</span>
                                        <span className={styles.value}>{req.userFullName}</span>
                                    </div>
                                    <div className={styles.arrow}>
                                        <i className="fas fa-arrow-right"></i>
                                    </div>
                                    <div className={styles.party}>
                                        <span className={styles.label}>Архівний профіль:</span>
                                        <span className={styles.value}>{req.targetPlayerName}</span>
                                        <span className={styles.subValue}>Група: {req.targetPlayerGroup}</span>
                                    </div>
                                </div>
                                {req.status === 'Pending' ? (
                                    <div className={styles.actions}>
                                        <button onClick={() => handleApprove(req.id)} className={styles.approveBtn}>
                                            <i className="fas fa-check"></i> Схвалити
                                        </button>
                                        <button onClick={() => handleReject(req.id)} className={styles.rejectBtn}>
                                            <i className="fas fa-times"></i> Відхилити
                                        </button>
                                    </div>
                                ) : req.adminComment && (
                                    <div className={styles.comment}>
                                        <strong>Коментар адміна:</strong> {req.adminComment}
                                    </div>
                                )}
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};
