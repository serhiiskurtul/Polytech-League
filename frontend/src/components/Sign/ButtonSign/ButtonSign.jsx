import './ButtonSign.css';

export default function ButtonSign({ children, onClick, type = "button", disabled = false }) {
  return (
    <button 
      className="btn-primary" 
      onClick={onClick}
      type={type}
      disabled={disabled}
    >
      {children}
    </button>
  );
}
