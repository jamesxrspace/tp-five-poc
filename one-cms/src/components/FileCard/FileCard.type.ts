import { FlexProps } from '@chakra-ui/react';

export type FileCardProps = FlexProps & {
  file: string | File;
  onRemove?: (file: string | File) => void;
};
