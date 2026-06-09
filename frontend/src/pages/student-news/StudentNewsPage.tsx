import React, { useEffect, useState } from 'react';
import { getExternalAchievements, createExternalAchievement, updateExternalAchievement, deleteExternalAchievement, ExternalAchievement, ExternalAchievementMedia } from '@/entities/achievement/api/externalAchievements.api';
import styles from './StudentNewsPage.module.css';
import { useAuth } from '@/entities/session';
import { toast } from 'react-hot-toast';
import { getPhotoUrl } from '@/shared/utils/photo.utils';

const StudentNewsPage: React.FC = () => {
  const { user } = useAuth();
  const isAdmin = user?.role === 1 || user?.role === 'Admin';
  
  const [achievements, setAchievements] = useState<ExternalAchievement[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddForm, setShowAddForm] = useState(false);
  const [editingArticleId, setEditingArticleId] = useState<number | null>(null);
  const [fullscreenMedia, setFullscreenMedia] = useState<ExternalAchievementMedia | null>(null);
  
  const [newEntry, setNewEntry] = useState({
    studentName: '',
    title: '',
    content: '',
    date: new Date().toISOString().split('T')[0],
    externalLink: ''
  });
  const [selectedPhotos, setSelectedPhotos] = useState<File[]>([]);
  const [selectedVideos, setSelectedVideos] = useState<File[]>([]);

  useEffect(() => {
    fetchAchievements();
  }, []);

  const fetchAchievements = async () => {
    try {
      setLoading(true);
      const data = await getExternalAchievements();
      setAchievements(data);
    } catch (error) {
      toast.error('Помилка при завантаженні новин');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingArticleId) {
        await updateExternalAchievement(editingArticleId, {
          ...newEntry,
          photos: selectedPhotos,
          videos: selectedVideos
        });
        toast.success('Зміни збережено!');
      } else {
        await createExternalAchievement({
          ...newEntry,
          photos: selectedPhotos,
          videos: selectedVideos
        });
        toast.success('Статтю додано!');
      }
      setShowAddForm(false);
      setEditingArticleId(null);
      setNewEntry({
        studentName: '',
        title: '',
        content: '',
        date: new Date().toISOString().split('T')[0],
        externalLink: ''
      });
      setSelectedPhotos([]);
      setSelectedVideos([]);
      fetchAchievements();
    } catch (error) {
      toast.error('Помилка при збереженні');
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Видалити цю статтю?')) {
      try {
        await deleteExternalAchievement(id);
        toast.success('Видалено!');
        fetchAchievements();
      } catch (error) {
        toast.error('Помилка при видаленні');
      }
    }
  };

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <h1 className={styles.title}>Життя поза коледжем</h1>
        <p className={styles.subtitle}>Досягнення наших студентів на всеукраїнських та міжнародних аренах</p>
        
        {isAdmin && (
          <button 
            className={styles.addBtn}
            onClick={() => {
              if (showAddForm) {
                setEditingArticleId(null);
                setNewEntry({
                  studentName: '',
                  title: '',
                  content: '',
                  date: new Date().toISOString().split('T')[0],
                  externalLink: ''
                });
              }
              setShowAddForm(!showAddForm);
            }}
          >
            <i className={`fas ${showAddForm ? 'fa-times' : 'fa-plus'}`}></i>
            {showAddForm ? 'Закрити форму' : 'Додати статтю'}
          </button>
        )}
      </header>

      {showAddForm && isAdmin && (
        <div className={styles.formWrapper}>
          <h2 style={{ marginBottom: '1rem', color: '#111827' }}>
            {editingArticleId ? 'Редагувати статтю' : 'Додати нову статтю'}
          </h2>
          <form onSubmit={handleSubmit} className={styles.form}>
            <input 
              type="text" 
              placeholder="Ім'я студента" 
              value={newEntry.studentName}
              onChange={(e) => setNewEntry({ ...newEntry, studentName: e.target.value })}
              required
            />
            <input 
              type="text" 
              placeholder="Заголовок досягнення" 
              value={newEntry.title}
              onChange={(e) => setNewEntry({ ...newEntry, title: e.target.value })}
              required
            />
            <textarea 
              placeholder="Опис досягнення / Текст статті" 
              value={newEntry.content}
              onChange={(e) => setNewEntry({ ...newEntry, content: e.target.value })}
              required
              rows={5}
            />
            <input 
              type="date" 
              value={newEntry.date}
              onChange={(e) => setNewEntry({ ...newEntry, date: e.target.value })}
              required
            />
            <input 
              type="url" 
              placeholder="Посилання на оригінал статті (опціонально)" 
              value={newEntry.externalLink}
              onChange={(e) => setNewEntry({ ...newEntry, externalLink: e.target.value })}
            />
            <div className={styles.fileInputWrapper}>
              <label style={{ color: '#aaa', fontSize: '0.9rem' }}>Фото (декілька):</label>
              <input 
                type="file" 
                accept="image/*"
                multiple
                onChange={(e) => setSelectedPhotos(Array.from(e.target.files || []))}
                style={{ background: '#222', color: '#fff', padding: '0.5rem', borderRadius: '5px', border: '1px solid #444' }}
              />
            </div>
            <div className={styles.fileInputWrapper}>
              <label style={{ color: '#aaa', fontSize: '0.9rem' }}>Відео (декілька):</label>
              <input 
                type="file" 
                accept="video/*"
                multiple
                onChange={(e) => setSelectedVideos(Array.from(e.target.files || []))}
                style={{ background: '#222', color: '#fff', padding: '0.5rem', borderRadius: '5px', border: '1px solid #444' }}
              />
            </div>
            <button type="submit" className={styles.submitBtn}>
              {editingArticleId ? 'Зберегти зміни' : 'Зберегти'}
            </button>
          </form>
        </div>
      )}

      {loading ? (
        <div className={styles.loading}>Завантаження...</div>
      ) : achievements.length === 0 ? (
        <div className={styles.empty}>Поки немає жодної статті.</div>
      ) : (
        <div className={styles.grid}>
          {achievements.map((item) => (
            <div key={item.id} className={styles.card}>
              <div className={styles.cardHeader}>
                <div className={styles.date}>{new Date(item.date).toLocaleDateString('uk-UA')}</div>
                <div className={styles.studentName}>{item.studentName}</div>
              </div>
              
              {item.media && item.media.length > 0 && (
                <div className={styles.mediaGallery}>
                  {item.media.map((media) => (
                    <div 
                      key={media.id} 
                      className={styles.mediaItem}
                      onClick={() => setFullscreenMedia(media)}
                      style={{ cursor: 'pointer' }}
                    >
                      {media.type === 'photo' ? (
                        <img src={getPhotoUrl(media.url)!} alt={item.title} className={styles.photo} loading="lazy" decoding="async" />
                      ) : (
                        <div className={styles.video} style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', backgroundColor: '#1e293b' }}>
                          <i className="fas fa-video" style={{ fontSize: '3rem', color: '#475569' }}></i>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}
              
              <div className={styles.cardBody}>
                <h2 className={styles.cardTitle}>{item.title}</h2>
                <p className={styles.content} style={{ whiteSpace: 'pre-line' }}>{item.content}</p>
                
                {item.externalLink && (
                  <a href={item.externalLink} target="_blank" rel="noopener noreferrer" className={styles.link}>
                    Читати оригінал <i className="fas fa-external-link-alt"></i>
                  </a>
                )}
              </div>

              {isAdmin && (
                <div className={styles.adminActions}>
                  <button 
                    onClick={() => {
                      setEditingArticleId(item.id);
                      setNewEntry({
                        studentName: item.studentName,
                        title: item.title,
                        content: item.content,
                        date: new Date(item.date).toISOString().split('T')[0],
                        externalLink: item.externalLink || ''
                      });
                      setShowAddForm(true);
                      window.scrollTo({ top: 0, behavior: 'smooth' });
                    }} 
                    className={styles.editBtn}
                    style={{ marginRight: '0.5rem' }}
                  >
                    <i className="fas fa-edit"></i> Редагувати
                  </button>
                  <button onClick={() => handleDelete(item.id)} className={styles.deleteBtn}>
                    <i className="fas fa-trash"></i> Видалити
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {fullscreenMedia && (
        <div 
          className={styles.lightbox}
          onClick={() => setFullscreenMedia(null)}
        >
          <div className={styles.lightboxContent} onClick={(e) => e.stopPropagation()}>
            <button className={styles.closeBtn} onClick={() => setFullscreenMedia(null)}>
              <i className="fas fa-times"></i>
            </button>
            {fullscreenMedia.type === 'photo' ? (
              <img src={getPhotoUrl(fullscreenMedia.url)!} alt="Fullscreen" className={styles.fullscreenImage} />
            ) : (
              <video src={getPhotoUrl(fullscreenMedia.url)!} controls preload="none" className={styles.fullscreenVideo} />
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default StudentNewsPage;
