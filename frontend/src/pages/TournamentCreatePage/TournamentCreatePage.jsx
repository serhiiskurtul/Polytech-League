import React, { useState, useEffect } from 'react';
import styles from './TournamentCreatePage.module.css';

const TournamentCreatePage = () => {
  // Стан форми
  const [formData, setFormData] = useState({
    name: '',
    sport: 'football',
    type: 'single_elimination',
    description: '',
    maxTeams: 8,
    minTeams: 2,
    registrationDeadline: '',
    startDate: '',
    endDate: '',
    location: '',
    venue: '',
    rules: '',
    prizes: '',
    entryFee: 0,
    isPublic: true,
    requireApproval: false,
    categories: [],
  });

  const [categories, setCategories] = useState([
    'Чоловіки',
    'Жінки',
    'Змішані'
  ]);

    const [availableSports, setAvailableSports] = useState([
        { id: 'table_tennis', name: 'Настільний теніс', icon: 'fas fa-table-tennis' },
        { id: 'armwrestling', name: 'Армреслінг', icon: 'fas fa-hand-rock' },
        { id: 'futsal', name: 'Футзал', icon: 'fas fa-futbol' },
        { id: 'volleyball', name: 'Волейбол', icon: 'fas fa-volleyball-ball' },
        { id: 'beach_volleyball', name: 'Пляжний волейбол', icon: 'fas fa-sun' },
        { id: 'basketball', name: 'Баскетбол', icon: 'fas fa-basketball-ball' },
        { id: 'badminton', name: 'Бадмінтон', icon: 'fas fa-feather-alt' },
        { id: 'chess', name: 'Шахи', icon: 'fas fa-chess' },
        { id: 'checkers', name: 'Шашки', icon: 'fas fa-th-large' }
    ]);

  const [tournamentTypes, setTournamentTypes] = useState([
    { id: 'single_elimination', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
    { id: 'double_elimination', name: 'Двоєступінчаста система', description: 'Друга спроба при поразці' },
    { id: 'round_robin', name: 'Кругова система', description: 'Всі грають з усіма' },
    { id: 'swiss', name: 'Швейцарська система', description: 'Для шахів та шашок' },
    { id: 'groups_playoff', name: 'Груповий етап + плей-офф', description: 'Найпопулярніший формат' }
  ]);

  const [selectedCategories, setSelectedCategories] = useState([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [previewImage, setPreviewImage] = useState(null);

  // Обробник зміни полів форми
  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  // Обробник зміни дат
  const handleDateChange = (name, value) => {
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // Обробник вибору категорій
  const handleCategoryToggle = (category) => {
    if (selectedCategories.includes(category)) {
      setSelectedCategories(prev => prev.filter(c => c !== category));
    } else {
      setSelectedCategories(prev => [...prev, category]);
    }
  };

  // Обробник завантаження зображення
  const handleImageUpload = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewImage(reader.result);
      };
      reader.readAsDataURL(file);
    }
  };

  // Валідація форми
  const validateForm = () => {
    const errors = [];
    
    if (!formData.name.trim()) errors.push('Назва турніру обов\'язкова');
    if (!formData.description.trim()) errors.push('Опис турніру обов\'язковий');
    if (!formData.startDate) errors.push('Дата початку обов\'язкова');
    if (!formData.registrationDeadline) errors.push('Термін реєстрації обов\'язковий');
    if (formData.maxTeams < formData.minTeams) errors.push('Максимальна кількість команд не може бути меншою за мінімальну');
    if (new Date(formData.registrationDeadline) > new Date(formData.startDate)) {
      errors.push('Термін реєстрації не може бути пізніше дати початку');
    }
    if (selectedCategories.length === 0) errors.push('Оберіть хоча б одну категорію');
    
    return errors;
  };

  // Обробник відправки форми
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const errors = validateForm();
    if (errors.length > 0) {
      alert(`Помилки валідації:\n${errors.join('\n')}`);
      return;
    }
    
    setIsSubmitting(true);
    
    // Імітація запиту до API
    setTimeout(() => {
      const tournamentData = {
        ...formData,
        categories: selectedCategories,
        createdAt: new Date().toISOString(),
        createdBy: 'Адміністратор',
        status: 'pending',
        participants: 0
      };
      
      console.log('Турнір створено:', tournamentData);
      alert('Турнір успішно створено!');
      setIsSubmitting(false);
      
      // Скидання форми
      setFormData({
        name: '',
        sport: 'football',
        type: 'single_elimination',
        description: '',
        maxTeams: 8,
        minTeams: 2,
        registrationDeadline: '',
        startDate: '',
        endDate: '',
        location: '',
        venue: '',
        rules: '',
        prizes: '',
        entryFee: 0,
        isPublic: true,
        requireApproval: false,
        categories: [],
      });
      setSelectedCategories([]);
      setPreviewImage(null);
    }, 1500);
  };

  // Заповнення тестовими датами
  const fillTestData = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);
    const twoWeeks = new Date();
    twoWeeks.setDate(twoWeeks.getDate() + 14);
    
    setFormData({
      name: 'Чемпіонат коледжу з футзалу 2024',
      sport: 'football',
      type: 'groups_playoff',
      description: 'Щорічний чемпіонат серед студентських команд. Гра за кубок декана та цінні призи.',
      maxTeams: 16,
      minTeams: 4,
      registrationDeadline: tomorrow.toISOString().split('T')[0],
      startDate: nextWeek.toISOString().split('T')[0],
      endDate: twoWeeks.toISOString().split('T')[0],
      location: 'Спортивний комплекс "Університетський"',
      venue: 'Зал №3',
      rules: '1. Команда складається з 5 гравців та 3 запасних\n2. Тривалість матчу - 2 тайми по 20 хвилин\n3. Заборонена груба гра\n4. Рішення судді остаточні',
      prizes: '1 місце - 5000 грн та кубок\n2 місце - 3000 грн\n3 місце - 1500 грн\nНайкращий бомбардир - 1000 грн',
      entryFee: 500,
      isPublic: true,
      requireApproval: true,
      categories: [],
    });
    
    setSelectedCategories(['Чоловіки', 'Студенти']);
  };

  // Розрахунок тривалості турніру
  const calculateDuration = () => {
    if (formData.startDate && formData.endDate) {
      const start = new Date(formData.startDate);
      const end = new Date(formData.endDate);
      const diffTime = Math.abs(end - start);
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
      return diffDays + 1; // +1 для включення початкового дня
    }
    return 0;
  };

  useEffect(() => {
    // Оновлення категорій у формі при зміні вибраних
    setFormData(prev => ({
      ...prev,
      categories: selectedCategories
    }));
  }, [selectedCategories]);

  return (
    <div className={styles.tournamentCreatePage}>
      <div className={styles.container}>
        {/* Заголовок сторінки */}
        <div className={styles.pageHeader}>
          <div className={styles.headerContent}>
            <h1 className={styles.pageTitle}>
              <i className="fas fa-plus-circle"></i> Створення турніру
            </h1>
            <button 
              className={styles.previewBtn}
              onClick={() => alert('Попередній перегляд тимчасово недоступний')}
            >
              <i className="fas fa-eye"></i> Попередній перегляд
            </button>
          </div>
          <p className={styles.pageSubtitle}>
            Заповніть форму для створення нового турніру. Всі поля позначені * є обов'язковими.
          </p>
        </div>

        <div className={styles.contentWrapper}>
          {/* Форма створення */}
          <div className={styles.formSection}>
            <form onSubmit={handleSubmit} className={styles.tournamentForm}>
              
              {/* Основний блок */}
              <div className={styles.formBlock}>
                <h3 className={styles.blockTitle}>
                  <i className="fas fa-info-circle"></i> Основна інформація
                </h3>
                
                <div className={styles.formGrid}>
                  {/* Назва турніру */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-trophy"></i> Назва турніру *
                    </label>
                    <input
                      type="text"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      className={styles.formInput}
                      placeholder="Наприклад: Чемпіонат коледжу з футзалу 2024"
                      required
                    />
                  </div>

                  {/* Вид спорту */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-running"></i> Вид спорту *
                    </label>
                    <div className={styles.sportsGrid}>
                      {availableSports.map(sport => (
                        <label 
                          key={sport.id} 
                          className={`${styles.sportOption} ${
                            formData.sport === sport.id ? styles.selected : ''
                          }`}
                        >
                          <input
                            type="radio"
                            name="sport"
                            value={sport.id}
                            checked={formData.sport === sport.id}
                            onChange={handleInputChange}
                            className={styles.radioInput}
                          />
                          <div className={styles.sportIcon}>
                            <i className={sport.icon}></i>
                          </div>
                          <span className={styles.sportName}>{sport.name}</span>
                        </label>
                      ))}
                    </div>
                  </div>

                  {/* Формат турніру */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-chess-board"></i> Формат турніру *
                    </label>
                    <select
                      name="type"
                      value={formData.type}
                      onChange={handleInputChange}
                      className={styles.formSelect}
                      required
                    >
                      {tournamentTypes.map(type => (
                        <option key={type.id} value={type.id}>
                          {type.name} - {type.description}
                        </option>
                      ))}
                    </select>
                  </div>

                  {/* Кількість команд/учасників */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-users"></i> Кількість команд/учасників
                    </label>
                    <div className={styles.rangeGroup}>
                      <div className={styles.rangeInput}>
                        <span className={styles.rangeLabel}>Мін:</span>
                        <input
                          type="number"
                          name="minTeams"
                          value={formData.minTeams}
                          onChange={handleInputChange}
                          className={styles.numberInput}
                          min="2"
                          max="64"
                        />
                      </div>
                      <div className={styles.rangeSeparator}>-</div>
                      <div className={styles.rangeInput}>
                        <span className={styles.rangeLabel}>Макс:</span>
                        <input
                          type="number"
                          name="maxTeams"
                          value={formData.maxTeams}
                          onChange={handleInputChange}
                          className={styles.numberInput}
                          min={formData.minTeams}
                          max="128"
                        />
                      </div>
                    </div>
                    <div className={styles.rangeHint}>
                      Оптимально: 8, 16, 32 для олімпійської системи
                    </div>
                  </div>

                  {/* Опис */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-align-left"></i> Опис турніру *
                    </label>
                    <textarea
                      name="description"
                      value={formData.description}
                      onChange={handleInputChange}
                      className={styles.formTextarea}
                      placeholder="Детальний опис турніру, його цілей та особливостей..."
                      rows="4"
                      required
                    />
                  </div>
                </div>
              </div>

              {/* Дати та місце */}
              <div className={styles.formBlock}>
                <h3 className={styles.blockTitle}>
                  <i className="fas fa-calendar-alt"></i> Дати та місце проведення
                </h3>
                
                <div className={styles.formGrid}>
                  {/* Термін реєстрації */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-user-clock"></i> Термін реєстрації *
                    </label>
                    <input
                      type="date"
                      value={formData.registrationDeadline}
                      onChange={(e) => handleDateChange('registrationDeadline', e.target.value)}
                      className={styles.formInput}
                      required
                    />
                  </div>

                  {/* Дата початку */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-play-circle"></i> Дата початку *
                    </label>
                    <input
                      type="date"
                      value={formData.startDate}
                      onChange={(e) => handleDateChange('startDate', e.target.value)}
                      className={styles.formInput}
                      required
                    />
                  </div>

                  {/* Дата завершення */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-flag-checkered"></i> Дата завершення
                    </label>
                    <input
                      type="date"
                      value={formData.endDate}
                      onChange={(e) => handleDateChange('endDate', e.target.value)}
                      className={styles.formInput}
                    />
                    {formData.startDate && formData.endDate && (
                      <div className={styles.durationBadge}>
                        Тривалість: {calculateDuration()} днів
                      </div>
                    )}
                  </div>

                  {/* Місце проведення */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-map-marker-alt"></i> Місто/Локація
                    </label>
                    <input
                      type="text"
                      name="location"
                      value={formData.location}
                      onChange={handleInputChange}
                      className={styles.formInput}
                      placeholder="Наприклад: Київ"
                    />
                  </div>

                  {/* Місце проведення (детально) */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-building"></i> Спортивний об'єкт
                    </label>
                    <input
                      type="text"
                      name="venue"
                      value={formData.venue}
                      onChange={handleInputChange}
                      className={styles.formInput}
                      placeholder="Наприклад: Зал №3, Спорткомплекс 'Університетський'"
                    />
                  </div>
                </div>
              </div>

              {/* Категорії та обмеження */}
              <div className={styles.formBlock}>
                <h3 className={styles.blockTitle}>
                  <i className="fas fa-filter"></i> Категорії та обмеження
                </h3>
                
                <div className={styles.formGrid}>
                  {/* Категорії */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-tags"></i> Категорії учасників *
                    </label>
                    <div className={styles.categoriesGrid}>
                      {categories.map(category => (
                        <button
                          key={category}
                          type="button"
                          className={`${styles.categoryBtn} ${
                            selectedCategories.includes(category) ? styles.selected : ''
                          }`}
                          onClick={() => handleCategoryToggle(category)}
                        >
                          {category}
                          {selectedCategories.includes(category) && (
                            <i className="fas fa-check"></i>
                          )}
                        </button>
                      ))}
                    </div>
                  </div>
                </div>
              </div>

              {/* Деталі турніру */}
              <div className={styles.formBlock}>
                <h3 className={styles.blockTitle}>
                  <i className="fas fa-cog"></i> Деталі та налаштування
                </h3>
                
                <div className={styles.formGrid}>
                  {/* Правила */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-book"></i> Правила турніру
                    </label>
                    <textarea
                      name="rules"
                      value={formData.rules}
                      onChange={handleInputChange}
                      className={styles.formTextarea}
                      placeholder="Основні правила та регламент..."
                      rows="4"
                    />
                  </div>

                  {/* Призи */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-gift"></i> Призи та нагороди
                    </label>
                    <textarea
                      name="prizes"
                      value={formData.prizes}
                      onChange={handleInputChange}
                      className={styles.formTextarea}
                      placeholder="Опишіть призовий фонд..."
                      rows="4"
                    />
                  </div>

                  {/* Вступний внесок */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-money-bill-wave"></i> Вступний внесок (грн)
                    </label>
                    <input
                      type="number"
                      name="entryFee"
                      value={formData.entryFee}
                      onChange={handleInputChange}
                      className={styles.formInput}
                      min="0"
                      step="50"
                    />
                    {formData.entryFee > 0 && (
                      <div className={styles.feeNote}>
                        Збір буде відображатися при реєстрації
                      </div>
                    )}
                  </div>

                  {/* Налаштування видимості */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-eye"></i> Видимість
                    </label>
                    <div className={styles.checkboxGroup}>
                      <label className={styles.checkboxLabel}>
                        <input
                          type="checkbox"
                          name="isPublic"
                          checked={formData.isPublic}
                          onChange={handleInputChange}
                          className={styles.checkboxInput}
                        />
                        <span className={styles.checkboxText}>Публічний турнір</span>
                      </label>
                      <label className={styles.checkboxLabel}>
                        <input
                          type="checkbox"
                          name="requireApproval"
                          checked={formData.requireApproval}
                          onChange={handleInputChange}
                          className={styles.checkboxInput}
                        />
                        <span className={styles.checkboxText}>Модерація заявок</span>
                      </label>
                    </div>
                  </div>

                  {/* Завантаження зображення */}
                  <div className={styles.formGroup}>
                    <label className={styles.formLabel}>
                      <i className="fas fa-image"></i> Зображення турніру
                    </label>
                    <div className={styles.imageUpload}>
                      {previewImage ? (
                        <div className={styles.imagePreview}>
                          <img src={previewImage} alt="Попередній перегляд" />
                          <button
                            type="button"
                            className={styles.removeImage}
                            onClick={() => setPreviewImage(null)}
                          >
                            <i className="fas fa-times"></i>
                          </button>
                        </div>
                      ) : (
                        <label className={styles.uploadLabel}>
                          <input
                            type="file"
                            accept="image/*"
                            onChange={handleImageUpload}
                            className={styles.uploadInput}
                          />
                          <div className={styles.uploadArea}>
                            <i className="fas fa-cloud-upload-alt"></i>
                            <p>Перетягніть зображення або клацніть для вибору</p>
                            <span>Рекомендовано: 1200×600 px, JPG/PNG</span>
                          </div>
                        </label>
                      )}
                    </div>
                  </div>
                </div>
              </div>

              {/* Кнопки дій */}
              <div className={styles.formActions}>
                <button
                  type="button"
                  className={styles.testDataBtn}
                  onClick={fillTestData}
                >
                  <i className="fas fa-vial"></i> Заповнити тестові дані
                </button>
                
                <div className={styles.submitActions}>
                  <button
                    type="button"
                    className={styles.cancelBtn}
                    onClick={() => window.history.back()}
                  >
                    <i className="fas fa-times"></i> Скасувати
                  </button>
                  <button
                    type="submit"
                    className={styles.submitBtn}
                    disabled={isSubmitting}
                  >
                    {isSubmitting ? (
                      <>
                        <i className="fas fa-spinner fa-spin"></i> Створення...
                      </>
                    ) : (
                      <>
                        <i className="fas fa-check"></i> Створити турнір
                      </>
                    )}
                  </button>
                </div>
              </div>
            </form>
          </div>

          {/* Панель попереднього перегляду */}
          <div className={styles.previewSection}>
            <div className={styles.previewCard}>
              <h3 className={styles.previewTitle}>
                <i className="fas fa-eye"></i> Попередній перегляд
              </h3>
              
              <div className={styles.previewContent}>
                {formData.name ? (
                  <>
                    {/* Заголовок турніру */}
                    <div className={styles.previewHeader}>
                      <div className={styles.previewSport}>
                        <i className={
                          availableSports.find(s => s.id === formData.sport)?.icon || 'fas fa-trophy'
                        }></i>
                        <span>{availableSports.find(s => s.id === formData.sport)?.name || 'Спорт'}</span>
                      </div>
                      <h4 className={styles.previewName}>{formData.name}</h4>
                    </div>

                    {/* Дати */}
                    <div className={styles.previewDates}>
                      <div className={styles.dateItem}>
                        <i className="fas fa-user-clock"></i>
                        <div>
                          <div className={styles.dateLabel}>Реєстрація до:</div>
                          <div className={styles.dateValue}>
                            {formData.registrationDeadline || 'Не вказано'}
                          </div>
                        </div>
                      </div>
                      <div className={styles.dateItem}>
                        <i className="fas fa-play-circle"></i>
                        <div>
                          <div className={styles.dateLabel}>Початок:</div>
                          <div className={styles.dateValue}>
                            {formData.startDate || 'Не вказано'}
                          </div>
                        </div>
                      </div>
                    </div>

                    {/* Деталі */}
                    <div className={styles.previewDetails}>
                      <div className={styles.detailItem}>
                        <i className="fas fa-users"></i>
                        <span>{formData.minTeams}-{formData.maxTeams} команд</span>
                      </div>
                      <div className={styles.detailItem}>
                        <i className="fas fa-chess-board"></i>
                        <span>
                          {tournamentTypes.find(t => t.id === formData.type)?.name || 'Формат'}
                        </span>
                      </div>
                      {formData.entryFee > 0 && (
                        <div className={styles.detailItem}>
                          <i className="fas fa-money-bill-wave"></i>
                          <span>{formData.entryFee} грн</span>
                        </div>
                      )}
                    </div>

                    {/* Опис */}
                    {formData.description && (
                      <div className={styles.previewDescription}>
                        <p>{formData.description.substring(0, 150)}...</p>
                      </div>
                    )}

                    {/* Категорії */}
                    {selectedCategories.length > 0 && (
                      <div className={styles.previewCategories}>
                        {selectedCategories.map(cat => (
                          <span key={cat} className={styles.categoryTag}>{cat}</span>
                        ))}
                      </div>
                    )}
                  </>
                ) : (
                  <div className={styles.emptyPreview}>
                    <i className="fas fa-info-circle"></i>
                    <p>Заповніть форму, щоб побачити попередній перегляд</p>
                  </div>
                )}
              </div>
            </div>

            {/* Статистика форми */}
            <div className={styles.statsCard}>
              <h4 className={styles.statsTitle}>Статистика заповнення</h4>
              <div className={styles.statsGrid}>
                <div className={styles.statItem}>
                  <div className={styles.statValue}>
                    {Object.values(formData).filter(v => v && v.toString().trim()).length}
                  </div>
                  <div className={styles.statLabel}>Заповнених полів</div>
                </div>
                <div className={styles.statItem}>
                  <div className={styles.statValue}>{selectedCategories.length}</div>
                  <div className={styles.statLabel}>Категорій</div>
                </div>
                <div className={styles.statItem}>
                  <div className={styles.statValue}>
                    {calculateDuration() || '?'}
                  </div>
                  <div className={styles.statLabel}>Днів тривалість</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TournamentCreatePage;