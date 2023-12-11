import { Heading } from '@chakra-ui/react';
import { withTitle } from '@/utils/page';

export const DailyDatabase = withTitle('navigation.develop.dailyDatabase', () => {
  return (
    <div>
      <Heading>DailyDatabase</Heading>
    </div>
  );
});

export default DailyDatabase;
