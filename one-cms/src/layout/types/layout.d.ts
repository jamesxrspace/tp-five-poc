import { BoxProps, FlexProps } from '@chakra-ui/react';
import { IconType } from 'react-icons';

export interface SidebarWithHeaderProps {
  children: React.ReactElement | null;
}

export interface LinkItemProps {
  name: string;
  icon?: IconType;
  path: string;
  children?: LinkItemProps[];
}

export interface NestedNavItemProps extends FlexProps {
  name: string;
  icon?: IconType;
  path: string;
  routeChildren?: LinkItemProps[];
  onClose: () => void;
}

export interface NavItemProps extends FlexProps {
  name?: string;
  icon?: IconType;
  isActivePath?: boolean;
}

export interface NavbarProps extends FlexProps {
  onOpen: () => void;
}

export interface SidebarProps extends BoxProps {
  onClose: () => void;
}
