import {
  Select as ChakraReactSelect,
  GroupBase,
  MultiValue as MultiValueType,
  SingleValue,
} from 'chakra-react-select';
import { MultiValue } from './components/MultiValue';
import { getStylesConfig } from './Select.constant';
import { OnChange, Option as OptionType, SelectProps } from './Select.type';
import { getValue } from './Select.util';

const Select = <T extends OptionType, IsMulti extends boolean = false>({
  value: simpleValue,
  defaultValue: simpleDefaultValue,
  options = [],
  isMulti,
  closeMenuOnSelect = !isMulti,
  hideSelectedOptions = false,
  selectedOptionStyle = 'check',
  components,
  isFitted = false,
  getOptionValue = (option: T) => option.value,
  onChange,
  ...rest
}: SelectProps<T, IsMulti, GroupBase<T>>) => {
  return (
    <ChakraReactSelect<T, IsMulti, GroupBase<T>>
      value={getValue(options, simpleValue, getOptionValue, isMulti ?? false)}
      defaultValue={getValue(options, simpleDefaultValue, getOptionValue, isMulti ?? false)}
      options={options}
      isMulti={isMulti}
      closeMenuOnSelect={closeMenuOnSelect}
      selectedOptionStyle={selectedOptionStyle}
      hideSelectedOptions={hideSelectedOptions}
      getOptionValue={getOptionValue}
      {...rest}
      chakraStyles={getStylesConfig<T>(isFitted)}
      components={{
        MultiValue,
        ...components,
      }}
      onChange={(selected, meta) => {
        if (isMulti) {
          const options = selected as MultiValueType<T>;
          const value = options.map(getOptionValue);
          (onChange as OnChange<T, true>)?.(value, options, meta);
        } else {
          const option = selected as SingleValue<T>;
          const value = option ? getOptionValue(option) : null;
          (onChange as OnChange<T, false>)?.(value, option, meta);
        }
      }}
    />
  );
};

export default Select;
