import * as React from 'react';
import { GlobalSnackbar } from 'uno-material-ui';
import { SnackbarContext } from './GlobalContext';

export const SnackbarContextProvider: React.FunctionComponent = ({ children }) => {
    const [getMessage, setMessage] = React.useState({
        messageText: '',
        messageType: 'success',
    });

    const [providerValue] = React.useState({
        sendMessage: (messageText: string, messageType = 'success') =>
            setMessage({
                messageText,
                messageType,
            }),
    });

    return (
        <SnackbarContext.Provider value={providerValue}>
            {children}
            <GlobalSnackbar seconds={5000} message={getMessage} mobile={false} />
        </SnackbarContext.Provider>
    );
};
