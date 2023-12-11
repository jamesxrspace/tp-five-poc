import { createContext, ReactNode } from 'react';
import { ActorRefFrom } from 'xstate';
import { authMachine, authService } from '@/machines/auth/';

interface GlobalStateContextType {
  authService: ActorRefFrom<typeof authMachine>;
}

export const GlobalStateContext = createContext({} as GlobalStateContextType);

export const GlobalStateProvider = ({ children }: { children: ReactNode }) => {
  return (
    <GlobalStateContext.Provider value={{ authService }}>{children}</GlobalStateContext.Provider>
  );
};
