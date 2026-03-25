import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import api from '../../../shared/api/axiosInstance.js';

/** Єдине джерело правди: які види спорту відправляються на API (ключ = formData.sport). */
const TOURNAMENT_CREATE_ENDPOINTS = {
  chess: '/chess-events',
  checkers: '/checkers-events',
  table_tennis: '/table-tennis-events',
  armwrestling: '/armwrestling-events',
  futsal: '/futsal-events',
  'basketball-3x3': '/basketball3x3-events',
  volleyball: '/volleyball-events',
  'beach-volleyball': '/beach-volleyball-events',
  badminton: '/badminton-events',
};

export const useTournamentForm = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    name: '',
    sport: '',
    type: 'groups_playoff',
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

    if (!formData.sport) errors.push('Оберіть вид спорту');
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

    const endpoint = TOURNAMENT_CREATE_ENDPOINTS[formData.sport];

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

    if (formData.sport === 'futsal') {
      const typeMapping = {
        'single_elimination': 'SingleElimination',
        'double_elimination': 'DoubleElimination',
        'round_robin': 'RoundRobin',
        'swiss': 'Swiss',
        'groups_playoff': 'GroupsPlayoff'
      };
      payload.tournamentType = typeMapping[formData.type] || 'SingleElimination';
      delete payload.timeControl;
      delete payload.system;
      delete payload.participantsCount;
    }

    if (formData.sport === 'basketball-3x3') {
      const typeMapping = {
        'single_elimination': 'SingleElimination',
        'double_elimination': 'DoubleElimination',
        'round_robin': 'RoundRobin',
        'swiss': 'Swiss',
        'groups_playoff': 'GroupsPlayoff'
      };
      payload.tournamentType = typeMapping[formData.type] || 'SingleElimination';
      delete payload.timeControl;
      delete payload.system;
      delete payload.participantsCount;
    }

    if (formData.sport === 'volleyball' || formData.sport === 'beach-volleyball') {
      const typeMapping = {
        'single_elimination': 'SingleElimination',
        'double_elimination': 'DoubleElimination',
        'round_robin': 'RoundRobin',
        'swiss': 'Swiss',
        'groups_playoff': 'GroupsPlayoff'
      };
      payload.tournamentType = typeMapping[formData.type] || 'SingleElimination';
      delete payload.timeControl;
      delete payload.system;
      delete payload.participantsCount;
    }

    if (formData.sport === 'badminton') {
      const typeMapping = {
        'single_elimination': 'SingleElimination',
        'double_elimination': 'DoubleElimination',
        'round_robin': 'RoundRobin',
        'swiss': 'Swiss',
        'groups_playoff': 'GroupsPlayoff'
      };
      payload.tournamentType = typeMapping[formData.type] || 'SingleElimination';
      delete payload.timeControl;
      delete payload.system;
      delete payload.participantsCount;
    }

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
      name: 'Весняний турнір з волейболу Polytech',
      sport: 'volleyball',
      type: 'groups_playoff',
      description: 'Командний турнір з класичного волейболу. Реєстрація команд, ігрові сети та фінальні фінали.',
      maxTeams: 16,
      minTeams: 4,
      registrationDeadline: tomorrow.toISOString().split('T')[0],
      startDate: nextWeek.toISOString().split('T')[0],
      endDate: twoWeeks.toISOString().split('T')[0],
      location: 'Спорткомплекс Polytech, зал волейболу',
      venue: 'Main Court A',
      rules: '1. Правила FIVB\n2. До 5 сетів, партія до 25\n3. Ліберо за реєстрацією',
      prizes: '1 місце — кубок, 2–3 — медалі та дипломи',
      isPublic: true,
      requireApproval: true,
      categories: ['Чоловіки'],
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
