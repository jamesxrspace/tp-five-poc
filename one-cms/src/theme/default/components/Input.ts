import { defineStyleConfig } from '@chakra-ui/react';

const Input = defineStyleConfig({
  variants: {
    outline: {
      field: {
        _focusVisible: {
          borderColor: 'purple.400',
          boxShadow: 'none',
          _invalid: {
            borderColor: 'red.400',
          },
        },
      },
    },
    filled: {
      field: {
        _focusVisible: {
          borderColor: 'purple.400',
          boxShadow: 'none',
        },
      },
    },
  },
});

export default Input;
