import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getChampions, createHallOfFameEntry, deleteHallOfFameEntry } from '@/entities/tournament/api/tournaments.api';
import { Champion } from '@/entities/tournament/model/tournament.types';
import styles from './ChampionsPage.module.css';
import { useAuth } from '@/entities/session';
import { toast } from 'react-hot-toast';

const ChampionsPage: React.FC = () => {
  const [champions, setChampions] = useState<Champion[]>([]);
  const [loading, setLoading] = useState(true);
  
  const { user } = useAuth();
  const isAdmin = user?.role === 1 || user?.role === 'Admin' || user?.role === '1';
  
  const [showAddForm, setShowAddForm] = useState(false);
  const [newEntry, setNewEntry] = useState({
    sport: 'chess',
    seasonName: '',
    teamName: ''
  });
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const getSportTitle = (sportType: string) => {
    switch (sportType?.toLowerCase()) {
      case 'chess': return 'Шахи';
      case 'checkers': return 'Шашки';
      case 'table-tennis': return 'Настільний теніс';
      case 'armwrestling': return 'Армрестлінг';
      case 'badminton': return 'Бадмінтон';
      case 'football': return 'Футбол';
      case 'basketball-3x3': return 'Баскетбол 3x3';
      case 'volleyball': return 'Волейбол';
      default: return sportType || 'Спорт';
    }
  };

  const getSportIcon = (sportType: string) => {
    switch (sportType?.toLowerCase()) {
      case 'chess': return 'fa-chess-king';
      case 'checkers': return 'fa-chess-board';
      case 'table-tennis': return 'fa-table-tennis-paddle-ball';
      case 'armwrestling': return 'fa-handshake-angle';
      case 'badminton': return 'fa-shuttlecock';
      case 'football': return 'fa-futbol';
      case 'basketball-3x3': return 'fa-basketball-ball';
      case 'volleyball': return 'fa-volleyball-ball';
      default: return 'fa-trophy';
    }
  };

  const getWeightClassLabel = (wc: string) => {
    switch (wc) {
      case 'Under60kg': return 'до 60 кг';
      case 'Under70kg': return 'до 70 кг';
      case 'Under80kg': return 'до 80 кг';
      case 'Under90kg': return 'до 90 кг';
      case 'Plus90kg': return 'понад 90 кг';
      default: return wc;
    }
  };

  const fetchChampions = async () => {
    setLoading(true);
    try {
      const data = await getChampions();
      setChampions(data);
    } catch (error) {
      console.error('Error fetching champions:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchChampions();
  }, []);

  // Group by seasonName and sort
  const groupedChampions = champions.reduce((acc, champion) => {
    const season = champion.seasonName || 'Невідомо';
    if (!acc[season]) acc[season] = [];
    acc[season].push(champion);
    return acc;
  }, {} as Record<string, Champion[]>);

  // Sort descending by season name (e.g. 2024 before 2023)
  const sortedSeasons = Object.keys(groupedChampions).sort((a, b) => b.localeCompare(a));

  if (loading) {
    return (
      <div className={styles.loadingContainer}>
        <div className={styles.loader}></div>
        <p>Завантаження зали слави...</p>
      </div>
    );
  }

  return (
    <div className={styles.pageContainer}>
      <header className={styles.header}>
        <div className={styles.headerContent}>
          <i className="fas fa-crown" style={{ fontSize: '3.5rem', color: '#6366f1', marginBottom: '1.5rem', filter: 'drop-shadow(0 0 15px rgba(99, 102, 241, 0.4))' }}></i>
          <h1 className={styles.title}>Зала слави</h1>
          <p className={styles.subtitle}>Історія чемпіонів Polytech League крізь роки</p>
        </div>
      </header>

      <main className={styles.content}>
        {isAdmin && (
          <div className={styles.adminSection}>
            <button 
              className={`${styles.addLegendBtn} ${showAddForm ? styles.btnCancel : ''}`} 
              onClick={() => setShowAddForm(!showAddForm)}
            >
              <i className={`fas ${showAddForm ? 'fa-times' : 'fa-plus'}`}></i>
              {showAddForm ? 'Скасувати' : 'Додати легенду'}
            </button>
            
            {showAddForm && (
              <form 
                className={styles.addForm}
                onSubmit={async (e) => {
                  e.preventDefault();
                  try {
                    await createHallOfFameEntry({ 
                      sport: newEntry.sport, 
                      seasonName: newEntry.seasonName, 
                      teamName: newEntry.teamName, 
                      file: selectedFile 
                    });
                    toast.success('Легенду додано!');
                    setShowAddForm(false);
                    setNewEntry({ sport: 'chess', seasonName: '', teamName: '' });
                    setSelectedFile(null);
                    fetchChampions();
                  } catch (err) {
                    toast.error('Помилка при додаванні');
                  }
                }}
              >
                <div className={styles.formGroup}>
                  <label>Вид спорту</label>
                  <select
                    value={newEntry.sport}
                    onChange={(e) => setNewEntry({ ...newEntry, sport: e.target.value })}
                    className={styles.formInput}
                    required
                  >
                    <option value="chess">Шахи</option>
                    <option value="checkers">Шашки</option>
                    <option value="table-tennis">Настільний теніс</option>
                    <option value="armwrestling">Армрестлінг</option>
                    <option value="badminton">Бадмінтон</option>
                    <option value="football">Футбол</option>
                    <option value="volleyball">Волейбол</option>
                    <option value="basketball-3x3">Баскетбол 3x3</option>
                  </select>
                </div>
                
                <div className={styles.formGroup}>
                  <label>Сезон / Рік</label>
                  <input 
                    type="text" 
                    placeholder="напр. 2023-2024 або 2024" 
                    value={newEntry.seasonName}
                    onChange={(e) => setNewEntry({ ...newEntry, seasonName: e.target.value })}
                    required
                    className={styles.formInput}
                  />
                </div>

                <div className={styles.formGroup}>
                  <label>Ім'я або назва команди</label>
                  <input 
                    type="text" 
                    placeholder="Ім'я або назва команди" 
                    value={newEntry.teamName}
                    onChange={(e) => setNewEntry({ ...newEntry, teamName: e.target.value })}
                    required
                    className={styles.formInput}
                  />
                </div>

                <div className={styles.formGroup}>
                  <label>Фото (опціонально)</label>
                  <input 
                    type="file" 
                    accept="image/*"
                    onChange={(e) => setSelectedFile(e.target.files?.[0] || null)}
                    className={styles.formInput}
                  />
                </div>
                
                <button type="submit" className={styles.submitBtn}>
                  <i className="fas fa-save"></i> Зберегти
                </button>
              </form>
            )}
          </div>
        )}

        {sortedSeasons.length === 0 ? (
          <div className={styles.emptyState}>
            <div className={styles.emptyIconWrapper}>
              <i className="fas fa-history"></i>
            </div>
            <p>Історія чемпіонів поки порожня</p>
          </div>
        ) : (
          <div className={styles.timeline}>
            {sortedSeasons.map((season) => (
              <div key={season} className={styles.timelineSeason}>
                <div className={styles.seasonMarker}>
                  <span className={styles.seasonYear}>{season}</span>
                  <span className={styles.seasonCount}>
                    {groupedChampions[season].length} {groupedChampions[season].length === 1 ? 'чемпіон' : groupedChampions[season].length >= 2 && groupedChampions[season].length <= 4 ? 'чемпіони' : 'чемпіонів'}
                  </span>
                </div>
                
                <div className={styles.timelineItems}>
                  {groupedChampions[season].map((champion, index) => {
                    const isManual = champion.playerId === 0;
                    const isLeft = index % 2 === 0;
                    
                    const cardContent = (
                      <>
                        <div className={styles.sportBadge}>
                          <i className={`fas ${getSportIcon(champion.sportType)}`}></i>
                          {getSportTitle(champion.sportType)}
                        </div>
                        
                        <div className={styles.cardLayout}>
                          <div className={styles.photoContainer}>
                            <img
                              src={champion.playerPhotoUrl || '/champion-placeholder.png'}
                              alt={champion.playerName}
                              className={styles.playerPhoto}
                              onError={(e) => {
                                (e.target as HTMLImageElement).src = '/champion-placeholder.png';
                              }}
                            />
                          </div>
                          <div className={styles.cardInfo}>
                            {isManual ? (
                              <div className={styles.playerName}>{champion.playerName}</div>
                            ) : (
                              <Link
                                to={`/players/${champion.playerId}`}
                                className={styles.playerNameLink}
                                onClick={(e) => e.stopPropagation()}
                              >
                                {champion.playerName}
                              </Link>
                            )}
                            
                            <div className={styles.tournamentDetails}>
                              {champion.tournamentName && (
                                <div className={styles.detailRow}>
                                  <i className="fas fa-trophy"></i> 
                                  <span>{champion.tournamentName}</span>
                                </div>
                              )}
                              
                              {champion.weightClass && champion.weightClass !== 'None' && (
                                <div className={styles.detailPill}>
                                  {getWeightClassLabel(champion.weightClass)}
                                </div>
                              )}
                            </div>
                          </div>
                        </div>

                        {isManual && isAdmin && (
                          <button 
                            className={styles.deleteBtn}
                            onClick={async (e) => {
                              e.preventDefault();
                              e.stopPropagation();
                              if (window.confirm('Видалити цей запис?')) {
                                try {
                                  await deleteHallOfFameEntry(champion.tournamentId);
                                  toast.success('Видалено!');
                                  fetchChampions();
                                } catch (err) {
                                  toast.error('Помилка при видаленні');
                                }
                              }
                            }}
                            title="Видалити"
                          >
                            <i className="fas fa-trash"></i>
                          </button>
                        )}
                      </>
                    );

                    return (
                      <div 
                        key={`${champion.playerId}-${champion.tournamentId}-${index}`} 
                        className={`${styles.timelineItem} ${isLeft ? styles.itemLeft : styles.itemRight}`}
                      >
                        <div className={styles.timelineDot}></div>
                        <div className={styles.timelineConnector}></div>
                        {isManual ? (
                          <div className={styles.timelineCard}>
                            {cardContent}
                          </div>
                        ) : (
                          <Link to={`/tournaments/${champion.tournamentId}`} className={styles.timelineCard}>
                            {cardContent}
                          </Link>
                        )}
                      </div>
                    );
                  })}
                </div>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  );
};

export default ChampionsPage;
