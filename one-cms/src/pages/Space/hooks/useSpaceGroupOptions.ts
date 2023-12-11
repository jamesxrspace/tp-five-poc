import { useInfiniteQuery } from '@tanstack/react-query';
import { DEFAULT_PAGE_SIZE } from '@/components/Pagination/Pagination.constant';
import { Option } from '@/components/Select/Select.type';
import { useApi } from '@/hooks/useApi';

export const useSpaceGroupOptions = () => {
  const { spaceGroupApi } = useApi();

  const { isLoading, data, fetchNextPage, hasNextPage } = useInfiniteQuery({
    queryKey: ['spaceGroupList'],
    queryFn: ({ pageParam = 0 }) =>
      spaceGroupApi.getSpaceGroupList({ offset: pageParam, size: DEFAULT_PAGE_SIZE }),
    initialPageParam: 0,
    getNextPageParam: (lastPage, pages) =>
      pages.length > Math.ceil((lastPage.data?.total ?? 0) / DEFAULT_PAGE_SIZE) - 1
        ? undefined
        : pages.length,
  });

  const spaceGroups = data?.pages.map((x) => x.data?.items || []).flat() || [];
  const spaceGroupOptions: Option[] = spaceGroups.map((spaceGroup) => ({
    label: spaceGroup.name || '',
    value: spaceGroup.spaceGroupId || '',
  }));

  return { isLoading, spaceGroupOptions, fetchNextPage, hasNextPage };
};
