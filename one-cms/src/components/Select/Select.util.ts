import { GetOptionValue, GroupBase, OptionsOrGroups } from 'react-select';
import { Option, SelectProps } from './Select.type';

const flatten = <T extends Option>(arr: OptionsOrGroups<T, GroupBase<T>>): Array<T> => {
  return arr.reduce(
    (acc: Array<T>, val: T | GroupBase<T>) =>
      'options' in val && Array.isArray(val.options)
        ? acc.concat(flatten((val as GroupBase<T>).options))
        : acc.concat(val as T),
    [],
  );
};

export const getValue = <T extends Option>(
  opts: OptionsOrGroups<T, GroupBase<T>>,
  val: SelectProps['value'],
  getOptVal: GetOptionValue<T>,
  isMulti: boolean,
) => {
  if (val === undefined) return undefined;

  const options = flatten<T>(opts);
  const value = isMulti
    ? options.filter((o: T) => (val as string[]).includes(getOptVal(o)))
    : options.find((o: T) => getOptVal(o) === val);

  return value;
};
