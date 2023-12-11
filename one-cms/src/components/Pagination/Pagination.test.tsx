import Pagination from './Pagination';
import { PaginationProps } from './Pagination.type';
import { render, screen, fireEvent, waitFor } from '@/test/utils';

describe('Pagination', () => {
  let props: PaginationProps;
  let mockPageSize: number;
  let mockPageIndex: number;

  beforeEach(() => {
    mockPageIndex = 2;
    mockPageSize = 10;

    const onPageChangeMock = jest.fn(({ pageIndex, pageSize }) => {
      mockPageIndex = pageIndex;
      mockPageSize = pageSize;
    });

    props = {
      totalItems: 100,
      pageIndex: mockPageIndex,
      pageSize: mockPageSize,
      pageSizeOptions: [10, 20, 30, 50],
      onPageChange: onPageChangeMock,
    };
  });

  describe('State Test', () => {
    it('should render next page button', () => {
      render(<Pagination {...props} />);

      const nextButton = screen.getByTestId('next-page-button');
      expect(nextButton).toBeInTheDocument();
    });

    it('should render previous page button', () => {
      render(<Pagination {...props} />);

      const previousButton = screen.getByTestId('previous-page-button');
      expect(previousButton).toBeInTheDocument();
    });

    it('should render first page button', () => {
      render(<Pagination {...props} />);

      const firstButton = screen.getByTestId('first-page-button');
      expect(firstButton).toBeInTheDocument();
    });

    it('should render last page button', () => {
      render(<Pagination {...props} />);

      const lastButton = screen.getByTestId('last-page-button');
      expect(lastButton).toBeInTheDocument();
    });

    it('should have expected page size options', () => {
      render(<Pagination {...props} />);

      const expectedOptions = [10, 20, 30, 50];
      const pageSizeOptions = screen.getByTestId('page-size-select').querySelectorAll('option');

      pageSizeOptions.forEach((item) => {
        expect(expectedOptions).toContain(Number(item.value));
      });
    });
  });

  describe('Functional Test', () => {
    it('should change to the next page', async () => {
      const { getByTestId } = render(<Pagination {...props} />);
      const nextButton = getByTestId('next-page-button');

      fireEvent.click(nextButton);

      await waitFor(() => {
        expect(props.onPageChange).toBeCalledTimes(1);
        expect(mockPageIndex).toBe(3);
      });
    });

    it('should change to the previous page', async () => {
      const { getByTestId } = render(<Pagination {...props} />);
      const previousButton = getByTestId('previous-page-button');

      fireEvent.click(previousButton);

      await waitFor(() => {
        expect(props.onPageChange).toBeCalledTimes(1);
        expect(mockPageIndex).toBe(1);
      });
    });

    it('should change to the first page', async () => {
      const { getByTestId } = render(<Pagination {...props} />);
      const firstButton = getByTestId('first-page-button');

      fireEvent.click(firstButton);

      await waitFor(() => {
        expect(props.onPageChange).toBeCalledTimes(1);
        expect(mockPageIndex).toBe(0);
      });
    });

    it('should change to the last page', async () => {
      const { getByTestId } = render(<Pagination {...props} />);
      const lastButton = getByTestId('last-page-button');

      fireEvent.click(lastButton);

      await waitFor(() => {
        expect(props.onPageChange).toBeCalledTimes(1);
        expect(mockPageIndex).toBe(9);
      });
    });

    it('should change to another page size when user selected a new one', async () => {
      const { getByTestId } = render(<Pagination {...props} />);
      const pageSizeSelect = getByTestId('page-size-select');

      fireEvent.change(pageSizeSelect, { target: { value: 30 } });

      await waitFor(() => {
        expect(props.onPageChange).toBeCalledTimes(1);
        expect(mockPageSize).toBe(30);
      });
    });
  });
});
