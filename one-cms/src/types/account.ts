import { Option } from '@/components/Select/Select.type';

export interface AccountUser {
  id?: string;
  username: string;
  nickname: string;
  email: string;
  isEmailVerified: boolean;
  roles: Option[];
}
