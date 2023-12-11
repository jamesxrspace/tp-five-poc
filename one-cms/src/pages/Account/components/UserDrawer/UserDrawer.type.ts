import { AccountUser } from '@/types/account';

export interface UserDrawerProps<T> {
  isOpen: boolean;
  onClose: () => void;
  data?: AccountUser;
  roleList: T[];
}

export type UserDrawerForm = AccountUser;
