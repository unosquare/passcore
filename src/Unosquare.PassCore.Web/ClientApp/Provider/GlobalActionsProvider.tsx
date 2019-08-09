import * as React from 'react';
import { fetchRequest } from '../Utils/FetchRequest';
import { GlobalActionsContext, GlobalContext, SnackbarContext } from './GlobalContext';

const enum RequestMethod {
    Delete = 'DELETE',
    Get = 'GET',
    Post = 'POST',
    Put = 'PUT',
}

export const GlobalActionsProvider: React.FunctionComponent<any> = ({
    children,
}) => {

    const { alerts } = React.useContext(GlobalContext);
    const { sendMessage } = React.useContext(SnackbarContext);

    const provActions = {
        // here should go all the actions.
        changePassword: async (data: any) => {
            const response = await fetchRequest(
                'api/password',
                RequestMethod.Post,
                JSON.stringify({
                    ...data,
                }));

            //if errors should call the snackbar message if there is no error, return response
            //ToDo: use the sendMessage to show posible errors (using the alerts properties).
            return response;
        },
    };

    return (
        <GlobalActionsContext.Provider value={provActions}>
            {children}
        </GlobalActionsContext.Provider>
    );
};
