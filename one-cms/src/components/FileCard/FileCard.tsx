import { Flex, Icon, IconButton, Image, Link, Skeleton, Text } from '@chakra-ui/react';
import { useQuery } from '@tanstack/react-query';
import { useMemo } from 'react';
import { FiTrash2 } from 'react-icons/fi';
import { RiFile2Line } from 'react-icons/ri';
import { FileCardProps } from './FileCard.type';
import { isImageFile, isImageUrl } from '@/utils/file';

export const FileCard = ({ file, onRemove, ...props }: FileCardProps) => {
  const isUrl = typeof file === 'string';

  const { data: isImage, isLoading } = useQuery({
    queryKey: ['file', file],
    queryFn: () => {
      if (typeof file === 'string') {
        return isImageUrl(file);
      }
      return isImageFile(file);
    },
    initialData: false,
    retry: false,
  });

  const imgSrc = useMemo(
    () => (isImage ? (isUrl ? file : URL.createObjectURL(file)) : undefined),
    [file, isImage, isUrl],
  );

  return (
    <Flex
      gap="12px"
      align="center"
      border="1px solid"
      borderColor="gray.400"
      borderRadius="md"
      px={4}
      py={2}
      {...props}
    >
      {isImage ? (
        <Image src={imgSrc} boxSize={10} objectFit="cover" />
      ) : (
        <Icon as={RiFile2Line} fontSize="24px" />
      )}
      <Skeleton flex="1" minW={0} isTruncated isLoaded={!isLoading}>
        {isUrl ? (
          <Link size="preTitle" href={file} isExternal isTruncated>
            {file.split('/').pop() || file}
          </Link>
        ) : (
          <Text size="preTitle" isTruncated>
            {file.name}
          </Text>
        )}
      </Skeleton>
      <IconButton
        variant="ghost"
        size="sm"
        icon={<FiTrash2 />}
        aria-label="remove file"
        onClick={() => onRemove?.(file)}
      />
    </Flex>
  );
};
