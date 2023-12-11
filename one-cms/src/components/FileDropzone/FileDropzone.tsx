import {
  Center,
  FormControl,
  FormErrorMessage,
  Icon,
  Spinner,
  Text,
  useColorModeValue,
} from '@chakra-ui/react';
import * as R from 'ramda';
import { useCallback, useState } from 'react';
import { DropEvent, FileRejection, useDropzone } from 'react-dropzone';
import { useTranslation } from 'react-i18next';
import { RiUpload2Line } from 'react-icons/ri';
import { FileDropzoneProps } from './FileDropzone.type';
import { formatBytes } from '@/utils/file';

export const FileDropzone = ({
  isDisabled = false,
  isUploading = false,
  description,
  dropzoneOptions,
  ...props
}: FileDropzoneProps) => {
  const { t } = useTranslation();
  const hoverBg = useColorModeValue('blackAlpha.100', 'whiteAlpha.100');

  const { accept = {}, maxSize = 2097152, maxFiles, onDrop } = dropzoneOptions || {};

  const handleDrop = useCallback(
    (files: File[], fileRejections: FileRejection[], e: DropEvent) => {
      setIsInvalid(fileRejections.length > 0);

      onDrop?.(files, fileRejections, e);
    },
    [onDrop],
  );
  const handleDropRejected = () => setIsInvalid(true);

  const { getRootProps, getInputProps, isDragReject, isDragAccept, fileRejections } = useDropzone({
    ...dropzoneOptions,
    disabled: isDisabled,
    onDrop: handleDrop,
    onDropRejected: handleDropRejected,
  });

  const [isInvalid, setIsInvalid] = useState(false);
  const errorMessage = R.pathOr('', [0, 'errors', 0, 'message'], fileRejections);

  return (
    <FormControl isInvalid={isInvalid}>
      <Center
        flexDir="column"
        border="1px dashed"
        borderColor="gray.400"
        borderRadius="md"
        p={4}
        w="full"
        bgColor={isDragAccept ? 'primaryAlpha.300' : undefined}
        _hover={{
          bgColor: isDragAccept
            ? 'primaryAlpha.300'
            : isDragReject || isDisabled
            ? undefined
            : hoverBg,
        }}
        cursor={isDragReject || isDisabled ? 'not-allowed' : 'pointer'}
        {...props}
        {...getRootProps()}
      >
        {isUploading ? <Spinner /> : <Icon as={RiUpload2Line} mb="12px" fontSize="24px" />}
        <input {...getInputProps()} multiple={!maxFiles || maxFiles > 1} />
        <Text size="body" fontWeight="medium" mb="8px">
          {description || t('upload.dropzone.description')}
        </Text>
        <Text size="preTitle" fontWeight="medium">
          <Text as="span" textTransform="uppercase" color="inherit">
            {R.values(accept).flat().join(', ')}
          </Text>
          {!R.isNil(maxSize) && (
            <Text as="span" color="inherit">{` (max. ${formatBytes(maxSize)})`}</Text>
          )}
        </Text>
      </Center>

      {errorMessage && <FormErrorMessage mt={2}>{errorMessage}</FormErrorMessage>}
    </FormControl>
  );
};
