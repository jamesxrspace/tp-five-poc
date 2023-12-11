import { MutationCache, QueryCache, QueryClient } from '@tanstack/react-query';
import { handleApiError } from './utils';
import { AuthEvent, authService } from '@/machines/auth';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
  queryCache: new QueryCache({
    onError: (error) => handleApiError(error, () => authService.send(AuthEvent.LOGOUT)),
  }),
  mutationCache: new MutationCache({
    onError: (error) => handleApiError(error, () => authService.send(AuthEvent.LOGOUT)),
  }),
});
