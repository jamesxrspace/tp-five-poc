import {
  Box,
  ButtonGroup,
  Card,
  CardBody,
  Flex,
  Image,
  Stack,
  Tag,
  TagLabel,
  TagLeftIcon,
  Text,
} from '@chakra-ui/react';
import { useTranslation } from 'react-i18next';
import { FaUnity } from 'react-icons/fa6';
import { FiEdit, FiTrash2 } from 'react-icons/fi';
import { RiCalendar2Line } from 'react-icons/ri';
import { SpaceCardProps } from './SpaceCard.type';
import FallbackImage from '@/components/FallbackImage/FallbackImage';
import IconButtonWithTooltip from '@/components/IconButtonWithTooltip/IconButtonWithTooltip';
import { formatDate } from '@/utils/date';

const SpaceCard = ({ space, onEditSpace, onDeleteSpace }: SpaceCardProps) => {
  const { t } = useTranslation('space');
  return (
    <Card
      key={space.spaceId}
      direction="row"
      variant="outline"
      overflow="hidden"
      h={{ base: 'auto', md: 20 }}
    >
      <Box hideBelow="md" w={20}>
        <Image src={space.thumbnail} fallback={<FallbackImage />} objectFit="cover" h="full" />
      </Box>

      <CardBody p={{ base: 3, '2xl': 4 }} flex="1">
        <Flex gap={{ base: 2, md: 8 }} justify="space-between" align="center" h="full">
          <Stack spacing={{ base: 1, md: 2 }} flex="1">
            <Text
              title={space.name}
              fontSize={{ base: 'sm', md: 'md' }}
              fontWeight="semibold"
              isTruncated
            >
              {space.name}
            </Text>

            {(space.startAt || space.endAt) && (
              <Flex gap={2} color="GrayText">
                <RiCalendar2Line />
                <Text fontSize="xs" isTruncated>
                  {`${formatDate(space.startAt)} - ${formatDate(space.endAt)}`}
                </Text>
              </Flex>
            )}
          </Stack>

          {space.addressable && (
            <Tag variant="outline" size="sm" hideBelow="lg">
              <TagLeftIcon as={FaUnity} />
              <TagLabel>{space.addressable}</TagLabel>
            </Tag>
          )}

          <ButtonGroup spacing={{ base: 0, md: 2 }}>
            <IconButtonWithTooltip
              variant="ghost"
              aria-label="edit"
              icon={<FiEdit />}
              label={t('spaceList.action.edit')}
              onClick={() => onEditSpace(space)}
            />

            <IconButtonWithTooltip
              variant="ghost"
              aria-label="edit"
              icon={<FiTrash2 />}
              label={t('spaceList.action.delete')}
              onClick={() => onDeleteSpace(space)}
            />
          </ButtonGroup>
        </Flex>
      </CardBody>
    </Card>
  );
};
export default SpaceCard;
