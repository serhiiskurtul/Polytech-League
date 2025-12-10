import './Logo.css';
import { Link } from 'react-router-dom';

export default function Logo() {
  return (
    <Link to="/" className="logo-link">
        <div className="logo-icon">
            <div className="field">
                <div className="center-circle"></div>
                <div className="center-line"></div>
            </div>
        </div>
    </Link>
  );
}