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
import { PaginationState } from '@tanstack/react-table';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { FiPlus, FiSearch } from 'react-icons/fi';
import AddSpaceGroupDrawer from './components/AddSpaceGroupDrawer/AddSpaceGroupDrawer';
import { AddSpaceGroupFormValues } from './components/AddSpaceGroupDrawer/AddSpaceGroupDrawer.type';
import EditSpaceGroupDrawer from './components/EditSpaceGroupDrawer/EditSpaceGroupDrawer';
import { EditSpaceGroupFormValues } from './components/EditSpaceGroupDrawer/EditSpaceGroupDrawer.type';
import SpaceGroupCard from './components/SpaceGroupCard/SpaceGroupCard';
import { SpaceGroup } from '@/api/openapi';
import { queryClient } from '@/api/reactQuery';
import DeleteAlert from '@/components/DeleteAlert/DeleteAlert';
import Loading from '@/components/Loading/Loading';
import Pagination from '@/components/Pagination/Pagination';
import {
  DEFAULT_PAGE_SIZE,
  DEFAULT_PAGE_SIZE_OPTIONS,
} from '@/components/Pagination/Pagination.constant';
import { useApi } from '@/hooks/useApi';
import { withTitle } from '@/utils/page';

export const GroupList = withTitle('navigation.space.groupList', () => {
  const { t } = useTranslation('space');
  const toast = useToast();

  const [editingGroup, setEditingGroup] = useState<SpaceGroup>();
  const [deletingGroup, setDeletingGroup] = useState<SpaceGroup>();

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

  const [{ pageIndex, pageSize }, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: DEFAULT_PAGE_SIZE,
  });

  const { spaceGroupApi } = useApi();
  const { data, isLoading } = useQuery({
    queryKey: ['spaceGroups', { pageSize, pageIndex }],
    queryFn: () => spaceGroupApi.getSpaceGroupList({ offset: pageIndex, size: pageSize }),
  });
  const groups = data?.data?.items ?? [];
  const refetchSpaceGroups = () => queryClient.invalidateQueries({ queryKey: ['spaceGroups'] });
  const addMutation = useMutation({
    mutationFn: (data: AddSpaceGroupFormValues) =>
      spaceGroupApi.createSpaceGroup({
        createSpaceGroupRequest: data,
      }),
    onSuccess: () => {
      refetchSpaceGroups();
      onCloseAddDrawer();
      toast({
        status: 'success',
        description: t('spaceGroupList.addGroup.successMessage'),
      });
    },
    onError: () => {
      toast({
        status: 'error',
        description: t('spaceGroupList.addGroup.errorMessage'),
      });
    },
  });
  const editMutation = useMutation({
    mutationFn: (data: EditSpaceGroupFormValues) =>
      spaceGroupApi.updateSpaceGroup({
        spaceGroupId: editingGroup?.spaceGroupId ?? '',
        updateSpaceGroupRequest: data,
      }),
    onSuccess: () => {
      refetchSpaceGroups();
      onCloseEditDrawer();
      toast({
        status: 'success',
        description: t('spaceGroupList.editGroup.successMessage'),
      });
    },
    onError: () => {
      toast({
        status: 'error',
        description: t('spaceGroupList.editGroup.errorMessage'),
      });
    },
  });
  const deleteMutation = useMutation({
    mutationFn: (spaceGroupId: string) => spaceGroupApi.deleteSpaceGroup({ spaceGroupId }),
    onSuccess: () => {
      refetchSpaceGroups();
      onCloseDeleteAlert();
      toast({
        status: 'success',
        description: t('spaceGroupList.deleteGroup.successMessage'),
      });
    },
    onError: () => {
      toast({
        status: 'error',
        description: t('spaceGroupList.deleteGroup.errorMessage'),
      });
    },
  });

  const openEditDrawer = (data: SpaceGroup) => {
    onOpenEditDrawer();
    setEditingGroup(data);
  };
  const openDeleteAlert = (data: SpaceGroup) => {
    onOpenDeleteAlert();
    setDeletingGroup(data);
  };
  const deleteSpaceGroup = () => {
    if (deletingGroup?.spaceGroupId) {
      deleteMutation.mutate(deletingGroup?.spaceGroupId);
    }
  };

  return (
    <Stack w="full" spacing={6} h="full">
      <Heading>{t('spaceGroupList.title')}</Heading>

      <Flex justify="flex-end" gap={4}>
        <InputGroup>
          <InputLeftElement pointerEvents="none">
            <FiSearch />
          </InputLeftElement>
          <Input variant="filled" placeholder={t('spaceGroupList.searchPlaceholder')} />
        </InputGroup>

        <Button variant="primary" hideBelow="md" leftIcon={<FiPlus />} onClick={onOpenAddDrawer}>
          {t('spaceGroupList.addGroup.title')}
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
        <AddSpaceGroupDrawer
          isOpen={isOpenAddDrawer}
          onClose={onCloseAddDrawer}
          onAddSpaceGroup={addMutation.mutate}
        />
      </Flex>

      {isLoading ? (
        <Loading />
      ) : (
        <Stack w="full">
          {groups.map((group) => (
            <SpaceGroupCard
              key={group.spaceGroupId}
              spaceGroup={group}
              onEditSpaceGroup={openEditDrawer}
              onDeleteSpaceGroup={openDeleteAlert}
            />
          ))}
        </Stack>
      )}
      <Pagination
        pageIndex={pageIndex}
        pageSize={pageSize}
        pageSizeOptions={DEFAULT_PAGE_SIZE_OPTIONS}
        onPageChange={setPagination}
        totalItems={data?.data?.total ?? 0}
      />

      <EditSpaceGroupDrawer
        isOpen={isOpenEditDrawer}
        onClose={onCloseEditDrawer}
        spaceGroup={editingGroup}
        onEditSpaceGroup={editMutation.mutate}
      />
      <DeleteAlert
        title="Delete Space"
        description={`Are you sure you want to delete ${deletingGroup?.name}?`}
        isOpen={isOpenDeleteAlert}
        onClose={onCloseDeleteAlert}
        onDelete={deleteSpaceGroup}
      />
    </Stack>
  );
});

export default GroupList;
