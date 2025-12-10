import './InputSign.css';

export default function InputSign({
  labelInput,
  type = 'text',
  placeholder,
  register,
  error,
  ...rest
}) {
  return (
    <div className="input-group">
      <label className="input-label">{labelInput}</label>
      
      <input
        type={type}
        placeholder={placeholder}
        className={`custom-input ${error ? 'input-error' : ''}`}
        {...register}
        {...rest}
      />

      {error && (
        <span className="error-message">{error}</span>
      )}
    </div>
  );
}