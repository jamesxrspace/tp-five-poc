import { defineStyleConfig } from '@chakra-ui/react';

const Drawer = defineStyleConfig({
  baseStyle: {
    header: {
      px: 6,
      py: 4,
    },
    closeButton: {
      top: 4,
      right: 6,
    },
  },
});
export default Drawer;
