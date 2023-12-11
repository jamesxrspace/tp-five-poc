import {
  Box,
  Flex,
  Table as ChakraTable,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  TableContainer,
  Input,
  Icon,
} from '@chakra-ui/react';
import { rankItem } from '@tanstack/match-sorter-utils';
import {
  useReactTable,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  getPaginationRowModel,
  flexRender,
  SortingState,
  FilterFn,
} from '@tanstack/react-table';
import * as R from 'ramda';
import { useState, useEffect, useCallback, useMemo, ChangeEvent } from 'react';
import { useTranslation } from 'react-i18next';
import { FiChevronUp, FiChevronDown } from 'react-icons/fi';
import Pagination from '../Pagination/Pagination';
import { DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE_OPTIONS } from '../Pagination/Pagination.constant';
import { PaginationState } from '../Pagination/Pagination.type';
import { TableProps } from './Table.type';
import { useDebounce } from '@/hooks/useDebounce';

export const Table = <T extends object>({
  data = [],
  size = 'md',
  columns,
  isFilterable = false,
  isPaginated = false,
  isSortable = false,
  fetchData,
  defaultPageSize = DEFAULT_PAGE_SIZE,
  pageSizeOptions = DEFAULT_PAGE_SIZE_OPTIONS,
  renderInputBar,
  ...rest
}: TableProps<T>) => {
  const { t } = useTranslation();
  const [innerData, setInnerData] = useState<T[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [filterInputValue, setFilterInputValue] = useState('');
  const [globalFilter, setGlobalFilter] = useState('');
  const [sorting, setSorting] = useState<SortingState>([]);
  const [totalItems, setTotalItems] = useState(0);
  const isAsyncLoading = !R.isNil(fetchData);
  const [{ pageIndex, pageSize }, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: defaultPageSize,
  });
  const pagination = useMemo(
    () => ({
      pageIndex,
      pageSize,
    }),
    [pageIndex, pageSize],
  );

  const onInputChange = useCallback(
    (e: ChangeEvent<HTMLInputElement>) => setFilterInputValue(e.target.value),
    [],
  );

  const allColumnFilterFn: FilterFn<T> = useCallback((row, columnId, value, addMeta) => {
    // Rank the item
    const itemRank = rankItem(row.getValue(columnId), value);

    // Store the itemRank info
    addMeta({
      itemRank,
    });

    // Return if the item should be filtered in/out
    return itemRank.passed;
  }, []);

  const debouncedFilterInputValue = useDebounce(filterInputValue, 500);
  useEffect(() => {
    setGlobalFilter(debouncedFilterInputValue);
  }, [debouncedFilterInputValue, setGlobalFilter]);

  const { getHeaderGroups, getRowModel } = useReactTable({
    columns,
    data: innerData,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: isFilterable ? getFilteredRowModel() : undefined,
    getSortedRowModel: isSortable ? getSortedRowModel() : undefined,
    getPaginationRowModel: isPaginated ? getPaginationRowModel() : undefined,
    manualFiltering: isAsyncLoading,
    manualPagination: isAsyncLoading,
    manualSorting: isAsyncLoading,
    state: {
      pagination,
      globalFilter,
      sorting,
    },
    filterFns: {
      allColumnFilter: allColumnFilterFn,
    },
    onPaginationChange: setPagination,
    onGlobalFilterChange: setGlobalFilter,
    onSortingChange: setSorting,
    globalFilterFn: allColumnFilterFn,
  });

  useEffect(() => {
    setPagination((state) => ({
      ...state,
      pageIndex: 0,
    }));
  }, [fetchData, globalFilter, sorting]);

  useEffect(() => {
    if (!isAsyncLoading) {
      setInnerData(data);
      setTotalItems(data.length);
    }
  }, [isAsyncLoading, data]);

  useEffect(() => {
    const fetchDataAsync = async () => {
      setIsLoading(true);
      try {
        if (!fetchData) {
          return;
        }
        const { data, total } = await fetchData({
          pageIndex,
          pageSize,
          globalFilter,
          sorting,
        });
        setInnerData(data);
        setTotalItems(+total);
      } catch (error) {
        console.error('Failed to fetch data:', error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchDataAsync();
  }, [fetchData, pageIndex, pageSize, globalFilter, sorting]);

  return (
    <>
      {isFilterable &&
        (renderInputBar ? (
          renderInputBar({
            isLoading,
            inputValue: filterInputValue,
            onInputChange,
          })
        ) : (
          <Box mx={2} textAlign="left">
            <Input
              data-testid="search-input"
              value={filterInputValue}
              onChange={onInputChange}
              disabled={isLoading}
              my={2}
              maxW={{ base: '100%', md: '300px' }}
              placeholder="Please enter keywords"
            />
          </Box>
        ))}

      <TableContainer>
        <ChakraTable size={size} {...rest}>
          <Thead>
            {getHeaderGroups().map((headerGroup) => (
              <Tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <Th key={header.id}>
                    {!header.isPlaceholder && (
                      <Flex
                        alignItems="center"
                        cursor={isSortable && header.column.getCanSort() ? 'pointer' : 'default'}
                        onClick={header.column.getToggleSortingHandler()}
                      >
                        {flexRender(header.column.columnDef.header, header.getContext())}

                        {isSortable &&
                          {
                            false: null,
                            asc: <Icon as={FiChevronUp} boxSize={3} />,
                            desc: <Icon as={FiChevronDown} boxSize={3} />,
                          }[`${header.column.getIsSorted()}`]}
                      </Flex>
                    )}
                  </Th>
                ))}
              </Tr>
            ))}
          </Thead>

          <Tbody data-testid="tbody">
            {isLoading && (
              <Tr>
                <Td colSpan={getHeaderGroups()?.[0]?.headers.length}>
                  <Box data-testid="loading" textAlign="center" p={4}>
                    {t('table.loading')}...
                  </Box>
                </Td>
              </Tr>
            )}

            {!isLoading && !getRowModel().rows.length && (
              <Tr>
                <Td colSpan={getHeaderGroups()?.[0]?.headers.length}>
                  <Box textAlign="center" p={4}>
                    {t('table.noData')}
                  </Box>
                </Td>
              </Tr>
            )}

            {!isLoading &&
              getRowModel().rows &&
              getRowModel().rows.map((row) => (
                <Tr key={row.id}>
                  {row.getVisibleCells().map((cell) => (
                    <Td key={cell.id}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </Td>
                  ))}
                </Tr>
              ))}
          </Tbody>
        </ChakraTable>
      </TableContainer>

      {isPaginated && (
        <Pagination
          m={4}
          totalItems={totalItems}
          pageIndex={pageIndex}
          pageSize={pageSize}
          pageSizeOptions={pageSizeOptions}
          disablePageSizeSelect={isLoading}
          onPageChange={setPagination}
        />
      )}
    </>
  );
};

export default Table;
