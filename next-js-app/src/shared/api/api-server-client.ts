import createClient from 'openapi-fetch';

import { backendBaseUrl } from '@/shared/config';
import { getAccessToken } from '../lib/cookies';
import type { paths } from './v1';

export const getServerApiClient = async () => {
  const accessToken = await getAccessToken();

  return createClient<paths>({
    baseUrl: backendBaseUrl,
    headers: {
      'Content-Type': 'application/json',
      ...(accessToken && { Authorization: `Bearer ${accessToken}` })
    }
  });
};
