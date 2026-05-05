import React, { useEffect, useRef, useState, useMemo } from 'react';
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
}

interface Point {
  x: number;
  y: number;
}

const UniversalBracket: React.FC<UniversalBracketProps> = ({ 
  rounds, 
  isAdmin, 
  onDeleteMatch, 
  onOpenScorers, 
  renderMatchCard 
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const cardRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const [lines, setLines] = useState<string[]>([]);

  // Function to calculate and draw SVG paths
  const drawLines = () => {
    if (!containerRef.current) return;

    const newLines: string[] = [];
    const containerRect = containerRef.current.getBoundingClientRect();

    // Loop through rounds except the last one
    for (let i = 0; i < rounds.length - 1; i++) {
      const currentRound = rounds[i];
      const nextRound = rounds[i + 1];

      currentRound.matches.forEach((match, matchIdx) => {
        // Find corresponding match in next round
        // In Single Elimination, every 2 matches in current round lead to 1 in next round
        const nextMatchIdx = Math.floor(matchIdx / 2);
        const nextMatch = nextRound.matches[nextMatchIdx];

        const currentCard = cardRefs.current[`${i}-${match.id}`];
        const nextCard = cardRefs.current[`${i + 1}-${nextMatch?.id}`];

        if (currentCard && nextCard) {
          const startRect = currentCard.getBoundingClientRect();
          const endRect = nextCard.getBoundingClientRect();

          // Coordinates relative to the scrollable container
          const startX = startRect.right - containerRect.left + containerRef.current!.scrollLeft;
          const startY = startRect.top + startRect.height / 2 - containerRect.top + containerRef.current!.scrollTop;
          
          const endX = endRect.left - containerRect.left + containerRef.current!.scrollLeft;
          const endY = endRect.top + endRect.height / 2 - containerRect.top + containerRef.current!.scrollTop;

          // Midpoint for the "fork"
          const midX = startX + (endX - startX) / 2;

          // Draw a line with a right angle (elbow)
          // M startX startY L midX startY L midX endY L endX endY
          const path = `M ${startX} ${startY} L ${midX} ${startY} L ${midX} ${endY} L ${endX} ${endY}`;
          newLines.push(path);
        }
      });
    }

    setLines(newLines);
  };

  useEffect(() => {
    // Initial draw
    const timer = setTimeout(drawLines, 100);

    // Re-draw on resize
    const observer = new ResizeObserver(() => drawLines());
    if (containerRef.current) observer.observe(containerRef.current);

    window.addEventListener('resize', drawLines);

    return () => {
      clearTimeout(timer);
      observer.disconnect();
      window.removeEventListener('resize', drawLines);
    };
  }, [rounds]);

  return (
    <div className={styles.bracketContainer} ref={containerRef}>
      <svg className={styles.svgOverlay}>
        {lines.map((d, idx) => (
          <path key={idx} d={d} className={styles.connectorLine} />
        ))}
      </svg>

      <div className={styles.bracketWrapper}>
        {rounds.map((round, roundIdx) => (
          <div key={roundIdx} className={styles.roundColumn}>
            <div className={styles.roundHeader}>
              <div className={styles.roundTitle}>{round.title}</div>
            </div>

            <div className={styles.matchesList}>
              {round.matches.map((match) => (
                <div key={match.id} className={styles.matchCardWrapper}>
                  <div 
                    className={styles.matchCard}
                    ref={(el) => { cardRefs.current[`${roundIdx}-${match.id}`] = el; }}
                  >
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
