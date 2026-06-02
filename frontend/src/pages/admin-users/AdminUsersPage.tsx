import { useState, useEffect } from 'react';
import { getUsers, blockUser, unblockUser, deleteUser, changeRole, UserResponse } from '@/entities/user/api/users.api';
import { getAllPlayers, deletePlayer, searchPlayers, getPlayersRankings, mergePlayersManually } from '@/entities/player/api/players.api';
import { PlayerDto } from '@/entities/player/model/player.types';
import { getPhotoUrl } from '@/shared/utils/photo.utils';
import { useAuth } from '@/entities/session';
import { Link } from 'react-router-dom';
import styles from './AdminUsersPage.module.css';
import toast from 'react-hot-toast';

export const AdminUsersPage = () => {
  const [users, setUsers] = useState<UserResponse[]>([]);
  const { user: currentUser } = useAuth();
  const [players, setPlayers] = useState<PlayerDto[]>([]);
  const [activeTab, setActiveTab] = useState<'users' | 'players'>('users');
  const [showDuplicatesOnly, setShowDuplicatesOnly] = useState(false);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [userSortBy, setUserSortBy] = useState<string | undefined>(undefined);
  const [userSortDirection, setUserSortDirection] = useState<'Asc' | 'Desc' | undefined>(undefined);
  const pageSize = 100;

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const currentPageSize = showDuplicatesOnly ? 10000 : pageSize;
      const currentPage = showDuplicatesOnly ? 1 : page;
      const response = await getUsers(currentPage, currentPageSize, searchTerm, userSortBy, userSortDirection);
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
      // 1. Fetch base player data (either searched or all)
      let basePlayers: PlayerDto[];
      if (searchTerm) {
        basePlayers = await searchPlayers(searchTerm);
      } else {
        basePlayers = await getAllPlayers();
      }
      
      // 2. Fetch players with rankings to get calculated scores
      const rankedPlayers = await getPlayersRankings();
      
      // 3. Create a map of scores for quick lookup
      const scoresMap = new Map<number, number>();
      rankedPlayers.forEach(p => {
        if (p.id) scoresMap.set(p.id, p.performanceScore || 0);
      });
      
      // 4. Merge scores into the base list
      const mergedData = basePlayers.map(p => ({
        ...p,
        performanceScore: p.id ? (scoresMap.get(p.id) || 0) : 0
      }));
      
      setPlayers(mergedData);
      setTotalCount(mergedData.length);
    } catch (error) {
      console.error('Failed to fetch players:', error);
      toast.error('Не вдалося завантажити список гравців');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setPage(1);
    setShowDuplicatesOnly(false);
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
  }, [page, searchTerm, activeTab, showDuplicatesOnly, userSortBy, userSortDirection]);

  const handleSort = (field: string) => {
    if (userSortBy === field) {
      if (userSortDirection === 'Asc') {
        setUserSortDirection('Desc');
      } else if (userSortDirection === 'Desc') {
        setUserSortBy(undefined);
        setUserSortDirection(undefined);
      }
    } else {
      setUserSortBy(field);
      setUserSortDirection('Asc');
    }
  };

  const getSortIcon = (field: string) => {
    if (userSortBy !== field) return <i className="fas fa-sort" style={{ opacity: 0.3, marginLeft: '5px' }}></i>;
    return userSortDirection === 'Asc' 
      ? <i className="fas fa-sort-up" style={{ marginLeft: '5px' }}></i>
      : <i className="fas fa-sort-down" style={{ marginLeft: '5px' }}></i>;
  };

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

  const handleChangeRole = async (id: number, newRole: number) => {
    try {
      await changeRole(id, newRole);
      toast.success('Роль змінено');
      fetchUsers();
    } catch (error: any) {
      toast.error(error.response?.data || 'Помилка при зміні ролі');
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

  const handleMergePlayers = async (sourceId: number) => {
    const targetIdStr = prompt(
      `Об'єднання гравців.\nВи дійсно хочете ПЕРЕНЕСТИ всі дані з цього гравця (ID: ${sourceId})?\nВведіть ID цільового гравця (який залишиться і отримає всі досягнення та прив'язку до акаунту):`
    );
    if (!targetIdStr) return;
    const targetId = parseInt(targetIdStr.trim(), 10);
    if (isNaN(targetId)) {
      toast.error('Некоректний ID цільового гравця');
      return;
    }
    if (sourceId === targetId) {
      toast.error('Не можна об\'єднати гравця з самим собою');
      return;
    }
    if (!window.confirm(`Ви дійсно хочете перенести всі дані гравця з ID ${sourceId} в гравця з ID ${targetId}?\nГравець ${sourceId} буде видалений.`)) {
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
              : (data?.detail || data?.title || 'Не вдалося об\'єднати гравців');
          }
        }
      );
      fetchPlayers();
    } catch (error: any) {
      // Error is handled by toast.promise
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

        <div className={styles.tabsRow}>
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

          <button 
            className={`${styles.duplicateBtn} ${showDuplicatesOnly ? styles.duplicateBtnActive : ''}`}
            onClick={() => setShowDuplicatesOnly(!showDuplicatesOnly)}
            title={activeTab === 'users' ? "Показати користувачів з однаковими іменами" : "Показати гравців з однаковими іменами"}
          >
            <i className="fas fa-copy"></i> {showDuplicatesOnly ? (activeTab === 'users' ? 'Всі користувачі' : 'Всі гравці') : 'Знайти дублікати'}
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
                  <th onClick={() => handleSort('FirstName')} style={{ cursor: 'pointer', userSelect: 'none' }}>
                    Користувач {getSortIcon('FirstName')}
                  </th>
                  <th>Пошта підтверджена</th>
                  <th>Роль</th>
                  <th>Статус</th>
                  <th onClick={() => handleSort('CreatedAt')} style={{ cursor: 'pointer', userSelect: 'none' }}>
                    Дата реєстрації {getSortIcon('CreatedAt')}
                  </th>
                  <th onClick={() => handleSort('LastVisitAt')} style={{ cursor: 'pointer', userSelect: 'none' }}>
                    Останній візит {getSortIcon('LastVisitAt')}
                  </th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {(() => {
                  let displayUsers = [...users];
                  if (showDuplicatesOnly) {
                    const nameCounts = new Map<string, number>();
                    displayUsers.forEach(u => {
                      const key = `${u.firstName.trim().toLowerCase()}_${u.lastName.trim().toLowerCase()}`;
                      nameCounts.set(key, (nameCounts.get(key) || 0) + 1);
                    });
                    displayUsers = displayUsers.filter(u => {
                      const key = `${u.firstName.trim().toLowerCase()}_${u.lastName.trim().toLowerCase()}`;
                      return (nameCounts.get(key) || 0) > 1;
                    }).sort((a, b) => {
                      const keyA = `${a.firstName.trim().toLowerCase()}_${a.lastName.trim().toLowerCase()}`;
                      const keyB = `${b.firstName.trim().toLowerCase()}_${b.lastName.trim().toLowerCase()}`;
                      return keyA.localeCompare(keyB);
                    });
                  }

                  return displayUsers.slice(showDuplicatesOnly ? 0 : 0, showDuplicatesOnly ? displayUsers.length : displayUsers.length).map((user) => (
                    <tr key={user.id} className={showDuplicatesOnly ? styles.duplicateRow : ''}>
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
                        <span className={`${styles.statusBadge} ${user.isEmailConfirmed ? styles.verifiedStatus : styles.unverifiedStatus}`}>
                          {user.isEmailConfirmed ? 'Підтверджена' : 'Не підтверджена'}
                        </span>
                      </td>
                      <td>
                        <select
                          className={styles.roleSelect}
                          value={user.role === 'Admin' ? 1 : user.role === 'Fizorg' ? 2 : user.role}
                          onChange={(e) => handleChangeRole(user.id, parseInt(e.target.value))}
                          disabled={user.id === currentUser?.id}
                          title={user.id === currentUser?.id ? "Ви не можете змінити власну роль" : ""}
                          style={{
                            padding: '4px 8px',
                            borderRadius: '6px',
                            border: '1px solid var(--border)',
                            background: 'var(--surface-light)',
                            color: 'var(--text-primary)',
                            fontSize: '0.875rem'
                          }}
                        >
                          <option value={0}>Користувач</option>
                          <option value={2}>Фізорг</option>
                          {(user.role === 'Admin' || user.role === 1) && (
                            <option value={1}>Адмін</option>
                          )}
                        </select>
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
                        {user.lastVisitAt ? new Intl.DateTimeFormat('uk-UA', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }).format(new Date(user.lastVisitAt)) : '-'}
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
                  ));
                })()}
              </tbody>
            </table>
          ) : (
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>Гравець</th>
                  <th>Група</th>
                  <th>Рейтинг</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody>
                {(() => {
                  let displayPlayers = [...players];
                  if (showDuplicatesOnly) {
                    const nameCounts = new Map<string, number>();
                    displayPlayers.forEach(p => {
                      const key = `${p.firstName.toLowerCase()}_${p.lastName.toLowerCase()}`;
                      nameCounts.set(key, (nameCounts.get(key) || 0) + 1);
                    });
                    displayPlayers = displayPlayers.filter(p => {
                      const key = `${p.firstName.toLowerCase()}_${p.lastName.toLowerCase()}`;
                      return (nameCounts.get(key) || 0) > 1;
                    }).sort((a, b) => {
                      const keyA = `${a.firstName.toLowerCase()}_${a.lastName.toLowerCase()}`;
                      const keyB = `${b.firstName.toLowerCase()}_${b.lastName.toLowerCase()}`;
                      return keyA.localeCompare(keyB);
                    });
                  }

                  return displayPlayers.slice((page - 1) * pageSize, page * pageSize).map((player) => {
                    const photoSrc = getPhotoUrl(player.photoUrl);
                    return (
                      <tr key={player.id} className={showDuplicatesOnly ? styles.duplicateRow : ''}>
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
                              <Link to={`/players/${player.id}`} className={styles.playerLink}>
                                <span className={styles.userName}>{player.firstName} {player.lastName}</span>
                              </Link>
                              <span className={styles.userEmail}>
                                ID: {player.id}
                                {player.userEmail && (
                                  <>
                                    <span style={{ margin: '0 6px', opacity: 0.5 }}>•</span>
                                    <i className="fa-regular fa-envelope" style={{ marginRight: '4px', opacity: 0.7 }}></i>
                                    {player.userEmail}
                                  </>
                                )}
                              </span>
                            </div>
                          </div>
                        </td>
                        <td>{player.group || '-'}</td>
                        <td>{player.performanceScore}</td>
                        <td>
                          <div className={styles.actions}>
                            <button 
                              className={styles.actionBtn}
                              onClick={() => player.id && handleMergePlayers(player.id)}
                              title="Об'єднати з іншим гравцем"
                              style={{ color: '#3b82f6' }}
                            >
                              <i className="fas fa-compress-alt"></i>
                            </button>
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
                  });
                })()}
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
