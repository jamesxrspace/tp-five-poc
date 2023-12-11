import { Button, Center, Heading, Text } from '@chakra-ui/react';
import { Link } from 'react-router-dom';
import { withTitle } from '@/utils/page';

const NotFound = withTitle('navigation.common.notFound', () => {
  return (
    <Center flexDirection="column" gap={6} h="100vh">
      <Heading size="3xl">Not Found</Heading>
      <Text>Please check your URL</Text>
      <Link to="/">
        <Button colorScheme="purple">Back to Home</Button>
      </Link>
    </Center>
  );
});

export default NotFound;
