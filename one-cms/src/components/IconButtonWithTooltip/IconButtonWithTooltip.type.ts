import { IconButtonProps, TooltipProps } from '@chakra-ui/react';

export type IconButtonWithTooltipProps = IconButtonProps & Pick<TooltipProps, 'label'>;
