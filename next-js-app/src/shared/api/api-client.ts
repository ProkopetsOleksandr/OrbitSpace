import createClient from 'openapi-fetch';

import type { paths } from './v1';

export const useApiClient = () => {
  return createClient<paths>({
    baseUrl: '/api/proxy'
  });
};
