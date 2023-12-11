import { createBrowserRouter, redirect } from 'react-router-dom';
import AuthRoute from './components/AuthRoute';
import { ACCOUNT_MODULE_ROUTES } from './modules/Account';
import { DEVELOP_MODULE_ROUTES } from './modules/Develop';
import { EVENT_MODULE_ROUTES } from './modules/Event';
import { SPACE_MODULE_ROUTES } from './modules/Space';
import { generateRoutes } from './utils';
import Layout from '@/layout';
import Login from '@/pages/Login';
import NotFound from '@/pages/NotFound';
import Unauthorized from '@/pages/Unauthorized';

export const DYNAMIC_ROUTES = [
  SPACE_MODULE_ROUTES,
  ACCOUNT_MODULE_ROUTES,
  EVENT_MODULE_ROUTES,
  DEVELOP_MODULE_ROUTES,
];

export const router = createBrowserRouter([
  {
    path: '/',
    element: (
      <AuthRoute>
        <Layout />
      </AuthRoute>
    ),
    children: [
      {
        index: true,
        // Use redirect function to avoid page flickering
        // https://github.com/remix-run/react-router/issues/8295
        loader: () => redirect('/space/list'),
      },
      ...DYNAMIC_ROUTES.map(generateRoutes),
    ],
  },
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/unauthorized',
    element: <Unauthorized />,
  },
  {
    path: '*',
    element: <NotFound />,
  },
]);
