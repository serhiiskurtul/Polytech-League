import { useState, useEffect } from 'react';
import { getUsers, blockUser, unblockUser, deleteUser, UserResponse } from '@/shared/api/users.api';
import { getAllPlayers, deletePlayer, searchPlayers } from '@/shared/api/players.api';
import { PlayerDto } from '@/shared/types/player.types';
import { getPhotoUrl } from '@/shared/utils/photo.utils';
import { Link } from 'react-router-dom';
import styles from './AdminUsersPage.module.css';
import toast from 'react-hot-toast';

export const AdminUsersPage = () => {
  const [users, setUsers] = useState<UserResponse[]>([]);
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [activeTab, setActiveTab] = useState<'users' | 'players'>('users');
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 100;

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

  const fetchPlayers = async () => {
    setLoading(true);
    try {
      let data: PlayerDto[];
      if (searchTerm) {
        data = await searchPlayers(searchTerm);
      } else {
        data = await getAllPlayers();
      }
      // Simple client-side pagination for players if needed, 
      // but usually the list isn't massive for a start.
      setPlayers(data);
      setTotalCount(data.length);
    } catch (error) {
      console.error('Failed to fetch players:', error);
      toast.error('Не вдалося завантажити список гравців');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setPage(1);
  }, [activeTab]);

  useEffect(() => {
    const timer = setTimeout(() => {
      if (activeTab === 'users') {
        fetchUsers();
      } else {
        fetchPlayers();
      }
    }, 500);
    return () => clearTimeout(timer);
  }, [page, searchTerm, activeTab]);

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

  const handleDeletePlayer = async (id: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цього гравця? Всі його досягнення та статистика будуть втрачені.')) return;
    try {
      await deletePlayer(id);
      toast.success('Гравця видалено');
      fetchPlayers();
    } catch (error) {
      toast.error('Помилка при видаленні гравця');
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
            <div className={styles.statsBadge}>
              {activeTab === 'users' ? 'Всього акаунтів: ' : 'Всього гравців: '}
              <span>{totalCount}</span>
            </div>
          </div>
        </header>

        <div className={styles.tabs}>
          <button 
            className={`${styles.tab} ${activeTab === 'users' ? styles.activeTab : ''}`}
            onClick={() => setActiveTab('users')}
          >
            Користувачі
          </button>
          <button 
            className={`${styles.tab} ${activeTab === 'players' ? styles.activeTab : ''}`}
            onClick={() => setActiveTab('players')}
          >
            Гравці (Рейтинг)
          </button>
        </div>

        <div className={styles.controls}>
          <div className={styles.searchWrapper}>
            <i className={`fas fa-search ${styles.searchIcon}`}></i>
            <input
              type="text"
              placeholder={activeTab === 'users' ? "Пошук за ім'ям або email..." : "Пошук за ім'ям, групою або ID..."}
              className={styles.searchInput}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        <div className={styles.tableContainer}>
          {loading && page === 1 ? (
            <div className={styles.loading}>Завантаження...</div>
          ) : activeTab === 'users' ? (
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
          ) : (
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Гравець</th>
                  <th>Група</th>
                  <th>Рейтинг</th>
                  <th>Статус</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {players.slice((page - 1) * pageSize, page * pageSize).map((player) => {
                  const photoSrc = getPhotoUrl(player.photoUrl);
                  return (
                    <tr key={player.id}>
                      <td>
                        <div className={styles.userCell}>
                          {photoSrc ? (
                            <img src={photoSrc} alt="" className={styles.avatar} />
                          ) : (
                            <div className={styles.avatar}>
                              {player.firstName[0]}{player.lastName[0]}
                            </div>
                          )}
                          <div>
                            <span className={styles.userName}>{player.firstName} {player.lastName}</span>
                            <span className={styles.userEmail}>ID: {player.id}</span>
                          </div>
                        </div>
                      </td>
                      <td>{player.group || '-'}</td>
                      <td>{player.performanceScore}</td>
                      <td>
                        <span className={`${styles.roleBadge} ${player.isExternal ? styles.blockedStatus : styles.userRole}`}>
                          {player.isExternal ? 'Зовнішній' : 'Студент'}
                        </span>
                      </td>
                      <td>
                        <div className={styles.actions}>
                          <Link 
                            to={`/players/${player.id}`} 
                            className={styles.actionBtn}
                            title="Перейти в профіль"
                          >
                            <i className="fas fa-external-link-alt"></i>
                          </Link>
                          <button 
                            className={`${styles.actionBtn} ${styles.deleteBtn}`}
                            onClick={() => player.id && handleDeletePlayer(player.id)}
                            title="Видалити гравця"
                          >
                            <i className="fas fa-trash-alt"></i>
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          )}
          
          {!loading && (activeTab === 'users' ? users.length === 0 : players.length === 0) && (
            <div className={styles.loading}>{activeTab === 'users' ? 'Користувачів' : 'Гравців'} не знайдено</div>
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
