import type { paths } from '@/types/api-types';
import createClient from 'openapi-fetch';
import { useMemo } from "react";
import { useAuth } from '@clerk/nextjs';

export const useApiClient = () => {
    const { getToken } = useAuth();

    return useMemo(() => {
        const client = createClient<paths>({
            baseUrl: process.env.NEXT_PUBLIC_API_URL,
        });

        client.use({
            onRequest: async ({ request }) => {
                const token = await getToken();
                if (token) {
                    request.headers.set('Authorization', `Bearer ${token}`);
                }

                if (!request.headers.has("Content-Type")) {
                    request.headers.set("Content-Type", "application/json");
                }

                return request;
            },
        });

        return client;
    }, [getToken]);
};