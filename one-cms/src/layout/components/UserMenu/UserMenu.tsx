import {
  Avatar,
  Flex,
  Menu,
  MenuButton,
  MenuDivider,
  MenuItem,
  MenuList,
  Text,
  Badge,
  Stack,
} from '@chakra-ui/react';
import { useTranslation } from 'react-i18next';
import { useAuth } from '@/hooks/useAuth';
import { AuthEvent } from '@/machines/auth';

const UserMenu = () => {
  const { t } = useTranslation();
  const [authState, authSend] = useAuth();
  const { profile, roles } = authState.context;

  return (
    <Menu>
      <MenuButton py={2} transition="all 0.3s" _focus={{ boxShadow: 'none' }}>
        <Avatar size="sm" name={profile.username} />
      </MenuButton>
      <MenuList maxW={200}>
        <Stack px={3}>
          <Text fontSize="sm" fontWeight="bold" my={1}>
            {t('userMenu.account')}
          </Text>
          <Text fontSize="sm">{profile.username}</Text>
          <Text fontSize="xs" color="gray.400">
            {profile.email}
          </Text>
          <Flex wrap="wrap" gap={2} my={1} data-testid="role-tags">
            {roles?.map((role) => (
              <Badge key={role.id} variant="subtle" colorScheme="purple" size="sm">
                {role.name}
              </Badge>
            ))}
          </Flex>
        </Stack>
        <MenuDivider />
        <MenuItem onClick={() => authSend({ type: AuthEvent.LOGOUT })}>
          {t('userMenu.logout')}
        </MenuItem>
      </MenuList>
    </Menu>
  );
};

export default UserMenu;
