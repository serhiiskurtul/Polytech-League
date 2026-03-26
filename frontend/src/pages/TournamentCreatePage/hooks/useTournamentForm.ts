import { useState, useEffect, ChangeEvent, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import api from '../../../shared/api/axiosInstance';

/** Єдине джерело правди: які види спорту відправляються на API (ключ = formData.sport). */
const TOURNAMENT_CREATE_ENDPOINTS: Record<string, string> = {
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

export interface TournamentFormData {
  name: string;
  sport: string;
  type: string;
  description: string;
  maxTeams: number;
  minTeams: number;
  registrationDeadline: string;
  startDate: string;
  endDate: string;
  location: string;
  venue: string;
  rules: string;
  prizes: string;
  isPublic: boolean;
  requireApproval: boolean;
  categories: string[];
  // Шахові / шашкові специфічні поля
  date: string;
  timeControl: string;
  system: string;
  variant: string;
  participantsCount: number;
  refereeIds: string[];
  level: string;
  // Настільний теніс специфічні поля
  tournamentType: string;
  category: string;
  // Армреслінг специфічні поля
  armHand: string;
  armWeightClass: string;
  armCategory: string;
}

export const useTournamentForm = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState<TournamentFormData>({
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

  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    setFormData(prev => ({
      ...prev,
      categories: selectedCategories
    }));
  }, [selectedCategories]);

  const handleInputChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    const checked = (e.target as HTMLInputElement).checked;
    
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleCategoryToggle = (category: string) => {
    if (selectedCategories.includes(category)) {
      setSelectedCategories(prev => prev.filter(c => c !== category));
    } else {
      setSelectedCategories(prev => [...prev, category]);
    }
  };

  const validateForm = () => {
    const errors: string[] = [];

    if (!formData.sport) errors.push('Оберіть вид спорту');
    if (!formData.name.trim()) errors.push('Назва турніру обов\'язкова');
    if (!formData.description.trim()) errors.push('Опис турніру обов\'язковий');
    if (!formData.startDate) errors.push('Дата початку обов\'язкова');
    if (!formData.registrationDeadline) errors.push('Термін реєстрації обов\'язковий');
    if (!formData.location.trim()) errors.push('Локація обов\'язкова');
    if (formData.maxTeams < formData.minTeams) errors.push('Максимальна кількість команд не може бути меншою за мінімальну');
    if (new Date(formData.registrationDeadline) > new Date(formData.startDate)) {
      errors.push('Термін реєстрації не може бути пізніше дати початку');
    }
    if (selectedCategories.length === 0) errors.push('Оберіть хоча б одну категорію');

    return errors;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const errors = validateForm();

    if (errors.length > 0) {
      errors.forEach(err => toast.error(err));
      return;
    }

    setIsSubmitting(true);
    try {
      const endpoint = TOURNAMENT_CREATE_ENDPOINTS[formData.sport];
      if (!endpoint) throw new Error('Невідомий вид спорту');

      // Базовий об'єкт, який спільний для всіх типів івентів у бекенді
      const basePayload = {
        name: formData.name,
        date: formData.startDate, // На бекенді поле називається Date
        location: formData.location,
        description: formData.description,
        refereeIds: formData.refereeIds || [],
        level: formData.level,
      };

      let payload: any = { ...basePayload };

      // Додаємо специфічні поля залежно від виду спорту
      switch (formData.sport) {
        case 'futsal':
        case 'basketball-3x3':
        case 'volleyball':
        case 'beach-volleyball':
        case 'badminton':
          payload.tournamentType = formData.tournamentType;
          break;

        case 'chess':
        case 'checkers':
          payload.timeControl = formData.timeControl;
          payload.participantsCount = formData.participantsCount;
          payload.system = formData.system;
          if (formData.sport === 'checkers') {
            payload.variant = formData.variant;
          }
          break;

        case 'table_tennis':
          payload.tournamentType = formData.tournamentType;
          payload.category = formData.category;
          break;

        case 'armwrestling':
          payload.hand = formData.armHand;
          payload.weightClass = formData.armWeightClass;
          payload.category = formData.armCategory;
          break;
      }

      await api.post(endpoint, payload);
      toast.success('Турнір успішно створено!');
      navigate('/tournaments-page');
    } catch (error: any) {
      console.error('Submit error:', error);
      const errorMessage = error.response?.data?.message || error.response?.data?.title || 'Помилка при створенні турніру';
      toast.error(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  const fillTestData = () => {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);

    setFormData(prev => ({
      ...prev,
      name: 'Тестовий турнір ' + Math.floor(Math.random() * 1000),
      sport: 'futsal',
      description: 'Це автоматично згенерований опис для тестування форми створення турніру.',
      location: 'Спортивний зал №1',
      registrationDeadline: tomorrow.toISOString().split('T')[0],
      startDate: nextWeek.toISOString().split('T')[0],
      maxTeams: 16,
      minTeams: 4
    }));
    setSelectedCategories([]);
  };

  const calculateDuration = () => {
    if (!formData.startDate || !formData.endDate) return 0;
    const start = new Date(formData.startDate);
    const end = new Date(formData.endDate);
    const diffTime = Math.abs(end.getTime() - start.getTime());
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
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
