import React from 'react';
import { PlayerDto } from '@/shared/types/player.types';
import styles from '../TournamentViewPage.module.css';

interface TournamentPodiumProps {
  firstPlace?: PlayerDto | null;
  secondPlace?: PlayerDto | null;
  thirdPlace?: PlayerDto | null;
}

const TournamentPodium: React.FC<TournamentPodiumProps> = ({ 
  firstPlace, 
  secondPlace, 
  thirdPlace 
}) => {
  if (!firstPlace && !secondPlace && !thirdPlace) return null;

  const renderPlace = (player: PlayerDto | null | undefined, rank: number, className: string) => {
    if (!player) return null;

    return (
      <div className={`${styles.podiumPlace} ${className}`}>
        <div className={styles.podiumRank}>
          {rank === 1 && <i className="fas fa-crown"></i>}
          {rank !== 1 && rank}
        </div>
        <div className={styles.podiumAvatar}>
          {player.firstName[0]}{player.lastName[0]}
        </div>
        <div className={styles.podiumName}>
          {player.firstName} {player.lastName}
        </div>
        <div className={styles.podiumGroup}>
          {player.group}
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
