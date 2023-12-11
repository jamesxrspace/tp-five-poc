import { FiCodepen, FiList, FiTag } from 'react-icons/fi';
import { RiListCheck2 } from 'react-icons/ri';
import { Navigate } from 'react-router-dom';
import { CustomRouteObject } from '@/router/type';

export const SPACE_MODULE_ROUTES: CustomRouteObject = {
  name: 'Space',
  translationKey: 'common:navigation.space.space',
  path: '/space',
  icon: FiCodepen,
  children: [
    {
      index: true,
      element: <Navigate to="/space/list" replace={true} />,
    },
    {
      path: 'list',
      name: 'Space List',
      translationKey: 'common:navigation.space.list',
      icon: FiList,
      async lazy() {
        const { SpaceList } = await import('@/pages/Space/SpaceList');
        return { Component: SpaceList };
      },
    },
    {
      path: 'group/list',
      name: 'Group List',
      translationKey: 'common:navigation.space.groupList',
      icon: RiListCheck2,
      async lazy() {
        const { GroupList } = await import('@/pages/Space/GroupList');
        return { Component: GroupList };
      },
    },
    {
      path: 'recommended-tag',
      name: 'Recommended Tag',
      translationKey: 'common:navigation.space.recommendedTag',
      icon: FiTag,
      async lazy() {
        const { RecommendedTag } = await import('@/pages/Space/RecommendedTag');
        return { Component: RecommendedTag };
      },
    },
  ],
};
