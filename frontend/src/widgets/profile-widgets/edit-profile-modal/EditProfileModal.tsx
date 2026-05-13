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

const AVAILABLE_GROUPS = [
  '211', '221', '222', '231', '241', '311', '312', '321', 
  '322', '331', '332', '341', '411', '421', '422', '431', '433', 
  '441', '511', '512', '513', '521', '522', '523', '531', '532', '541'
];

const EditProfileModal = ({ isOpen, onClose, onSave, initialData, isAdmin }: EditProfileModalProps) => {
  const [formData, setFormData] = React.useState({
    firstName: initialData.firstName || '',
    lastName: initialData.lastName || '',
    group: initialData.group || '',
    studentId: initialData.studentId || '',
    studyYears: initialData.studyYears || '',
    city: initialData.city || '',
    bio: initialData.bio || '',
    instagramUrl: initialData.instagramUrl || '',
    isExternal: initialData.isExternal || false,
    footballShirtNumber: initialData.footballShirtNumber !== null && initialData.footballShirtNumber !== undefined ? initialData.footballShirtNumber : '',
    volleyballShirtNumber: initialData.volleyballShirtNumber !== null && initialData.volleyballShirtNumber !== undefined ? initialData.volleyballShirtNumber : '',
    basketballShirtNumber: initialData.basketballShirtNumber !== null && initialData.basketballShirtNumber !== undefined ? initialData.basketballShirtNumber : '',
    previousNames: initialData.previousNames || '',
    gender: initialData.gender || 'None'
  });

  const [loading, setLoading] = React.useState(false);

  // Sync state with initialData when modal opens or initialData changes
  React.useEffect(() => {
    if (isOpen) {
      setFormData({
        firstName: initialData.firstName || '',
        lastName: initialData.lastName || '',
        group: initialData.group || '',
        studentId: initialData.studentId || '',
        studyYears: initialData.studyYears || '',
        city: initialData.city || '',
        bio: initialData.bio || '',
        instagramUrl: initialData.instagramUrl || '',
        isExternal: initialData.isExternal || false,
        footballShirtNumber: initialData.footballShirtNumber !== null && initialData.footballShirtNumber !== undefined ? initialData.footballShirtNumber : '',
        volleyballShirtNumber: initialData.volleyballShirtNumber !== null && initialData.volleyballShirtNumber !== undefined ? initialData.volleyballShirtNumber : '',
        basketballShirtNumber: initialData.basketballShirtNumber !== null && initialData.basketballShirtNumber !== undefined ? initialData.basketballShirtNumber : '',
        previousNames: initialData.previousNames || '',
        gender: initialData.gender || 'None'
      });
    }
  }, [isOpen, initialData]);

  if (!isOpen) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value, type } = e.target as HTMLInputElement;
    const val = type === 'checkbox' ? (e.target as HTMLInputElement).checked : value;
    setFormData(prev => ({ ...prev, [name]: val }));
  };


  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    if (formData.studentId && !/^[A-Za-z]{2}\d{8}$/.test(formData.studentId)) {
      import('react-hot-toast').then(({ toast }) => {
        toast.error('Студентський квиток має бути у форматі: 2 літери та 8 цифр (наприклад, PH12345678)');
      });
      setLoading(false);
      return;
    }
    
    if (formData.gender === 'None') {
      import('react-hot-toast').then(({ toast }) => {
        toast.error('Будь ласка, оберіть вашу стать. Це обов\'язково для участі в змаганнях.');
      });
      setLoading(false);
      return;
    }

    const sanitizedData = {
      ...formData,
      studentId: formData.studentId.toUpperCase(),
      footballShirtNumber: (formData.footballShirtNumber === '' || formData.footballShirtNumber === null) ? null : parseInt(formData.footballShirtNumber.toString()),
      volleyballShirtNumber: (formData.volleyballShirtNumber === '' || formData.volleyballShirtNumber === null) ? null : parseInt(formData.volleyballShirtNumber.toString()),
      basketballShirtNumber: (formData.basketballShirtNumber === '' || formData.basketballShirtNumber === null) ? null : parseInt(formData.basketballShirtNumber.toString()),
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
            <div className={styles.nameRow}>
              <div className={styles.formGroup}>
                <label htmlFor="firstName">Ім'я</label>
                <input
                  type="text"
                  id="firstName"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="lastName">Прізвище</label>
                <input
                  type="text"
                  id="lastName"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="gender">Стать (Обов'язково)</label>
              <div className={styles.genderToggle}>
                <button 
                  type="button" 
                  className={`${styles.genderOption} ${formData.gender === 'Male' ? styles.activeGender : ''}`}
                  onClick={() => setFormData(prev => ({ ...prev, gender: 'Male' }))}
                >
                  <i className="fas fa-mars"></i> Чоловік
                </button>
                <button 
                  type="button" 
                  className={`${styles.genderOption} ${formData.gender === 'Female' ? styles.activeGender : ''}`}
                  onClick={() => setFormData(prev => ({ ...prev, gender: 'Female' }))}
                >
                  <i className="fas fa-venus"></i> Жінка
                </button>
              </div>
            </div>

            {!isAdmin && (
              <>
                <div className={styles.formGroup}>
                  <label htmlFor="previousNames"><i className="fas fa-history"></i> Попереднє прізвище (якщо змінювали)</label>
                  <input
                    type="text"
                    id="previousNames"
                    name="previousNames"
                    value={formData.previousNames}
                    onChange={handleChange}
                    placeholder="Введіть старе прізвище"
                  />
                  <small className={styles.helpText}>Допоможе знайти ваш профіль за старим прізвищем</small>
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="group">Група</label>
                  <select
                    id="group"
                    name="group"
                    value={formData.group}
                    onChange={(e) => setFormData(prev => ({ ...prev, group: e.target.value }))}
                    className={styles.selectInput}
                  >
                    <option value="">Оберіть групу</option>
                    {AVAILABLE_GROUPS.map(g => (
                      <option key={g} value={g}>{g} група</option>
                    ))}
                  </select>
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="studentId">Студентський квиток (ID)</label>
                  <input
                    type="text"
                    id="studentId"
                    name="studentId"
                    value={formData.studentId}
                    onChange={handleChange}
                    placeholder="Приклад: PH12345678"
                    maxLength={10}
                  />
                  <small className={styles.helpText}>2 літери та 8 цифр</small>
                </div>
                
                <div className={styles.formGroup}>
                  <label htmlFor="studyYears">Роки навчання</label>
                  <input
                    type="text"
                    id="studyYears"
                    name="studyYears"
                    value={formData.studyYears}
                    onChange={handleChange}
                    placeholder="Наприклад: 2021-2025"
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
              <label htmlFor="bio"><i className="fas fa-info-circle"></i> Про себе</label>
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
                <label htmlFor="instagramUrl"><i className="fab fa-instagram"></i> Instagram</label>
                <input
                  type="text"
                  id="instagramUrl"
                  name="instagramUrl"
                  value={formData.instagramUrl}
                  onChange={handleChange}
                  placeholder="@username або посилання"
                />
                <small style={{ fontSize: '11px', color: 'var(--muted)', marginTop: '4px' }}>
                  Введіть нікнейм або повне посилання на профіль
                </small>
              </div>
            </div>
            
            {/* Conditional Shirt Numbers based on participation */}
            {(initialData.sports?.includes('football') || 
              initialData.sports?.includes('volleyball') || 
              initialData.sports?.includes('basketball_3x3')) && (
              <div className={styles.shirtNumbersGrid}>
                {initialData.sports?.includes('football') && (
                  <div className={styles.formGroup}>
                    <label htmlFor="footballShirtNumber">Номер у футзалі</label>
                    <input
                      type="number"
                      id="footballShirtNumber"
                      name="footballShirtNumber"
                      value={formData.footballShirtNumber}
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
            )}



            {/* isExternal removed as requested */}

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
