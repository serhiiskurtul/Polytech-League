import React from 'react';
import styles from './UserTeams.module.css';

const UserTeams = ({ teams, onTeamClick }) => {
  return (
    <section className={styles.card}>
      <div className={styles.cardHeader}>
        <h2 className={styles.cardTitle}><i className="fas fa-users"></i> Мої команди</h2>
      </div>
      
      <div className={styles.teamsGrid}>
        {teams.map(team => (
          <div 
            key={team.id} 
            className={styles.teamCard}
            onClick={() => onTeamClick(team.name)}
          >
            <div className={styles.teamLogo}>{team.logo}</div>
            <div className={styles.teamName}>{team.name}</div>
            <div className={styles.teamSport}>{team.sport}</div>
            <div className={team.role === 'Капітан' ? styles.teamRoleCaptain : styles.teamRoleMember}>
              {team.role}
            </div>
          </div>
        ))}
      </div>
    </section>
  );
};

export default UserTeams;