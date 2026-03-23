import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import api from '../../../shared/api/axiosInstance.js';

export const useTournamentForm = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    name: '',
    sport: '',
    type: 'Swiss',
    description: '',
    maxTeams: 32,
    minTeams: 2,
    registrationDeadline: '',
    startDate: '',
    endDate: '',
    location: '',
    venue: '',
    rules: '',
    prizes: '',
    isPublic: true,
    requireApproval: false,
    categories: [],
    // Шахові / шашкові специфічні поля
    date: '',
    timeControl: '10+5',
    system: 'Swiss',
    variant: 'International', // Для шашок
    participantsCount: 32,
    refereeIds: [],
    level: 'CollegeGroup',
    // Настільний теніс специфічні поля
    tournamentType: 'SingleElimination',
    category: 'Men',
    // Армреслінг специфічні поля
    armHand: 'Right',
    armWeightClass: 'Under80kg',
    armCategory: 'Men'
  });

  const [selectedCategories, setSelectedCategories] = useState([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    setFormData(prev => ({
      ...prev,
      categories: selectedCategories
    }));
  }, [selectedCategories]);

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleCategoryToggle = (category) => {
    if (selectedCategories.includes(category)) {
      setSelectedCategories(prev => prev.filter(c => c !== category));
    } else {
      setSelectedCategories(prev => [...prev, category]);
    }
  };

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

    if (['chess', 'checkers', 'table_tennis'].includes(formData.sport)) {
      if (formData.sport !== 'table_tennis' && !formData.timeControl.trim()) {
          errors.push(`Контроль часу обов'язковий для ${formData.sport === 'chess' ? 'шахів' : 'шашок'}`);
      }
      if (formData.participantsCount < 2 && formData.sport !== 'table_tennis') errors.push('Кількість учасників повинна бути щонайменше 2');
    }

    return errors;
  };

  const handleSubmit = async (e) => {
    if (e) e.preventDefault();

    const errors = validateForm();
    if (errors.length > 0) {
      alert(`Помилки валідації:\n${errors.join('\n')}`);
      return;
    }

    if (!['chess', 'checkers', 'table_tennis', 'armwrestling'].includes(formData.sport)) {
      alert('Наразі створення змагань доступне тільки для шахів, шашок, настільного тенісу та армреслінгу. Підтримка інших видів спорту буде додана пізніше.');
      return;
    }

    setIsSubmitting(true);

    const payload = {
      name: formData.name.trim(),
      date: formData.startDate ? new Date(formData.startDate).toISOString() : null,
      location: formData.location.trim(),
      description: formData.description.trim(),
      timeControl: formData.timeControl,
      system: formData.system,
      participantsCount: Number(formData.participantsCount) || formData.maxTeams,
      refereeIds: formData.refereeIds || [],
      level: formData.level || 'CollegeGroup',
    };

    if (formData.sport === 'checkers') {
        payload.variant = formData.variant;
    }

    if (formData.sport === 'table_tennis') {
        payload.tournamentType = formData.tournamentType;
        payload.category = formData.category;
        delete payload.timeControl;
        delete payload.system;
        delete payload.participantsCount;
    }

    if (formData.sport === 'armwrestling') {
        payload.hand = formData.armHand;
        payload.weightClass = formData.armWeightClass;
        payload.category = formData.armCategory;
        delete payload.timeControl;
        delete payload.system;
        delete payload.participantsCount;
    }

    const endpointMap = {
        'chess': '/chess-events',
        'checkers': '/checkers-events',
        'table_tennis': '/table-tennis-events',
        'armwrestling': '/armwrestling-events'
    };
    const endpoint = endpointMap[formData.sport];

    try {
      const res = await api.post(endpoint, payload);

      if (res.status >= 200 && res.status < 300) {
        toast.success('Змагання успішно створено!', {
          duration: 4000,
          style: {
            border: '1px solid #10b981',
          },
        });
        navigate('/tournaments-page');
      } else {
        throw new Error(`Unexpected status: ${res.status}`);
      }
    } catch (err) {
      console.error('Помилка створення турніру:', err);
      alert('Помилка: ' + (err.response?.data?.message || err.message || 'Щось пішло не так'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const fillTestData = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);
    const twoWeeks = new Date();
    twoWeeks.setDate(twoWeeks.getDate() + 14);

    setFormData({
      name: 'Чемпіонат коледжу з шахів 2024',
      sport: 'chess',
      type: 'swiss',
      description: 'Щорічний чемпіонат серед студентських команд. Гра за кубок декана та цінні призи.',
      maxTeams: 32,
      minTeams: 4,
      registrationDeadline: tomorrow.toISOString().split('T')[0],
      startDate: nextWeek.toISOString().split('T')[0],
      endDate: twoWeeks.toISOString().split('T')[0],
      location: 'Бібліотека, зал №4',
      venue: 'Зал №4',
      rules: '1. Швейцарська система\n2. Контроль часу: 15+10\n3. Заборонені підказки\n4. Рішення судді остаточні',
      prizes: '1 місце - 2000 грн\n2 місце - 1000 грн\n3 місце - 500 грн',
      isPublic: true,
      requireApproval: true,
      categories: ['Чоловіки', 'Змішані'],
      // Шахові / шашкові поля
      date: nextWeek.toISOString().split('T')[0],
      timeControl: '15+10',
      system: 'Swiss',
      variant: 'International',
      participantsCount: 32,
      refereeIds: [],
      level: 'CollegeGroup',
      tournamentType: 'SingleElimination',
      category: 'Men',
      armHand: 'Right',
      armWeightClass: 'Under80kg',
      armCategory: 'Men'
    });

    setSelectedCategories(['Чоловіки', 'Змішані']);
  };

  const calculateDuration = () => {
    if (formData.startDate && formData.endDate) {
      const start = new Date(formData.startDate);
      const end = new Date(formData.endDate);
      const diffTime = Math.abs(end - start);
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
      return diffDays + 1;
    }
    return 0;
  };

  return {
    formData,
    selectedCategories,
    isSubmitting,
    handleInputChange,
    handleCategoryToggle,
    handleSubmit,
    fillTestData,
    calculateDuration
  };
};
