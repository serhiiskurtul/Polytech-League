import React, { useState, useEffect } from 'react';
import styles from './AdminCollegesPage.module.css';
import { getColleges, createCollege, updateCollege, deleteCollege, College } from '@/shared/api/colleges.api';
import toast from 'react-hot-toast';

const AdminCollegesPage: React.FC = () => {
  const [colleges, setColleges] = useState<College[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCollege, setEditingCollege] = useState<College | null>(null);
  const [formData, setFormData] = useState<Partial<College>>({
    name: '',
    shortName: '',
    city: '',
    logoUrl: ''
  });

  useEffect(() => {
    fetchColleges();
  }, []);

  const fetchColleges = async () => {
    try {
      const data = await getColleges();
      setColleges(data);
    } catch (error) {
      toast.error('Не вдалося завантажити список коледжів');
    } finally {
      setIsLoading(false);
    }
  };

  const handleOpenModal = (college: College | null = null) => {
    if (college) {
      setEditingCollege(college);
      setFormData(college);
    } else {
      setEditingCollege(null);
      setFormData({ name: '', shortName: '', city: '', logoUrl: '' });
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingCollege(null);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingCollege) {
        await updateCollege(editingCollege.id, formData);
        toast.success('Коледж оновлено');
      } else {
        await createCollege(formData);
        toast.success('Коледж додано');
      }
      fetchColleges();
      handleCloseModal();
    } catch (error) {
      toast.error('Сталася помилка при збереженні');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Ви впевнені, що хочете видалити цей коледж? Це може вплинути на результати змагань.')) return;
    try {
      await deleteCollege(id);
      toast.success('Коледж видалено');
      fetchColleges();
    } catch (error) {
      toast.error('Не вдалося видалити коледж');
    }
  };

  if (isLoading) return <div className={styles.loading}>Завантаження...</div>;

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <div>
          <h1>Керування коледжами</h1>
          <p>Додавання та редагування навчальних закладів для всеукраїнського заліку</p>
        </div>
        <button className={styles.addBtn} onClick={() => handleOpenModal()}>
          <i className="fas fa-plus"></i> Додати коледж
        </button>
      </header>

      <div className={styles.grid}>
        {colleges.map(college => (
          <div key={college.id} className={styles.card}>
            <div className={styles.cardInfo}>
              <div className={styles.logoContainer}>
                {college.logoUrl ? (
                  <img src={college.logoUrl} alt={college.name} />
                ) : (
                  <i className="fas fa-university"></i>
                )}
              </div>
              <div className={styles.details}>
                <h3>{college.name}</h3>
                <span className={styles.shortName}>{college.shortName}</span>
                <span className={styles.city}><i className="fas fa-map-marker-alt"></i> {college.city || 'Місто не вказано'}</span>
              </div>
            </div>
            <div className={styles.actions}>
              <button onClick={() => handleOpenModal(college)} className={styles.editBtn}>
                <i className="fas fa-edit"></i>
              </button>
              <button onClick={() => handleDelete(college.id)} className={styles.deleteBtn}>
                <i className="fas fa-trash"></i>
              </button>
            </div>
          </div>
        ))}
      </div>

      {isModalOpen && (
        <div className={styles.modalOverlay}>
          <div className={styles.modal}>
            <h2>{editingCollege ? 'Редагувати коледж' : 'Додати новий коледж'}</h2>
            <form onSubmit={handleSubmit}>
              <div className={styles.formGroup}>
                <label>Повна назва</label>
                <input 
                  name="name" 
                  value={formData.name} 
                  onChange={handleInputChange} 
                  required 
                  placeholder="Наприклад: Відокремлений структурний підрозділ..."
                />
              </div>
              <div className={styles.formGroup}>
                <label>Абревіатура / Коротка назва</label>
                <input 
                  name="shortName" 
                  value={formData.shortName} 
                  onChange={handleInputChange} 
                  required 
                  placeholder="Наприклад: ВСП ППК"
                />
              </div>
              <div className={styles.formGroup}>
                <label>Місто</label>
                <input 
                  name="city" 
                  value={formData.city} 
                  onChange={handleInputChange} 
                  placeholder="Наприклад: Львів"
                />
              </div>
              <div className={styles.formGroup}>
                <label>URL Логотипу (опціонально)</label>
                <input 
                  name="logoUrl" 
                  value={formData.logoUrl} 
                  onChange={handleInputChange} 
                  placeholder="https://example.com/logo.png"
                />
              </div>
              <div className={styles.modalActions}>
                <button type="button" onClick={handleCloseModal} className={styles.cancelBtn}>Скасувати</button>
                <button type="submit" className={styles.saveBtn}>Зберегти</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminCollegesPage;
