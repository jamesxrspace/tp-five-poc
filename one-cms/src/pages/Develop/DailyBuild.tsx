import {
  Card,
  CardBody,
  Flex,
  Heading,
  Stack,
  Text,
  Link,
  Button,
  useDisclosure,
  useToast,
} from '@chakra-ui/react';
import { useQuery, useMutation } from '@tanstack/react-query';
import { PaginationState } from '@tanstack/react-table';
import { useState, useCallback } from 'react';
import { DailyBuildBuildTypeEnum } from '@/api/openapi';
import { queryClient } from '@/api/reactQuery';
import DeleteAlert from '@/components/DeleteAlert/DeleteAlert';
import Loading from '@/components/Loading/Loading';
import Pagination from '@/components/Pagination/Pagination';
import {
  DEFAULT_PAGE_SIZE,
  DEFAULT_PAGE_SIZE_OPTIONS,
} from '@/components/Pagination/Pagination.constant';
import Select from '@/components/Select/Select';
import { Option } from '@/components/Select/Select.type';
import { DailyBuildWritePermission } from '@/constants/permission';
import { useApi } from '@/hooks/useApi';
import { useAuth } from '@/hooks/useAuth';
import { withTitle } from '@/utils/page';

const APK_TYPE_LIST: Option[] = [
  { value: DailyBuildBuildTypeEnum.Apk, label: 'APK' },
  { value: DailyBuildBuildTypeEnum.VrApk, label: 'VR APK' },
];

export const DailyBuildPage = withTitle('navigation.develop.dailyBuild', () => {
  const [state] = useAuth();
  const toast = useToast();
  const [{ pageIndex, pageSize }, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: DEFAULT_PAGE_SIZE,
  });
  const [selectedTypes, setSelectedTypes] = useState<string[]>([]);

  const { dailyBuildApi } = useApi();
  const { data, isLoading } = useQuery({
    queryKey: ['dailyBuildList', { pageSize, pageIndex }, selectedTypes],
    queryFn: () => {
      const buildTypes = selectedTypes as DailyBuildBuildTypeEnum[];
      return dailyBuildApi.dailyBuildList({
        offset: pageIndex,
        size: pageSize,
        buildTypes: buildTypes.length > 0 ? buildTypes : undefined,
      });
    },
  });
  const refetchDailyBuild = () => queryClient.invalidateQueries({ queryKey: ['dailyBuildList'] });
  const files = data?.data?.items ?? [];

  const onSelectChange = useCallback((select: string[]) => {
    setSelectedTypes(select);
    setPagination((state) => ({
      ...state,
      pageIndex: 0,
    }));
  }, []);

  const {
    isOpen: isOpenDeleteAlert,
    onOpen: onOpenDeleteAlert,
    onClose: onCloseDeleteAlert,
  } = useDisclosure();

  const deleteBuildMutation = useMutation({
    mutationFn: (filePath: string) => dailyBuildApi.dailyBuildDelete({ filePath }),
    onSuccess: () => {
      toast({
        status: 'success',
        description: 'Delete item successfully',
      });
      refetchDailyBuild();
    },
    onError: () => {
      toast({
        status: 'error',
        description: 'Delete item failed',
      });
    },
  });

  const handleDelete = useCallback(
    (filePath: string) => {
      deleteBuildMutation.mutate(filePath);
      onCloseDeleteAlert();
    },
    [deleteBuildMutation, onCloseDeleteAlert],
  );

  return (
    <Stack w="full" spacing={6} h="full">
      <Heading>Daily Build</Heading>
      <Select
        placeholder="Build Type"
        isMulti
        isFitted
        options={APK_TYPE_LIST}
        value={selectedTypes}
        onChange={onSelectChange}
      />
      <Stack spacing={4} w="full">
        {isLoading ? (
          <Loading />
        ) : (
          files.map((file) => (
            <Card key={file.url} direction="row" variant="outline" overflow="hidden">
              <CardBody py={{ base: 2, md: 3, '2xl': 4 }} px={{ base: 3, md: 6 }} minW={0} flex="1">
                <Flex
                  gap={{ base: 2, md: 8 }}
                  justify="space-between"
                  align="center"
                  fontSize={{ base: 'sm', md: 'md' }}
                >
                  <Text>{file?.key?.slice(0, 8)}</Text>
                  <Text>{file.date?.toLocaleString()}</Text>
                  <Text>{file.buildType}</Text>
                  <Link href={file.url} isExternal fontWeight="semibold">
                    Download
                  </Link>
                  {state.context.permissions.includes(DailyBuildWritePermission) && (
                    <Button onClick={onOpenDeleteAlert}>Delete</Button>
                  )}
                  <DeleteAlert
                    isOpen={isOpenDeleteAlert}
                    onClose={onCloseDeleteAlert}
                    onDelete={() => handleDelete(file?.filePath ?? '')}
                  />
                </Flex>
              </CardBody>
            </Card>
          ))
        )}
      </Stack>
      <Pagination
        pageIndex={pageIndex}
        pageSize={pageSize}
        pageSizeOptions={DEFAULT_PAGE_SIZE_OPTIONS}
        onPageChange={setPagination}
        totalItems={data?.data?.total ?? 0}
      />
    </Stack>
  );
});

export default DailyBuildPage;
