import { FiServer, FiDatabase, FiCloud } from 'react-icons/fi';
import { Navigate } from 'react-router-dom';
import { CustomRouteObject } from '@/router/type';

export const DEVELOP_MODULE_ROUTES: CustomRouteObject = {
  name: 'Develop',
  translationKey: 'common:navigation.develop.develop',
  path: '/develop',
  icon: FiServer,
  canEnter: ({ profile }) => !!profile.email?.endsWith('@xrspace.io'),
  children: [
    {
      index: true,
      element: <Navigate to="/develop/dailybuild" replace={true} />,
    },
    {
      path: 'daily-build',
      name: 'Daily Build',
      translationKey: 'common:navigation.develop.dailyBuild',
      icon: FiCloud,
      async lazy() {
        const { DailyBuildPage } = await import('@/pages/Develop/DailyBuild');
        return { Component: DailyBuildPage };
      },
    },
    {
      path: 'daily-database',
      name: 'Daily Database',
      translationKey: 'common:navigation.develop.dailyDatabase',
      icon: FiDatabase,
      async lazy() {
        const { DailyDatabase } = await import('@/pages/Develop/DailyDatabase');
        return { Component: DailyDatabase };
      },
    },
  ],
};
