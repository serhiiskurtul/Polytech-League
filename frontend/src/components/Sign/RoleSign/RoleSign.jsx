import './RoleSign.css';

export default function RoleSign({
  labelInput = 'Роль',
  options = [],
  register,
  error,
}) {
  return (
    <div className="input-group">
      <label className="input-label">{labelInput}</label>

      <select
        className={`custom-select ${error ? 'input-error' : ''}`}
        {...register}
        defaultValue=""
      >
        <option value="" disabled>
          Оберіть роль
        </option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

      {error && <span className="error-message">{error}</span>}
    </div>
  );
}