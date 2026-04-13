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

interface TeamSportsTournamentViewProps {
  sport: 'volleyball' | 'beach_volleyball' | 'futsal' | 'basketball_3x3' | 'badminton';
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
  const [editingRosterMatchId, setEditingRosterMatchId] = useState<number | null>(null);
  const [editingSide, setEditingSide] = useState<'home' | 'away' | null>(null);
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
      
      const payload: any = { ...match };
      if (sport === 'volleyball' || sport === 'beach_volleyball') {
        payload.homeSetsWon = homeScore;
        payload.awaySetsWon = awayScore;
      } else {
        payload.homeTeamScore = homeScore;
        payload.awayTeamScore = awayScore;
      }

      await updateApi(matchId, payload);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні рахунку');
    }
  };

  const handleTogglePlayerInRoster = async (matchId: number, side: 'home' | 'away', playerId: number) => {
    try {
      const match = details.matches.find((m: any) => m.id === matchId);
      const currentRosterIds = side === 'home' 
        ? (match.homeRosterIds || match.homeRoster?.map((p: any) => p.id) || [])
        : (match.awayRosterIds || match.awayRoster?.map((p: any) => p.id) || []);
      
      let newRosterIds: number[];
      if (currentRosterIds.includes(playerId)) {
        newRosterIds = currentRosterIds.filter((id: number) => id !== playerId);
      } else {
        newRosterIds = [...currentRosterIds, playerId];
      }

      const updateApi = getMatchUpdateApi();
      const payload = { 
        ...match,
        [`${side}RosterIds`]: newRosterIds 
      };

      await updateApi(matchId, payload);
      onRefresh();
    } catch (err) {
      console.error(err);
      alert('Помилка при оновленні складу');
    }
  };

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

      const payload = {
        eventId: details.id,
        homeTeamId: parseInt(newMatchForm.homeTeamId),
        awayTeamId: parseInt(newMatchForm.awayTeamId),
        stage: newMatchForm.stage
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
      case 'beach_volleyball': return 'Пляжний Волейбол';
      case 'futsal': return 'Футзал';
      case 'basketball_3x3': return 'Баскетбол 3х3';
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
          <button className={activeTab === 'info' ? styles.activeTab : ''} onClick={() => setActiveTab('info')}>Інфо</button>
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
            {details.matches?.map((match: any) => (
              <div key={match.id} className={styles.matchCard}>
                <div className={styles.matchStage}>{match.stage || 'Груповий етап'}</div>
                
                <div className={styles.matchTeams}>
                  <div className={styles.teamRow}>
                    <div className={styles.teamLogo}>{match.homeTeam?.name?.[0] || 'T'}</div>
                    <div className={styles.teamName}>{match.homeTeam?.name || 'Home'}</div>
                    <div className={styles.rosterInfo}>
                        {match.homeRoster?.map((p: any) => `${p.lastName} ${p.firstName[0]}.`).join(', ') || 'Склад не вказано'}
                        {isAdmin && (
                            <button className={styles.rosterEditBtn} onClick={() => { setEditingRosterMatchId(match.id); setEditingSide('home'); }}>
                                <i className="fas fa-user-plus"></i>
                            </button>
                        )}
                    </div>
                    <div className={styles.scoreInput}>
                      <input 
                        type="number" 
                        defaultValue={sport.includes('volleyball') ? match.homeSetsWon : match.homeTeamScore}
                        onBlur={(e) => handleUpdateScore(match.id, parseInt(e.target.value), sport.includes('volleyball') ? match.awaySetsWon : (match.awayTeamScore || 0))}
                        disabled={!isAdmin}
                      />
                    </div>
                  </div>

                  <div className={styles.vsDivider}>VS</div>

                  <div className={styles.teamRow}>
                    <div className={styles.teamLogo}>{match.awayTeam?.name?.[0] || 'T'}</div>
                    <div className={styles.teamName}>{match.awayTeam?.name || 'Away'}</div>
                    <div className={styles.rosterInfo}>
                        {match.awayRoster?.map((p: any) => `${p.lastName} ${p.firstName[0]}.`).join(', ') || 'Склад не вказано'}
                        {isAdmin && (
                            <button className={styles.rosterEditBtn} onClick={() => { setEditingRosterMatchId(match.id); setEditingSide('away'); }}>
                                <i className="fas fa-user-plus"></i>
                            </button>
                        )}
                    </div>
                    <div className={styles.scoreInput}>
                      <input 
                        type="number" 
                        defaultValue={sport.includes('volleyball') ? match.awaySetsWon : match.awayTeamScore}
                        onBlur={(e) => handleUpdateScore(match.id, sport.includes('volleyball') ? match.homeSetsWon : (match.homeTeamScore || 0), parseInt(e.target.value))}
                        disabled={!isAdmin}
                      />
                    </div>
                  </div>
                </div>
              </div>
            ))}
            {(!details.matches || details.matches.length === 0) && <p className={styles.noData}>Матчів ще не заплановано</p>}
          </div>
        )}

        {activeTab === 'standings' && (
          <div className={styles.standingsTable}>
            <table className={styles.leagueTable}>
              <thead>
                <tr>
                  <th>#</th>
                  <th>Команда (Група)</th>
                  <th>І</th>
                  <th>В</th>
                  <th>П</th>
                  <th>{sport.includes('volleyball') ? 'Сети' : 'М\'ячі'}</th>
                  <th>Очки</th>
                </tr>
              </thead>
              <tbody>
                {details.teams?.map((team: any, index: number) => (
                  <tr key={team.id}>
                    <td>{index + 1}</td>
                    <td className={styles.tableName}>{team.name}</td>
                    <td>0</td>
                    <td>0</td>
                    <td>0</td>
                    <td>0 - 0</td>
                    <td className={styles.tablePoints}>0</td>
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

        {activeTab === 'info' && (
          <div className={styles.eventInfo}>
            <div className={styles.infoCard}>
              <h4><i className="fas fa-scroll"></i> Правила</h4>
              <p>{details?.rules || 'Standard league rules'}</p>
            </div>
            <div className={styles.infoCard}>
              <h4><i className="fas fa-gift"></i> Призи</h4>
              <p>{details?.prizes || 'Certificates and cups'}</p>
            </div>
            <div className={styles.infoCard}>
              <h4><i className="fas fa-user-tie"></i> Судді</h4>
              <p>{details?.referees || 'Official college referee board'}</p>
            </div>
          </div>
        )}
      </div>

      {/* Roster Edit Modal (Simple but effective) */}
      {editingRosterMatchId && (
        <div className={styles.rosterModalOverlay} onClick={() => setEditingRosterMatchId(null)}>
            <div className={styles.rosterModal} onClick={e => e.stopPropagation()}>
                <div className={styles.modalHeader}>
                    <h3>Редагування складу: {editingSide === 'home' ? 'Господарі' : 'Гості'}</h3>
                    <button className={styles.closeButton} onClick={() => setEditingRosterMatchId(null)}>&times;</button>
                </div>
                <div className={styles.searchBox}>
                    <input 
                        type="text" 
                        placeholder="Пошук гравця (Прізвище, група)..." 
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                    />
                </div>
                <div className={styles.playersList}>
                    {filteredPlayers.map(player => {
                        const match = details.matches.find((m: any) => m.id === editingRosterMatchId);
                        const roster = editingSide === 'home' ? match.homeRoster : match.awayRoster;
                        const isSelected = roster?.some((p: any) => p.id === player.id);
                        
                        return (
                            <div key={player.id} className={`${styles.playerPickItem} ${isSelected ? styles.selected : ''}`} onClick={() => handleTogglePlayerInRoster(editingRosterMatchId, editingSide!, player.id!)}>
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
      )}

      {/* Create Match Modal */}
      {isCreateMatchModalOpen && (
        <div className={styles.rosterModalOverlay} onClick={() => setIsCreateMatchModalOpen(false)}>
          <div className={`${styles.rosterModal} ${styles.createMatchModal}`} onClick={e => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h3>Спланувати новий матч</h3>
              <button className={styles.closeButton} onClick={() => setIsCreateMatchModalOpen(false)}>&times;</button>
            </div>
            
            <div className={styles.createMatchForm}>
              <div className={styles.formGroup}>
                <label>Господарі</label>
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
                <label>Гості</label>
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
