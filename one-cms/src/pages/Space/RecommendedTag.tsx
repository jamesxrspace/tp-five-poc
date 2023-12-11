import { Heading } from '@chakra-ui/react';
import { withTitle } from '@/utils/page';

export const RecommendedTag = withTitle('navigation.space.recommendedTag', () => {
  return (
    <div>
      <Heading>RecommendedTag</Heading>
    </div>
  );
});

export default RecommendedTag;
