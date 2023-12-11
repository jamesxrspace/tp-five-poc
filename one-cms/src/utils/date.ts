import { format } from 'date-fns';
import { DateFormat } from '@/constants/date';

export const formatDate = (date: Date | undefined | null, dateFormat = DateFormat.DATE): string =>
  date ? format(date, dateFormat) : '';

export const formatDateTime = (
  date: Date | undefined | null,
  dateFormat = DateFormat.DATE_TIME,
): string => (date ? format(date, dateFormat) : '');
