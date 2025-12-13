import { Link } from 'react-router-dom';
import './SimpleButton.css';

export default function SimpleButton({ to, text, onClick, fullWidth = false, ...props }) {
  const className = `view-btn ${fullWidth ? 'full-width' : ''}`;

  const content = (
    <button className={className} onClick={onClick} type="button" {...props}>
      {text}
    </button>
  );

  if (to) {
    return (
      <Link to={to} className="view-link">
        {content}
      </Link>
    );
  }

  return content;
}