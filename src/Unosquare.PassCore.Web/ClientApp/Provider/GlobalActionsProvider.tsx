import * as React from 'react';
import { fetchRequest } from '../Utils/FetchRequest';
import { GlobalActionsContext, GlobalContext, SnackbarContext } from './GlobalContext';

const enum RequestMethod {
    Delete = 'DELETE',
    Get = 'GET',
    Post = 'POST',
    Put = 'PUT',
}

export const GlobalActionsProvider: React.FunctionComponent<any> = ({ children }) => {

    const { alerts } = React.useContext(GlobalContext);
    const { sendMessage } = React.useContext(SnackbarContext);

    const provActions = {
        changePassword: async (data: any) => {
            const response = await fetchRequest(
                'api/password',
                RequestMethod.Post,
                JSON.stringify({ ...data },
                ));

            if (response.errors && response.errors.length) {
                let errorAlertMessage = '';
                response.errors.forEach((error: any) => {
                    switch (error.errorCode) {
                        case 0:
                            errorAlertMessage += error.message;
                            break;
                        case 1:
                            errorAlertMessage += alerts.errorFieldRequired;
                            break;
                        case 2:
                            errorAlertMessage += alerts.errorFieldMismatch;
                            break;
                        case 3:
                            errorAlertMessage += alerts.errorInvalidUser;
                            break;
                        case 4:
                            errorAlertMessage += alerts.errorInvalidCredentials;
                            break;
                        case 5:
                            errorAlertMessage += alerts.errorCaptcha;
                            break;
                        case 6:
                            errorAlertMessage += alerts.errorPasswordChangeNotAllowed;
                            break;
                        case 7:
                            errorAlertMessage += alerts.errorInvalidDomain;
                            break;
                        case 8:
                            errorAlertMessage += alerts.errorConnectionLdap;
                            break;
                        case 9:
                            errorAlertMessage += alerts.errorComplexPassword;
                            break;
                    }
                });

                sendMessage(errorAlertMessage, 'error');
                return false;
            }

            return true;
        },
    };

    return (
        <GlobalActionsContext.Provider value={provActions}>
            {children}
        </GlobalActionsContext.Provider>
    );
};
