import { Navigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { AuthContext } from '@/machines/auth';
import { Permission } from '@/types/auth';

interface PermissionRouteProps {
  canEnter?: (context: AuthContext) => boolean;
  permissions?: Permission[];
}

const PermissionRoute = ({ canEnter, children }: React.PropsWithChildren<PermissionRouteProps>) => {
  const [authState] = useAuth();

  if (canEnter && !canEnter(authState.context)) {
    return <Navigate to="/404" />;
  }
  return children as JSX.Element;
};
export default PermissionRoute;
