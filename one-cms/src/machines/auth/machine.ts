import * as R from 'ramda';
import { createMachine, assign } from 'xstate';
import {
  AuthContext,
  AuthEventType,
  AuthStateType,
  ErrorEvent,
  FetchAccessTokenEvent,
  FetchProfileEvent,
  FetchRolesEvent,
} from './type';
import { Configuration, LoginApi } from '@/api/openapi';
import { handleApiError } from '@/api/utils';
import { authing } from '@/constants/authing';
import { GAME_SERVER_URL } from '@/constants/environment';
import { ADMIN_ROLE, ADMIN_USERS, QA_ROLE, QA_USERS, USER_ROLE } from '@/constants/permission';
import i18n from '@/i18n';
import { router } from '@/router';

export enum AuthState {
  CHECKING_IS_LOGGED_IN = 'CHECKING_IS_LOGGED_IN',
  AUTHING_LOGGING_IN = 'AUTHING_LOGGING_IN',
  INTERNAL_LOGGING_IN = 'INTERNAL_LOGGING_IN',
  FETCHING_ACCESS_TOKEN = 'FETCHING_ACCESS_TOKEN',
  LOGGING_IN_TO_GAME_SERVER = 'LOGGING_IN_TO_GAME_SERVER',
  FETCHING_PERMISSIONS = 'FETCHING_PERMISSIONS',
  CHECKING_PERMISSIONS = 'CHECKING_PERMISSIONS',
  LOGGED_IN = 'LOGGED_IN',
  LOGGING_OUT = 'LOGGING_OUT',
  FETCHING_USER_PROFILE = 'FETCHING_USER_PROFILE',
  LOGGED_OUT = 'LOGGED_OUT',
  UNAUTHORIZED = 'UNAUTHORIZED',
  ERROR = 'ERROR',
}
export enum AuthEvent {
  COMPLETE_LOGIN = 'COMPLETE_LOGIN',
  LOGOUT = 'LOGOUT',
  SWITCH_ACCOUNT = 'SWITCH_ACCOUNT',
}

