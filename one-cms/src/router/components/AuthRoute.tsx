import { Center, Spinner } from '@chakra-ui/react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { AuthState } from '@/machines/auth';

const AuthRoute = ({ children }: { children: JSX.Element }) => {
  const [authState] = useAuth();

  if (authState.matches(AuthState.LOGGED_IN)) {
    return children;
  }

  if (authState.matches(AuthState.UNAUTHORIZED)) {
    return <Navigate to="/unauthorized" />;
  }

  return (
    <Center h="100vh">
      <Spinner size="xl" color="purple.400" />
    </Center>
  );
};
export default AuthRoute;
