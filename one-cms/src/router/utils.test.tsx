import { FiCodepen, FiList } from 'react-icons/fi';
import { Navigate } from 'react-router-dom';
import { CustomRouteObject } from './type';
import { generateRoutes } from './utils';

jest.mock('@/machines/auth');

describe('Route', () => {
  test('generate routes from custom route object', () => {
    const SpaceIndexComponent = <Navigate to="/space/list" replace={true} />;
    const SpaceListComponent = <div />;
    const customRoutes: CustomRouteObject = {
      name: 'Space',
      path: '/space',
      icon: FiCodepen,
      children: [
        {
          index: true,
          element: SpaceIndexComponent,
        },
        {
          path: '/list',
          name: 'Space List',
          icon: FiList,
          element: SpaceListComponent,
        },
      ],
    };

    const routes = generateRoutes(customRoutes);

    expect(routes).toEqual({
      name: 'Space',
      path: '/space',
      icon: FiCodepen,
      element: undefined,
      lazy: undefined,
      children: [
        {
          index: true,
          element: SpaceIndexComponent,
        },
        {
          path: '/list',
          name: 'Space List',
          icon: FiList,
          element: SpaceListComponent,
          lazy: undefined,
          children: [],
        },
      ],
    });
  });
});
