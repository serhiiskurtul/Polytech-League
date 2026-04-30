import { useState, useEffect } from 'react';
import { getUsers, blockUser, unblockUser, deleteUser, UserResponse } from '@/shared/api/users.api';
import styles from './AdminUsersPage.module.css';
import toast from 'react-hot-toast';

export const AdminUsersPage = () => {
  const [users, setUsers] = useState<UserResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 10;

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const response = await getUsers(page, pageSize, searchTerm);
      setUsers(response.data);
      setTotalCount(response.totalCount);
    } catch (error) {
      console.error('Failed to fetch users:', error);
      toast.error('Не вдалося завантажити список користувачів');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      fetchUsers();
    }, 500);
    return () => clearTimeout(timer);
  }, [page, searchTerm]);

  const handleBlock = async (id: number, isBlocked: boolean) => {
    try {
      if (isBlocked) {
        await unblockUser(id);
        toast.success('Користувача розблоковано');
      } else {
        if (!window.confirm('Ви впевнені, що хочете заблокувати цього користувача? Він не зможе увійти в систему.')) return;
        await blockUser(id);
        toast.success('Користувача заблоковано');
      }
      fetchUsers();
    } catch (error) {
      toast.error('Помилка при зміні статусу користувача');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Ви впевнені, що хочете ПОВНІСТЮ видалити цей аккаунт? Цю дію неможливо скасувати.')) return;
    try {
      await deleteUser(id);
      toast.success('Аккаунт видалено');
      fetchUsers();
    } catch (error) {
      toast.error('Помилка при видаленні аккаунту');
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className={styles.adminUsersPage}>
      <div className={styles.container}>
        <header className={styles.header}>
          <div className={styles.titleSection}>
            <h1>Керування користувачами</h1>
            <p>Перегляд, блокування та видалення облікових записів</p>
          </div>
        </header>

        <div className={styles.controls}>
          <div className={styles.searchWrapper}>
            <i className={`fas fa-search ${styles.searchIcon}`}></i>
            <input
              type="text"
              placeholder="Пошук за ім'ям або email..."
              className={styles.searchInput}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        <div className={styles.tableContainer}>
          {loading && page === 1 ? (
            <div className={styles.loading}>Завантаження...</div>
          ) : (
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Користувач</th>
                  <th>Роль</th>
                  <th>Статус</th>
                  <th>Дата реєстрації</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.id}>
                    <td>
                      <div className={styles.userCell}>
                        <div className={styles.avatar}>
                          {user.firstName[0]}{user.lastName[0]}
                        </div>
                        <div>
                          <span className={styles.userName}>{user.firstName} {user.lastName}</span>
                          <span className={styles.userEmail}>{user.email}</span>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span className={`${styles.roleBadge} ${user.role === 1 || user.role === 'Admin' ? styles.adminRole : styles.userRole}`}>
                        {user.role === 1 || user.role === 'Admin' ? 'Адмін' : 'Користувач'}
                      </span>
                    </td>
                    <td>
                      <span className={`${styles.statusBadge} ${user.isBlocked ? styles.blockedStatus : styles.activeStatus}`}>
                        {user.isBlocked ? 'Заблокований' : 'Активний'}
                      </span>
                    </td>
                    <td>
                      {user.createdAt ? new Intl.DateTimeFormat('uk-UA', { day: '2-digit', month: 'long', year: 'numeric' }).format(new Date(user.createdAt)) : '-'}
                    </td>
                    <td>
                      <div className={styles.actions}>
                        <button 
                          className={`${styles.actionBtn} ${user.isBlocked ? styles.unblockBtn : styles.blockBtn}`}
                          onClick={() => handleBlock(user.id, user.isBlocked)}
                          title={user.isBlocked ? "Розблокувати" : "Заблокувати"}
                        >
                          <i className={`fas ${user.isBlocked ? 'fa-unlock' : 'fa-ban'}`}></i>
                        </button>
                        <button 
                          className={`${styles.actionBtn} ${styles.deleteBtn}`}
                          onClick={() => handleDelete(user.id)}
                          title="Видалити назавжди"
                        >
                          <i className="fas fa-trash-alt"></i>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
          
          {!loading && users.length === 0 && (
            <div className={styles.loading}>Користувачів не знайдено</div>
          )}
        </div>

        {totalPages > 1 && (
          <div className={styles.pagination}>
            <button 
              className={styles.pageBtn}
              disabled={page === 1}
              onClick={() => setPage(p => p - 1)}
            >
              <i className="fas fa-chevron-left"></i>
            </button>
            <span>Сторінка {page} з {totalPages}</span>
            <button 
              className={styles.pageBtn}
              disabled={page === totalPages}
              onClick={() => setPage(p => p + 1)}
            >
              <i className="fas fa-chevron-right"></i>
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
