import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';

export default function ProtectedRoute({ children, requireAuth = true }) {
  const { user, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return <div style={{ /* твій стиль завантаження */ }}>Завантаження...</div>;
  }

  if (requireAuth && !user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (!requireAuth && user && location.pathname === '/login') {
    return <Navigate to="/" replace />;
  }

  return children;
}