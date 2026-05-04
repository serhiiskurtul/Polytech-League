export const CATEGORIES = [
  'Чоловіки',
  'Жінки',
  'Змішані'
];

export const AVAILABLE_SPORTS = [
  { id: 'futsal', name: 'Футзал', icon: 'fas fa-futbol' },
  { id: 'basketball-3x3', name: 'Баскетбол 3x3', icon: 'fas fa-basketball-ball' },
  { id: 'volleyball', name: 'Волейбол', icon: 'fas fa-volleyball-ball' },
  { id: 'chess', name: 'Шахи', icon: 'fas fa-chess' },
  { id: 'checkers', name: 'Шашки', icon: 'fas fa-chess-board' },
  { id: 'beach_volleyball', name: 'Пляжний волейбол', icon: 'fas fa-umbrella-beach' },
  { id: 'badminton', name: 'Бадмінтон', icon: 'fas fa-feather-alt' },
  { id: 'armwrestling', name: 'Армреслінг', icon: 'fas fa-fist-raised' },
  { id: 'table_tennis', name: 'Настільний теніс', icon: 'fas fa-table-tennis' },
  { id: 'athletics', name: 'Легка атлетика', icon: 'fas fa-running' }
];

export const TOURNAMENT_TYPES = [
  { id: 'SingleElimination', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
  { id: 'DoubleElimination', name: 'Двоєступінчаста система', description: 'Друга спроба при поразці' },
  { id: 'RoundRobin', name: 'Кругова система', description: 'Всі грають з усіма' },
  { id: 'Swiss', name: 'Швейцарська система', description: 'Для шахів та шашок' },
  { id: 'GroupsPlayoff', name: 'Груповий етап + плей-офф', description: 'Найпопулярніший формат' }
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

export const ARMWRESTLING_WEIGHT_CLASSES = [
  { id: 'Under60kg', name: 'До 60 кг' },
  { id: 'Under70kg', name: 'До 70 кг' },
  { id: 'Under80kg', name: 'До 80 кг' },
  { id: 'Under90kg', name: 'До 90 кг' },
  { id: 'Plus90kg', name: '90+ кг' }
];

export const ARMWRESTLING_HANDS = [
  { id: 'Right', name: 'Права рука' },
  { id: 'Left', name: 'Ліва рука' },
  { id: 'Both', name: 'Обидві руки' }
];

export const ARMWRESTLING_CATEGORIES = [
  { id: 'Men', name: 'Чоловіки' },
  { id: 'Women', name: 'Жінки' },
  { id: 'Open', name: 'Відкрита' }
];

export const BASKETBALL3X3_TOURNAMENT_TYPES = [
  { id: 'SingleElimination', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
  { id: 'DoubleElimination', name: 'Подвійна олімпійська система (є другий шанс)', description: 'Друга спроба при поразці' },
  { id: 'RoundRobin', name: 'Кругова система', description: 'Всі грають з усіма' },
  { id: 'GroupsPlayoff', name: 'Груповий етап + плей-офф', description: 'Найпопулярніший формат' }
];

export const BADMINTON_TOURNAMENT_TYPES = [
  { id: 'SingleElimination', name: 'Олімпійська система', description: 'Програєш - вилітаєш' },
  { id: 'DoubleElimination', name: 'Подвійна олімпійська система (є другий шанс)', description: 'Друга спроба при поразці' },
  { id: 'RoundRobin', name: 'Кругова система', description: 'Всі грають з усіма' },
  { id: 'GroupsPlayoff', name: 'Груповий етап + плей-офф', description: 'Найпопулярніший формат' }
];
