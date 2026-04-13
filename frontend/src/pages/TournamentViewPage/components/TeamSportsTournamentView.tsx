import React, { useState, useEffect } from 'react';
import { PlayerDto } from '@/shared/types/player.types';
import { getAllPlayers } from '@/shared/api';
import { 
  updateVolleyballMatch, 
  updateBeachVolleyballMatch, 
  updateFutsalMatch, 
  updateBasketball3x3Match,
  updateBadmintonMatch,
  createVolleyballMatch,
  createBeachVolleyballMatch,
  createFutsalMatch,
  createBasketball3x3Match,
  createBadmintonMatch,
  createVolleyballTeam,
  createBeachVolleyballTeam,
  createFutsalTeam,
  createBasketball3x3Team,
  createBadmintonTeam,
  updateVolleyballTeam,
  updateBeachVolleyballTeam,
  updateFutsalTeam,
  updateBasketball3x3Team,
  updateBadmintonTeam
} from '@/shared/api';
import TournamentPodium from './TournamentPodium';
import styles from '../TournamentViewPage.module.css';

type Sport = 'volleyball' | 'beach_volleyball' | 'beach-volleyball' | 'futsal' | 'basketball_3x3' | 'basketball3x3' | 'basketball-3x3' | 'badminton';

interface TeamSportsTournamentViewProps {
  sport: Sport;
  details: any;
  isAdmin: boolean;
  onRefresh: () => void;
}

