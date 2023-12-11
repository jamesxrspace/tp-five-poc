import { Center, CenterProps, Spinner, useColorModeValue } from '@chakra-ui/react';

const Loading = (props: CenterProps) => {
  const color = useColorModeValue('purple.400', 'white');
  return (
    <Center {...props}>
      <Spinner size="lg" color={color} />
    </Center>
  );
};
export default Loading;
