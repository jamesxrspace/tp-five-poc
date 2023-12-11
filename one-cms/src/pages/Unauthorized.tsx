import { Button, Center, Heading, Text } from '@chakra-ui/react';
import { useAuth } from '@/hooks/useAuth';
import { AuthEvent } from '@/machines/auth';
import { withTitle } from '@/utils/page';

const Unauthorized = withTitle('navigation.common.unauthorized', () => {
  const [, send] = useAuth();

  return (
    <Center flexDirection="column" gap={6} h="100vh">
      <Heading size="3xl">Unauthorized</Heading>
      <Text>Please ask administrators for permissions</Text>
      <Button colorScheme="purple" onClick={() => send({ type: AuthEvent.SWITCH_ACCOUNT })}>
        Login with Another Account
      </Button>
    </Center>
  );
});
export default Unauthorized;
