import React from 'react';
import { PlayerDto } from '@/shared/types/player.types';
import styles from '../TournamentViewPage.module.css';

interface TournamentPodiumProps {
  firstPlace?: any | null;
  secondPlace?: any | null;
  thirdPlace?: any | null;
  isTeam?: boolean;
}

const TournamentPodium: React.FC<TournamentPodiumProps> = ({ 
  firstPlace, 
  secondPlace, 
  thirdPlace,
  isTeam = false
}) => {
  if (!firstPlace && !secondPlace && !thirdPlace) return null;

  const renderPlace = (participant: any | null | undefined, rank: number, className: string) => {
    if (!participant) return null;

    const displayName = isTeam 
      ? participant.name 
      : `${participant.firstName} ${participant.lastName}`;
    
    const displayAvatar = isTeam
      ? (participant.name?.[0] || 'T')
      : `${participant.firstName?.[0] || ''}${participant.lastName?.[0] || ''}`;

    return (
      <div className={`${styles.podiumPlace} ${className}`}>
        <div className={styles.podiumRank}>
          {rank === 1 && <i className="fas fa-crown"></i>}
          {rank !== 1 && rank}
        </div>
        <div className={styles.podiumAvatar}>
          {displayAvatar}
        </div>
        <div className={styles.podiumName}>
          {displayName}
        </div>
        <div className={styles.podiumGroup}>
          {!isTeam && participant.group}
        </div>
      </div>
    );
  };

  return (
    <div className={styles.podiumContainer}>
      {/* 2nd Place */}
      {renderPlace(secondPlace, 2, styles.secondPlace)}
      
      {/* 1st Place */}
      {renderPlace(firstPlace, 1, styles.firstPlace)}
      
      {/* 3rd Place */}
      {renderPlace(thirdPlace, 3, styles.thirdPlace)}
    </div>
  );
};

export default TournamentPodium;
