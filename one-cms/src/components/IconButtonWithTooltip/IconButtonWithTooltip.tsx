import { Tooltip, IconButton } from '@chakra-ui/react';
import { IconButtonWithTooltipProps } from './IconButtonWithTooltip.type';

const IconButtonWithTooltip = ({
  label,
  'aria-label': ariaLabel,
  icon,
  onClick,
  isDisabled,
  ...rest
}: IconButtonWithTooltipProps) => (
  <Tooltip label={label}>
    <IconButton
      aria-label={ariaLabel}
      onClick={onClick}
      isDisabled={isDisabled}
      icon={icon}
      {...rest}
    />
  </Tooltip>
);

export default IconButtonWithTooltip;
