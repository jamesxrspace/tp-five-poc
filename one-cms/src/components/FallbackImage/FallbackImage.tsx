import { Center, CenterProps, Icon, useColorModeValue } from '@chakra-ui/react';
import { FiImage } from 'react-icons/fi';

const FallbackImage = (props: CenterProps) => {
  const bg = useColorModeValue('gray.200', 'whiteAlpha.300');

  return (
    <Center w="full" h="full" bg={bg} {...props}>
      <Icon as={FiImage} pos="absolute" boxSize={6} color="gray.400" />
    </Center>
  );
};
export default FallbackImage;
