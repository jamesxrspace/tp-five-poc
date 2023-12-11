import { useInfiniteQuery } from '@tanstack/react-query';
import { DEFAULT_PAGE_SIZE } from '@/components/Pagination/Pagination.constant';
import { Option } from '@/components/Select/Select.type';
import { useApi } from '@/hooks/useApi';

export const useSpaceOptions = () => {
  const { spaceApi } = useApi();
  const { isLoading, data, fetchNextPage, hasNextPage } = useInfiniteQuery({
    queryKey: ['spaceList'],
    queryFn: ({ pageParam = 0 }) =>
      spaceApi.getSpaceList({ offset: pageParam, size: DEFAULT_PAGE_SIZE }),
    initialPageParam: 0,
    getNextPageParam: (lastPage, pages) =>
      pages.length > Math.ceil((lastPage.data?.total ?? 0) / DEFAULT_PAGE_SIZE) - 1
        ? undefined
        : pages.length,
  });
  const spaceList = data?.pages.map((x) => x.data?.items || []).flat() || [];
  const spaceOptions: Option[] = spaceList.map((spaceList) => ({
    label: spaceList.name || '',
    value: spaceList.spaceId || '',
  }));

  return { isLoading, spaceOptions, fetchNextPage, hasNextPage };
};
