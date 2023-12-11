import { defineStyleConfig } from '@chakra-ui/react';

const Button = defineStyleConfig({
  variants: {
    primary: {
      bg: 'purple.500',
      color: 'white',
    },
    secondary: {
      borderWidth: 1,
      borderColor: 'purple.500',
      color: 'purple.500',
      _hover: {
        borderColor: 'purple.400',
        color: 'purple.400',
      },
      _dark: {
        borderColor: 'purple.300',
        color: 'purple.300',
        _hover: {
          borderColor: 'purple.200',
          color: 'purple.200',
        },
      },
    },
    tertiary: {
      color: 'purple.600',
      _hover: {
        textDecor: 'underline',
      },
      _dark: {
        color: 'purple.400',
      },
    },
  },
});

export default Button;
