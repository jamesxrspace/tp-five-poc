import {
  Box,
  CloseButton,
  Collapse,
  Flex,
  Icon,
  Text,
  useColorModeValue,
  useDisclosure,
} from '@chakra-ui/react';
import { Namespace } from 'i18next';
import { useMemo, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { FiChevronDown } from 'react-icons/fi';
import { Link, useLocation } from 'react-router-dom';
import { LinkItemProps, NavItemProps, NestedNavItemProps, SidebarProps } from '../../types/layout';
import { useAuth } from '@/hooks/useAuth';
import { DYNAMIC_ROUTES } from '@/router';
import { CustomRouteObject } from '@/router/type';

const Sidebar = ({ onClose, ...rest }: SidebarProps) => {
  const { t } = useTranslation<Namespace>();

  const backgroundColor = useColorModeValue('white', undefined);
  const borderRightColor = useColorModeValue('gray.200', 'gray.700');

  const [authState] = useAuth();

  const linkItems: LinkItemProps[] = useMemo(() => {
    const isVisibleRoutes = (route: CustomRouteObject) =>
      !route.isHidden && (route.canEnter?.(authState.context) ?? true);
    const transformRouteToLinkItem = (route: CustomRouteObject): LinkItemProps => ({
      name: route.translationKey ? t(route.translationKey) : route.name || '',
      path: route.path ?? '',
      icon: route.icon,
      children: route.children?.filter(isVisibleRoutes).map(transformRouteToLinkItem),
    });

    return DYNAMIC_ROUTES.filter(isVisibleRoutes).map(transformRouteToLinkItem);
  }, [authState.context, t]);

  return (
    <Box
      bgColor={backgroundColor}
      borderRight="1px"
      borderRightColor={borderRightColor}
      w={{ base: 'full', xl: 60 }}
      h="full"
      {...rest}
    >
      <Flex alignItems="center" justifyContent="space-between" p={{ base: 4, md: 6 }}>
        <Link to="/">
          <Text ml={2} fontSize="2xl" fontFamily="helvetica" fontWeight="thin">
            {t('logo')}
          </Text>
        </Link>
        <CloseButton hideFrom="xl" onClick={onClose} />
      </Flex>

      {linkItems.map((item) => {
        return (
          <NestedNavItem
            key={item.name}
            name={item.name}
            icon={item.icon}
            path={item.path}
            routeChildren={item.children}
            onClose={onClose}
          />
        );
      })}
    </Box>
  );
};

export const NestedNavItem = ({ path, name, icon, onClose, routeChildren }: NestedNavItemProps) => {
  const location = useLocation();

  const isActive = useMemo(() => {
    const isChildPathActive = (basePath: string, children?: LinkItemProps[]): boolean => {
      return !!children?.some(
        (child) =>
          `${basePath}/${child.path}` === location.pathname ||
          isChildPathActive(`${basePath}/${child.path}`, child.children),
      );
    };

    return isChildPathActive(path, routeChildren);
  }, [path, routeChildren, location.pathname]);

  const { isOpen, onOpen, onToggle } = useDisclosure({ defaultIsOpen: isActive });
  useEffect(() => {
    if (isActive) {
      onOpen();
    }
  }, [isActive, onOpen]);

  return (
    <Box mx={2}>
      {!routeChildren ? (
        <Box textDecoration="none" _focus={{ boxShadow: 'none' }}>
          <Link to={path} onClick={onClose}>
            <NavItem key={name} icon={icon} isActivePath={path === location.pathname}>
              <Text fontSize="sm" whiteSpace="normal" wordBreak="break-word">
                {name}
              </Text>
            </NavItem>
          </Link>
        </Box>
      ) : (
        <>
          <NavItem key={name} icon={icon} onClick={onToggle} userSelect="none">
            <Flex justifyContent="space-between" alignItems="center" w="full">
              <Text fontSize="sm" whiteSpace="normal" wordBreak="break-word">
                {name}
              </Text>
              <Icon
                as={FiChevronDown}
                transform={isOpen ? 'rotate(180deg)' : 'none'}
                transition="transform 0.3s ease-in-out"
              />
            </Flex>
          </NavItem>

          <Collapse in={isOpen}>
            {routeChildren
              .filter(({ path }) => path)
              .map((subItem) => (
                <NestedNavItem
                  key={subItem.name}
                  name={subItem.name}
                  icon={subItem.icon}
                  path={`${path}/${subItem.path}`}
                  routeChildren={subItem.children}
                  onClose={onClose}
                >
                  {subItem.name}
                </NestedNavItem>
              ))}
          </Collapse>
        </>
      )}
    </Box>
  );
};

const NavItem = ({ icon, children, isActivePath = false, ...rest }: NavItemProps) => {
  const activeColor = useColorModeValue('purple.500', 'purple.300');
  const hoverColor = useColorModeValue('purple.400', 'purple.400');

  return (
    <Flex
      align="center"
      borderRadius="lg"
      px={4}
      py={3}
      color={isActivePath ? activeColor : 'inherit'}
      fontWeight={isActivePath ? 'semibold' : 'inherit'}
      role="group"
      cursor="pointer"
      _hover={{
        color: hoverColor,
      }}
      {...rest}
    >
      <>
        {icon && <Icon mr={2} fontSize={16} _groupHover={{ color: hoverColor }} as={icon} />}
        {children}
      </>
    </Flex>
  );
};

export default Sidebar;
