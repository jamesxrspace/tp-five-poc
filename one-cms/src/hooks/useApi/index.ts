import { useContext } from 'react';
import { ApiClientContext } from '@/api';

export const useApi = () => {
  return useContext(ApiClientContext);
};
