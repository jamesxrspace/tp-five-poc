import { Namespace, ParseKeys } from 'i18next';
import account from '../../public/locales/en/account.json';
import common from '../../public/locales/en/common.json';
import space from '../../public/locales/en/space.json';
import { DEFAULT_NS } from './i18n.constant';

declare module 'i18next' {
  interface CustomTypeOptions {
    defaultNS: typeof DEFAULT_NS;
    resources: {
      common: typeof common;
      space: typeof space;
      account: typeof account;
    };
  }
}

export type TranslationKey = ParseKeys<Namespace>;
