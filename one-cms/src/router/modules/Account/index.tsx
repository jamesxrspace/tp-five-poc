import { FiUser, FiUsers, FiSlash, FiSearch, FiSettings } from 'react-icons/fi';
import { MdOutlineGroups2 } from 'react-icons/md';
import { Navigate } from 'react-router-dom';
import { CustomRouteObject } from '@/router/type';

export const ACCOUNT_MODULE_ROUTES: CustomRouteObject = {
  name: 'Account',
  translationKey: 'common:navigation.account.account',
  path: '/account',
  icon: FiUsers,
  children: [
    {
      index: true,
      element: <Navigate to="/account/user-management" replace={true} />,
    },
    {
      path: 'user-management',
      name: 'User Management',
      translationKey: 'navigation.account.userManagement',
      icon: FiUser,
      async lazy() {
        const { UserManagement } = await import('@/pages/Account/UserManagement');
        return { Component: UserManagement };
      },
    },
    {
      path: 'role',
      name: 'Role',
      translationKey: 'navigation.account.role',
      icon: MdOutlineGroups2,
      children: [
        {
          index: true,
          element: <Navigate to="/account/role/settings" replace={true} />,
        },
        {
          path: 'settings',
          name: 'Settings',
          translationKey: 'navigation.account.roleSettings',
          icon: FiSettings,
          async lazy() {
            const { RoleSettings } = await import('@/pages/Account/RoleSettings');
            return { Component: RoleSettings };
          },
        },
      ],
    },
    {
      path: 'ban-ip',
      name: 'Ban IP',
      translationKey: 'navigation.account.banIp',
      icon: FiSlash,
      async lazy() {
        const { BanIP } = await import('@/pages/Account/BanIP');
        return { Component: BanIP };
      },
    },
    {
      path: 'batch-user-search',
      name: 'Batch User Search',
      translationKey: 'navigation.account.batchUserSearch',
      icon: FiSearch,
      async lazy() {
        const { BatchUserSearch } = await import('@/pages/Account/BatchUserSearch');
        return { Component: BatchUserSearch };
      },
    },
  ],
};
