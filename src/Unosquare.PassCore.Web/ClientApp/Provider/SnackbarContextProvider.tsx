import * as React from 'react';
import { SnackbarContainer, snackbarService } from 'uno-material-ui';
import { SnackbarContext } from './GlobalContext';

interface ISnackbarProvdierProps {
    children: any;
}

export const SnackbarContextProvider: React.FunctionComponent<ISnackbarProvdierProps> = ({
    children,
}: ISnackbarProvdierProps) => {
    const [providerValue] = React.useState({
        sendMessage: async (messageText: string, messageType = 'success') =>
            await snackbarService.showSnackbar(messageText, messageType),
    });

    return (
        <SnackbarContext.Provider value={providerValue}>
            {children}
            <SnackbarContainer />
        </SnackbarContext.Provider>
    );
};
