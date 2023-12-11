import { Permission, Role } from '@/types/auth';

export const QA_USERS = ['rod.hsieh@xrspace.io', 'tiffany.yeh@xrspace.io'];
export const ADMIN_USERS = [
  'cj.gu@xrspace.io',
  'morris.tseng@xrspace.io',
  'cmj.tsai@xrspace.io',
  'lizzy.yang@xrspace.io',
  'patrick.lin@xrspace.io',
];

export const DailyBuildReadPermission: Permission = {
  id: '1',
  name: 'view daily build',
  type: 'read',
};

export const DailyBuildWritePermission: Permission = {
  id: '2',
  name: 'delete daily build',
  type: 'write',
};

export const ADMIN_ROLE: Role = {
  id: '1',
  name: 'Admin',
  permissions: [DailyBuildReadPermission, DailyBuildWritePermission],
};

export const QA_ROLE: Role = {
  id: '2',
  name: 'QA',
  permissions: [DailyBuildReadPermission, DailyBuildWritePermission],
};

export const USER_ROLE: Role = {
  id: '3',
  name: 'User',
  permissions: [DailyBuildReadPermission],
};
