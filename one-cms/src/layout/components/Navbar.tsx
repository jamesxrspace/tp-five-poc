import { Flex, HStack, IconButton, Show, useColorMode, useColorModeValue } from '@chakra-ui/react';
import { FiMenu, FiMoon, FiSun } from 'react-icons/fi';
import { NavbarProps } from '../types/layout';
import LanguageMenu from './LanguageMenu/LanguageMenu';
import UserMenu from './UserMenu/UserMenu';

const Navbar = ({ onOpen, ...rest }: NavbarProps) => {
  const { colorMode, toggleColorMode } = useColorMode();
  const backgroundColor = useColorModeValue('white', undefined);
  const borderBottomColor = useColorModeValue('gray.100', 'gray.700');

  return (
    <Flex
      px={{ base: 4, md: 6 }}
      py={{ base: 0, md: 1 }}
      height={16}
      alignItems="center"
      bgColor={backgroundColor}
      borderBottomWidth="1px"
      borderBottomColor={borderBottomColor}
      justifyContent="space-between"
      {...rest}
    >
      <Flex gap={{ base: 2, md: 4 }} align="center">
        <IconButton
          hideFrom="xl"
          onClick={onOpen}
          variant="ghost"
          size={{ base: 'sm', md: 'xl' }}
          aria-label="open menu"
          icon={<FiMenu />}
        />
      </Flex>

      <HStack spacing={{ base: 2, md: 3 }}>
        <Show above="xl">
          <LanguageMenu />
          <IconButton
            variant="ghost"
            aria-label="open menu"
            onClick={toggleColorMode}
            icon={colorMode === 'light' ? <FiSun /> : <FiMoon />}
          />
        </Show>
        <UserMenu />
      </HStack>
    </Flex>
  );
};

export default Navbar;
