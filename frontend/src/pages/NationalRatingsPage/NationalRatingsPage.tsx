import React, { useState, useEffect } from 'react';
import styles from './NationalRatingsPage.module.css';
import { getSeasons, getNationalRating } from '@/shared/api/seasonRating.api';
import { Season, SeasonRatingResponse } from '@/shared/types/seasonRating.types';
import { useAuth } from '@/app/providers/AuthContext';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import * as XLSX from 'xlsx';

const NationalRatingsPage: React.FC = () => {
  const { user } = useAuth();
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';
  
  const [seasons, setSeasons] = useState<Season[]>([]);
  const [selectedSeasonId, setSelectedSeasonId] = useState<number | null>(null);
  const [ratingData, setRatingData] = useState<SeasonRatingResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);

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
      const data = await getNationalRating(seasonId);
      setRatingData(data);
    } catch (error) {
      console.error('Error fetching national rating:', error);
      toast.error('Не вдалося завантажити всеукраїнські рейтинги');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSeasonChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const id = parseInt(e.target.value);
    setSelectedSeasonId(id);
    fetchRating(id);
  };

  const exportToExcel = () => {
    if (!ratingData) return;

    const headers = ['Коледж', ...ratingData.sports.flatMap(s => [`${s} (Очко)`, `${s} (Місце)`]), 'Загальна сума очок', 'Підсумкове місце'];
    const data = ratingData.rows.map(row => {
      const obj: any = { 'Коледж': row.groupName };
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
    XLSX.utils.book_append_sheet(wb, ws, "NationalRatings");
    XLSX.writeFile(wb, `National_Ratings_${ratingData.seasonName}.xlsx`);
  };

  if (isLoading && !ratingData) {
    return (
      <div className={styles.container}>
        <div className={styles.loading}>
          <div className={styles.spinner}></div>
          <p>Завантаження всеукраїнських рейтингів...</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <div className={styles.titleSection}>
          <h1>
            Всеукраїнський залік 
            <span className={styles.nationalBadge}>National</span>
          </h1>
          <p>Загальний облік результатів змагань між коледжами</p>
        </div>

        <div className={styles.controls}>
          {isAdmin && (
            <Link to="/admin/colleges" className={styles.manageBtn}>
              <i className="fas fa-edit"></i> Керування коледжами
            </Link>
          )}
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
          <p>Дані про всеукраїнські змагання з'являться після створення першого сезону.</p>
        </div>
      ) : ratingData && ratingData.rows.length > 0 ? (
        <div className={styles.tableContainer}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th rowSpan={2} className={styles.mainHeader}>Коледж / Навчальний заклад</th>
                {ratingData.sports.map(sport => (
                  <th key={sport} colSpan={2} className={styles.mainHeader}>{sport}</th>
                ))}
                <th colSpan={2} className={styles.mainHeader}>Підсумковий результат</th>
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
              {ratingData.rows.map((row) => (
                <tr key={row.groupName} className={styles.row}>
                  <td className={styles.collegeCell}>
                    <div className={styles.collegeInfo}>
                        <div className={styles.collegeName}>{row.groupName}</div>
                    </div>
                  </td>
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
          <i className="fas fa-university"></i>
          <h3>Немає даних для цього сезону</h3>
          <p>Схоже, що в цьому сезоні ще не було всеукраїнських змагань або результати ще не внесено.</p>
        </div>
      )}
    </div>
  );
};

export default NationalRatingsPage;
