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
    instagramUrl: initialData.instagramUrl || '',
    isExternal: initialData.isExternal || false,
    futsalShirtNumber: initialData.futsalShirtNumber || '',
    volleyballShirtNumber: initialData.volleyballShirtNumber || '',
    basketballShirtNumber: initialData.basketballShirtNumber || ''
  });

  const [loading, setLoading] = useState(false);

  if (!isOpen) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value, type } = e.target as HTMLInputElement;
    const val = type === 'checkbox' ? (e.target as HTMLInputElement).checked : value;
    setFormData(prev => ({ ...prev, [name]: val }));
  };


  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    const sanitizedData = {
      ...formData,
      futsalShirtNumber: formData.futsalShirtNumber === '' ? null : parseInt(formData.futsalShirtNumber.toString()),
      volleyballShirtNumber: formData.volleyballShirtNumber === '' ? null : parseInt(formData.volleyballShirtNumber.toString()),
      basketballShirtNumber: formData.basketballShirtNumber === '' ? null : parseInt(formData.basketballShirtNumber.toString()),
    };

    try {
      await onSave(sanitizedData);
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
            
            {/* Conditional Shirt Numbers based on participation */}
            <div className={styles.shirtNumbersGrid}>
              {initialData.sports?.includes('futsal') && (
                <div className={styles.formGroup}>
                  <label htmlFor="futsalShirtNumber">Номер у футзалі</label>
                  <input
                    type="number"
                    id="futsalShirtNumber"
                    name="futsalShirtNumber"
                    value={formData.futsalShirtNumber}
                    onChange={handleChange}
                    placeholder="№"
                  />
                </div>
              )}
              {initialData.sports?.includes('volleyball') && (
                <div className={styles.formGroup}>
                  <label htmlFor="volleyballShirtNumber">Номер у волейболі</label>
                  <input
                    type="number"
                    id="volleyballShirtNumber"
                    name="volleyballShirtNumber"
                    value={formData.volleyballShirtNumber}
                    onChange={handleChange}
                    placeholder="№"
                  />
                </div>
              )}
              {initialData.sports?.includes('basketball_3x3') && (
                <div className={styles.formGroup}>
                  <label htmlFor="basketballShirtNumber">Номер у баскетболі</label>
                  <input
                    type="number"
                    id="basketballShirtNumber"
                    name="basketballShirtNumber"
                    value={formData.basketballShirtNumber}
                    onChange={handleChange}
                    placeholder="№"
                  />
                </div>
              )}
            </div>


            {isAdmin && (
              <div className={styles.formGroup} style={{ flexDirection: 'row', alignItems: 'center', gap: '10px', marginTop: '10px' }}>
                <input
                  type="checkbox"
                  id="isExternal"
                  name="isExternal"
                  checked={formData.isExternal}
                  onChange={handleChange}
                  style={{ width: '20px', height: '20px', cursor: 'pointer' }}
                />
                <label htmlFor="isExternal" style={{ margin: 0, cursor: 'pointer', fontWeight: '600', color: '#e11d48' }}>
                  Студент іншого закладу (не показувати в рейтингу)
                </label>
              </div>
            )}
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
