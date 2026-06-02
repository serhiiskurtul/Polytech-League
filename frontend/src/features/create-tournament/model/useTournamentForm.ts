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
  football: '/football-events',
  'basketball_3x3': '/basketball3x3-events',
  volleyball: '/volleyball-events',
  'beach_volleyball': '/beach-volleyball-events',
  badminton: '/badminton-events',
  athletics: '/athletics-events',
};

export interface TournamentFormData {
  name: string;
  sport: string;
  type: string;
  description: string;
  maxTeams: number;
  minTeams: number;
  registrationDeadline: string;
  registrationDeadlineTime: string;
  registrationStartDate: string;
  registrationStartTime: string;
  startDate: string;
  startTime: string;
  endDate: string;
  endTime: string;
  location: string;
  venue: string;
  rules: string;
  prizes: string;
  schedule: string;
  isPublic: boolean;
  requireApproval: boolean;
  disableNotifications: boolean;
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
  // Армрестлінг специфічні поля
  armHand: string;
  armWeightClass: string;
  armCategory: string;
  isTeam: boolean;
}

export const useTournamentForm = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState<TournamentFormData>({
    name: '',
    sport: '',
    type: 'GroupsPlayoff',
    description: '',
    maxTeams: 32,
    minTeams: 2,
    registrationDeadline: '',
    registrationDeadlineTime: '23:59',
    registrationStartDate: '',
    registrationStartTime: '00:00',
    startDate: '',
    startTime: '10:00',
    endDate: '',
    endTime: '20:00',
    location: '',
    venue: '',
    rules: '',
    prizes: '',
    schedule: '',
    isPublic: true,
    requireApproval: false,
    disableNotifications: false,
    categories: [],
    // Шахові / шашкові специфічні поля
    date: '',
    timeControl: '10+5',
    system: 'Swiss',
    variant: 'International', // Для шашок
    participantsCount: 32,
    refereeIds: [],
    level: 'CollegeGroup',
    // Настільний теніс та інші специфічні поля
    tournamentType: '',
    category: 'Men',
    // Армрестлінг специфічні поля
    armHand: 'Right',
    armWeightClass: 'Under80kg',
    armCategory: 'Men',
    isTeam: false
  });

  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const categoryName = selectedCategories[0];
    const mappedCategory = categoryName === 'Чоловіки' ? 'Men' : categoryName === 'Жінки' ? 'Women' : 'Mixed';
    const mappedArmCategory = categoryName === 'Чоловіки' ? 'Men' : categoryName === 'Жінки' ? 'Women' : 'Open';

    setFormData(prev => ({
      ...prev,
      categories: selectedCategories,
      category: mappedCategory,
      armCategory: mappedArmCategory
    }));
  }, [selectedCategories]);

  useEffect(() => {
    // Автоматично встановлюємо IsTeam для видів спорту, що завжди є командними
    const teamSports = ['football', 'basketball_3x3', 'volleyball', 'beach_volleyball'];
    if (teamSports.includes(formData.sport)) {
      setFormData(prev => ({ ...prev, isTeam: true }));
    } else if (['table_tennis', 'armwrestling'].includes(formData.sport)) {
      // Для індивідуальних видів спорту за замовчуванням false
      setFormData(prev => ({ ...prev, isTeam: false }));
    }
    // Badminton залишаємо як є або за замовчуванням true/false - користувач сам перемкне
  }, [formData.sport]);

  useEffect(() => {
    if (formData.sport === 'badminton' && formData.level === 'Regional') {
      setFormData(prev => ({ ...prev, isTeam: false }));
    }
  }, [formData.sport, formData.level]);

  const handleInputChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    const checked = (e.target as HTMLInputElement).checked;

    setFormData(prev => {
      const newState = {
        ...prev,
        [name]: type === 'checkbox' ? checked : value
      };

      // Mutual exclusivity for isPublic and requireApproval
      if (type === 'checkbox') {
        if (name === 'isPublic' && checked) {
          newState.requireApproval = false;
        } else if (name === 'requireApproval' && checked) {
          newState.isPublic = false;
        }
      }

      return newState;
    });
  };

  const handleCategoryToggle = (category: string) => {
    setSelectedCategories([category]);
  };

  const validateForm = () => {
    const errors: string[] = [];

    if (!formData.sport) errors.push('Оберіть вид спорту');
    if (!formData.name.trim()) errors.push('Назва турніру обов\'язкова');
    if (!formData.startDate) errors.push('Дата початку обов\'язкова');
    if (formData.level === 'CollegeGroup') {
      if (formData.registrationDeadline && formData.startDate && new Date(formData.registrationDeadline) > new Date(formData.startDate)) {
        errors.push('Термін реєстрації не може бути пізніше дати початку');
      }
    }
    if (!formData.location.trim()) errors.push('Локація обов\'язкова');
    if (formData.maxTeams < formData.minTeams) errors.push('Максимальна кількість команд не може бути меншою за мінімальну');
    if (selectedCategories.length === 0) errors.push('Оберіть хоча б одну категорію');

    if (formData.level === 'CollegeGroup' && !formData.isPublic && !formData.requireApproval) {
      errors.push('Оберіть тип реєстрації: Відкрите змагання або Модерація заявок');
    }

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
      // Helper to combine date and time strings into ISO string
      const combineDateTime = (dateStr: string, timeStr: string) => {
        if (!dateStr) return null;
        const [year, month, day] = dateStr.split('-');
        const [hours, minutes] = timeStr.split(':');
        const date = new Date(Number(year), Number(month) - 1, Number(day), Number(hours), Number(minutes));
        return date.toISOString();
      };

      // Fix inconsistency for basketball_3x3 vs basketball-3x3
      let sportKey = formData.sport;
      if (sportKey === 'basketball-3x3') sportKey = 'basketball_3x3';
      if (sportKey === 'beach-volleyball') sportKey = 'beach_volleyball'; // fallback if still using hyphen
      const endpoint = TOURNAMENT_CREATE_ENDPOINTS[sportKey];
      if (!endpoint) throw new Error('Невідомий вид спорту');

      // Базовий об'єкт, який спільний для всіх типів івентів у бекенді
      const basePayload = {
        name: formData.name,
        date: combineDateTime(formData.startDate, formData.startTime),
        location: formData.location,
        description: formData.description || null,
        refereeIds: formData.refereeIds.map(id => Number(id)) || [],
        level: formData.level,
        rules: formData.rules,
        prizes: formData.prizes,
        schedule: formData.schedule,
        isTeam: formData.isTeam,
        isPublic: formData.level === 'CollegeGroup' ? formData.isPublic : false,
        isModerated: formData.level === 'CollegeGroup' ? formData.requireApproval : false,
        disableNotifications: formData.disableNotifications,
        registrationStart: formData.level === 'CollegeGroup' && formData.registrationStartDate ? combineDateTime(formData.registrationStartDate, formData.registrationStartTime) : null,
        registrationEnd: formData.level === 'CollegeGroup' && formData.registrationDeadline ? combineDateTime(formData.registrationDeadline, formData.registrationDeadlineTime) : null,
        endDate: formData.endDate ? combineDateTime(formData.endDate, formData.endTime) : null,
        category: formData.category,
      };

      let payload: any = { ...basePayload };

      // Додаємо специфічні поля залежно від виду спорту
      switch (sportKey) {
        case 'football':
        case 'basketball_3x3':
        case 'volleyball':
        case 'beach_volleyball':
        case 'badminton':
          payload.tournamentType = formData.tournamentType || null;
          break;

        case 'chess':
        case 'checkers':
          payload.timeControl = formData.timeControl || null;
          payload.participantsCount = formData.participantsCount;
          payload.system = formData.system || null;
          if (formData.sport === 'checkers') {
            payload.variant = formData.variant || null;
          }
          break;

        case 'table_tennis':
          payload.tournamentType = formData.tournamentType || 'RoundRobin';
          break;

        case 'armwrestling':
          payload.hand = formData.armHand || null;
          payload.weightClass = formData.armWeightClass || null;
          payload.category = formData.armCategory || null;
          payload.timeControl = formData.timeControl || null;
          payload.participantsCount = formData.participantsCount;
          payload.tournamentType = formData.tournamentType || null;
          break;
      }

      await api.post(endpoint, payload);
      toast.success('Турнір успішно створено!');
      navigate('/tournaments');
    } catch (error: any) {
      console.error('Submit error:', error);
      const errorMessage = error.response?.data?.message || error.response?.data?.title || 'Помилка при створенні турніру';
      toast.error(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
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
    calculateDuration
  };
};
