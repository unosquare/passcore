import * as React from 'react';
import { SnackbarContainer, snackbarService } from 'uno-material-ui';
import { SnackbarContext } from './GlobalContext';

export const SnackbarContextProvider: React.FunctionComponent = ({ children }) => {
    const [providerValue] = React.useState({
        sendMessage: (messageText: string, messageType = 'success') =>
            snackbarService.showSnackbar(messageText, messageType),
    });

    return (
        <SnackbarContext.Provider value={providerValue}>
            {children}
            <SnackbarContainer />
        </SnackbarContext.Provider>
    );
};
