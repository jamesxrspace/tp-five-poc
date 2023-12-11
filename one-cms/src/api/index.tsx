import { ReactNode, createContext, useMemo } from 'react';
import {
  Configuration,
  LoginApi,
  SpaceApi,
  SpaceGroupApi,
  DailyBuildApi,
  AssetApi,
} from './openapi';
import { useAuth } from '@/hooks/useAuth';

interface ApiClientContextType {
  loginApi: LoginApi;
  spaceGroupApi: SpaceGroupApi;
  spaceApi: SpaceApi;
  dailyBuildApi: DailyBuildApi;
  assetApi: AssetApi;
}

export const ApiClientContext = createContext({} as ApiClientContextType);
export const ApiClientProvider = ({ children }: { children: ReactNode }) => {
  const [authState] = useAuth();
  const { accessToken } = authState.context;

  const apiClient = useMemo<ApiClientContextType>(() => {
    const config = accessToken
      ? new Configuration({
          accessToken,
          basePath: process.env.REACT_APP_GAME_SERVER_URL,
        })
      : undefined;

    return {
      loginApi: new LoginApi(config),
      spaceGroupApi: new SpaceGroupApi(config),
      spaceApi: new SpaceApi(config),
      dailyBuildApi: new DailyBuildApi(config),
      assetApi: new AssetApi(config),
    };
  }, [accessToken]);

  return <ApiClientContext.Provider value={apiClient}>{children}</ApiClientContext.Provider>;
};
