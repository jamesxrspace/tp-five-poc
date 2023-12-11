// jest-dom adds custom jest matchers for asserting on DOM nodes.
// allows you to do things like:
// expect(element).toHaveTextContent(/react/i)
// learn more: https://github.com/testing-library/jest-dom
import { ReactNode } from 'react';
import '@testing-library/jest-dom';

jest.mock('@/i18n', () => ({
  t: (str: string) => str,
  hasLoadedNamespace: () => true,
  loadNamespaces: () => new Promise(() => {}),
}));
jest.mock('react-i18next', () => ({
  useTranslation: () => {
    return {
      t: (str: string) => str,
      i18n: {
        changeLanguage: () => new Promise(() => {}),
      },
    };
  },
  Trans: ({ children }: { children: ReactNode }) => children,
}));
jest.mock('@/i18n', () => ({
  t: (str: string) => str,
  i18n: {
    changeLanguage: () => new Promise(() => {}),
  },
}));

beforeEach(() => {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: jest.fn().mockImplementation((query) => ({
      matches: false,
      media: query,
      onchange: null,
      addListener: jest.fn(),
      removeListener: jest.fn(),
      addEventListener: jest.fn(),
      removeEventListener: jest.fn(),
      dispatchEvent: jest.fn(),
    })),
  });
});
