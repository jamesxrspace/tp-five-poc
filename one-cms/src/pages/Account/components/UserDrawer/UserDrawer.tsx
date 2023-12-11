import {
  Button,
  Drawer,
  DrawerBody,
  DrawerCloseButton,
  DrawerContent,
  DrawerFooter,
  DrawerHeader,
  DrawerOverlay,
  FormControl,
  FormLabel,
  Input,
  Switch,
  Checkbox,
  Flex,
  SimpleGrid,
  useBreakpointValue,
} from '@chakra-ui/react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { UserDrawerForm, UserDrawerProps } from './UserDrawer.type';
import { Option } from '@/components/Select/Select.type';

const UserDrawer = ({ isOpen, onClose, data, roleList }: UserDrawerProps<Option>) => {
  const { t } = useTranslation('account');

  const { register, handleSubmit } = useForm<UserDrawerForm>({
    defaultValues: {
      id: data?.id,
      username: data?.username,
      nickname: data?.nickname,
      email: data?.email,
      isEmailVerified: data?.isEmailVerified,
      roles: data?.roles,
    },
  });

  const drawerSize = useBreakpointValue({ base: 'full', md: 'lg' });

  const onSubmit = (data: UserDrawerForm) => {
    // ToDo: Adjust based on future requirements
    console.warn('on submit', data);
  };

  return (
    <Drawer size={drawerSize} isOpen={isOpen} placement="right" onClose={onClose}>
      <DrawerOverlay />
      <DrawerContent>
        <DrawerCloseButton />
        <DrawerHeader>{t('userManagement.editUser')}</DrawerHeader>

        <DrawerBody>
          <SimpleGrid columns={{ sm: 1, md: 2 }} spacing={3} mb={3}>
            <FormControl>
              <FormLabel>XR ID</FormLabel>
              <Input disabled placeholder="Type here..." {...register('id')} />
            </FormControl>
            <FormControl>
              <FormLabel>{t('userManagement.email')}</FormLabel>
              <Input disabled placeholder="Type here..." {...register('email')} />
            </FormControl>

            <FormControl>
              <FormLabel>{t('userManagement.username')}</FormLabel>
              <Input placeholder="Type here..." {...register('username')} />
            </FormControl>

            <FormControl>
              <FormLabel>{t('userManagement.nickname')}</FormLabel>
              <Input placeholder="Type here..." {...register('nickname')} />
            </FormControl>
          </SimpleGrid>

          <FormControl mb={3}>
            <FormLabel>{t('userManagement.verifiedEmail')}</FormLabel>
            <Switch {...register('isEmailVerified')} colorScheme="purple" />
          </FormControl>

          <FormControl mb={3}>
            <FormLabel>{t('userManagement.roles')}</FormLabel>
            <Flex wrap="wrap" flexDirection={{ base: 'column', md: 'row' }}>
              {roleList.map((role, index) => (
                <Checkbox
                  size="lg"
                  key={index}
                  mr={{ base: 0, md: 3 }}
                  my={{ base: 3, md: 0 }}
                  value={role.value}
                  flexGrow={1}
                  colorScheme="purple"
                  defaultChecked={data?.roles?.some((i) => i.value === role.value)}
                  {...register('roles')}
                >
                  {role.label}
                </Checkbox>
              ))}
            </Flex>
          </FormControl>
        </DrawerBody>

        <DrawerFooter justifyContent={{ base: 'center', md: 'flex-end' }}>
          <Button variant="outline" mr={3} w={{ base: 32, md: 24 }} onClick={onClose}>
            {t('userManagement.cancel')}
          </Button>
          <Button variant="primary" w={{ base: 32, md: 24 }} onClick={handleSubmit(onSubmit)}>
            {t('userManagement.submit')}
          </Button>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  );
};
export default UserDrawer;
