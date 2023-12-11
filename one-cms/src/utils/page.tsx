import { Namespace } from 'i18next';
import React from 'react';
import { Helmet, HelmetProvider } from 'react-helmet-async';
import { useTranslation } from 'react-i18next';
import { TranslationKey } from '@/i18n/i18n';

export const withTitle = (s: TranslationKey, BaseComp: React.FC) => {
  const WithTitleComp = () => {
    const { t } = useTranslation<Namespace>();
    return (
      <>
        <HelmetProvider>
          <Helmet>
            <title>One CMS | {t(s)}</title>
          </Helmet>
          <BaseComp />
        </HelmetProvider>
      </>
    );
  };

  WithTitleComp.displayName = 'WithTitleComp';
  return WithTitleComp;
};
