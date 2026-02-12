import { isServer, QueryClient } from '@tanstack/react-query';

function makeQueryClient(): QueryClient {
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

let browserQueryClient: QueryClient | undefined;

export function getQueryClient(): QueryClient {
  if (isServer) {
    return makeQueryClient();
  }

  if (!browserQueryClient) {
    browserQueryClient = makeQueryClient();
  }

  return browserQueryClient;
}
