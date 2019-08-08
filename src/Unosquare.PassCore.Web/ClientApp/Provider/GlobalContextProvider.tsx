import * as React from 'react';
import { GlobalContext } from './GlobalContext';

export const GlobalContextProvider: React.FunctionComponent<any> = ({
    children,
    settings,
}) => {
    console.log('settings: ', settings);

    const [getProviderValue, setProviderValue] = React.useState({ ...settings });

    return (
        <GlobalContext.Provider value={getProviderValue}>
            {children}
        </GlobalContext.Provider>
    );
};
