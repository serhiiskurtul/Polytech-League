import React from 'react';
import { getPhotoUrl } from '@/shared/utils/photo.utils';
import styles from '../TournamentViewPage.module.css';

interface TournamentPodiumProps {
  firstPlace?: any | null;
  secondPlace?: any | null;
  thirdPlace?: any | null;
  isTeam?: boolean;
  onViewRoster?: (teamId: number) => void;
}

const TournamentPodium: React.FC<TournamentPodiumProps> = ({ 
  firstPlace, 
  secondPlace, 
  thirdPlace,
  isTeam = false,
  onViewRoster
}) => {
  if (!firstPlace && !secondPlace && !thirdPlace) return null;

  const renderPlace = (participant: any | null | undefined, rank: number, className: string) => {
    if (!participant) return null;

    const displayName = isTeam 
      ? participant.name 
      : `${participant.firstName} ${participant.lastName}`;
    
    const initials = isTeam
      ? (participant.name?.length > 10 ? participant.name.substring(0, 10) + '...' : participant.name)
      : `${participant.firstName?.[0] || ''}${participant.lastName?.[0] || ''}`;

    const photoSrc = !isTeam ? getPhotoUrl(participant.photoUrl) : null;

    return (
      <div className={`${styles.podiumPlace} ${className}`}>
        <div className={styles.podiumRank}>
          {rank === 1 && <i className="fas fa-crown"></i>}
          {rank !== 1 && rank}
        </div>
        <div 
          className={styles.podiumAvatar}
          onClick={() => isTeam && participant.id && onViewRoster && onViewRoster(participant.id)}
          style={{ cursor: (isTeam && onViewRoster) ? 'pointer' : 'default' }}
        >
          {photoSrc ? (
            <img 
              src={photoSrc} 
              alt={displayName} 
              className={styles.podiumPhoto}
            />
          ) : (
            initials
          )}
        </div>
        <div 
          className={styles.podiumName}
          onClick={() => isTeam && participant.id && onViewRoster && onViewRoster(participant.id)}
          style={{ cursor: (isTeam && onViewRoster) ? 'pointer' : 'default' }}
        >
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
