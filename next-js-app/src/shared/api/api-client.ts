import createClient from 'openapi-fetch';

import type { paths } from './v1';

export const getApiClient = () => {
  return createClient<paths>({
    baseUrl: '/api/proxy'
  });
};
