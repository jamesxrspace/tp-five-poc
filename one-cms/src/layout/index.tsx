import {
  Drawer,
  DrawerContent,
  useDisclosure,
  useColorModeValue,
  useBreakpointValue,
  Flex,
  Show,
  Box,
} from '@chakra-ui/react';
import { useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import Navbar from './components/Navbar';
import Sidebar from './components/Sidebar/Sidebar';

export function Layout({ children }: { children?: React.ReactNode }) {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const isSmallDevice = useBreakpointValue({ base: true, sm: false });
  const backgroundColor = useColorModeValue('gray.50', undefined);

  useEffect(() => {
    if (isSmallDevice) {
      onClose();
    }
  }, [isSmallDevice, onClose]);

  return (
    <>
      <Flex h="100vh" bgColor={backgroundColor}>
        <Sidebar hideBelow="xl" onClose={onClose} />
        <Flex direction="column" flex="1" w="full">
          <Navbar onOpen={onOpen} />
          <Box px={{ base: 5, md: 8 }} py={6} flex="1" overflowY="auto">
            <Box>{children ?? <Outlet />}</Box>
          </Box>
        </Flex>
      </Flex>
      <Show below="xl">
        <Drawer
          isOpen={isOpen}
          placement="left"
          onClose={onClose}
          returnFocusOnClose={false}
          onOverlayClick={onClose}
          size="full"
        >
          <DrawerContent>
            <Sidebar onClose={onClose} />
          </DrawerContent>
        </Drawer>
      </Show>
    </>
  );
}

export default Layout;
