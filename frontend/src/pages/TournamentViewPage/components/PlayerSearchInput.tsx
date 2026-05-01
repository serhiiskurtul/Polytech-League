import React, { useState, useRef, useEffect } from 'react';
import { PlayerDto } from '@/shared/types/player.types';
import styles from './PlayerSearchInput.module.css';

interface PlayerSearchInputProps {
  label: string;
  players: PlayerDto[];
  selectedId: number;
  onSelect: (playerId: number) => void;
  onQuickAdd: (name: string) => void;
  placeholder?: string;
}

const PlayerSearchInput: React.FC<PlayerSearchInputProps> = ({
  label,
  players,
  selectedId,
  onSelect,
  onQuickAdd,
  placeholder = "Введіть ім'я..."
}) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [isOpen, setIsOpen] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);

  // Set initial search term if a player is already selected
  useEffect(() => {
    if (selectedId) {
      const player = players.find(p => p.id === selectedId);
      if (player) {
        setSearchTerm(`${player.firstName} ${player.lastName}`);
      }
    } else {
        setSearchTerm('');
    }
  }, [selectedId, players]);

  // Close dropdown when clicking outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredPlayers = players.filter(p => {
    const fullName = `${p.firstName} ${p.patronymic || ''} ${p.lastName}`.toLowerCase();
    const search = searchTerm.toLowerCase();
    return fullName.includes(search) || 
           p.group?.toLowerCase().includes(search) ||
           p.studentId?.toLowerCase().includes(search);
  }).slice(0, 8);

  const exactMatch = players.find(p => 
    `${p.firstName} ${p.lastName}`.toLowerCase() === searchTerm.toLowerCase()
  );

  return (
    <div className={styles.searchWrapper} ref={wrapperRef}>
      <label className={styles.label}>{label}</label>
      <div className={styles.inputContainer}>
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value);
            setIsOpen(true);
            if (e.target.value === '') onSelect(0);
          }}
          onFocus={() => setIsOpen(true)}
          placeholder={placeholder}
          className={styles.searchInput}
        />
        {searchTerm && (
          <button 
            type="button" 
            className={styles.clearBtn}
            onClick={() => { setSearchTerm(''); onSelect(0); }}
          >
            <i className="fas fa-times"></i>
          </button>
        )}
      </div>

      {isOpen && (searchTerm.length > 0 || filteredPlayers.length > 0) && (
        <ul className={styles.resultsList}>
          {filteredPlayers.map(player => (
            <li 
              key={player.id} 
              className={styles.resultItem}
              onClick={() => {
                if (player.id) {
                  onSelect(player.id);
                  setSearchTerm(`${player.firstName} ${player.lastName}`);
                  setIsOpen(false);
                }
              }}
            >
              <div className={styles.playerAvatar}>
                {player.firstName[0]}{player.lastName[0]}
              </div>
              <div className={styles.playerInfo}>
                <span className={styles.playerName}>
                  {player.firstName} {player.patronymic} {player.lastName}
                </span>
                <span className={styles.playerGroup}>
                  {player.group} {player.studyYears ? `• ${player.studyYears}` : ''}
                </span>
                {player.studentId && (
                  <span className={styles.studentId}>ID: {player.studentId}</span>
                )}
              </div>
            </li>
          ))}

          {!exactMatch && searchTerm.length >= 2 && (
            <li 
              className={`${styles.resultItem} ${styles.addOption}`}
              onClick={() => {
                onQuickAdd(searchTerm);
                setIsOpen(false);
              }}
            >
              <i className="fas fa-user-plus"></i>
              <span>Додати <strong>"{searchTerm}"</strong>...</span>
            </li>
          )}

          {filteredPlayers.length === 0 && !searchTerm && (
            <li className={styles.noResults}>Почніть вводити ім'я</li>
          )}
        </ul>
      )}
    </div>
  );
};

export default PlayerSearchInput;
