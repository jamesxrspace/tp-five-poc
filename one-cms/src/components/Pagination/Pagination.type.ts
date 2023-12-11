import { FlexProps } from '@chakra-ui/react';

export interface PaginationProps extends FlexProps {
  totalItems: number;
  pageIndex: number;
  pageSize: number;
  pageSizeOptions?: number[];
  disablePageSizeSelect?: boolean;
  onPageChange: (pagination: PaginationState) => void;
}

export interface PaginationState {
  pageIndex: number;
  pageSize: number;
}
