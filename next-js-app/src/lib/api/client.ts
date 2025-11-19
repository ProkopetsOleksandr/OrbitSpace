import type { paths } from '@/types/api-types';
import { useAuth } from '@clerk/nextjs';
import createClient, { type Client } from 'openapi-fetch';
import { useMemo } from 'react';

export const apiClient = createClient<paths>({
  baseUrl: process.env.NEXT_PUBLIC_API_URL
});

export type ApiClient = Client<paths>;

export const useApiClient = () => {
  const { getToken } = useAuth();

  const client = useMemo(() => {
    const client = createClient<paths>({
      baseUrl: process.env.NEXT_PUBLIC_API_URL,
    });

    client.use({
      onRequest: async ({ request }) => {
        const token = await getToken();

        console.log(token);

        if (token) {
          request.headers.set('Authorization', `Bearer ${token}`);
        }
        return request;
      },
    });

    return client;
  }, [getToken]);

  return client;
};
