import { Outlet, type RouteObject } from 'react-router-dom';
import PermissionRoute from './components/PermissionRoute';
import { CustomRouteObject } from './type';

export const generateRoutes = ({
  path,
  index,
  element,
  children,
  lazy,
  canEnter,
  ...rest
}: CustomRouteObject): RouteObject => {
  if (index) {
    return { index, element };
  }

  return {
    path: path || '',
    element: canEnter ? (
      <PermissionRoute canEnter={canEnter}>{element || <Outlet />}</PermissionRoute>
    ) : (
      element
    ),
    lazy,
    children: children?.map(generateRoutes) ?? [],
    ...rest,
  };
};
