import { DoneInvokeEvent } from 'xstate';
import { AuthEvent, AuthState } from './machine';
import { GetProfileResponse, Profile, ResponseError } from '@/api/openapi';
import { Permission, Role } from '@/types/auth';

export interface AuthContext {
  profile: Profile;
  accessToken?: string;
  roles: Role[];
  permissions: Permission[];
}

export type FetchAccessTokenEvent = DoneInvokeEvent<string>;
export type FetchRolesEvent = DoneInvokeEvent<Role[]>;
export type FetchProfileEvent = DoneInvokeEvent<GetProfileResponse>;
export type SwitchAccountEvent = { type: AuthEvent.SWITCH_ACCOUNT };
export type CompleteLoginEvent = { type: AuthEvent.COMPLETE_LOGIN; data: string };
export type LogoutEvent = { type: AuthEvent.LOGOUT };
export type ErrorEvent = { type: AuthState.ERROR; data: ResponseError };
export type AuthEventType =
  | FetchAccessTokenEvent
  | FetchRolesEvent
  | FetchProfileEvent
  | SwitchAccountEvent
  | CompleteLoginEvent
  | LogoutEvent
  | ErrorEvent;

export type AuthStateType = {
  value: AuthState;
  context: AuthContext;
};
