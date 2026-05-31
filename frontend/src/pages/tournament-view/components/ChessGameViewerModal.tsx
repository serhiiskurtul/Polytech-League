import React, { useState, useEffect, useMemo } from 'react';
import { Chess } from 'chess.js';
import { Chessboard } from 'react-chessboard';
import styles from './ChessGameViewerModal.module.css';

interface ChessGameViewerModalProps {
  pgn: string;
  whitePlayer: string;
  blackPlayer: string;
  onClose: () => void;
}

const ChessGameViewerModal: React.FC<ChessGameViewerModalProps> = ({ pgn, whitePlayer, blackPlayer, onClose }) => {
  const game = useMemo(() => new Chess(), []);
  const [fen, setFen] = useState(game.fen());
  const [moveIndex, setMoveIndex] = useState(-1);
  const [history, setHistory] = useState<string[]>([]);

  useEffect(() => {
    if (!pgn) return;
    try {
      game.loadPgn(pgn);
      const movesArray = game.history();
      setHistory(movesArray);
      game.reset();
      setFen(game.fen());
      setMoveIndex(-1);
    } catch (e) {
      console.error("Invalid PGN", e);
    }
  }, [pgn, game]);

  const goToMove = (index: number) => {
    game.reset();
    for (let i = 0; i <= index; i++) {
        try {
            game.move(history[i]);
        } catch (e) {
            console.error("Move error", e);
            break;
        }
    }
    setFen(game.fen());
    setMoveIndex(index);
  };

  const handleNext = () => {
    if (moveIndex < history.length - 1) {
      goToMove(moveIndex + 1);
    }
  };

  const handlePrev = () => {
    if (moveIndex >= 0) {
      goToMove(moveIndex - 1);
    }
  };

  const handleFirst = () => {
    goToMove(-1);
  };

  const handleLast = () => {
    goToMove(history.length - 1);
  };

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h3><i className="fas fa-chess-board"></i> Перегляд партії</h3>
          <button className={styles.closeButton} onClick={onClose}>&times;</button>
        </div>
        <div className={styles.modalBody}>
          <div className={styles.playersInfo}>
            <div className={styles.player}>
              <span className={styles.colorIndicatorWhite}></span>
              <strong>{whitePlayer || 'Білі'}</strong>
            </div>
            <div className={styles.vs}>VS</div>
            <div className={styles.player}>
              <span className={styles.colorIndicatorBlack}></span>
              <strong>{blackPlayer || 'Чорні'}</strong>
            </div>
          </div>

          <div className={styles.gameContainer}>
            <div className={styles.boardWrapper}>
              <Chessboard options={{ position: fen }} />
            </div>
            <div className={styles.controlsWrapper}>
              <div className={styles.moveHistory}>
                {history.length === 0 && <div style={{ color: '#9ca3af', width: '100%', textAlign: 'center' }}>Немає доступних ходів у PGN</div>}
                {history.map((move, idx) => (
                  <span
                    key={idx}
                    className={`${styles.moveItem} ${moveIndex === idx ? styles.activeMove : ''}`}
                    onClick={() => goToMove(idx)}
                  >
                    {idx % 2 === 0 ? <span className={styles.moveNumber}>{Math.floor(idx / 2) + 1}.</span> : ''} {move}
                  </span>
                ))}
              </div>
              <div className={styles.navControls}>
                <button onClick={handleFirst} disabled={moveIndex === -1} title="На початок">
                  <i className="fas fa-fast-backward"></i>
                </button>
                <button onClick={handlePrev} disabled={moveIndex === -1} title="Назад">
                  <i className="fas fa-step-backward"></i>
                </button>
                <button onClick={handleNext} disabled={moveIndex === history.length - 1} title="Вперед">
                  <i className="fas fa-step-forward"></i>
                </button>
                <button onClick={handleLast} disabled={moveIndex === history.length - 1} title="В кінець">
                  <i className="fas fa-fast-forward"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChessGameViewerModal;
