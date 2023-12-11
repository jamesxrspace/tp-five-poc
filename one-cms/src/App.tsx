import { QueryClientProvider } from '@tanstack/react-query';
import { Suspense } from 'react';
import { RouterProvider } from 'react-router-dom';
import { ApiClientProvider } from './api';
import { queryClient } from './api/reactQuery';
import { router } from './router';
import { GlobalStateProvider } from '@/machines';

function App() {
  return (
    <Suspense fallback={<div></div>}>
      <GlobalStateProvider>
        <QueryClientProvider client={queryClient}>
          <ApiClientProvider>
            <RouterProvider router={router}></RouterProvider>
          </ApiClientProvider>
        </QueryClientProvider>
      </GlobalStateProvider>
    </Suspense>
  );
}

export default App;
