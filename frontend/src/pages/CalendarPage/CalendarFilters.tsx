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
    { id: 'basketball', label: 'Баскетбол', icon: 'fas fa-basketball-ball' },
    { id: 'volleyball', label: 'Волейбол', icon: 'fas fa-volleyball-ball' },
    { id: 'chess', label: 'Шахи', icon: 'fas fa-chess' },
    { id: 'table-tennis', label: 'Настільний теніс', icon: 'fas fa-table-tennis' }
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
