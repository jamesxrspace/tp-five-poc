import {
  Stack,
  Heading,
  Input,
  Flex,
  InputGroup,
  InputLeftElement,
  useDisclosure,
  Tag,
  Text,
  Icon,
} from '@chakra-ui/react';
import { ColumnDef } from '@tanstack/react-table';
import { useCallback, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { FiSearch, FiEdit } from 'react-icons/fi';
import { RiCheckboxCircleFill, RiQuestionFill } from 'react-icons/ri';
import UserDrawer from './components/UserDrawer/UserDrawer';
import IconButtonWithTooltip from '@/components/IconButtonWithTooltip/IconButtonWithTooltip';
import Select from '@/components/Select/Select';
import { Option } from '@/components/Select/Select.type';
import Table from '@/components/Table/Table';
import { renderInputBarProps } from '@/components/Table/Table.type';
import { AccountUser } from '@/types/account';
import { withTitle } from '@/utils/page';

const MOCK_ROLE_LIST: Option[] = [
  { value: 'developer', label: 'Developer' },
  { value: 'designer', label: 'Designer' },
  { value: 'gm', label: 'GM' },
  { value: 'xrspace', label: 'XRSPACE' },
  { value: 'user', label: 'User' },
];

const MOCK_TABLE_DATA = [
  {
    id: '1',
    username: 'Bret',
    nickname: 'Leanne Graham',
    email: 'Sincere@april.biz',
    isEmailVerified: true,
    roles: [
      {
        value: 'xrspace',
        label: 'XRSPACE',
      },
    ],
  },
  {
    id: '2',
    username: 'Antonette',
    nickname: 'Ervin Howell',
    email: 'Shanna@melissa.tv',
    isEmailVerified: false,
    roles: [
      {
        value: 'gm',
        label: 'GM',
      },
    ],
  },
  {
    id: '3',
    username: 'Samantha',
    nickname: 'Clementine Bauch',
    email: 'Nathan@yesenia.net',
    isEmailVerified: true,
    roles: [
      {
        value: 'xrspace',
        label: 'XRSPACE',
      },
    ],
  },
  {
    id: '4',
    username: 'Karianne',
    nickname: 'Patricia Lebsack',
    email: 'Julianne.OConner@kory.org',
    isEmailVerified: false,
    roles: [
      {
        value: 'developer',
        label: 'Developer',
      },
    ],
  },
  {
    id: '5',
    username: 'Kamren',
    nickname: 'Chelsey Dietrich',
    email: 'Lucio_Hettinger@annie.ca',
    isEmailVerified: true,
    roles: [
      {
        value: 'designer',
        label: 'Designer',
      },
    ],
  },
  {
    id: '6',
    username: 'Leopoldo_Corkery',
    nickname: 'Mrs. Dennis Schulist',
    email: 'Karley_Dach@jasper.info',
    isEmailVerified: false,
    roles: [
      {
        value: 'designer',
        label: 'Designer',
      },
    ],
  },
  {
    id: '7',
    username: 'Elwyn.Skiles',
    nickname: 'Kurtis Weissnat',
    email: 'Telly.Hoeger@billy.biz',
    isEmailVerified: true,
    roles: [
      {
        value: 'designer',
        label: 'Designer',
      },
    ],
  },
  {
    id: '8',
    username: 'Maxime_Nienow',
    nickname: 'Nicholas Runolfsdottir V',
    email: 'Sherwood@rosamond.me',
    isEmailVerified: false,
    roles: [
      {
        value: 'developer',
        label: 'Developer',
      },
    ],
  },
  {
    id: '9',
    username: 'Delphine',
    nickname: 'Glenna Reichert',
    email: 'Chaim_McDermott@dana.io',
    isEmailVerified: true,
    roles: [
      {
        value: 'designer',
        label: 'Designer',
      },
    ],
  },
  {
    id: '10',
    username: 'Moriah.Stanton',
    nickname: 'Clementina DuBuque',
    email: 'Rey.Padberg@karina.biz',
    isEmailVerified: false,
    roles: [
      {
        value: 'gm',
        label: 'GM',
      },
    ],
  },
];

export const UserManagement = withTitle('navigation.account.userManagement', () => {
  const { t } = useTranslation('account');
  const [editData, setEditData] = useState<AccountUser>();
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);
  const roleList = MOCK_ROLE_LIST;
  const tableData: AccountUser[] = MOCK_TABLE_DATA;

  const { isOpen: isOpenDrawer, onOpen: onOpenDrawer, onClose: onCloseDrawer } = useDisclosure();
  const handleOpenDrawer = useCallback(
    (data?: AccountUser) => {
      setEditData(data);
      onOpenDrawer();
    },
    [onOpenDrawer],
  );

  const columns = useMemo<ColumnDef<AccountUser>[]>(
    () => [
      {
        header: 'XR ID',
        accessorKey: 'id',
        enableSorting: false,
      },
      {
        header: t('userManagement.username'),
        accessorKey: 'username',
      },
      {
        header: t('userManagement.nickname'),
        accessorKey: 'nickname',
      },
      {
        header: t('userManagement.email'),
        accessorKey: 'email',
        cell: ({ row }) => (
          <Flex gap={1} alignItems="center">
            <Text>{row.original.email}</Text>
            <Icon
              as={row.original.isEmailVerified ? RiCheckboxCircleFill : RiQuestionFill}
              w={6}
              h={6}
              color={row.original.isEmailVerified ? 'green.700' : 'gray'}
            />
          </Flex>
        ),
      },
      {
        header: t('userManagement.roles'),
        accessorKey: 'roles',
        cell: ({ row }) =>
          row.original.roles.map((i) => (
            <Tag key={i.value} variant="solid" colorScheme="purple">
              {i.label}
            </Tag>
          )),
      },
      {
        header: t('userManagement.actions'),
        accessorKey: '',
        cell: ({ row }) => (
          <IconButtonWithTooltip
            variant="ghost"
            aria-label="edit"
            size="sm"
            onClick={() => handleOpenDrawer(row.original)}
            icon={<FiEdit />}
            label={t('userManagement.edit')}
          />
        ),
      },
    ],
    [t, handleOpenDrawer],
  );

  const renderInputBar = ({
    isLoading = false,
    inputValue,
    onInputChange,
  }: renderInputBarProps) => {
    return (
      <>
        <Flex justify="flex-end" gap={4}>
          <InputGroup>
            <InputLeftElement pointerEvents="none">
              <FiSearch />
            </InputLeftElement>
            <Input
              variant="filled"
              value={inputValue}
              disabled={isLoading}
              onChange={onInputChange}
              placeholder={t('userManagement.search')}
            />
          </InputGroup>
        </Flex>
        <Select
          isMulti
          isFitted
          options={roleList}
          value={selectedRoles}
          onChange={setSelectedRoles}
          placeholder={t('userManagement.roles')}
        />
      </>
    );
  };

  return (
    <Stack h="full" spacing={4}>
      <Heading>{t('userManagement.title')}</Heading>

      <Stack w="full" overflowX="auto" gap={4}>
        <Table
          isPaginated
          isSortable
          isFilterable
          size="sm"
          data={tableData}
          columns={columns}
          renderInputBar={renderInputBar}
        />
      </Stack>

      <UserDrawer
        key={editData ? `drawer-trigger-${editData.id}` : 'drawer-trigger'}
        isOpen={isOpenDrawer}
        onClose={onCloseDrawer}
        data={editData}
        roleList={roleList}
      />
    </Stack>
  );
});

export default UserManagement;
