import {
  Box,
  ButtonGroup,
  Card,
  CardBody,
  Flex,
  Image,
  Stack,
  Tag,
  Text,
  useBreakpointValue,
} from '@chakra-ui/react';
import * as R from 'ramda';
import { Fragment } from 'react';
import { useTranslation } from 'react-i18next';
import { FiEdit, FiTrash2 } from 'react-icons/fi';
import { RiCalendar2Line } from 'react-icons/ri';
import { SpaceGroupCardProps } from './SpaceGroupCard.type';
import FallbackImage from '@/components/FallbackImage/FallbackImage';
import IconButtonWithTooltip from '@/components/IconButtonWithTooltip/IconButtonWithTooltip';
import { formatDate } from '@/utils/date';

const SpaceGroupCard = ({
  spaceGroup,
  onEditSpaceGroup,
  onDeleteSpaceGroup,
}: SpaceGroupCardProps) => {
  const { t } = useTranslation('space');

  const displayTagCount = useBreakpointValue({ lg: 2, '2xl': 4 }) || 2;

  const spaces = spaceGroup.spaces || [];

  return (
    <Card
      key={spaceGroup.spaceGroupId}
      direction="row"
      variant="outline"
      overflow="hidden"
      h={{ base: 'auto', md: 20 }}
    >
      <Box hideBelow="md" w={20}>
        <Image src={spaceGroup.thumbnail} fallback={<FallbackImage />} h="full" objectFit="cover" />
      </Box>

      <CardBody p={{ base: 3, '2xl': 4 }} flex="1">
        <Flex gap={{ base: 2, md: 8 }} justify="space-between" align="center" h="full">
          <Stack spacing={{ base: 1, md: 2 }}>
            <Text
              title={spaceGroup.name}
              fontSize={{ base: 'sm', md: 'md' }}
              fontWeight="semibold"
              isTruncated
            >
              {spaceGroup.name}
            </Text>

            {(spaceGroup.startAt || spaceGroup.endAt) && (
              <Flex gap={2} color="GrayText">
                <RiCalendar2Line />
                <Text fontSize="xs" color="GrayText">
                  {`${formatDate(spaceGroup.startAt)} - ${formatDate(spaceGroup.endAt)}`}
                </Text>
              </Flex>
            )}
          </Stack>

          <Flex
            gap={2}
            justify="flex-end"
            align="center"
            flex="1"
            wrap="wrap"
            hideBelow="lg"
            minW={0}
            w={72}
          >
            {R.take(displayTagCount, spaces).map((space, index) => (
              <Fragment key={space.spaceId}>
                <Tag size="sm" variant="outline" title={space.name} maxW={24}>
                  <Text isTruncated>{space.name}</Text>
                </Tag>

                {spaces.length > displayTagCount && index === displayTagCount - 1 && (
                  <Tag size="sm" variant="outline">
                    {`+${spaces.length - displayTagCount}`}
                  </Tag>
                )}
              </Fragment>
            ))}
          </Flex>

          <ButtonGroup spacing={{ base: 0, md: 2 }}>
            <IconButtonWithTooltip
              variant="ghost"
              aria-label="edit"
              icon={<FiEdit />}
              label={t('spaceGroupList.action.edit')}
              onClick={() => onEditSpaceGroup(spaceGroup)}
            />
            <IconButtonWithTooltip
              variant="ghost"
              aria-label="edit"
              icon={<FiTrash2 />}
              label={t('spaceGroupList.action.delete')}
              onClick={() => onDeleteSpaceGroup(spaceGroup)}
            />
          </ButtonGroup>
        </Flex>
      </CardBody>
    </Card>
  );
};
export default SpaceGroupCard;
