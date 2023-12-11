import { useDebounce } from './index';
import { act, renderHook, waitFor } from '@/test/utils';

describe('useDebounce hook', () => {
  it('should return debounced value after specified delay', async () => {
    const { result, rerender } = renderHook(({ value, delay }) => useDebounce(value, delay), {
      initialProps: {
        value: 'initial',
        delay: 500,
      },
    });

    expect(result.current).toBe('initial');

    act(() => {
      rerender({ value: 'updated', delay: 500 });
    });

    await waitFor(() => expect(result.current).toBe('updated'));
  });
});
