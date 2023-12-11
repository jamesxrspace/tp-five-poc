import { useActor } from '@xstate/react';
import { useContext } from 'react';
import { GlobalStateContext } from '@/machines';

export const useAuth = () => {
  const { authService } = useContext(GlobalStateContext);
  const actor = useActor(authService);

  return actor;
};
