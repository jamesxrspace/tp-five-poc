import { FiList } from 'react-icons/fi';
import { MdEvent } from 'react-icons/md';
import { Navigate } from 'react-router-dom';
import { CustomRouteObject } from '@/router/type';

export const EVENT_MODULE_ROUTES: CustomRouteObject = {
  name: 'Event',
  path: '/event',
  translationKey: 'navigation.event.event',
  icon: MdEvent,
  children: [
    {
      index: true,
      element: <Navigate to="/event/promotion" replace={true} />,
    },
    {
      path: 'promotion',
      name: 'Promotion',
      translationKey: 'navigation.event.promotion',
      icon: FiList,
      async lazy() {
        const { Promotion } = await import('@/pages/Event/Promotion');
        return { Component: Promotion };
      },
    },
  ],
};
