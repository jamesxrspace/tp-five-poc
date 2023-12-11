import {
  Button,
  Flex,
  Heading,
  IconButton,
  Input,
  InputGroup,
  InputLeftElement,
  Stack,
  useDisclosure,
  useToast,
} from '@chakra-ui/react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { FiPlus, FiSearch } from 'react-icons/fi';
import AddSpaceDrawer from './components/AddSpaceDrawer/AddSpaceDrawer';
import { AddSpaceFormValues } from './components/AddSpaceDrawer/AddSpaceDrawer.type';
import EditSpaceDrawer from './components/EditSpaceDrawer/EditSpaceDrawer';
import { EditSpaceFormValues } from './components/EditSpaceDrawer/EditSpaceDrawer.type';
import SpaceCard from './components/SpaceCard/SpaceCard';
import { Space } from '@/api/openapi';
import { queryClient } from '@/api/reactQuery';
import DeleteAlert from '@/components/DeleteAlert/DeleteAlert';
import Loading from '@/components/Loading/Loading';
import Pagination from '@/components/Pagination/Pagination';
import {
  DEFAULT_PAGE_SIZE,
  DEFAULT_PAGE_SIZE_OPTIONS,
} from '@/components/Pagination/Pagination.constant';
import { PaginationState } from '@/components/Pagination/Pagination.type';
import { useApi } from '@/hooks/useApi';
import { withTitle } from '@/utils/page';

export const SpaceList = withTitle('navigation.space.list', () => {
  const { t } = useTranslation('space');
  const toast = useToast();

  const [editingSpace, setEditingSpace] = useState<Space>();
  const [deletingSpace, setDeletingSpace] = useState<Space>();

  const [{ pageIndex, pageSize }, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: DEFAULT_PAGE_SIZE,
  });
  const { spaceApi } = useApi();
  const { data, isLoading } = useQuery({
    queryKey: ['spaces', { pageSize, pageIndex }],
    queryFn: () => spaceApi.getSpaceList({ offset: pageIndex, size: pageSize }),
  });
  const spaces = data?.data?.items ?? ([] as Space[]);

  const refetchSpaces = () => queryClient.invalidateQueries({ queryKey: ['spaces'] });
  const addMutation = useMutation({
    mutationFn: (data: AddSpaceFormValues) => spaceApi.createSpace({ createSpaceBody: data }),
    onSuccess: () => {
      refetchSpaces();
      onCloseAddDrawer();
      toast({
        status: 'success',
        description: t('spaceList.addSpace.successMessage'),
      });
    },
    onError: () => {
      toast({
        status: 'error',
        description: t('spaceList.addSpace.errorMessage'),
      });
    },
  });
  const editMutation = useMutation({
    mutationFn: (data: EditSpaceFormValues) =>
      spaceApi.updateSpace({ spaceId: editingSpace?.spaceId ?? '', updateSpaceBody: data }),
    onSuccess: () => {
      refetchSpaces();
      onCloseEditDrawer();
      toast({
        status: 'success',
        description: t('spaceList.editSpace.successMessage'),
      });
    },
    onError: () => {
      toast({
        status: 'error',
        description: t('spaceList.editSpace.errorMessage'),
      });
    },
  });
  const deleteMutation = useMutation({
    mutationFn: (spaceId: string) => spaceApi.deleteSpace({ spaceId }),
    onSuccess: () => {
      refetchSpaces();
      onCloseDeleteAlert();
      toast({
        status: 'success',
        description: t('spaceList.deleteSpace.successMessage'),
      });
    },
    onError: () => {
      toast({
        status: 'error',
        description: t('spaceList.deleteSpace.errorMessage'),
      });
    },
  });

  const {
    isOpen: isOpenAddDrawer,
    onOpen: onOpenAddDrawer,
    onClose: onCloseAddDrawer,
  } = useDisclosure();
  const {
    isOpen: isOpenEditDrawer,
    onOpen: onOpenEditDrawer,
    onClose: onCloseEditDrawer,
  } = useDisclosure();
  const {
    isOpen: isOpenDeleteAlert,
    onOpen: onOpenDeleteAlert,
    onClose: onCloseDeleteAlert,
  } = useDisclosure();

  const openEditDrawer = (data: Space) => {
    onOpenEditDrawer();
    setEditingSpace(data);
  };
  const openDeleteAlert = (data: Space) => {
    onOpenDeleteAlert();
    setDeletingSpace(data);
  };
  const deleteSpace = () => {
    if (deletingSpace?.spaceId) {
      deleteMutation.mutate(deletingSpace?.spaceId);
    }
  };

  return (
    <Stack spacing={6} h="full">
      <Heading>{t('spaceList.title')}</Heading>

      <Flex justify="flex-end" gap={4}>
        <InputGroup>
          <InputLeftElement pointerEvents="none">
            <FiSearch />
          </InputLeftElement>
          <Input variant="filled" placeholder={t('spaceList.searchPlaceholder')} />
        </InputGroup>

        <Button variant="primary" leftIcon={<FiPlus />} hideBelow="md" onClick={onOpenAddDrawer}>
          {t('spaceList.addSpace.title')}
        </Button>
        <IconButton
          variant="primary"
          aria-label="add space"
          icon={<FiPlus />}
          hideFrom="md"
          pos="fixed"
          bottom={10}
          right={6}
          zIndex="docked"
          isRound
          onClick={onOpenAddDrawer}
        />

        <AddSpaceDrawer
          isOpen={isOpenAddDrawer}
          onClose={onCloseAddDrawer}
          onAddSpace={addMutation.mutate}
        />
        {editingSpace && (
          <EditSpaceDrawer
            isOpen={isOpenEditDrawer}
            onClose={onCloseEditDrawer}
            space={editingSpace}
            onEditSpace={editMutation.mutate}
          />
        )}
      </Flex>

      {isLoading ? (
        <Loading />
      ) : (
        <Stack w="full">
          {spaces.map((space) => (
            <SpaceCard
              key={space.spaceId}
              space={space}
              onEditSpace={openEditDrawer}
              onDeleteSpace={openDeleteAlert}
            />
          ))}
        </Stack>
      )}
      <Pagination
        pb={8}
        totalItems={data?.data?.total ?? 0}
        pageIndex={pageIndex}
        pageSize={pageSize}
        onPageChange={setPagination}
        pageSizeOptions={DEFAULT_PAGE_SIZE_OPTIONS}
      />

      <DeleteAlert
        title="Delete Space"
        description={`Are you sure you want to delete ${deletingSpace?.name}?`}
        isOpen={isOpenDeleteAlert}
        onClose={onCloseDeleteAlert}
        onDelete={deleteSpace}
      />
    </Stack>
  );
});
