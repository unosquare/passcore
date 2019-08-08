import * as React from 'react';
import { fetchController } from '../Utils/RequestController';
import { GlobalActionsContext } from './GlobalContext';

const enum RequestMethod {
    Delete = 'DELETE',
    Get = 'GET',
    Post = 'POST',
    Put = 'PUT',
}

const fetchRequest = async (
    url: string,
    method = RequestMethod.Get,
    accessToken = '',
    body?: any,
) => {
    try {
        const data = await fetchController(
            url,
            accessToken,
            method,
            body);

        if (!data || data.error) {
            return null;
        }

        return data;
    } catch (e) {

    }
};

export const GlobalActionsProvider: React.FunctionComponent<any> = ({
    children,
}) => {

    const provActions = {
        // here should go all the actions.
        changePassword: async (data: any) => {
            const response = await fetchRequest('api/password', RequestMethod.Post,
                JSON.stringify({
                    ...data,
                }));

            //ToDo: use response to see the possible error messages.
        },
    };

    return (
        <GlobalActionsContext.Provider value={provActions}>
            {children}
        </GlobalActionsContext.Provider>
    );
};
