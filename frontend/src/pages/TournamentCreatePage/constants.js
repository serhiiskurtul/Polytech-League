export const CATEGORIES = [
  'Чоловіки',
  'Жінки',
  'Змішані'
];

export const AVAILABLE_SPORTS = [
  { id: 'table_tennis', name: 'Настільний теніс', icon: 'fas fa-table-tennis' },
  { id: 'armwrestling', name: 'Армреслінг', icon: 'fas fa-hand-rock' },
  { id: 'futsal', name: 'Футзал', icon: 'fas fa-futbol' },
  { id: 'volleyball', name: 'Волейбол', icon: 'fas fa-volleyball-ball' },
  { id: 'beach_volleyball', name: 'Пляжний волейбол', icon: 'fas fa-sun' },
  { id: 'basketball', name: 'Баскетбол', icon: 'fas fa-basketball-ball' },
  { id: 'badminton', name: 'Бадмінтон', icon: 'fas fa-feather-alt' },
  { id: 'chess', name: 'Шахи', icon: 'fas fa-chess' },
  { id: 'checkers', name: 'Шашки', icon: 'fas fa-th-large' }
];

export const TOURNAMENT_TYPES = [
  { id: 'single_elimination', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
  { id: 'double_elimination', name: 'Двоєступінчаста система', description: 'Друга спроба при поразці' },
  { id: 'round_robin', name: 'Кругова система', description: 'Всі грають з усіма' },
  { id: 'swiss', name: 'Швейцарська система', description: 'Для шахів та шашок' },
  { id: 'groups_playoff', name: 'Груповий етап + плей-офф', description: 'Найпопулярніший формат' }
];

export const CHESS_SYSTEMS = [
  { id: 'Swiss', name: 'Швейцарська система', description: 'Для шахів та шашок' },
  { id: 'RoundRobin', name: 'Кругова система', description: 'Всі грають з усіма' },
  { id: 'Knockout', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
];

export const CHECKERS_SYSTEMS = [
  { id: 'Swiss', name: 'Швейцарська система', description: 'Для шахів та шашок' },
  { id: 'RoundRobin', name: 'Кругова система', description: 'Всі грають з усіма' },
  { id: 'Knockout', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
];

export const CHECKERS_VARIANTS = [
  { id: 'Russian', name: 'Російські (64 клітинки)' },
  { id: 'International', name: 'Міжнародні (100 клітинок)' },
  { id: 'English', name: 'Англійські (64 клітинки)' }
];

export const TABLE_TENNIS_TOURNAMENT_TYPES = [
  { id: 'SingleElimination', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
  { id: 'DoubleElimination', name: 'Подвійна олімпійська система (є другий шанс)', description: 'Друга спроба при поразці' },
  { id: 'RoundRobin', name: 'Кругова система', description: 'Всі грають з усіма' }
];

export const TABLE_TENNIS_CATEGORIES = [
  { id: 'Men', name: 'Чоловіки' },
  { id: 'Women', name: 'Жінки' },
  { id: 'Mixed', name: 'Змішані' }
];
