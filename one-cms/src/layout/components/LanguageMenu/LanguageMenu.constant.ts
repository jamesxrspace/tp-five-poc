import { Language } from './LanguageMenu.type';
import { LanguageCode } from '@/i18n/i18n.constant';

export const SUPPRTED_LANGUAGES: Language[] = [
  {
    name: 'English',
    code: LanguageCode.English,
  },
  {
    name: '繁體中文',
    code: LanguageCode.TraditionalChinese,
  },
];
