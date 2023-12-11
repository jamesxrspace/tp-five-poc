import Table from './Table';
import { TableProps } from './Table.type';
import { render, screen, waitFor } from '@/test/utils';

describe('Table', () => {
  interface ISeedData {
    id: string;
    username: string;
    email: string;
  }

  const columns: TableProps<ISeedData>['columns'] = [
    {
      header: 'id',
      accessorKey: 'id',
    },
    {
      header: 'username',
      accessorKey: 'username',
    },
    {
      header: 'email',
      accessorKey: 'email',
    },
  ];
  const mockData: ISeedData[] = Array.from({ length: 10 }, (_, i) => ({
    id: `id-${i}`,
    username: `username-${i}`,
    email: `email-${i}@test.com`,
  }));
  let mockFetchData: jest.Mock<Promise<{ data: ISeedData[]; total: number }>>;

  beforeEach(() => {
    mockFetchData = jest.fn(() => Promise.resolve({ data: mockData, total: 100 }));
  });

  describe('State Test', () => {
    it('should render a loading indicator', async () => {
      render(<Table columns={columns} fetchData={mockFetchData} />);

      await waitFor(() => {
        const loadingElement = screen.getByTestId('loading');
        expect(loadingElement).toBeInTheDocument();
      });
    });
  });

  describe('Functionality Test', () => {
    it('should render pagination when "isPaginated" prop is true', async () => {
      render(<Table columns={columns} isPaginated fetchData={mockFetchData} />);

      await waitFor(() => {
        const paginationElement = screen.getByTestId('pagination');
        expect(paginationElement).toBeInTheDocument();
      });
    });

    it('should render a searchInput when "isFilterable" prop is true', async () => {
      render(<Table columns={columns} isFilterable fetchData={mockFetchData} />);

      await waitFor(() => {
        const searchInputElement = screen.getByTestId('search-input');
        expect(searchInputElement).toBeInTheDocument();
      });
    });

    it('should include pointer class when "isSortable" prop is true', async () => {
      render(<Table columns={columns} isSortable fetchData={mockFetchData} />);

      await waitFor(() => {
        const headerElement = screen.getByText('username').closest('div');
        expect(headerElement).toHaveStyle('cursor: pointer');
      });
    });

    it('should render content from fetchData when the data fetched successfully', async () => {
      render(<Table columns={columns} isPaginated fetchData={mockFetchData} />);

      await waitFor(() => {
        const trElement = screen.getByTestId('tbody').querySelectorAll('tr');
        const pageSizeDefault = 10;
        expect(trElement.length).toBe(pageSizeDefault);
      });
    });
  });
});
