import { QueryClient } from '@tanstack/react-query';

export function getQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 1000 * 60 * 5, // 5 min
        gcTime: 1000 * 60 * 5,
        retry: 3,
        refetchOnWindowFocus: false,
        refetchOnReconnect: true
      }
    }
  });
}
