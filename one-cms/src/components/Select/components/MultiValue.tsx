import { Text } from '@chakra-ui/react';
import { GroupBase, MultiValueProps } from 'react-select';
import { DISPLAY_OPTION_AMOUNT } from '../Select.constant';
import { Option } from '../Select.type';

export const MultiValue = <T extends Option, IsMulti extends boolean = false>({
  children,
  ...props
}: MultiValueProps<T, IsMulti, GroupBase<T>>) => {
  const { selectProps, index } = props;
  const selectedValues = selectProps.value as T[];

  return (
    <Text isTruncated>
      {selectedValues.length <= DISPLAY_OPTION_AMOUNT && `${index > 0 ? ', ' : ''}${children}`}
      {index === DISPLAY_OPTION_AMOUNT &&
        selectedValues.length > DISPLAY_OPTION_AMOUNT &&
        `${selectedValues.length} selected`}
    </Text>
  );
};
