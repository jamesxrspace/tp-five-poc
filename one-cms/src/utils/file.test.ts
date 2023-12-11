import { formatBytes } from './file';

describe('formatBytes', () => {
  test('0 bytes should be `0 Bytes`', () => {
    const bytes = 0;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('0 Bytes');
  });
  test('500 bytes should be `500 Bytes`', () => {
    const bytes = 500;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('500 Bytes');
  });
  test('1024 bytes should be `1 KB`', () => {
    const bytes = 1024;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('1 KB');
  });
  test('10240 bytes should be `500 KB`', () => {
    const bytes = 1024 * 500;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('500 KB');
  });
  test('1048576 bytes should be `1 MB`', () => {
    const bytes = 1024 * 1024;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('1 MB');
  });
  test('1572864 bytes should be `1.5 MB`', () => {
    const bytes = 1024 * 1024 * 1.5;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('1.5 MB');
  });
  test('10485760 bytes should be `10 MB`', () => {
    const bytes = 1024 * 1024 * 10;
    const formatedBytes = formatBytes(bytes);
    expect(formatedBytes).toBe('10 MB');
  });
});
