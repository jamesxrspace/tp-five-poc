import { extendTheme, type ThemeConfig } from '@chakra-ui/react';
import { borders } from './borders';
import { breakpoints } from './breakpoints';
import { colors } from './colors';
import components from './components';
import { layerStyles } from './layerStyles';
import { textStyles } from './textStyles';

const themeConfig: ThemeConfig = {
  initialColorMode: 'dark',
  useSystemColorMode: false,
};

export const DEFAULT_THEME = extendTheme({
  styles: {
    global: {
      body: {
        color: 'gray.700',
        _dark: {
          color: 'whiteAlpha.900',
        },
      },
    },
  },
  borders,
  breakpoints,
  colors,
  config: themeConfig,
  components,
  layerStyles,
  textStyles,
});
