import { Heading } from '@chakra-ui/react';
import { withTitle } from '@/utils/page';

export const RoleSettings = withTitle('navigation.account.roleSettings', () => {
  return (
    <div>
      <Heading>RoleSettings</Heading>
    </div>
  );
});

export default RoleSettings;
