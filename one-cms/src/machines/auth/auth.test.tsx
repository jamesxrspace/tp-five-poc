import { interpret } from 'xstate';
import { waitFor } from 'xstate/lib/waitFor';
import { AuthEvent, AuthState, authMachine } from './machine';

const MOCK_TOKEN = 'mock_token';

jest.mock('@/router', () => ({
  navigate: jest.fn(),
}));

describe('auth flow', () => {
  describe('get access token by local auth', () => {
    let isRedirected = false;
    const mockAuthMachine = authMachine.withConfig({
      guards: {
        hasAccessTokenInStorage: () => false,
        isAuthingDisabled: () => true,
      },
      actions: {
        redirectToInternalLogin: () => {
          isRedirected = true;
        },
      },
    });

    it('should redirect to login page', () => {
      interpret(mockAuthMachine).start();

      expect(isRedirected).toBeTruthy();
    });

    it('should have access token before login to game server', async () => {
      const authService = interpret(mockAuthMachine).start();

      authService.send({ type: AuthEvent.COMPLETE_LOGIN, data: MOCK_TOKEN });
      const state = await waitFor(authService, (state) =>
        state.matches(AuthState.LOGGING_IN_TO_GAME_SERVER),
      );

      expect(state.context.accessToken).toBe(MOCK_TOKEN);
    });
  });

  describe('get access token by Authing', () => {
    it('should redirect to Authing login page', () => {
      let isRedirected = false;
      const mockAuthMachine = authMachine.withConfig({
        guards: {
          hasAccessTokenInStorage: () => false,
          isAuthingDisabled: () => false,
          isAuthingRedirectCallback: () => false,
        },
        actions: {
          redirectToAuthingLogin: () => {
            isRedirected = true;
          },
        },
      });

      const authService = interpret(mockAuthMachine).start();

      expect(authService.getSnapshot().value).toBe(AuthState.AUTHING_LOGGING_IN);
      expect(isRedirected).toBeTruthy();
    });

    it('should fetch token after redirected by Authing', () => {
      const mockAuthMachine = authMachine.withConfig({
        guards: {
          hasAccessTokenInStorage: () => false,
          isAuthingDisabled: () => false,
          isAuthingRedirectCallback: () => true,
        },
      });

      const authService = interpret(mockAuthMachine).start();
      const state = authService.getSnapshot().value;

      expect(state).toBe(AuthState.FETCHING_ACCESS_TOKEN);
    });

    it('should have access token before login to game server', async () => {
      const mockAuthMachine = authMachine.withConfig({
        guards: {
          hasAccessTokenInStorage: () => false,
          isAuthingDisabled: () => false,
          isAuthingRedirectCallback: () => true,
        },
        services: {
          fetchAccessToken: () => Promise.resolve(MOCK_TOKEN),
        },
      });

      const authService = interpret(mockAuthMachine).start();
      const state = await waitFor(authService, (state) =>
        state.matches(AuthState.LOGGING_IN_TO_GAME_SERVER),
      );

      expect(state.context.accessToken).toBe(MOCK_TOKEN);
    });
  });

  describe('has access token', () => {
    it('should be unauthorized if no permission', async () => {
      const mockAuthMachine = authMachine.withConfig({
        guards: {
          hasAccessTokenInStorage: () => true,
        },
        services: {
          fetchUserProfile: () => Promise.resolve({ username: 'admin', email: 'admin@localhost' }),
          fetchUserRoles: () => Promise.resolve([]),
        },
      });

      const authService = interpret(mockAuthMachine).start();
      const state = await waitFor(authService, (state) => state.matches(AuthState.UNAUTHORIZED));

      expect(state.matches(AuthState.UNAUTHORIZED)).toBeTruthy();
    });

    it('should be logged in if has both token and permissions', async () => {
      const mockAuthMachine = authMachine.withConfig({
        guards: {
          hasAccessTokenInStorage: () => true,
        },
        services: {
          fetchUserProfile: () => Promise.resolve({ username: 'admin', email: 'admin@localhost' }),
          fetchUserRoles: () => Promise.resolve([{ id: 1, name: 'admin' }]),
        },
      });

      const authService = interpret(mockAuthMachine).start();
      const state = await waitFor(authService, (state) => state.matches(AuthState.LOGGED_IN));

      expect(state.matches(AuthState.LOGGED_IN)).toBeTruthy();
    });
  });
});
