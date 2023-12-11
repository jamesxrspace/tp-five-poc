import i18n from 'i18next';

import LanguageDetector from 'i18next-browser-languagedetector';
import Backend from 'i18next-http-backend';
import { initReactI18next } from 'react-i18next';
import { DEFAULT_NS, LanguageCode } from './i18n.constant';

i18n.use(Backend).use(LanguageDetector).use(initReactI18next).init({
  fallbackLng: LanguageCode.English,
  defaultNS: DEFAULT_NS,
});

export default i18n;
