import { Navigate, Outlet } from 'react-router-dom';
import Loading from '~/components/Loading';
import { useAuthContext } from '~/contexts/authContext';

const ProtectedRoute = ({ requiredRole = null }) => {
  const { user, isLoading } = useAuthContext();
  if (isLoading) return <Loading />;

  if (!user) {
    return <Navigate to="/sign-in" replace />;
  }

  if (requiredRole && user.role !== requiredRole) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <Outlet />;
};

export default ProtectedRoute;
