import { auth } from '@clerk/nextjs/server';
import createClient from 'openapi-fetch';

import { backendBaseUrl } from '@/shared/config';
import type { paths } from './v1';

export const getServerClient = async () => {
  const { getToken } = await auth();
  const token = getToken();

  return createClient<paths>({
    baseUrl: backendBaseUrl,
    headers: {
      Authorization: token ? `Bearer ${token}` : '',
      ContentType: 'application/json'
    }
  });
};
