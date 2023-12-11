import { Props as ChakraReactSelectProps, OptionBase } from 'chakra-react-select';
import type { ActionMeta, GroupBase, OnChangeValue as ReactOnChangeValue } from 'react-select';

export interface Option extends OptionBase {
  label: string;
  value: string;
}

type OptionValue<T extends Option, IsMulti extends boolean = false> = IsMulti extends true
  ? Array<T['value']>
  : T['value'] | null;
export type OnChange<T extends Option, IsMulti extends boolean = false> = (
  value: OptionValue<T, IsMulti>,
  valueAsOption: ReactOnChangeValue<T, IsMulti>,
  actionMeta: ActionMeta<Option>,
) => void;

export type SelectProps<
  T extends Option = Option,
  IsMulti extends boolean = false,
  Group extends GroupBase<T> = GroupBase<T>,
> = Omit<ChakraReactSelectProps<T, IsMulti, Group>, 'value' | 'defaultValue' | 'onChange'> & {
  value?: null | string | string[];
  defaultValue?: null | string | string[];
  onChange: OnChange<T, IsMulti>;
  isFitted?: boolean;
};
