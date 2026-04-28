import React, { useState } from 'react';
import { PlayerDto } from '@/shared/types/player.types';
import styles from './EditProfileModal.module.css';

interface EditProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: Partial<PlayerDto>) => Promise<void>;
  initialData: PlayerDto;
  isAdmin?: boolean;
}

const EditProfileModal = ({ isOpen, onClose, onSave, initialData, isAdmin }: EditProfileModalProps) => {
  const [formData, setFormData] = useState({

    group: initialData.group || '',
    studentId: initialData.studentId || '',
    faculty: initialData.faculty || '',
    city: initialData.city || '',
    bio: initialData.bio || '',
    instagramUrl: initialData.instagramUrl || ''
  });
  const [loading, setLoading] = useState(false);

  if (!isOpen) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await onSave(formData);
      onClose();
    } catch (error) {
      console.error('Failed to save profile:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modalContent} onClick={e => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>Редагувати профіль</h2>
          <button className={styles.closeBtn} onClick={onClose}>&times;</button>
        </div>
        
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.formBody}>
            {!isAdmin && (
              <>
                <div className={styles.formGroup}>
                  <label htmlFor="group">Група</label>
                  <input
                    type="text"
                    id="group"
                    name="group"
                    value={formData.group}
                    onChange={handleChange}
                    placeholder="Наприклад: ІТ-41"
                  />
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="studentId">Студентський квиток (ID)</label>
                  <input
                    type="text"
                    id="studentId"
                    name="studentId"
                    value={formData.studentId}
                    onChange={handleChange}
                    placeholder="Наприклад: KB 12345678"
                  />
                </div>
                
                <div className={styles.formGroup}>
                  <label htmlFor="faculty">Факультет</label>
                  <input
                    type="text"
                    id="faculty"
                    name="faculty"
                    value={formData.faculty}
                    onChange={handleChange}
                    placeholder="Наприклад: ФІОТ"
                  />
                </div>
                
                <div className={styles.formGroup}>
                  <label htmlFor="city">Місто</label>
                  <input
                    type="text"
                    id="city"
                    name="city"
                    value={formData.city}
                    onChange={handleChange}
                    placeholder="Наприклад: Київ"
                  />
                </div>
              </>
            )}
            
            <div className={styles.formGroup}>

              <label htmlFor="bio">Про себе</label>
              <textarea
                id="bio"
                name="bio"
                value={formData.bio}
                onChange={handleChange}
                placeholder="Розкажіть трохи про себе..."
                rows={3}
              />
            </div>

            <div className={styles.socialGroup}>
              <div className={styles.formGroup}>
                <label htmlFor="instagramUrl"><i className="fab fa-instagram"></i> Instagram URL</label>
                <input
                  type="text"
                  id="instagramUrl"
                  name="instagramUrl"
                  value={formData.instagramUrl}
                  onChange={handleChange}
                  placeholder="https://instagram.com/user"
                />
              </div>
            </div>
          </div>
          
          <div className={styles.modalActions}>
            <button type="button" className={styles.cancelBtn} onClick={onClose} disabled={loading}>
              Скасувати
            </button>
            <button type="submit" className={styles.saveBtn} disabled={loading}>
              {loading ? 'Збереження...' : 'Зберегти зміни'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditProfileModal;
