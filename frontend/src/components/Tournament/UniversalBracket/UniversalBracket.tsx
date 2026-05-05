import React, { useMemo, useState, useEffect, useRef } from 'react';
import styles from './UniversalBracket.module.css';

export interface BracketTeam {
  name: string;
  score?: number | string;
  isWinner?: boolean;
}

export interface BracketMatch {
  id: string | number;
  teams: [BracketTeam, BracketTeam];
  status?: string;
  originalMatch?: any;
}

export interface BracketRound {
  title: string;
  matches: BracketMatch[];
}

interface UniversalBracketProps {
  rounds: BracketRound[];
  isAdmin?: boolean;
  onDeleteMatch?: (id: string | number) => void;
  onOpenScorers?: (match: any) => void;
  renderMatchCard?: (match: BracketMatch) => React.ReactNode;
  cardHeight?: number;
}

const BASE_GAP = 40;
const COLUMN_WIDTH = 280;
const COLUMN_GAP = 120;

const UniversalBracket: React.FC<UniversalBracketProps> = ({ 
  rounds, 
  isAdmin, 
  onDeleteMatch, 
  onOpenScorers, 
  renderMatchCard,
  cardHeight = 140 
}) => {
  const containerRef = useRef<HTMLDivElement>(null);

  // 1. Calculate absolute Y-coordinates for every match mathematically
  const matchPositions = useMemo(() => {
    const positions: number[][] = []; // positions[round][matchIndex] = yCoordinate

    rounds.forEach((round, rIdx) => {
      positions[rIdx] = [];
        round.matches.forEach((_, mIdx) => {
        if (rIdx === 0) {
          // First round matches are evenly spaced
          positions[rIdx][mIdx] = mIdx * (cardHeight + BASE_GAP) + (cardHeight / 2);
        } else {
          // Subsequent rounds: Match is centered between its two "parents" from previous round
          const p1Y = positions[rIdx - 1][mIdx * 2];
          const p2Y = positions[rIdx - 1][mIdx * 2 + 1];

          if (p1Y !== undefined && p2Y !== undefined) {
            positions[rIdx][mIdx] = (p1Y + p2Y) / 2;
          } else if (p1Y !== undefined) {
            // Handle single child (e.g. bye)
            positions[rIdx][mIdx] = p1Y;
          } else {
            // Fallback for missing parents (shouldn't happen in clean data)
            positions[rIdx][mIdx] = mIdx * (cardHeight + BASE_GAP * Math.pow(2, rIdx)) + (cardHeight / 2);
          }
        }
      });
    });

    return positions;
  }, [rounds, cardHeight]);

  // 2. Generate SVG paths based on the calculated positions
  const svgPaths = useMemo(() => {
    const paths: string[] = [];

    for (let r = 0; r < rounds.length - 1; r++) {
      const currentRoundY = matchPositions[r];
      const nextRoundY = matchPositions[r + 1];

      currentRoundY.forEach((startY, mIdx) => {
        const nextMatchIdx = Math.floor(mIdx / 2);
        const endY = nextRoundY[nextMatchIdx];

        if (endY !== undefined) {
          // X coordinates
          const startX = r * (COLUMN_WIDTH + COLUMN_GAP) + COLUMN_WIDTH;
          const endX = (r + 1) * (COLUMN_WIDTH + COLUMN_GAP);
          const midX = startX + COLUMN_GAP / 2;

          // Draw the "elbow" connector
          // From match to midpoint X
          // Vertical line at midpoint X to target Y
          // From midpoint X to target match X
          const path = `M ${startX} ${startY} L ${midX} ${startY} L ${midX} ${endY} L ${endX} ${endY}`;
          paths.push(path);
        }
      });
    }

    return paths;
  }, [matchPositions, rounds]);

  // Calculate total area height
  const totalHeight = useMemo(() => {
    if (matchPositions[0]?.length > 0) {
      return matchPositions[0][matchPositions[0].length - 1] + cardHeight / 2 + BASE_GAP;
    }
    return 800;
  }, [matchPositions, cardHeight]);

  return (
    <div className={styles.bracketContainer} ref={containerRef}>
      <div className={styles.bracketWrapper} style={{ height: totalHeight + 100 }}>
        {/* SVG Connections Layer */}
        <svg 
          className={styles.svgLayer} 
          width={rounds.length * (COLUMN_WIDTH + COLUMN_GAP)} 
          height={totalHeight + 100}
        >
          {svgPaths.map((d, idx) => (
            <path key={idx} d={d} className={styles.connectorLine} />
          ))}
        </svg>

        {/* Rounds Columns */}
        {rounds.map((round, rIdx) => (
          <div key={rIdx} className={styles.roundColumn}>
            <div className={styles.roundHeader}>
              <div className={styles.roundTitle}>{round.title}</div>
            </div>

            <div className={styles.matchesArea}>
              {round.matches.map((match, mIdx) => (
                <div 
                  key={match.id} 
                  className={styles.matchCardPositioner}
                  style={{ top: matchPositions[rIdx][mIdx] }}
                >
                  <div className={styles.matchCard} style={{ height: cardHeight }}>
                    {renderMatchCard ? (
                      renderMatchCard(match)
                    ) : (
                      <>
                        {isAdmin && onDeleteMatch && (
                          <button 
                            className={styles.cardDeleteBtn} 
                            onClick={() => onDeleteMatch(match.id)}
                          >
                            ×
                          </button>
                        )}
                        {match.teams.map((team, tIdx) => (
                          <div 
                            key={tIdx} 
                            className={`${styles.teamRow} ${team.isWinner ? styles.isWinner : ''}`}
                          >
                            {team.isWinner && <div className={styles.winnerIndicator} />}
                            <div className={styles.teamInfo}>
                              <span className={styles.teamName}>{team.name}</span>
                            </div>
                            {team.score !== undefined && (
                              <div className={styles.score}>{team.score}</div>
                            )}
                          </div>
                        ))}
                        {isAdmin && onOpenScorers && (
                          <button 
                            className={styles.protocolBtn} 
                            onClick={() => onOpenScorers(match)}
                          >
                            Протокол
                          </button>
                        )}
                      </>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default UniversalBracket;
