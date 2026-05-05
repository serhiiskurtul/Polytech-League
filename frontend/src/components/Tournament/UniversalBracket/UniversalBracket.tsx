import React, { useMemo } from 'react';
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

const HEADER_HEIGHT = 100;
const COLUMN_WIDTH = 280;
const COLUMN_GAP = 120;
const BASE_GAP = 40;

const UniversalBracket: React.FC<UniversalBracketProps> = ({ 
  rounds, 
  isAdmin, 
  onDeleteMatch, 
  onOpenScorers, 
  renderMatchCard,
  cardHeight = 140 
}) => {
  // 1. DETERMINISTIC COORDINATE CALCULATION
  const { matchPositions, totalWidth, totalHeight } = useMemo(() => {
    const positions: number[][] = []; // positions[roundIndex][matchIndex] = Y-coordinate

    // Calculate Y for each match
    rounds.forEach((round, rIdx) => {
      positions[rIdx] = [];
      round.matches.forEach((_, mIdx) => {
        if (rIdx === 0) {
          // Round 1: index * (height + gap) + half-height + initial offset
          positions[rIdx][mIdx] = HEADER_HEIGHT + mIdx * (cardHeight + BASE_GAP) + cardHeight / 2;
        } else {
          // Subsequent rounds: Average of parents' Y positions
          const p1Y = positions[rIdx - 1][mIdx * 2];
          const p2Y = positions[rIdx - 1][mIdx * 2 + 1];

          if (p1Y !== undefined && p2Y !== undefined) {
            positions[rIdx][mIdx] = (p1Y + p2Y) / 2;
          } else if (p1Y !== undefined) {
            positions[rIdx][mIdx] = p1Y;
          } else {
            // Fallback for missing data
            positions[rIdx][mIdx] = HEADER_HEIGHT + mIdx * (cardHeight + BASE_GAP * Math.pow(2, rIdx)) + cardHeight / 2;
          }
        }
      });
    });

    const maxRoundIdx = rounds.length - 1;
    const maxMatchesFirstRound = rounds[0]?.matches.length || 0;
    
    return {
      matchPositions: positions,
      totalWidth: rounds.length * COLUMN_WIDTH + (rounds.length - 1) * COLUMN_GAP + 80,
      totalHeight: HEADER_HEIGHT + maxMatchesFirstRound * (cardHeight + BASE_GAP) + 100
    };
  }, [rounds, cardHeight]);

  // 2. DETERMINISTIC SVG PATH GENERATION
  const svgPaths = useMemo(() => {
    const paths: string[] = [];

    for (let r = 0; r < rounds.length - 1; r++) {
      const currentY = matchPositions[r];
      const nextY = matchPositions[r + 1];

      currentY.forEach((startY, mIdx) => {
        const nextMatchIdx = Math.floor(mIdx / 2);
        const endY = nextY[nextMatchIdx];

        if (endY !== undefined) {
          const startX = r * (COLUMN_WIDTH + COLUMN_GAP) + COLUMN_WIDTH;
          const endX = (r + 1) * (COLUMN_WIDTH + COLUMN_GAP);
          const midX = (startX + endX) / 2;

          // Precision bracket path: horizontal -> vertical -> horizontal
          const path = `M ${startX} ${startY} L ${midX} ${startY} L ${midX} ${endY} L ${endX} ${endY}`;
          paths.push(path);
        }
      });
    }

    return paths;
  }, [matchPositions, rounds]);

  return (
    <div className={styles.bracketContainer}>
      <div 
        className={styles.bracketWrapper} 
        style={{ width: totalWidth, height: totalHeight }}
      >
        {/* SVG CONNECTOR LAYER - Covers the entire coordinate space */}
        <svg 
          className={styles.svgLayer} 
          width={totalWidth} 
          height={totalHeight}
        >
          {svgPaths.map((d, idx) => (
            <path key={idx} d={d} className={styles.connectorLine} />
          ))}
        </svg>

        {/* ROUNDS & MATCHES - Strictly positioned by X/Y coordinates */}
        {rounds.map((round, rIdx) => {
          const left = rIdx * (COLUMN_WIDTH + COLUMN_GAP);

          return (
            <React.Fragment key={rIdx}>
              {/* Round Header */}
              <div 
                className={styles.roundColumn} 
                style={{ left, top: 0, height: HEADER_HEIGHT }}
              >
                <div className={styles.roundHeader}>
                  <div className={styles.roundTitle}>{round.title}</div>
                </div>
              </div>

              {/* Match Cards */}
              {round.matches.map((match, mIdx) => {
                const top = matchPositions[rIdx][mIdx];

                return (
                  <div 
                    key={match.id} 
                    className={styles.matchCardPositioner}
                    style={{ 
                      left, 
                      top, 
                      width: COLUMN_WIDTH 
                    }}
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
                );
              })}
            </React.Fragment>
          );
        })}
      </div>
    </div>
  );
};

export default UniversalBracket;
