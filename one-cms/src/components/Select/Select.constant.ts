import { ChakraStylesConfig } from 'chakra-react-select';
import { Option } from './Select.type';

export const DISPLAY_OPTION_AMOUNT = 2;

export const getStylesConfig = <T extends Option>(isFitted: boolean): ChakraStylesConfig<T> => ({
  container: (base) => ({
    ...base,
    maxWidth: isFitted ? 'fit-content' : '100%',
  }),
  option: (base, { isSelected, isFocused }) => ({
    ...base,
    backgroundColor: isFocused ? 'var(--menu-bg)' : isSelected ? 'transparent' : undefined,
    color: 'inherit',
    _active: {
      backgroundColor: 'var(--menu-bg)',
    },
    _focusVisible: {
      backgroundColor: 'var(--menu-bg)',
    },
  }),
  dropdownIndicator: (base, { isDisabled }) => ({
    ...base,
    px: 3,
    color: isDisabled ? 'var(--disabled)' : 'inherit',
    bg: 'transparent',
    cursor: isDisabled ? 'not-allowed' : 'pointer',
  }),
});
