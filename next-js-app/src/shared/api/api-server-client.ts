import { cookies } from 'next/headers';
import createClient from 'openapi-fetch';

import { backendBaseUrl } from '@/shared/config';
import type { paths } from './v1';

export const getServerApiClient = async () => {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  return createClient<paths>({
    baseUrl: backendBaseUrl,
    headers: {
      Authorization: token ? `Bearer ${token}` : '',
      ContentType: 'application/json'
    }
  });
};
