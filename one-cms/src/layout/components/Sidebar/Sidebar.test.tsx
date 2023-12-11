import { BrowserRouter, useLocation } from 'react-router-dom';
import { NestedNavItem } from './Sidebar';
import { render } from '@/test/utils';

// Mock the useLocation hook from react-router-dom
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useLocation: jest.fn(),
}));

const MockedIcon = () => <div>MockIcon</div>;

describe('Sidebar', () => {
  it('should auto expand the parent of the active route', () => {
    const onCloseMock = jest.fn();
    (useLocation as jest.Mock).mockReturnValue({
      pathname: '/root/sub-route-1',
    });

    const { getByText } = render(
      <BrowserRouter>
        <NestedNavItem
          path="/root"
          name="Root"
          icon={MockedIcon}
          onClose={onCloseMock}
          routeChildren={[
            {
              name: 'SubRoute 1',
              path: 'sub-route-1',
              icon: MockedIcon,
            },
            {
              name: 'SubRoute 2',
              path: 'sub-route-2',
              icon: MockedIcon,
              children: [
                {
                  name: 'SubSubRoute 1',
                  path: 'sub-sub-route-1',
                  icon: MockedIcon,
                },
              ],
            },
          ]}
        />
      </BrowserRouter>,
    );

    expect(getByText('SubRoute 1')).toBeVisible();
    expect(getByText('SubRoute 2')).toBeVisible();
    expect(getByText('SubSubRoute 1')).not.toBeVisible();
  });

  it('should auto expand the parents of the active route', () => {
    const onCloseMock = jest.fn();
    (useLocation as jest.Mock).mockReturnValue({
      pathname: '/root/sub-route-2/sub-sub-route-1',
    });

    const { getByText } = render(
      <BrowserRouter>
        <NestedNavItem
          path="/root"
          name="Root"
          icon={MockedIcon}
          onClose={onCloseMock}
          routeChildren={[
            {
              name: 'SubRoute 1',
              path: 'sub-route-1',
              icon: MockedIcon,
            },
            {
              name: 'SubRoute 2',
              path: 'sub-route-2',
              icon: MockedIcon,
              children: [
                {
                  name: 'SubSubRoute 1',
                  path: 'sub-sub-route-1',
                  icon: MockedIcon,
                },
              ],
            },
          ]}
        />
      </BrowserRouter>,
    );

    expect(getByText('SubRoute 1')).toBeVisible();
    expect(getByText('SubRoute 2')).toBeVisible();
    expect(getByText('SubSubRoute 1')).toBeVisible();
  });
});
