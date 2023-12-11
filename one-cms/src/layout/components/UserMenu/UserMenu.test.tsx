import { createMachine, interpret } from 'xstate';
import UserMenu from './UserMenu';
import { GlobalStateContext } from '@/machines';
import { AuthState, AuthContext, AuthEventType } from '@/machines/auth';
import { render } from '@/test/utils';

describe('UserMenu', () => {
  it('renders the role names as tags', () => {
    const mockAuthMachineDef = createMachine<AuthContext, AuthEventType>({
      id: 'auth',
      initial: AuthState.LOGGED_IN,
      predictableActionArguments: true,
      context: {
        profile: {},
        roles: [
          {
            id: '1',
            name: 'Admin',
            permissions: [{ id: '1', name: 'admin', type: 'write' }],
          },
          {
            id: '2',
            name: 'User',
            permissions: [
              { id: '1', name: 'view space', type: 'read' },
              { id: '1', name: 'write space', type: 'write' },
            ],
          },
        ],
        permissions: [],
      },
      states: {
        [AuthState.LOGGED_IN]: {},
      },
    });
    const mockAuthService = interpret(mockAuthMachineDef).start();
    const { getByTestId } = render(
      <GlobalStateContext.Provider value={{ authService: mockAuthService }}>
        <UserMenu />
      </GlobalStateContext.Provider>,
    );
    const roleTags = getByTestId('role-tags').querySelectorAll('span');
    expect(roleTags.length).toBe(2);
    expect(roleTags[0]).toHaveTextContent('Admin');
    expect(roleTags[1]).toHaveTextContent('User');
  });
});
