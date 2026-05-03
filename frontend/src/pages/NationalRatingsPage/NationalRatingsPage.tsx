import React, { useState, useEffect } from 'react';
import styles from './NationalRatingsPage.module.css';
import { getSeasons, getNationalRating, updateNationalRating } from '@/shared/api/seasonRating.api';
import { getColleges, College } from '@/shared/api/colleges.api';
import { Season, SeasonRatingResponse } from '@/shared/types/seasonRating.types';
import { useAuth } from '@/app/providers/AuthContext';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import * as XLSX from 'xlsx';

const ALL_SPORTS = [
  "Шахи", "Шашки", "Футзал", "Армреслінг", 
  "Настільний теніс", "Бадмінтон", "Волейбол", 
  "Баскетбол 3x3", "Пляжний волейбол" 
];

const NationalRatingsPage: React.FC = () => {
  const { user } = useAuth();
  const isAdmin = user?.role === 1 || user?.role === '1' || user?.role === 'Admin';
  
  const [seasons, setSeasons] = useState<Season[]>([]);
  const [colleges, setColleges] = useState<College[]>([]);
  const [selectedSeasonId, setSelectedSeasonId] = useState<number | null>(null);
  const [ratingData, setRatingData] = useState<SeasonRatingResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [editData, setEditData] = useState<any[]>([]);
  const [activeSports, setActiveSports] = useState<string[]>([]);
  const [showFullName, setShowFullName] = useState(false);

  useEffect(() => {
    fetchInitialData();
  }, []);

  const fetchInitialData = async () => {
    try {
      const [seasonsData, collegesData] = await Promise.all([getSeasons(), getColleges()]);
      setSeasons(seasonsData);
      setColleges(collegesData);
      if (seasonsData.length > 0) {
        const current = seasonsData.find(s => s.isCurrent) || seasonsData[0];
        setSelectedSeasonId(current.id);
        fetchRating(current.id);
      } else {
        setIsLoading(false);
      }
    } catch (error) {
      console.error('Error fetching initial data:', error);
      toast.error('Не вдалося завантажити дані');
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
    setIsEditing(false);
  };

  const startEditing = () => {
    if (!ratingData) return;
    
    // Prepare edit data: for each college, find if it has an entry or default to 0/-
    const initialEditData = colleges.map(college => {
      const row = ratingData.rows.find(r => r.collegeId === college.id);
      
      const sportResults: Record<string, string> = {};
      if (row?.sportResults) {
        Object.keys(row.sportResults).forEach(sport => {
          sportResults[sport] = row.sportResults[sport].rank?.toString() || '-';
        });
      }

      return {
        collegeId: college.id,
        collegeName: college.shortName || college.name,
        region: college.region || '—',
        participated: !!row,
        points: row?.totalPoints || 0,
        rank: row?.totalRank || '-',
        sportResults: sportResults
      };
    });
    
    const sortedEditData = initialEditData.sort((a, b) => {
      // Participating colleges come first
      if (a.participated && !b.participated) return -1;
      if (!a.participated && b.participated) return 1;

      // If both are participating, sort by rank
      if (a.participated && b.participated) {
        if (a.rank === '-' && b.rank === '-') return a.collegeName.localeCompare(b.collegeName);
        if (a.rank === '-') return 1;
        if (b.rank === '-') return -1;

        const rankA = parseInt(a.rank.toString().split('-')[0]);
        const rankB = parseInt(b.rank.toString().split('-')[0]);

        if (!isNaN(rankA) && !isNaN(rankB)) return rankA - rankB;
      }

      // Default to alphabetical by name
      return a.collegeName.localeCompare(b.collegeName);
    });

    setEditData(sortedEditData);
    setActiveSports(ratingData.sports || []);
    setIsEditing(true);
  };

  const handleEditChange = (collegeId: number, field: string, value: any) => {
    setEditData(prev => prev.map(item => 
      item.collegeId === collegeId ? { ...item, [field]: value } : item
    ));
  };

  const saveChanges = async () => {
    if (!selectedSeasonId) return;
    try {
      const entries = editData
        .filter(d => d.participated)
        .map(d => ({
          collegeId: d.collegeId,
          points: parseInt(d.points) || 0,
          rank: d.rank?.toString().trim() || '-',
          sportResults: d.sportResults || {}
        }));
      await updateNationalRating(selectedSeasonId, { entries, activeSports });
      toast.success('Рейтинги оновлено');
      setIsEditing(false);
      fetchRating(selectedSeasonId);
    } catch (error) {
      toast.error('Не вдалося зберегти зміни');
    }
  };

  const exportToExcel = () => {
    if (!ratingData) return;

    const data = ratingData.rows.map(row => {
      const college = colleges.find(c => (c.shortName || c.name) === row.groupName);
      return {
        'Коледж': row.groupName,
        'Регіон': college?.region || '—',
        'Загальна сума очок': row.totalPoints,
        'Підсумкове місце': row.totalRank
      };
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
          {ratingData && (
            <>
              <div className={styles.participationStats}>
                Взяло участь: <span>{ratingData.rows.length}</span> навчальних закладів
              </div>
              <div className={styles.nameToggleWrapper}>
                <span className={`${styles.toggleText} ${!showFullName ? styles.activeText : ''}`}>Скорочена</span>
                <button 
                  className={`${styles.toggleBtn} ${showFullName ? styles.toggleActive : ''}`}
                  onClick={() => setShowFullName(!showFullName)}
                  title={showFullName ? "Показати скорочені назви" : "Показати повні назви"}
                >
                  <div className={styles.toggleCircle}></div>
                </button>
                <span className={`${styles.toggleText} ${showFullName ? styles.activeText : ''}`}>Повна назва</span>
              </div>
            </>
          )}
        </div>

        <div className={styles.controls}>
          <Link to="/colleges" className={styles.collegesBtn}>
            <i className="fas fa-university"></i> Коледжі
          </Link>
          
          {isAdmin && (
            <>
              {isEditing ? (
                <button className={styles.saveBtn} onClick={saveChanges}>
                  <i className="fas fa-save"></i> Зберегти
                </button>
              ) : (
                <button className={styles.editBtn} onClick={startEditing}>
                  <i className="fas fa-edit"></i> Редагувати залік
                </button>
              )}
              <Link to="/admin/colleges" className={styles.manageBtn}>
                <i className="fas fa-cog"></i> Керування
              </Link>
            </>
          )}

          {seasons.length > 0 && (
            <select
              className={styles.seasonSelect}
              value={selectedSeasonId || ''}
              onChange={handleSeasonChange}
              disabled={isEditing}
            >
              {seasons.map(s => (
                <option key={s.id} value={s.id}>
                  Сезон {s.name} {s.isCurrent ? '(Поточний)' : ''}
                </option>
              ))}
            </select>
          )}

          {ratingData && ratingData.rows.length > 0 && !isEditing && (
            <button className={`${styles.exportBtn} ${styles.excel}`} onClick={exportToExcel}>
              <i className="fas fa-file-excel"></i> Excel
            </button>
          )}
        </div>
      </header>

      {isEditing && (
        <div className={styles.sportSelection}>
          <h3>Виберіть види спорту для відображення:</h3>
          <div className={styles.sportCheckboxes}>
            {ALL_SPORTS.map(sport => (
              <label key={sport} className={styles.sportCheckboxLabel}>
                <input 
                  type="checkbox" 
                  checked={activeSports.includes(sport)}
                  onChange={(e) => {
                    if (e.target.checked) {
                      setActiveSports(prev => [...prev, sport]);
                    } else {
                      setActiveSports(prev => prev.filter(s => s !== sport));
                    }
                  }}
                />
                {sport}
              </label>
            ))}
          </div>
        </div>
      )}

      {seasons.length === 0 ? (
        <div className={styles.emptyState}>
          <i className="fas fa-calendar-times"></i>
          <h3>Сезонів не знайдено</h3>
          <p>Дані про всеукраїнські змагання з'являться після створення першого сезону.</p>
        </div>
      ) : (ratingData && ratingData.rows.length > 0) || isEditing ? (
        <div className={styles.tableContainer}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th className={styles.mainHeader} rowSpan={2}>Коледж / Навчальний заклади</th>
                {isEditing && <th className={styles.mainHeader} rowSpan={2}>Участь</th>}
                <th className={styles.mainHeader} rowSpan={2}>Регіон</th>
                {ratingData?.sports && ratingData.sports.length > 0 && (
                  <th className={styles.mainHeader} colSpan={ratingData.sports.length}>Види спорту (місця)</th>
                )}
                <th className={styles.mainHeader} rowSpan={2}>Загальна сума очок (О)</th>
                <th className={styles.mainHeader} rowSpan={2}>Підсумкове місце (М)</th>
              </tr>
              {ratingData?.sports && ratingData.sports.length > 0 && (
                <tr>
                  {ratingData.sports.map(sport => (
                    <th key={sport} className={styles.subHeader}>{sport}</th>
                  ))}
                </tr>
              )}
            </thead>
            <tbody>
              {isEditing ? (
                editData.map((item) => {
                  const isCHPFK = item.collegeName === 'ЧПФК' || item.collegeName.includes('Чернівецький політехнічний');
                  return (
                    <tr key={item.collegeId} className={`${styles.row} ${!item.participated ? styles.inactiveRow : ''}`}>
                      <td className={styles.collegeCell}>
                        {(() => {
                          const college = colleges.find(c => c.id === item.collegeId);
                          return showFullName ? (college?.name || item.collegeName) : (college?.shortName || item.collegeName);
                        })()}
                      </td>
                      <td className={styles.editCell}>
                        <input 
                          type="checkbox" 
                          checked={item.participated} 
                          onChange={(e) => handleEditChange(item.collegeId, 'participated', e.target.checked)}
                          className={styles.participationCheckbox}
                        />
                      </td>
                      <td className={styles.regionCell}>{item.region}</td>
                      {ratingData?.sports.map(sport => (
                        <td key={sport} className={styles.editCell}>
                          {isCHPFK ? (
                            <input 
                              type="text" 
                              value={item.sportResults?.[sport] || '-'} 
                              onChange={(e) => {
                                const newSportResults = { ...(item.sportResults || {}) };
                                newSportResults[sport] = e.target.value;
                                handleEditChange(item.collegeId, 'sportResults', newSportResults);
                              }}
                              className={styles.editInputSmall}
                              disabled={!item.participated}
                            />
                          ) : (
                            <span className={styles.dash}>—</span>
                          )}
                        </td>
                      ))}
                      <td className={styles.editCell}>
                        <input 
                          type="number" 
                          value={item.points} 
                          onChange={(e) => handleEditChange(item.collegeId, 'points', e.target.value)}
                          className={styles.editInput}
                          disabled={!item.participated}
                        />
                      </td>
                      <td className={styles.editCell}>
                        <input 
                          type="text" 
                          value={item.rank} 
                          onChange={(e) => handleEditChange(item.collegeId, 'rank', e.target.value)}
                          className={styles.editInput}
                          disabled={!item.participated}
                        />
                      </td>
                    </tr>
                  );
                })
              ) : (
                ratingData!.rows.map((row) => {
                  const college = colleges.find(c => c.id === row.collegeId);
                  const isCHPFK = college?.shortName === 'ЧПФК' || college?.name?.includes('Чернівецький політехнічний');
                  
                  return (
                    <tr key={row.collegeId || row.groupName} className={styles.row}>
                      <td className={styles.collegeCell}>
                        <div className={styles.collegeInfo}>
                            <div className={styles.collegeName}>
                              {showFullName ? (college?.name || row.groupName) : (college?.shortName || row.groupName)}
                            </div>
                        </div>
                      </td>
                      <td className={styles.regionCell}>
                        {college?.region || '—'}
                      </td>
                      {ratingData?.sports.map(sport => (
                        <td key={sport} className={styles.sportRankCell}>
                          {isCHPFK ? (row.sportResults?.[sport]?.rank || '-') : '—'}
                        </td>
                      ))}
                      <td className={styles.totalPointsCell}>{row.totalPoints}</td>
                      <td className={styles.totalRankCell}>{row.totalRank || '-'}</td>
                    </tr>
                  );
                })
              )}
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
