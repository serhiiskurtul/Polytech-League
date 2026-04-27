import { useState, useEffect } from 'react';
import { getPendingMergeRequests, approveMergeRequest, rejectMergeRequest } from '@/shared/api/players.api';
import { toast } from 'react-hot-toast';
import styles from './AdminMergesPage.module.css';

export const AdminMergesPage = () => {
    const [requests, setRequests] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchRequests = async () => {
        try {
            const data = await getPendingMergeRequests();
            setRequests(data);
        } catch (error) {
            toast.error('Не вдалося завантажити запити');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchRequests();
    }, []);

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

    if (loading) return <div className="page-wrapper">Завантаження...</div>;

    return (
        <div className="page-wrapper">
            <div className="container">
                <div className="header-row">
                    <h1 style={{ color: '#1f2937', marginBottom: '30px' }}>
                        <i className="fas fa-users-cog" style={{ marginRight: '15px', color: '#3b82f6' }}></i>
                        Запити на об'єднання профілів
                    </h1>
                </div>

                {requests.length === 0 ? (
                    <div style={{ textAlign: 'center', padding: '100px 20px', background: 'white', borderRadius: '15px', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}>
                        <i className="fas fa-check-circle" style={{ fontSize: '64px', color: '#10b981', marginBottom: '20px' }}></i>
                        <p style={{ fontSize: '18px', color: '#6b7280' }}>Немає активних запитів на об'єднання</p>
                    </div>
                ) : (
                    <div className={styles.requestList}>
                        {requests.map(req => (
                            <div key={req.id} className={styles.requestCard}>
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
                                <div className={styles.actions}>
                                    <button onClick={() => handleApprove(req.id)} className={styles.approveBtn}>
                                        <i className="fas fa-check"></i> Схвалити
                                    </button>
                                    <button onClick={() => handleReject(req.id)} className={styles.rejectBtn}>
                                        <i className="fas fa-times"></i> Відхилити
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};