export const authMachine = createMachine<AuthContext, AuthEventType, AuthStateType>(
  {
    id: 'auth',
    initial: AuthState.CHECKING_IS_LOGGED_IN,
    predictableActionArguments: true,
    context: {
      profile: {},
      accessToken: '',
      roles: [],
      permissions: [],
    },
    states: {
      [AuthState.CHECKING_IS_LOGGED_IN]: {
        always: [
          {
            cond: 'hasAccessTokenInStorage',
            target: AuthState.FETCHING_USER_PROFILE,
            actions: ['assignAccessTokenFromStorage'],
          },
          { cond: 'isAuthingRedirectCallback', target: AuthState.FETCHING_ACCESS_TOKEN },
          { cond: 'isAuthingDisabled', target: AuthState.INTERNAL_LOGGING_IN },
          { target: AuthState.AUTHING_LOGGING_IN },
        ],
      },
      [AuthState.INTERNAL_LOGGING_IN]: {
        entry: ['redirectToInternalLogin'],
        on: {
          [AuthEvent.COMPLETE_LOGIN]: {
            target: AuthState.LOGGING_IN_TO_GAME_SERVER,
            actions: ['assignAccessToken', 'saveAccessTokenToStorage'],
          },
        },
      },
      [AuthState.AUTHING_LOGGING_IN]: {
        entry: ['redirectToAuthingLogin'],
      },
      [AuthState.FETCHING_ACCESS_TOKEN]: {
        invoke: {
          src: 'fetchAccessToken',
          onDone: {
            target: AuthState.LOGGING_IN_TO_GAME_SERVER,
            actions: ['assignAccessToken', 'saveAccessTokenToStorage'],
          },
          onError: {
            target: AuthState.ERROR,
          },
        },
      },
      [AuthState.LOGGING_IN_TO_GAME_SERVER]: {
        invoke: {
          src: 'loginToGameServer',
          onDone: {
            target: AuthState.FETCHING_USER_PROFILE,
          },
          onError: {
            target: AuthState.ERROR,
          },
        },
      },
      [AuthState.FETCHING_USER_PROFILE]: {
        invoke: {
          src: 'fetchUserProfile',
          onDone: {
            target: AuthState.FETCHING_PERMISSIONS,
            actions: ['assignUserProfile'],
          },
          onError: {
            target: AuthState.ERROR,
          },
        },
      },
      [AuthState.FETCHING_PERMISSIONS]: {
        invoke: {
          src: 'fetchUserRoles',
          onDone: {
            target: AuthState.CHECKING_PERMISSIONS,
            actions: ['assignRoles', 'assignPermissions'],
          },
          onError: {
            target: AuthState.ERROR,
          },
        },
      },
      [AuthState.CHECKING_PERMISSIONS]: {
        always: [
          {
            cond: 'hasAnyPermission',
            target: AuthState.LOGGED_IN,
          },
          { target: AuthState.UNAUTHORIZED },
        ],
      },
      [AuthState.LOGGED_IN]: {
        on: {
          [AuthEvent.LOGOUT]: {
            target: AuthState.LOGGING_OUT,
          },
        },
      },
      [AuthState.LOGGING_OUT]: {
        always: [
          {
            cond: 'isAuthingDisabled',
            target: AuthState.INTERNAL_LOGGING_IN,
            actions: ['deleteAccessTokenFromStorage', 'redirectToInternalLogin'],
          },
          {
            target: AuthState.LOGGED_OUT,
            actions: ['deleteAccessTokenFromStorage', 'redirectToLogout'],
          },
        ],
      },
      [AuthState.LOGGED_OUT]: {},
      [AuthState.UNAUTHORIZED]: {
        on: {
          [AuthEvent.SWITCH_ACCOUNT]: {
            target: AuthState.LOGGING_OUT,
          },
        },
      },
      [AuthState.ERROR]: {
        entry: ['handleApiError'],
        after: {
          3000: AuthState.LOGGING_OUT,
        },
      },
    },
  },
  {
    guards: {
      hasAccessTokenInStorage: () => !!localStorage.getItem('accessToken'),
      isAuthingRedirectCallback: () => authing?.isRedirectCallback() ?? false,
      isAuthingDisabled: () => !authing,
      hasAnyPermission: ({ permissions }) => !R.isEmpty(permissions), // TODO: implement in another branch
    },
    actions: {
      assignAccessTokenFromStorage: assign({
        accessToken: () => localStorage.getItem('accessToken') || '',
      }),
      assignAccessToken: assign({
        accessToken: (_, event: FetchAccessTokenEvent) => event.data || '',
      }),
      saveAccessTokenToStorage: ({ accessToken }) =>
        accessToken && localStorage.setItem('accessToken', accessToken),
      deleteAccessTokenFromStorage: () => localStorage.removeItem('accessToken'),
      assignRoles: assign({
        roles: (_, event: FetchRolesEvent) => event.data || [],
      }),
      assignPermissions: assign({
        permissions: ({ roles }) =>
          R.pipe(R.pluck('permissions'), R.flatten, R.uniqBy(R.prop('id')))(roles || []),
      }),
      assignUserProfile: assign({
        profile: (_, event: FetchProfileEvent) => event.data.data || {},
      }),
      redirectToInternalLogin: () => router.navigate('/login'),
      redirectToAuthingLogin: () => authing?.loginWithRedirect(),
      redirectToLogout: () => {
        if (!authing) {
          return router.navigate('/login');
        } else {
          return authing.logoutWithRedirect();
        }
      },
      handleApiError: async (_, event: ErrorEvent) => {
        if (!i18n.hasLoadedNamespace('common')) {
          await i18n.loadNamespaces('common');
        }
        handleApiError(event.data);
      },
    },
    services: {
      fetchAccessToken: () =>
        authing?.getLoginState().then((state) => {
          if (state?.accessToken) {
            return state.accessToken;
          }
          throw new Error('No access token found');
        }) ?? Promise.resolve(''),
      fetchUserRoles: (context) =>
        new Promise((resolve) => {
          setTimeout(() => {
            // TODO: implement in another branch
            // TEMP: hardcode roles
            resolve(
              R.cond([
                [(x) => ADMIN_USERS.includes(x), R.always([ADMIN_ROLE])],
                [(x) => QA_USERS.includes(x), R.always([QA_ROLE])],
                [R.T, R.always([USER_ROLE])],
              ])(context.profile.email),
            );
          }, 500);
        }),
      loginToGameServer: ({ accessToken }) =>
        new LoginApi(new Configuration({ accessToken, basePath: GAME_SERVER_URL })).postLogin(),
      fetchUserProfile: ({ accessToken }) =>
        new LoginApi(
          new Configuration({
            accessToken,
            basePath: GAME_SERVER_URL,
          }),
        ).getUserProfile(),
    },
  },
);
