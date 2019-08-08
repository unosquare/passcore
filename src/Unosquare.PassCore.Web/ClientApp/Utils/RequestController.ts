import { createFetchController } from 'uno-react';
import { AdminEndpoints } from './Routes';

export const fetchController = createFetchController({
    headersResolver: (url: string, accessToken: string) => {
        const headers: Headers = new Headers();

        if (url === AdminEndpoints.login) {
            headers.append('Content-Type', 'application/x-www-form-urlencoded');
        }
        else {
            headers.append('Accept', 'application/json');
            headers.append('Content-Type', 'application/json');
            headers.append('Authorization', `Bearer ${accessToken}`);
        }

        return headers;
    },
    responseResolver: async (response: any) => {
        const responseBody = await response.text();
        const responseJson = responseBody ? JSON.parse(responseBody) : {};

        switch (response.status) {
            case 401:
            case 200:
            case 204:
                return responseJson;
            case 400:
            case 500:
                return {
                    error: responseJson.Message,
                };
            case 404:
                return {
                    error: responseJson,
                };
            default:
                return {
                    error: 'Something went wrong, please try again',
                };
        }
    },
});
