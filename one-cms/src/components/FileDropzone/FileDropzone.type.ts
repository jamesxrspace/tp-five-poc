import { FlexProps } from '@chakra-ui/react';
import { DropzoneOptions } from 'react-dropzone';

export type FileDropzoneProps = FlexProps & {
  isDisabled?: boolean;
  isUploading?: boolean;
  description?: string;
  dropzoneOptions?: DropzoneOptions;
};
