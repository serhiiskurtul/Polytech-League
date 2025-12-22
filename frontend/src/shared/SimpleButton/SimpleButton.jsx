import { Link } from 'react-router-dom';
import './SimpleButton.css';

export default function SimpleButton({ to, text, onClick, fullWidth = false, insideLink = false, ...props }) {
  const className = `view-btn ${fullWidth ? 'full-width' : ''}`;

  const buttonContent = (
      <button className={className} onClick={onClick} type="button" {...props}>
        {text}
      </button>
  );

  // Якщо кнопка всередині іншого Link або to не передано — просто button
  if (!to || insideLink) {
    return buttonContent;
  }

  // Інакше обгортаємо у Link
  return (
      <Link to={to} className="view-link">
        {buttonContent}
      </Link>
  );
}
