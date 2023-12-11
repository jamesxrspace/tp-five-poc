import userEvent from '@testing-library/user-event';
import { uniq } from 'ramda';
import { GroupBase } from 'react-select';
import ChakraReactSelect from './Select';
import { Option } from '@/components/Select/Select.type';
import { act, render, screen, waitFor } from '@/test/utils';

describe('Select', () => {
  const mockOptions: Option[] = [
    { label: 'Option 1', value: '1' },
    { label: 'Option 2', value: '2' },
    { label: 'Option 3', value: '3' },
    { label: 'Option 4', value: '4' },
    { label: 'Option 5', value: '5' },
  ];
  const mockGroupOptions: GroupBase<Option>[] = [
    {
      label: 'First Group',
      options: [
        { label: 'Option 1', value: '1' },
        { label: 'Option 2', value: '2' },
        { label: 'Option 3', value: '3' },
      ],
    },
    {
      label: 'Second Group',
      options: [
        { label: 'Option 4', value: '4' },
        { label: 'Option 5', value: '5' },
      ],
    },
  ];

  beforeEach(() => {
    Object.defineProperty(window, 'matchMedia', {
      writable: true,
      value: jest.fn().mockImplementation((query) => ({
        matches: false,
        media: query,
        onchange: null,
        addEventListener: jest.fn(),
        removeEventListener: jest.fn(),
      })),
    });
  });

  describe('single select', () => {
    let mockValue: string | null;
    let onChangeMock: (newValue: string | null) => void;

    beforeEach(() => {
      onChangeMock = jest.fn((newValue: string | null) => {
        mockValue = newValue;
      });
    });

    test('select a value', async () => {
      const { container } = render(
        <ChakraReactSelect
          instanceId="mock-chakra-react-select"
          classNamePrefix="react-select"
          options={mockOptions}
          value={mockValue}
          onChange={onChangeMock}
        />,
      );

      act(() =>
        userEvent.click(container.getElementsByClassName('react-select__dropdown-indicator')[0]),
      );
      await waitFor(() => userEvent.click(screen.getByText('Option 1')));

      expect(onChangeMock).toHaveBeenCalledTimes(1);
      expect(mockValue).toEqual('1');
    });
  });

  describe('multiple select', () => {
    let mockValues: string[] | null;
    let onChangeMock: (newValue: string[]) => void;

    beforeEach(() => {
      mockValues = [];

      onChangeMock = jest.fn((newValue: string[]) => {
        if (!newValue) {
          mockValues = null;
        } else {
          mockValues = uniq((mockValues || []).concat(newValue));
        }
      });
    });

    test('select multiple values', async () => {
      const { container } = render(
        <ChakraReactSelect
          instanceId="mock-chakra-react-select"
          classNamePrefix="react-select"
          isMulti
          options={mockOptions}
          value={mockValues}
          onChange={onChangeMock}
        />,
      );

      act(() =>
        userEvent.click(container.getElementsByClassName('react-select__dropdown-indicator')[0]),
      );
      await waitFor(() => userEvent.click(screen.getByText('Option 1')));
      act(() =>
        userEvent.click(container.getElementsByClassName('react-select__dropdown-indicator')[0]),
      );
      await waitFor(() => userEvent.click(screen.getByText('Option 2')));

      expect(onChangeMock).toHaveBeenCalledTimes(2);
      expect(mockValues).toEqual(['1', '2']);
    });
  });

  describe('group select', () => {
    let mockValue: string | null;
    let onChangeMock: (newValue: string | null) => void;

    beforeEach(() => {
      onChangeMock = jest.fn((newValue: string | null) => {
        mockValue = newValue;
      });
    });

    test('select a value', async () => {
      const { container } = render(
        <ChakraReactSelect
          instanceId="mock-chakra-react-select"
          classNamePrefix="react-select"
          options={mockGroupOptions}
          value={mockValue}
          onChange={onChangeMock}
        />,
      );

      act(() =>
        userEvent.click(container.getElementsByClassName('react-select__dropdown-indicator')[0]),
      );
      await waitFor(() => userEvent.click(screen.getByText('Option 3')));

      expect(onChangeMock).toHaveBeenCalledTimes(1);
      expect(mockValue).toEqual('3');
    });
  });
});
