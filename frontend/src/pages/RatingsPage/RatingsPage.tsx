import React, { useState, useEffect } from 'react';
import styles from './RatingsPage.module.css';
import { getSeasons, getSeasonRating, createSeason, updateSeason, deleteSeason } from '@/shared/api/seasonRating.api';

import { Season, SeasonRatingResponse } from '@/shared/types/seasonRating.types';
import toast from 'react-hot-toast';
import * as XLSX from 'xlsx';
import { useAuth } from '@/app/providers/AuthContext';

const RatingsPage: React.FC = () => {
  const { user } = useAuth();
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';

  const [seasons, setSeasons] = useState<Season[]>([]);
  const [selectedSeasonId, setSelectedSeasonId] = useState<number | null>(null);
  const [ratingData, setRatingData] = useState<SeasonRatingResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const [showSeasonModal, setShowSeasonModal] = useState(false);
  const [editingSeason, setEditingSeason] = useState<Season | null>(null);

  useEffect(() => {
    fetchSeasons();
  }, []);

  const fetchSeasons = async () => {
    try {
      const data = await getSeasons();
      setSeasons(data);
      if (data.length > 0) {
        const current = data.find(s => s.isCurrent) || data[0];
        setSelectedSeasonId(current.id);
        fetchRating(current.id);
      } else {
        setIsLoading(false);
      }
    } catch (error) {
      console.error('Error fetching seasons:', error);
      toast.error('Не вдалося завантажити сезони');
      setIsLoading(false);
    }
  };

  const fetchRating = async (seasonId: number) => {
    setIsLoading(true);
    try {
      const data = await getSeasonRating(seasonId);
      setRatingData(data);
    } catch (error) {
      console.error('Error fetching rating:', error);
      toast.error('Не вдалося завантажити рейтинги');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSeasonChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const id = parseInt(e.target.value);
    setSelectedSeasonId(id);
    fetchRating(id);
  };

  const handleCreateSeason = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const seasonData = {
      name: formData.get('name') as string,
      startDate: new Date(formData.get('startDate') as string).toISOString(),
      endDate: new Date(formData.get('endDate') as string).toISOString(),
      isCurrent: formData.get('isCurrent') === 'on',
      spartakiadNumber: formData.get('spartakiadNumber') ? Number(formData.get('spartakiadNumber')) : undefined
    };


    try {
      if (editingSeason) {
        await updateSeason(editingSeason.id, seasonData);
        toast.success('Сезон оновлено');
      } else {
        await createSeason(seasonData);
        toast.success('Сезон створено');
      }
      setShowSeasonModal(false);
      setEditingSeason(null);
      fetchSeasons();
    } catch (error) {
      toast.error('Помилка при збереженні сезону');
    }
  };

  const handleDeleteSeason = async (id: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей сезон? Усі дані про змагання залишаться, але вони більше не будуть прив\'язані до цього сезону.')) return;
    try {
      await deleteSeason(id);
      toast.success('Сезон видалено');
      fetchSeasons();
    } catch (error) {
      toast.error('Помилка при видаленні сезону');
    }
  };



  const exportToExcel = () => {
    if (!ratingData) return;

    const headers = ['Група', ...ratingData.sports.flatMap(s => [`${s} (Очко)`, `${s} (Місце)`]), 'Загальна сума очок', 'Підсумкове місце'];
    const data = ratingData.rows.map(row => {
      const obj: any = { 'Група': row.groupName };
      ratingData.sports.forEach(s => {
        obj[`${s} (Очко)`] = row.sportResults[s]?.points || 0;
        obj[`${s} (Місце)`] = row.sportResults[s]?.rank || 0;
      });
      obj['Загальна сума очок'] = row.totalPoints;
      obj['Підсумкове місце'] = row.totalRank;
      return obj;
    });

    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Ratings");
    XLSX.writeFile(wb, `Ratings_${ratingData.seasonName}.xlsx`);
  };

  if (isLoading && !ratingData) {
    return (
      <div className={styles.container}>
        <div className={styles.loading}>
          <div className={styles.spinner}></div>
          <p>Завантаження рейтингів...</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <div className={styles.titleSection}>
          <h1>
            Рейтинги груп 
            {ratingData?.spartakiadNumber && (
              <span className={styles.spartakiadBadge}>
                {ratingData.spartakiadNumber}-та Спартакіада
              </span>
            )}
          </h1>
          <p>Підсумкова таблиця спартакіади коледжу</p>
        </div>


        <div className={styles.controls}>
          {seasons.length > 0 && (
            <select
              className={styles.seasonSelect}
              value={selectedSeasonId || ''}
              onChange={handleSeasonChange}
            >
              {seasons.map(s => (
                <option key={s.id} value={s.id}>
                  Сезон {s.name} {s.isCurrent ? '(Поточний)' : ''}
                </option>
              ))}
            </select>
          )}

          {isAdmin && (
            <button className={styles.manageBtn} onClick={() => { setEditingSeason(null); setShowSeasonModal(true); }}>
              <i className="fas fa-calendar-plus"></i> Керування сезонами
            </button>
          )}

          {ratingData && ratingData.rows.length > 0 && (
            <>
              <div className={styles.legend}>
                <div className={styles.legendItem}>
                  <span className={`${styles.dot} ${styles.scoreDot}`}></span> О - Очки
                </div>
                <div className={styles.legendItem}>
                  <span className={`${styles.dot} ${styles.rankDot}`}></span> М - Місце
                </div>
              </div>
              <button className={`${styles.exportBtn} ${styles.excel}`} onClick={exportToExcel}>
                <i className="fas fa-file-excel"></i> Excel
              </button>
            </>
          )}
        </div>
      </header>

      {seasons.length === 0 ? (
        <div className={styles.emptyState}>
          <i className="fas fa-calendar-times"></i>
          <h3>Сезонів не знайдено</h3>
          {isAdmin ? (
            <>
              <p>Будь ласка, створіть перший сезон, щоб почати відстежувати рейтинги груп.</p>
              <button className={styles.createBtn} onClick={() => setShowSeasonModal(true)}>
                Створити сезон
              </button>
            </>
          ) : (
            <p>Наразі немає активних або минулих сезонів.</p>
          )}
        </div>
      ) : ratingData && ratingData.rows.length > 0 ? (
        <div className={styles.tableContainer}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th rowSpan={2} className={styles.mainHeader}>Група</th>
                {ratingData.sports.map(sport => (
                  <th key={sport} colSpan={2} className={styles.mainHeader}>{sport}</th>
                ))}
                <th colSpan={2} className={styles.mainHeader}>Командний результат</th>
              </tr>
              <tr>
                {ratingData.sports.map(sport => (
                  <React.Fragment key={`${sport}-sub`}>
                    <th className={styles.subHeader}>О</th>
                    <th className={styles.subHeader}>М</th>
                  </React.Fragment>
                ))}
                <th className={styles.subHeader}>О</th>
                <th className={styles.subHeader}>М</th>
              </tr>
            </thead>
            <tbody>
              {ratingData.rows.map((row, idx) => (
                <tr key={row.groupName} className={styles.row}>
                  <td className={styles.groupCell}>{row.groupName}</td>
                  {ratingData.sports.map(sport => (
                    <React.Fragment key={`${row.groupName}-${sport}`}>
                      <td className={styles.scoreCell}>{row.sportResults[sport]?.points || '-'}</td>
                      <td className={styles.rankCell}>{row.sportResults[sport]?.rank || '-'}</td>
                    </React.Fragment>
                  ))}
                  <td className={styles.totalPointsCell}>{row.totalPoints}</td>
                  <td className={styles.totalRankCell}>{row.totalRank}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <div className={styles.emptyState}>
          <i className="fas fa-users-slash"></i>
          <h3>Немає даних для цього сезону</h3>
          <p>Схоже, що в цьому сезоні ще не було змагань для коледжних груп або результати ще не внесено.</p>
        </div>
      )}

      {showSeasonModal && isAdmin && (
        <div className={styles.modalOverlay}>
          <div className={styles.modal}>
            <div className={styles.modalHeader}>
              <h2>Керування сезонами</h2>
              <button className={styles.closeBtn} onClick={() => setShowSeasonModal(false)}>
                <i className="fas fa-times"></i>
              </button>
            </div>

            <div className={styles.seasonsList}>
              <h3>Список сезонів</h3>
              <div className={styles.listContainer}>
                {seasons.map(s => (
                  <div key={s.id} className={`${styles.seasonItem} ${editingSeason?.id === s.id ? styles.editing : ''}`}>
                    <div className={styles.seasonInfo}>
                      <span className={styles.seasonName}>{s.name}</span>
                      {s.isCurrent && <span className={styles.currentBadge}>Поточний</span>}
                      {s.spartakiadNumber && <span className={styles.spartakiadInfo}>{s.spartakiadNumber}-та Спартакіада</span>}
                    </div>
                    <div className={styles.seasonActions}>
                      <button onClick={() => setEditingSeason(s)} className={styles.editBtn} title="Редагувати">
                        <i className="fas fa-edit"></i>
                      </button>
                      <button onClick={() => handleDeleteSeason(s.id)} className={styles.deleteBtn} title="Видалити">
                        <i className="fas fa-trash"></i>
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className={styles.divider}></div>

            <div className={styles.formSection}>
              <h3>{editingSeason ? 'Редагувати сезон' : 'Додати новий сезон'}</h3>
              <form onSubmit={handleCreateSeason} key={editingSeason?.id || 'new'}>

              <div className={styles.formGroup}>
                <label>Назва сезону (напр. 2023-2024)</label>
                <input name="name" required defaultValue={editingSeason?.name} placeholder="2023-2024" />
              </div>
              <div className={styles.formGroup}>
                <label>Дата початку</label>
                <input name="startDate" type="date" required defaultValue={editingSeason?.startDate?.split('T')[0]} />
              </div>
              <div className={styles.formGroup}>
                <label>Дата завершення</label>
                <input name="endDate" type="date" required defaultValue={editingSeason?.endDate?.split('T')[0]} />
              </div>
              <div className={styles.formGroupCheckbox}>
                <input name="isCurrent" type="checkbox" defaultChecked={editingSeason?.isCurrent} />
                <label>Встановити як поточний сезон</label>
              </div>
              <div className={styles.formGroup}>
                <label>Номер Спартакіади</label>
                <input name="spartakiadNumber" type="number" defaultValue={editingSeason?.spartakiadNumber} />
              </div>


              <div className={styles.modalActions}>
                {editingSeason && (
                  <button type="button" onClick={() => setEditingSeason(null)} className={styles.cancelBtn}>
                    Скасувати редагування
                  </button>
                )}
                <button type="submit" className={styles.saveBtn}>
                  {editingSeason ? 'Оновити сезон' : 'Створити сезон'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      )}

    </div>
  );
};

export default RatingsPage;
