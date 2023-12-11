import { TableProps as ChakraTableProps } from '@chakra-ui/react';
import { ColumnDef, SortingState } from '@tanstack/react-table';
import { ReactNode, ChangeEvent } from 'react';

export interface fetchDataProps {
  pageIndex: number;
  pageSize: number;
  globalFilter?: string;
  sorting?: SortingState;
}

export interface renderInputBarProps {
  isLoading?: boolean;
  inputValue: string;
  onInputChange: (e: ChangeEvent<HTMLInputElement>) => void;
}

export type TableProps<T extends object> = ChakraTableProps & {
  size?: 'sm' | 'md' | 'lg';
  columns: ColumnDef<T>[];
  isFilterable?: boolean;
  isPaginated?: boolean;
  isSortable?: boolean;
  defaultPageSize?: number;
  pageSizeOptions?: number[];
  renderInputBar?: ({ isLoading, inputValue, onInputChange }: renderInputBarProps) => ReactNode;
} & (
    | {
        data: T[];
        fetchData?: undefined;
      }
    | {
        data?: undefined;
        fetchData: ({
          pageIndex,
          pageSize,
          globalFilter,
          sorting,
        }: fetchDataProps) => Promise<{ data: T[]; total: number }>;
      }
  );
