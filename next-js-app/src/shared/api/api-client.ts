'use client';

import { useAuth } from '@clerk/nextjs';
import createClient from 'openapi-fetch';
import { useMemo } from 'react';

import { backendBaseUrl } from '@/shared/config';
import type { paths } from './v1';

export const useApiClient = () => {
  const { getToken } = useAuth();

  return useMemo(() => {
    const client = createClient<paths>({
      baseUrl: backendBaseUrl
    });

    client.use({
      onRequest: async ({ request }) => {
        const token = await getToken();
        if (token) {
          console.log(token);
          request.headers.set('Authorization', `Bearer ${token}`);
        }

        if (!request.headers.has('Content-Type')) {
          request.headers.set('Content-Type', 'application/json');
        }

        return request;
      }
    });

    return client;
  }, [getToken]);
};
