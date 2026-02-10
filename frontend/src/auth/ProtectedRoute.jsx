import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

export default function ProtectedRoute({ children, requireAuth = true }) {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        fontSize: '18px'
      }}>
        Завантаження...
      </div>
    );
  }

  if (!requireAuth) {
    return children;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  return children;
}