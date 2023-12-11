import { Select, Flex, Text, ButtonGroup, useBreakpointValue } from '@chakra-ui/react';
import { useMemo } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import {
  RiArrowLeftSLine,
  RiSkipLeftLine,
  RiArrowRightSLine,
  RiSkipRightLine,
} from 'react-icons/ri';
import IconButtonWithTooltip from '../IconButtonWithTooltip/IconButtonWithTooltip';
import { DEFAULT_PAGE_SIZE_OPTIONS } from './Pagination.constant';
import { PaginationProps, PaginationState } from './Pagination.type';

export const Pagination = ({
  totalItems,
  pageIndex,
  pageSize,
  pageSizeOptions = DEFAULT_PAGE_SIZE_OPTIONS,
  disablePageSizeSelect,
  onPageChange,
  ...rest
}: PaginationProps) => {
  const { t } = useTranslation();
  const buttonSize = useBreakpointValue({ base: 'sm', md: 'md' });

  const totalPage = useMemo(() => Math.ceil(totalItems / pageSize), [totalItems, pageSize]);
  const adjustedPageIndex = useMemo(() => Math.min(pageIndex, totalPage), [pageIndex, totalPage]);

  const handlePageChange = (pagination: PaginationState) => {
    onPageChange?.(pagination);
  };

  return (
    <Flex data-testid="pagination" justifyContent="space-between" alignItems="center" {...rest}>
      <Flex gap={2}>
        <Text flexShrink="0">
          <Trans
            i18nKey="pagination.currentAndTotal"
            components={{
              bold: <Text fontWeight="bold" as="span" />,
            }}
            values={{ current: Math.min(pageIndex + 1, totalPage), total: totalPage }}
          />
        </Text>
      </Flex>

      <ButtonGroup size={buttonSize}>
        <IconButtonWithTooltip
          data-testid="first-page-button"
          label="First Page"
          aria-label="First Page"
          onClick={() => handlePageChange({ pageIndex: 0, pageSize })}
          isDisabled={adjustedPageIndex === 0}
          icon={<RiSkipLeftLine />}
        />

        <IconButtonWithTooltip
          data-testid="previous-page-button"
          label="Previous Page"
          aria-label="Previous Page"
          onClick={() => handlePageChange({ pageIndex: pageIndex - 1, pageSize })}
          isDisabled={adjustedPageIndex === 0}
          icon={<RiArrowLeftSLine />}
        />

        <IconButtonWithTooltip
          data-testid="next-page-button"
          label="Next Page"
          aria-label="Next Page"
          onClick={() => handlePageChange({ pageIndex: pageIndex + 1, pageSize })}
          isDisabled={adjustedPageIndex === totalPage - 1}
          icon={<RiArrowRightSLine />}
        />

        <IconButtonWithTooltip
          data-testid="last-page-button"
          label="Last Page"
          aria-label="Last Page"
          onClick={() => handlePageChange({ pageIndex: totalPage - 1, pageSize })}
          isDisabled={adjustedPageIndex === totalPage - 1}
          icon={<RiSkipRightLine />}
        />
      </ButtonGroup>

      <Flex gap={2} alignItems="center">
        <Text flexShrink="0" display={{ base: 'none', md: 'block' }}>
          {t('pagination.rowsPerPage')}
        </Text>
        <Select
          size={{ base: 'sm', md: 'md' }}
          data-testid="page-size-select"
          value={pageSize}
          isDisabled={disablePageSizeSelect}
          onChange={(e) => handlePageChange({ pageIndex: 0, pageSize: +e.target.value })}
        >
          {pageSizeOptions.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </Select>
      </Flex>
    </Flex>
  );
};

export default Pagination;
