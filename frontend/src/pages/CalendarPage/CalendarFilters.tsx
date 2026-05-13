interface Filter {
  id: string;
  label: string;
  icon: string;
}

interface CalendarFiltersProps {
  activeFilter: string;
  onFilterChange: (sport: string) => void;
}

const CalendarFilters = ({ activeFilter, onFilterChange }: CalendarFiltersProps) => {
  const filters: Filter[] = [
    { id: 'all', label: 'Всі види спорту', icon: 'fas fa-filter' },
    { id: 'football', label: 'Футзал', icon: 'fas fa-futbol' },
    { id: 'basketball-3x3', label: 'Баскетбол 3х3', icon: 'fas fa-basketball-ball' },
    { id: 'volleyball', label: 'Волейбол', icon: 'fas fa-volleyball-ball' },
    { id: 'beach-volleyball', label: 'Пляжний волейбол', icon: 'fas fa-umbrella-beach' },
    { id: 'chess', label: 'Шахи', icon: 'fas fa-chess' },
    { id: 'checkers', label: 'Шашки', icon: 'fas fa-chess-board' },
    { id: 'table-tennis', label: 'Настільний теніс', icon: 'fas fa-table-tennis' },
    { id: 'badminton', label: 'Бадмінтон', icon: 'fas fa-feather-alt' },
    { id: 'armwrestling', label: 'Армреслінг', icon: 'fas fa-fist-raised' }
  ];

  return (
    <div className="filters">
      {filters.map(filter => (
        <button
          key={filter.id}
          className={`filter-btn ${activeFilter === filter.id ? 'active' : ''}`}
          onClick={() => onFilterChange(filter.id)}
          data-sport={filter.id}
        >
          <i className={filter.icon}></i> {filter.label}
        </button>
      ))}
    </div>
  );
};

export default CalendarFilters;
