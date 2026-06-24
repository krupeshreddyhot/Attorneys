import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

type Props = {
  children: React.ReactNode;
  adminOnly?: boolean;
  superAdminOnly?: boolean;
};

const ProtectedRoute = ({ children, adminOnly = false, superAdminOnly = false }: Props) => {
  const { isAuthenticated, isAdministrator, isSuperAdmin } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (superAdminOnly && !isSuperAdmin) {
    return <Navigate to="/app/dashboard" replace />;
  }

  if (adminOnly && !isAdministrator && !isSuperAdmin) {
    return <Navigate to="/app/dashboard" replace />;
  }

  return children;
};

export default ProtectedRoute;