const TeamSportsTournamentView: React.FC<TeamSportsTournamentViewProps> = ({ 
  sport, 
  details, 
  isAdmin,
  onRefresh
}) => {
  const [activeTab, setActiveTab] = useState<'standings' | 'matches' | 'teams' | 'info'>('matches');
  const [allPlayers, setAllPlayers] = useState<PlayerDto[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  
  // Create Match State
  const [isCreateMatchModalOpen, setIsCreateMatchModalOpen] = useState(false);
  const [newMatchForm, setNewMatchForm] = useState({
    homeTeamId: '',
    awayTeamId: '',
    stage: 'Груповий етап'
  });

  // Create Team State
  const [isCreateTeamModalOpen, setIsCreateTeamModalOpen] = useState(false);
  const [newTeamForm, setNewTeamForm] = useState({
    name: ''
  });

  // Team Roster Management State
  const [editingTeamId, setEditingTeamId] = useState<number | null>(null);
  const [isTeamRosterModalOpen, setIsTeamRosterModalOpen] = useState(false);

  useEffect(() => {
    if (isAdmin) {
      loadAllPlayers();
    }
  }, [isAdmin]);

  const loadAllPlayers = async () => {
    try {
      const players = await getAllPlayers();
      setAllPlayers(players);
    } catch (err) {
      console.error('Failed to load players:', err);
    }
  };

  const getMatchUpdateApi = () => {
    switch (sport) {
      case 'volleyball': return updateVolleyballMatch;
      case 'beach_volleyball': return updateBeachVolleyballMatch;
      case 'futsal': return updateFutsalMatch;
      case 'basketball_3x3': return updateBasketball3x3Match;
      case 'badminton': return updateBadmintonMatch;
      default: return updateVolleyballMatch;
    }
  };

  const handleUpdateScore = async (matchId: number, homeScore: number, awayScore: number) => {
    try {
      const updateApi = getMatchUpdateApi();
      const match = details.matches.find((m: any) => m.id === matchId);
      
      const finalHomeScore = Math.max(0, homeScore);
      const finalAwayScore = Math.max(0, awayScore);
      
      const payload: any = { ...match };
      if (sport === 'volleyball' || sport === 'beach_volleyball') {
        payload.homeSetsWon = finalHomeScore;
        payload.awaySetsWon = finalAwayScore;
      } else {
        payload.homeTeamScore = finalHomeScore;
        payload.awayTeamScore = finalAwayScore;
      }

      await updateApi(matchId, payload);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні рахунку');
    }
  };

  const getTeamUpdateApi = () => {
    switch (sport) {
      case 'volleyball': return updateVolleyballTeam;
      case 'beach_volleyball': return updateBeachVolleyballTeam;
      case 'futsal': return updateFutsalTeam;
      case 'basketball_3x3': return updateBasketball3x3Team;
      case 'badminton': return updateBadmintonTeam;
      default: return updateVolleyballTeam;
    }
  };

  const handleUpdateTeamPoints = async (teamId: number, points: number) => {
    try {
      const updateApi = getTeamUpdateApi();
      const team = details.teams.find((t: any) => t.id === teamId);
      await updateApi(teamId, { ...team, points: Math.max(0, points) });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні очок');
    }
  };

  const calculateStandings = () => {
    if (!details.teams || !details.matches) return [];

    const standings = details.teams.map((team: any) => {
      const stats = {
        id: team.id,
        name: team.name,
        gp: 0, // Games Played
        w: 0,  // Wins
        d: 0,  // Draws
        l: 0,  // Losses
        gf: 0, // Goals For
        ga: 0, // Goals Against
        manualPoints: team.points
      };

      details.matches.forEach((match: any) => {
        const isHome = match.homeTeamId === team.id;
        const isAway = match.awayTeamId === team.id;

        if (isHome || isAway) {
          const isVolleyball = sport.includes('volleyball') || sport === 'badminton';
          const hScore = isVolleyball ? (match.homeSetsWon || 0) : (match.homeTeamScore || 0);
          const aScore = isVolleyball ? (match.awaySetsWon || 0) : (match.awayTeamScore || 0);
          
          // Only count if match has a score (at least one side > 0 or it's a 0-0 draw that was actually played)
          // For simplicity, we assume if stage is set, it might be played, but let's check if score is entered
          const isPlayed = isVolleyball ? (match.homeSetsWon !== null || match.awaySetsWon !== null) : (match.homeTeamScore !== null || match.awayTeamScore !== null);

          if (isPlayed) {
            stats.gp++;
            const tScore = isHome ? hScore : aScore;
            const oScore = isHome ? aScore : hScore;

            stats.gf += tScore;
            stats.ga += oScore;

            if (tScore > oScore) stats.w++;
            else if (tScore < oScore) stats.l++;
            else stats.d++;
          }
        }
      });

      // Traditional points calculation if not overridden or added to override
      // Priority: Manual Points > Auto-calculated Points
      const autoPoints = stats.w * 3 + stats.d * 1;
      const finalPoints = (stats.manualPoints !== undefined && stats.manualPoints !== null) 
        ? stats.manualPoints 
        : autoPoints;
      
      return { ...stats, points: finalPoints };
    });

    return standings.sort((a: any, b: any) => {
      // Priority 1: Points
      const aP = a.points;
      const bP = b.points;
      if (bP !== aP) return bP - aP;

      // Priority 2: Goal Difference
      const aDiff = a.gf - a.ga;
      const bDiff = b.gf - b.ga;
      if (bDiff !== aDiff) return bDiff - aDiff;

      // Priority 3: Goals Scored
      return b.gf - a.gf;
    });
  };

  const standingsData = calculateStandings();

  const handleCreateMatch = async () => {
    try {
      if (!newMatchForm.homeTeamId || !newMatchForm.awayTeamId) {
        alert('Будь ласка, оберіть обидві команди');
        return;
      }
      if (newMatchForm.homeTeamId === newMatchForm.awayTeamId) {
        alert('Команди мають бути різними');
        return;
      }

      const homeTeam = details.teams.find((t: any) => t.id === parseInt(newMatchForm.homeTeamId));
      const awayTeam = details.teams.find((t: any) => t.id === parseInt(newMatchForm.awayTeamId));

      const payload = {
        eventId: details.id,
        homeTeamId: parseInt(newMatchForm.homeTeamId),
        awayTeamId: parseInt(newMatchForm.awayTeamId),
        stage: newMatchForm.stage,
        homeRosterIds: homeTeam?.players?.map((p: any) => p.id) || [],
        awayRosterIds: awayTeam?.players?.map((p: any) => p.id) || []
      };

      let createApi;
      switch (sport) {
        case 'volleyball': createApi = createVolleyballMatch; break;
        case 'beach_volleyball': createApi = createBeachVolleyballMatch; break;
        case 'futsal': createApi = createFutsalMatch; break;
        case 'basketball_3x3': createApi = createBasketball3x3Match; break;
        case 'badminton': createApi = createBadmintonMatch; break;
        default: createApi = createVolleyballMatch;
      }

      await createApi(payload);
      setIsCreateMatchModalOpen(false);
      setNewMatchForm({ homeTeamId: '', awayTeamId: '', stage: 'Груповий етап' });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при створенні матчу');
    }
  };

  const handleCreateTeam = async () => {
    try {
      if (!newTeamForm.name) {
        alert('Будь ласка, введіть назву команди');
        return;
      }

      const payload = {
        name: newTeamForm.name,
        eventId: details.id
      };

      let createApi;
      switch (sport) {
        case 'volleyball': createApi = createVolleyballTeam; break;
        case 'beach_volleyball': createApi = createBeachVolleyballTeam; break;
        case 'futsal': createApi = createFutsalTeam; break;
        case 'basketball_3x3': createApi = createBasketball3x3Team; break;
        case 'badminton': createApi = createBadmintonTeam; break;
        default: createApi = createVolleyballTeam;
      }

      await createApi(payload);

      setIsCreateTeamModalOpen(false);
      setNewTeamForm({ name: '' });
      onRefresh();
    } catch (err) {
      console.error('Create Team Error:', err);
      alert(`Помилка при створенні команди: ${err instanceof Error ? err.message : 'Невідома помилка'}`);
    }
  };

  const handleTogglePlayerInTeam = async (teamId: number, playerId: number) => {
    try {
      const team = details.teams.find((t: any) => t.id === teamId);
      const currentParticipantIds = team.players?.map((p: any) => p.id) || [];
      
      let newPlayerIds: number[];
      if (currentParticipantIds.includes(playerId)) {
        newPlayerIds = currentParticipantIds.filter((id: number) => id !== playerId);
      } else {
        newPlayerIds = [...currentParticipantIds, playerId];
      }

      let updateApi;
      switch (sport) {
        case 'volleyball': updateApi = updateVolleyballTeam; break;
        case 'beach_volleyball': updateApi = updateBeachVolleyballTeam; break;
        case 'futsal': updateApi = updateFutsalTeam; break;
        case 'basketball_3x3': updateApi = updateBasketball3x3Team; break;
        case 'badminton': updateApi = updateBadmintonTeam; break;
        default: updateApi = updateVolleyballTeam;
      }

      await updateApi(teamId, { ...team, playerIds: newPlayerIds });
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні складу команди');
    }
  };

  const getSportLabel = () => {
    switch (sport) {
      case 'volleyball': return 'Волейбол';
      case 'beach_volleyball':
      case 'beach-volleyball': return 'Пляжний Волейбол';
      case 'futsal': return 'Футзал';
      case 'basketball_3x3':
      case 'basketball3x3':
      case 'basketball-3x3': return 'Баскетбол 3х3';
      case 'badminton': return 'Бадмінтон';
      default: return 'Спорт';
    }
  };

  const filteredPlayers = allPlayers.filter(p => 
    (p.firstName + ' ' + p.lastName).toLowerCase().includes(searchQuery.toLowerCase()) ||
    p.group?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className={styles.teamSportsView}>
      {/* Header Info */}
      <div className={styles.sportHeader}>
        <div className={styles.sportBadge}>
          <i className="fas fa-trophy"></i> {getSportLabel()}
        </div>
        <div className={styles.tabsNav}>
          <button className={activeTab === 'matches' ? styles.activeTab : ''} onClick={() => setActiveTab('matches')}>Матчі</button>
          <button className={activeTab === 'standings' ? styles.activeTab : ''} onClick={() => setActiveTab('standings')}>Таблиця</button>
          <button className={activeTab === 'teams' ? styles.activeTab : ''} onClick={() => setActiveTab('teams')}>Команди</button>
          {isAdmin && activeTab === 'matches' && (
            <button className={styles.addMatchGlobalBtn} onClick={() => setIsCreateMatchModalOpen(true)}>
              <i className="fas fa-plus-circle"></i> Додати матч
            </button>
          )}
          {isAdmin && activeTab === 'teams' && (
            <button className={styles.addMatchGlobalBtn} onClick={() => setIsCreateTeamModalOpen(true)}>
              <i className="fas fa-users-cog"></i> Створити команду
            </button>
          )}
        </div>
      </div>

      {/* Podium */}
      {(details.firstPlaceTeam || details.secondPlaceTeam || details.thirdPlaceTeam) && (
        <div className={styles.podiumSection}>
           <TournamentPodium 
             firstPlace={details.firstPlaceTeam}
             secondPlace={details.secondPlaceTeam}
             thirdPlace={details.thirdPlaceTeam}
             isTeam={true}
           />
        </div>
      )}

      {/* Main Content Areas */}
      <div className={styles.tabContent}>
        {activeTab === 'matches' && (
          <div className={styles.matchesGrid}>
            {details.matches?.map((match: any) => {
              const isVolleyball = sport.includes('volleyball') || sport === 'badminton';
              const homeScore = isVolleyball ? (match.homeSetsWon || match.homeScore || 0) : (match.homeTeamScore || 0);
              const awayScore = isVolleyball ? (match.awaySetsWon || match.awayScore || 0) : (match.awayTeamScore || 0);
              
              const isHomeWinner = homeScore > awayScore;
              const isAwayWinner = awayScore > homeScore;
              const isDraw = homeScore === awayScore && (homeScore > 0 || awayScore > 0);

              return (
                <div key={match.id} className={styles.matchCard}>
                  <div className={styles.matchStage}>{match.stage || 'Груповий етап'}</div>
                  
                  <div className={styles.matchTeams}>
                    <div className={`${styles.teamRow} ${isHomeWinner ? styles.winnerRow : ''} ${isDraw ? styles.drawRow : ''}`}>
                      <div className={`${styles.teamLogo} ${isHomeWinner ? styles.winnerLogo : ''}`}>
                        {match.homeTeam?.name?.[0] || 'T'}
                      </div>
                      <div className={`${styles.teamName} ${isHomeWinner ? styles.winnerName : ''} ${isDraw ? styles.drawName : ''}`}>
                        {match.homeTeam?.name || 'Home'}
                        {isHomeWinner && <i className={`fas fa-crown ${styles.crownIcon}`}></i>}
                        {isDraw && <i className={`fas fa-handshake ${styles.drawIcon}`}></i>}
                      </div>
                      <div className={styles.scoreInput}>
                        <input 
                          type="number" 
                          min="0"
                          defaultValue={isVolleyball ? (match.homeSetsWon || match.homeScore) : match.homeTeamScore}
                          onBlur={(e) => handleUpdateScore(match.id, parseInt(e.target.value) || 0, awayScore)}
                          disabled={!isAdmin}
                        />
                      </div>
                    </div>

                    <div className={styles.vsDivider}>VS</div>

                    <div className={`${styles.teamRow} ${isAwayWinner ? styles.winnerRow : ''} ${isDraw ? styles.drawRow : ''}`}>
                      <div className={`${styles.teamLogo} ${isAwayWinner ? styles.winnerLogo : ''}`}>
                        {match.awayTeam?.name?.[0] || 'T'}
                      </div>
                      <div className={`${styles.teamName} ${isAwayWinner ? styles.winnerName : ''} ${isDraw ? styles.drawName : ''}`}>
                        {match.awayTeam?.name || 'Away'}
                        {isAwayWinner && <i className={`fas fa-crown ${styles.crownIcon}`}></i>}
                        {isDraw && <i className={`fas fa-handshake ${styles.drawIcon}`}></i>}
                      </div>
                      <div className={styles.scoreInput}>
                        <input 
                          type="number" 
                          min="0"
                          defaultValue={isVolleyball ? (match.awaySetsWon || match.awayScore) : match.awayTeamScore}
                          onBlur={(e) => handleUpdateScore(match.id, homeScore, parseInt(e.target.value) || 0)}
                          disabled={!isAdmin}
                        />
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}
            {(!details.matches || details.matches.length === 0) && <p className={styles.noData}>Матчів ще не заплановано</p>}
          </div>
        )}

        {activeTab === 'standings' && (
          <div className={styles.standingsTable}>
            <table className={styles.leagueTable}>
              <thead>
                <tr>
                  <th title="Місце">#</th>
                  <th title="Назва команди">Команда (Група)</th>
                  <th title="Ігри">І</th>
                  <th title="Виграші">В</th>
                  <th title="Нічиї">Н</th>
                  <th title="Поразки">П</th>
                  <th title="Забиті - Пропущені">М'ячі (З-П)</th>
                  <th title="Очки (3 за перемогу, 1 за нічию)">Очки</th>
                </tr>
              </thead>
              <tbody>
                {standingsData.map((team: any, index: number) => (
                  <tr key={team.id}>
                    <td>{index + 1}</td>
                    <td className={styles.tableName}>{team.name}</td>
                    <td>{team.gp}</td>
                    <td>{team.w}</td>
                    <td>{team.d}</td>
                    <td>{team.l}</td>
                    <td>{team.gf} - {team.ga}</td>
                    <td className={styles.tablePoints}>
                      {isAdmin ? (
                        <input 
                          type="number" 
                          min="0"
                          className={styles.pointsInlineInput}
                          defaultValue={team.points}
                          onBlur={(e) => handleUpdateTeamPoints(team.id, parseInt(e.target.value) || 0)}
                        />
                      ) : (
                        team.points
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

         {activeTab === 'teams' && (
          <div className={styles.teamsGrid}>
            {details.teams?.map((team: any) => (
              <div key={team.id} className={styles.teamCard}>
                <div className={styles.teamCardHeader}>
                  <div className={styles.teamAvatar}>{team.name?.[0] || 'T'}</div>
                  <div className={styles.teamInfo}>
                    <h4>Команда {team.name}</h4>
                    <p>{team.players?.length || 0} гравців</p>
                  </div>
                  {isAdmin && (
                    <button className={styles.editTeamBtn} onClick={() => { setEditingTeamId(team.id); setIsTeamRosterModalOpen(true); }}>
                      <i className="fas fa-user-edit"></i> Склад
                    </button>
                  )}
                </div>
                <div className={styles.teamPlayersPreview}>
                  {team.players?.slice(0, 5).map((p: any) => (
                    <div key={p.id} className={styles.pMiniTag}>
                      {p.lastName} {p.firstName[0]}.
                    </div>
                  ))}
                  {team.players?.length > 5 && <span className={styles.morePlayers}>+{team.players.length - 5}</span>}
                  {(!team.players || team.players.length === 0) && <p className={styles.noPlayersHint}>Гравців ще не додано</p>}
                </div>
              </div>
            ))}
            {(!details.teams || details.teams.length === 0) && <p className={styles.noData}>У цьому турнірі ще немає команд</p>}
          </div>
        )}

      </div>

      {/* Create Match Modal */}
      {isCreateMatchModalOpen && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsCreateMatchModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.createMatchModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Спланувати новий матч</h3>
              <button className={styles.closeButton} onClick={() => setIsCreateMatchModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
              <div className={styles.createMatchForm}>
                <div className={styles.formGroup}>
                  <label>Команда 1</label>
                  <select 
                    value={newMatchForm.homeTeamId} 
                    onChange={e => setNewMatchForm({...newMatchForm, homeTeamId: e.target.value})}
                  >
                    <option value="">-- Оберіть команду --</option>
                    {details.teams?.map((t: any) => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <div className={styles.formGroup}>
                  <label>Команда 2</label>
                  <select 
                    value={newMatchForm.awayTeamId} 
                    onChange={e => setNewMatchForm({...newMatchForm, awayTeamId: e.target.value})}
                  >
                    <option value="">-- Оберіть команду --</option>
                    {details.teams?.map((t: any) => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <div className={styles.formGroup}>
                  <label>Етап змагання</label>
                  <input 
                    type="text" 
                    value={newMatchForm.stage} 
                    onChange={e => setNewMatchForm({...newMatchForm, stage: e.target.value})}
                    placeholder="напр. Груповий етап, Фінал..."
                  />
                </div>

                <button className={styles.saveBtn} onClick={handleCreateMatch}>
                  Створити матч
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
      {/* Create Team Modal */}
      {isCreateTeamModalOpen && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsCreateTeamModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.glassModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Створити нову команду (групу)</h3>
              <button className={styles.closeButton} onClick={() => setIsCreateTeamModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.modalBody}>
              <div className={styles.formGroup}>
                <label>Назва команди / Номер групи</label>
                <input 
                  type="text" 
                  value={newTeamForm.name} 
                  onChange={e => setNewTeamForm({...newTeamForm, name: e.target.value})}
                  placeholder="напр. 221"
                />
              </div>

              <div className={styles.modalFooter} style={{padding: '24px 0 0 0', background: 'transparent', borderTop: 'none'}}>
                <button className={styles.saveBtn} onClick={handleCreateTeam}>Створити команду</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Team Roster Management Modal */}
      {isTeamRosterModalOpen && editingTeamId && (
        <div className={styles.rosterModalOverlay} onClick={() => { setIsTeamRosterModalOpen(false); setEditingTeamId(null); }}>
          <div className={`${styles.rosterModal} ${styles.glassModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Управління складом: {details.teams.find((t: any) => t.id === editingTeamId)?.name}</h3>
              <button className={styles.closeButton} onClick={() => { setIsTeamRosterModalOpen(false); setEditingTeamId(null); }}>&times;</button>
            </div>
            <div className={styles.searchBox} style={{padding: '20px 40px 0 40px'}}>
                <input 
                    type="text" 
                    placeholder="Пошук гравця..." 
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    style={{width: '100%', padding: '12px 18px', borderRadius: '12px', border: '1px solid rgba(0,0,0,0.1)'}}
                />
            </div>
            <div className={styles.modalBody} style={{maxHeight: '400px'}}>
                <div className={styles.playersList}>
                    {filteredPlayers.map(player => {
                        const team = details.teams.find((t: any) => t.id === editingTeamId);
                        const isSelected = team.players?.some((p: any) => p.id === player.id);
                        
                        return (
                            <div key={player.id} className={`${styles.playerPickItem} ${isSelected ? styles.selected : ''}`} onClick={() => handleTogglePlayerInTeam(editingTeamId, player.id!)}>
                                <div className={styles.pickInfo}>
                                    <span className={styles.pName}>{player.lastName} {player.firstName}</span>
                                    <span className={styles.pGroup}>{player.group}</span>
                                </div>
                                <div className={styles.pickStatus}>
                                    {isSelected ? <i className="fas fa-check-circle"></i> : <i className="far fa-circle"></i>}
                                </div>
                            </div>
                        );
                    })}
                </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TeamSportsTournamentView;
