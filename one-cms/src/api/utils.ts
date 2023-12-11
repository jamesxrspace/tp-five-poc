import { ResponseError } from './openapi';
import { toast } from '@/constants/toast';
import i18n from '@/i18n';

export const handleApiError = (error: unknown, onTokenExpired?: (error: unknown) => void) => {
  if (error instanceof ResponseError && error.response.status === 401) {
    if (!toast.isActive('session-expired')) {
      toast({
        id: 'session-expired',
        status: 'error',
        title: i18n.t('toast.sessionExpired.title'),
        description: i18n.t('toast.sessionExpired.description'),
      });
    }

    onTokenExpired?.(error);
  }
  return error;
};
